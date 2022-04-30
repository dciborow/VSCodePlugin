// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ToggleLikeCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private Boolean _isLiked = true;

        public ToggleLikeCommand()
            : base(
                  "Toggle Like",
                  "Toggle Like",
                  "Others")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                var playback = this.VSCodePlugin.Api.GetPlayback();
                var trackId = playback.Item?.Id;
                if (String.IsNullOrEmpty(trackId))
                {
                    // Set plugin status and message
                    this.VSCodePlugin.CheckStatusCode(System.Net.HttpStatusCode.NotFound, "No track");
                    return;
                }

                var trackItemId = new List<String> { trackId };
                var tracksExist = this.VSCodePlugin.Api.CheckSavedTracks(trackItemId);
                if (tracksExist.List == null && tracksExist.Error != null)
                {
                    // Set plugin status and message
                    this.VSCodePlugin.CheckStatusCode(System.Net.HttpStatusCode.NotFound, "No track list");
                    return;
                }

                if (tracksExist.List.Any() && tracksExist.List.FirstOrDefault() == false)
                {
                    this.VSCodePlugin.CheckVSCodeResponse(this.VSCodePlugin.Api.SaveTrack, trackId);
                    this._isLiked = true;
                    this.ActionImageChanged();
                }
                else
                {
                    this.VSCodePlugin.CheckVSCodeResponse(this.VSCodePlugin.Api.RemoveSavedTracks, trackItemId);
                    this._isLiked = false;

                    this.ActionImageChanged();
                }
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode Toggle Like action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return this._isLiked ?
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.SongLike.png") :
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.SongDislike.png");
        }
    }
}
