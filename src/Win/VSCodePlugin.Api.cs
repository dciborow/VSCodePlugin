// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Loupedeck;
    using Newtonsoft.Json;
    using VSCodeAPI.Web;
    using VSCodeAPI.Web.Auth;
    using VSCodeAPI.Web.Models;

    /// <summary>
    /// Plugin VSCode API configuration and authorization
    /// </summary>
    public partial class VSCodePlugin : Plugin
    {
        private const String _clientId = "ClientId";
        private const String _clientSecret = "ClientSecret";
        private const String _tcpPorts = "TcpPorts";

        private static Token token = new Token();
        private static AuthorizationCodeAuth auth;
        private static String VSCodeTokenFilePath;

        private static Dictionary<String, String> _VSCodeConfiguration;

        private List<Int32> tcpPorts = new List<Int32>();

        internal VSCodeWebAPI Api { get; set; }

        internal String CurrentDeviceId { get; set; }

        internal Int32 PreviousVolume { get; set; }

        public Boolean VSCodeApiConnectionOk()
        {
            if (this.Api == null)
            {
                // User not logged in -> Automatically start login
                this.LoginToVSCode();

                // and skip action for now
                return false;
            }
            else if (token != null && DateTime.Now > token.CreateDate.AddSeconds(token.ExpiresIn) && !String.IsNullOrEmpty(token.RefreshToken))
            {
                this.RefreshToken(token.RefreshToken);
            }

            return true;
        }

        private Boolean ReadConfigurationFile()
        {
            // Get VSCode App configuration from VSCode-client.txt file: client id and client secret
            // Windows path: %LOCALAPPDATA%/Loupedeck/PluginData/VSCode/VSCode-client.txt
            var VSCodeClientConfigurationFile = this.ClientConfigurationFilePath;
            if (!File.Exists(VSCodeClientConfigurationFile))
            {
                // Check path
                Directory.CreateDirectory(Path.GetDirectoryName(VSCodeClientConfigurationFile));

                // Create the file
                using (FileStream fs = File.Create(VSCodeClientConfigurationFile))
                {
                    var info = new UTF8Encoding(true).GetBytes($"{_clientId}{Environment.NewLine}{_clientSecret}{Environment.NewLine}{_tcpPorts}");

                    // Add parameter titles to file.
                    fs.Write(info, 0, info.Length);
                }

                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, $"VSCode configuration is missing. Click More Details below", $"file:/{VSCodeClientConfigurationFile}");
                return false;
            }

            // Read configuration file, skip # comments, trim key and value
            _VSCodeConfiguration = File.ReadAllLines(VSCodeClientConfigurationFile)
                                                .Where(x => !x.StartsWith("#"))
                                                .Select(x => x.Split('='))
                                                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

            if (!(_VSCodeConfiguration.ContainsKey(_clientId) &&
                _VSCodeConfiguration.ContainsKey(_clientSecret) &&
                _VSCodeConfiguration.ContainsKey(_tcpPorts)))
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, $"Check VSCode API app 'ClientId' / 'ClientSecret' and 'TcpPorts' in configuration file. Click More Details below", $"file:/{VSCodeClientConfigurationFile}");
                return false;
            }

            // Check TCP Ports
            this.tcpPorts = _VSCodeConfiguration[_tcpPorts]
                .Split(',')
                .Select(x => new { valid = Int32.TryParse(x.Trim(), out var val), port = val })
                .Where(x => x.valid)
                .Select(x => x.port)
                .ToList();

            if (this.tcpPorts.Count == 0)
            {
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, $"Check 'TcpPorts' values in configuration file. Click More Details below", $"file:/{VSCodeClientConfigurationFile}");
                return false;
            }

            return true;
        }

        private void VSCodeConfiguration()
        {
            if (!this.ReadConfigurationFile())
            {
                return;
            }

            // Is there a token available
            token = null;
            VSCodeTokenFilePath = System.IO.Path.Combine(this.GetPluginDataDirectory(), "VSCode.json");
            if (File.Exists(VSCodeTokenFilePath))
            {
                token = this.ReadTokenFromLocalFile();
            }

            // Check token and the expiration datetime
            if (token != null && DateTime.Now < token.CreateDate.AddSeconds(token.ExpiresIn))
            {
                // Use the existing token
                this.Api = new VSCodeWebAPI
                {
                    AccessToken = token.AccessToken,
                    TokenType = "Bearer",
                };
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, "Connected", null);
            }
            else if (token != null && !String.IsNullOrEmpty(token.RefreshToken))
            {
                // Get a new access token based on the Refresh Token
                this.RefreshToken(token.RefreshToken);
            }
            else
            {
                // User has to login from Loupedeck application Plugin UI: Login - Login to VSCode. See LoginToVSCodeCommand.cs
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Login to VSCode as  user", null);
           }
        }

        private Token ReadTokenFromLocalFile()
        {
            var json = File.ReadAllText(VSCodeTokenFilePath);
            var localToken = JsonConvert.DeserializeObject<Token>(json);

            // Decrypt refresh token
            if (localToken != null && !String.IsNullOrEmpty(localToken.RefreshToken))
            {
                var secret = Convert.FromBase64String(localToken.RefreshToken);
                var plain = ProtectedData.Unprotect(secret, null, DataProtectionScope.CurrentUser);
                var encoding = new UTF8Encoding();
                localToken.RefreshToken = encoding.GetString(plain);
            }

            return localToken;
        }

        private void SaveTokenToLocalFile(Token newToken, String refreshToken)
        {
            // Decrypt refresh token
            var encoding = new UTF8Encoding();
            var plain = encoding.GetBytes(refreshToken);
            var secret = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);
            newToken.RefreshToken = Convert.ToBase64String(secret);

            File.WriteAllText(VSCodeTokenFilePath, JsonConvert.SerializeObject(newToken));
        }

        public void RefreshToken(String refreshToken)
        {
            auth = this.GetAuthorizationCodeAuth(out var timeout);

            Token newToken = auth.RefreshToken(refreshToken).Result;

            if (!String.IsNullOrWhiteSpace(newToken.Error))
            {
                Tracer.Error($"Error happened during refreshing VSCode account token: {newToken.Error}: {newToken.ErrorDescription}");
                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Failed getting access to VSCode. Login as  user", null);
            }

            if (this.Api == null)
            {
                this.Api = new VSCodeWebAPI
                {
                    AccessToken = newToken.AccessToken,
                    TokenType = "Bearer",
                };
            }

            this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, "Connected", null);

            this.Api.AccessToken = newToken.AccessToken;
            this.SaveTokenToLocalFile(newToken, refreshToken);
        }

        private PrivateProfile _privateProfile;

        private Paging<SimplePlaylist> GetUserPlaylists(Int32 offset = 0)
        {
            if (this.Api != null)
            {
                try
                {
                    if (this._privateProfile == null)
                    {
                        this._privateProfile = this.Api.GetPrivateProfile();
                    }

                    var profileId = this._privateProfile?.Id;
                    if (!String.IsNullOrEmpty(profileId))
                    {
                        var playlists = this.Api.GetUserPlaylists(profileId, 50, offset);
                        if (playlists?.Items != null && playlists.Items.Any())
                        {
                            return playlists;
                        }
                    }
                }
                catch (Exception e)
                {
                    Tracer.Trace(e, "VSCode playlists obtaining error");
                }
            }

            return new Paging<SimplePlaylist>
            {
                Items = new List<SimplePlaylist>(),
            };
        }

        public Paging<SimplePlaylist> GetAllPlaylists()
        {
            Paging<SimplePlaylist> playlists = this.GetUserPlaylists();
            if (playlists != null)
            {
                var totalPlaylistsCount = playlists.Total;
                while (playlists.Items.Count < totalPlaylistsCount)
                {
                    playlists.Items.AddRange(this.GetUserPlaylists(playlists.Items.Count).Items);
                }

                return playlists;
            }

            return null;
        }

        public void LoginToVSCode()
        {
            auth = this.GetAuthorizationCodeAuth(out var timeout);

            auth.AuthReceived += this.Auth_AuthReceived;

            auth.Start();
            auth.OpenBrowser();
        }

        private async void Auth_AuthReceived(Object sender, AuthorizationCode payload)
        {
            try
            {
                auth.Stop();

                var previousToken = await auth.ExchangeCode(payload.Code);
                if (!String.IsNullOrWhiteSpace(previousToken.Error))
                {
                    Tracer.Error($"Error happened during adding VSCode account: {previousToken.Error}: {previousToken.ErrorDescription}");
                    return;
                }

                this.Api = new VSCodeWebAPI
                {
                    AccessToken = previousToken.AccessToken,
                    TokenType = previousToken.TokenType,
                };

                this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, null, null);

                this.SaveTokenToLocalFile(previousToken, previousToken.RefreshToken);
            }
            catch (Exception ex)
            {
                Tracer.Error($"Error happened during VSCode authentication: {ex.Message}");
            }
        }

        public AuthorizationCodeAuth GetAuthorizationCodeAuth(out Int32 timeout)
        {
            timeout = 240000; // ?!?

            if (!NetworkHelpers.TryGetFreeTcpPort(this.tcpPorts, out var selectedPort))
            {
                Tracer.Error("No available ports for VSCode!");
                return null;
            }

            var scopes =
                VSCodeAPI.Web.Enums.Scope.PlaylistReadPrivate |
                VSCodeAPI.Web.Enums.Scope.Streaming |
                VSCodeAPI.Web.Enums.Scope.UserReadCurrentlyPlaying |
                VSCodeAPI.Web.Enums.Scope.UserReadPlaybackState |
                VSCodeAPI.Web.Enums.Scope.UserLibraryRead |
                VSCodeAPI.Web.Enums.Scope.UserLibraryModify |
                VSCodeAPI.Web.Enums.Scope.UserReadPrivate |
                VSCodeAPI.Web.Enums.Scope.UserModifyPlaybackState |
                VSCodeAPI.Web.Enums.Scope.PlaylistReadCollaborative |
                VSCodeAPI.Web.Enums.Scope.PlaylistModifyPublic |
                VSCodeAPI.Web.Enums.Scope.PlaylistModifyPrivate |
                VSCodeAPI.Web.Enums.Scope.PlaylistReadPrivate |
                VSCodeAPI.Web.Enums.Scope.UserReadEmail;

            return !this.ReadConfigurationFile()
                ? null
                : new AuthorizationCodeAuth(
                    _VSCodeConfiguration[_clientId], // VSCode API Client Id
                    _VSCodeConfiguration[_clientSecret], // VSCode API Client Secret
                    $"http://localhost:{selectedPort}", // selectedPort must correspond to that on the VSCode developers's configuration!
                    $"http://localhost:{selectedPort}",
                    scopes);
        }
    }
}
