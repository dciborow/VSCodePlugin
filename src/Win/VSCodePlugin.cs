// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using Loupedeck;

    /// <summary>
    /// Plugin main class - Loupedeck device commands and adjustment
    /// </summary>
    public partial class VSCodePlugin : Plugin
    {
        // This plugin has VSCode API -only actions.
        public override Boolean UsesApplicationApiOnly => true;

        // This plugin does not require an application (i.e. VSCode application installed on pc).
        public override Boolean HasNoApplication => true;

        public override void Load()
        {
            this.LoadPluginIcons();

            // Set everything ready and connect to VSCode API
            this.VSCodeConfiguration();

            // Get current (active) device id from internal cache
            this.CurrentDeviceId = this.GetCachedDeviceID();
        }

        public override void Unload()
        {
        }

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }

        private void LoadPluginIcons()
        {
            // Icons for Loupedeck application UI
            this.Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.PluginIcon16x16.png");
            this.Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.PluginIcon32x32.png");
            this.Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.PluginIcon48x48.png");
            this.Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.VSCodePlugin.Icons.PluginIcon256x256.png");
        }
    }
}
