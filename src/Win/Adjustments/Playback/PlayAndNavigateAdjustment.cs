// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class PlayAndNavigateAdjustment : PluginDynamicAdjustment
    {
        public PlayAndNavigateAdjustment()
            : base(
                  "Play And Navigate Tracks(s)",
                  "Play And Navigate Tracks(s) description",
                  "Playback",
                  true)
        {
        }

        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        protected override void ApplyAdjustment(String actionParameter, Int32 ticks)
        {
            try
            {
                if (ticks > 0)
                {
                    this.VSCodePlugin.CheckVSCodeResponse(this.SkipPlaybackToNext);
                }
                else
                {
                    this.VSCodePlugin.CheckVSCodeResponse(this.SkipPlaybackToPrevious);
                }
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode ApplyAdjustment action obtain an error: ", e);
            }
        }

        // Overwrite the RunCommand method that is called every time the user presses the encoder to which this command is assigned
        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.TogglePlayback);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode ApplyAdjustment action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            // Dial strip 50px
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width50.PlayAndNavigateTracks.png");
            return bitmapImage;
        }

        public ErrorResponse SkipPlaybackToNext() => this.VSCodePlugin.Api.SkipPlaybackToNext(this.VSCodePlugin.CurrentDeviceId);

        public ErrorResponse SkipPlaybackToPrevious() => this.VSCodePlugin.Api.SkipPlaybackToPrevious(this.VSCodePlugin.CurrentDeviceId);

        public ErrorResponse TogglePlayback()
        {
            var playback = this.VSCodePlugin.Api.GetPlayback();
            return playback.IsPlaying
                ? this.VSCodePlugin.Api.PausePlayback(this.VSCodePlugin.CurrentDeviceId)
                : this.VSCodePlugin.Api.ResumePlayback(this.VSCodePlugin.CurrentDeviceId, String.Empty, null, String.Empty, 0);
        }
    }
}
