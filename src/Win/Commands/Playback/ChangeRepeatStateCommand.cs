// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using VSCodeAPI.Web.Enums;
    using VSCodeAPI.Web.Models;

    internal class ChangeRepeatStateCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        private RepeatState _repeatState;

        public ChangeRepeatStateCommand()
            : base(
                  "Change Repeat State",
                  "Change Repeat State description",
                  "Playback")
        {
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                var playback = this.VSCodePlugin.Api.GetPlayback();
                switch (playback.RepeatState)
                {
                    case RepeatState.Off:
                        this._repeatState = RepeatState.Context;
                        this.VSCodePlugin.CheckVSCodeResponse(this.ChangeRepeatState, this._repeatState);
                        break;

                    case RepeatState.Context:
                        this._repeatState = RepeatState.Track;
                        this.VSCodePlugin.CheckVSCodeResponse(this.ChangeRepeatState, this._repeatState);
                        break;

                    case RepeatState.Track:
                        this._repeatState = RepeatState.Off;
                        this.VSCodePlugin.CheckVSCodeResponse(this.ChangeRepeatState, this._repeatState);
                        break;

                    default:
                        // Set plugin status and message
                        this.VSCodePlugin.CheckStatusCode(System.Net.HttpStatusCode.NotFound, "Not able to change repeat state (check device etc.)");
                        break;
                }
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode ChangeRepeatStateCommand action obtain an error: ", e);
            }
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            String icon;
            switch (this._repeatState)
            {
                case RepeatState.Off:
                    icon = "Loupedeck.VSCodePlugin.Icons.Width80.RepeatOff.png";
                    break;

                case RepeatState.Context:
                    icon = "Loupedeck.VSCodePlugin.Icons.Width80.RepeatList.png";
                    break;

                case RepeatState.Track:
                    icon = "Loupedeck.VSCodePlugin.Icons.Width80.Repeat.png";
                    break;

                default:
                    // Set plugin status and message
                    icon = "Loupedeck.VSCodePlugin.Icons.Width80.RepeatOff.png";
                    break;
            }

            var bitmapImage = EmbeddedResources.ReadImage(icon);
            return bitmapImage;
        }

        public ErrorResponse ChangeRepeatState(RepeatState repeatState)
        {
            var response = this.VSCodePlugin.Api.SetRepeatMode(repeatState, this.VSCodePlugin.CurrentDeviceId);

            this.ActionImageChanged();

            return response;
        }
    }
}
