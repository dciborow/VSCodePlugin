// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin.ParameterizedCommands
{
    using System;
    using System.Linq;
    using VSCodeAPI.Web.Models;

    internal class StartPlaylistCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public StartPlaylistCommand()
            : base()
        {
            // Profile actions do not belong to a group in the current UI, they are on the top level
            this.DisplayName = "Start Playlist"; // so this will be shown as "group name" for parameterized commands
            this.GroupName = "Not used";

            this.MakeProfileAction("list;Select playlist to play:");
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.StartPlaylist, actionParameter);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode StartPlaylistCommand action obtain an error: ", e);
            }
        }

        public ErrorResponse StartPlaylist(String contextUri)
        {
            return this.VSCodePlugin.Api.ResumePlayback(this.VSCodePlugin.CurrentDeviceId, contextUri, null, String.Empty);
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
