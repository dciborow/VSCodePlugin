// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class ShufflePlayCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private Boolean _shuffleState;

        public ShufflePlayCommand()
            : base(
                  "Shuffle Play",
                  "Shuffle Play description",
                  "Playback")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.ShufflePlay);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode ShufflePlayCommand action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return this._shuffleState ?
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Shuffle.png") :
                EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.ShuffleOff.png");
        }

        public ErrorResponse ShufflePlay()
        {
            var playback = this.VSCodePlugin.Api.GetPlayback();
            this._shuffleState = !playback.ShuffleState;
            var response = this.VSCodePlugin.Api.SetShuffle(this._shuffleState, this.VSCodePlugin.CurrentDeviceId);

            this.ActionImageChanged();

            return response;
        }
    }
}
