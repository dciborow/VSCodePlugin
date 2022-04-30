// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class TogglePlaybackCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private Boolean _isPlaying = true;

        public TogglePlaybackCommand()
            : base(
                  "Toggle Playback",
                  "Toggles audio playback",
                  "Playback")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.TogglePlayback);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode TogglePlayback action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return this._isPlaying ?
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Play.png") :
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Pause.png");
        }

        public ErrorResponse TogglePlayback()
        {
            var playback = this.VSCodePlugin.Api.GetPlayback();
            this._isPlaying = playback.IsPlaying;

            this.ActionImageChanged();

            return playback.IsPlaying
                ? this.VSCodePlugin.Api.PausePlayback(this.VSCodePlugin.CurrentDeviceId)
                : this.VSCodePlugin.Api.ResumePlayback(this.VSCodePlugin.CurrentDeviceId, String.Empty, null, String.Empty, 0);
        }
    }
}
