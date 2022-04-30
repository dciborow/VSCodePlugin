// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class MuteCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public MuteCommand()
            : base(
                  "Mute",
                  "Mute description",
                  "VSCode Volume")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.Mute);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode MuteCommand action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.MuteVolume.png");
            return bitmapImage;
        }

        public ErrorResponse Mute()
        {
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
