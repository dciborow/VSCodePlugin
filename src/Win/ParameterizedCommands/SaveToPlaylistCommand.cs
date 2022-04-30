// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin.ParameterizedCommands
{
    using System;
    using System.Linq;
    using VSCodeAPI.Web.Models;

    internal class SaveToPlaylistCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public SaveToPlaylistCommand()
            : base()
        {
            // Profile actions do not belong to a group in the current UI, they are on the top level
            this.DisplayName = "Save To Playlist"; // so this will be shown as "group name" for parameterized commands
            this.GroupName = "Not used";

            this.MakeProfileAction("list;Select playlist to save:");
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.SaveToPlaylist, actionParameter);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode SaveToPlaylistCommand action obtain an error: ", e);
            }
        }

        public ErrorResponse SaveToPlaylist(String playlistId)
        {
            var idWithUri = true;
            if (idWithUri)
            {
                playlistId = playlistId.Replace("VSCode:playlist:", String.Empty);
            }

            var playback = this.VSCodePlugin.Api.GetPlayback();
            var currentTrackUri = playback.Item.Uri;
            return this.VSCodePlugin.Api.AddPlaylistTrack(playlistId, currentTrackUri);
        }

        protected override PluginActionParameter[] GetParameters()
        {
            var playlists = this.VSCodePlugin.GetAllPlaylists();
            return playlists?.Items
                        .Select(x => new PluginActionParameter(x.Uri, x.Name, String.Empty))
                        .ToArray();
        }
    }
}
