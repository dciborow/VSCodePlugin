// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class PreviousTrackCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public PreviousTrackCommand()
            : base(
                  "Previous Track",
                  "Previous Track description",
                  "Playback")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.SkipPlaybackToPrevious);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode PreviousTrackCommand action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.PreviousTrack.png");
            return bitmapImage;
        }

        public ErrorResponse SkipPlaybackToPrevious() => this.VSCodePlugin.Api.SkipPlaybackToPrevious(this.VSCodePlugin.CurrentDeviceId);
    }
}
