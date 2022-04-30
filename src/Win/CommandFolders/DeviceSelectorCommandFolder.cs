// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin.CommandFolders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using VSCodeAPI.Web.Models;

    /// <summary>
    /// Dynamic folder (control center) for VSCode devices. https://developer.loupedeck.com/docs/Actions-taxonomy
    /// </summary>
    internal class DeviceSelectorCommandFolder : PluginDynamicFolder
    {
        private List<Device> _devices;

        private VSCodePlugin VSCodePlugin => this.Plugin as VSCodePlugin;

        public DeviceSelectorCommandFolder()
        {
            this.DisplayName = "Devices";
            this.GroupName = "Others";
            this.Navigation = PluginDynamicFolderNavigation.EncoderArea;
        }

        public override BitmapImage GetButtonImage(PluginImageSize imageSize)
        {
            var bitmapImage = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.Width80.Devices.png");
            return bitmapImage;
        }

        public override IEnumerable<String> GetButtonPressActionNames()
        {
            this._devices = this.VSCodePlugin?.Api?.GetDevices()?.Devices;
            if (this._devices != null && this._devices.Any())
            {
                this._devices.Add(new Device { Id = "activedevice", Name = "Active Device" });
                return this._devices.Select(x => this.CreateCommandName(x.Id));
            }

            return new List<String>();
        }

        public override String GetCommandDisplayName(String commandParameter, PluginImageSize imageSize)
        {
            var deviceDisplayName = this._devices.FirstOrDefault(x => x.Id == commandParameter)?.Name;
            if (deviceDisplayName != null && !deviceDisplayName.Contains(" ") && deviceDisplayName.Length > 9)
            {
                var updatedDisplayName = deviceDisplayName.Insert(9, "\n");
                return updatedDisplayName.Length > 18 ? updatedDisplayName.Insert(18, "\n") : updatedDisplayName;
            }

            return deviceDisplayName;
        }

        public override void RunCommand(String commandParameter)
        {
            try
            {
                this.VSCodePlugin.CheckVSCodeResponse(this.TransferPlayback, commandParameter);
            }
            catch (Exception e)
            {
                Tracer.Trace($"VSCode DeviceSelectorCommandFolder action obtain an error: ", e);
            }
        }

        public ErrorResponse TransferPlayback(String commandParameter)
        {
            if (commandParameter == "activedevice")
            {
                commandParameter = String.Empty;
            }

            this.VSCodePlugin.CurrentDeviceId = commandParameter;
            this.VSCodePlugin.SaveDeviceToCache(commandParameter);

            return this.VSCodePlugin.Api.TransferPlayback(this.VSCodePlugin.CurrentDeviceId, true);
        }
    }
}