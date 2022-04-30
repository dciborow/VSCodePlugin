// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class UnmuteCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public UnmuteCommand()
            : base(
                  "Unmute",
                  "Unmute description",
                  "VSCode Volume")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.Unmute);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode UnmuteCommand action obtain an error: ", e);
            }
        }


        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Volume.png");
            return bitmapImage;
        }

        public ErrorResponse Unmute()
        {
            var unmuteVolume = this.VSCodePlugin.PreviousVolume != 0 ? this.VSCodePlugin.PreviousVolume : 50;
            var result = this.VSCodePlugin.Api.SetVolume(unmuteVolume, this.VSCodePlugin.CurrentDeviceId);
            return result;
        }
    }
}
