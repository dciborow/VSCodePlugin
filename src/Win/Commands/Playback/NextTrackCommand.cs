// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class NextTrackCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public NextTrackCommand()
            : base(
                  "Next Track",
                  "Next Track description",
                  "Playback")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.SkipPlaybackToNext);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode NextTrackCommand action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.NextTrack.png");
            return bitmapImage;
        }

        public ErrorResponse SkipPlaybackToNext() => this.VSCodePlugin.Api.SkipPlaybackToNext(this.VSCodePlugin.CurrentDeviceId);
    }
}
