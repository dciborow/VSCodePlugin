// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using System.Net;
    using Loupedeck;
    using VSCodeAPI.Web.Models;

    /// <summary>
    /// Plugin: Check VSCode API responses
    /// </summary>
    public partial class VSCodePlugin : Plugin
    {
        public void CheckVSCodeResponse<T>(Func<T, ErrorResponse> apiCall, T param)
        {
            if (!this.VSCodeApiConnectionOk())
            {
                return;
            }

            var response = apiCall(param);

            this.CheckStatusCode(response.StatusCode(), response.Error?.Message);
        }

        public void CheckVSCodeResponse(Func<ErrorResponse> apiCall)
        {
            if (!this.VSCodeApiConnectionOk())
            {
                return;
            }

            var response = apiCall();

            this.CheckStatusCode(response.StatusCode(), response.Error?.Message);
        }

        internal void CheckStatusCode(HttpStatusCode statusCode, String VSCodeApiMessage)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:

                    if (this.PluginStatus.Status != Loupedeck.PluginStatus.Normal)
                    {
                        this.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, null, null);
                    }

                    break;

                case HttpStatusCode.Unauthorized:
                    // This should never happen?
                    this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Login to VSCode", null);
                    break;

                case HttpStatusCode.NotFound:
                    // User doesn't have device set or some other VSCode related thing. User action needed.
                    this.OnPluginStatusChanged(Loupedeck.PluginStatus.Warning, $"VSCode message: {VSCodeApiMessage}", null);
                    break;

                default:
                    if (this.PluginStatus.Status != Loupedeck.PluginStatus.Error)
                    {
                        this.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, VSCodeApiMessage, null);
                    }

                    break;
            }
        }
    }
}
