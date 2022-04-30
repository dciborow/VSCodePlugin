// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin.ParameterizedCommands
{
    using System;
    using VSCodeAPI.Web.Models;

    internal class DirectVolumeCommand : PluginDynamicCommand
    {
        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public DirectVolumeCommand()
            : base()
        {
            // Profile actions do not belong to a group in the current UI, they are on the top level
            this.DisplayName = "Direct Volume"; // so this will be shown as "group name" for parameterized commands
            this.GroupName = "Not used";

            this.MakeProfileAction("text;Enter volume level to set 0-100:");
        }

        protected override void RunCommand(String actionParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.SetVolume, actionParameter);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode DirectVolumeCommand action obtain an error: ", e);
            }
        }

        public ErrorResponse SetVolume(String percents)
        {
            var isConverted = Int32.TryParse(percents, out var volume);
            return isConverted ? this.SetVolume(volume) : null;
        }

        public ErrorResponse SetVolume(Int32 percents)
        {
            if (percents > 100)
            {
                percents = 100;
            }

            if (percents < 0)
            {
                percents = 0;
            }

            var response = this.VSCodePlugin.Api.SetVolume(percents, this.VSCodePlugin.CurrentDeviceId);
            return response;
        }
    }
}
