// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class ToggleMuteCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private Boolean _mute;

        public ToggleMuteCommand()
            : base(
                  "Toggle Mute",
                  "Toggles audio mute state",
                  "VSCode Volume")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.ToggleMute);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode ToggleMuteCommand action obtain an error: ", e);
            }
        }


        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return this._mute ?
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Volume.png") :
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.MuteVolume.png");
        }

        public ErrorResponse ToggleMute()
        {
            this.ActionImageChanged();

            var playback = this.VSCodePlugin.Api.GetPlayback();
            return playback?.Device.VolumePercent != 0 ? this.Mute() : this.Unmute();
        }

        public ErrorResponse Unmute()
        {
            this._mute = false;
            var unmuteVolume = this.VSCodePlugin.PreviousVolume != 0 ? this.VSCodePlugin.PreviousVolume : 50;
            var result = this.VSCodePlugin.Api.SetVolume(unmuteVolume, this.VSCodePlugin.CurrentDeviceId);
            return result;
        }

        public ErrorResponse Mute()
        {
            this._mute = true;
            var playback = this.VSCodePlugin.Api.GetPlayback();
            if (playback?.Device != null)
            {
                this.VSCodePlugin.PreviousVolume = playback.Device.VolumePercent;
            }

            var result = this.VSCodePlugin.Api.SetVolume(0, this.VSCodePlugin.CurrentDeviceId);

            return result;
        }
    }
}
