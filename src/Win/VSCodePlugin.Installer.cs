// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;

    public partial class VSCodePlugin : Plugin
    {
        public String ClientConfigurationFilePath => System.IO.Path.Combine(this.GetPluginDataDirectory(), "VSCode-client.txt");

        public override Boolean Install()
        {
            // Here we ensure the plugin data directory is there.
            // See Storing-Plugin-Data
            var pluginDataDirectory = this.GetPluginDataDirectory();
            if (!IoHelpers.EnsureDirectoryExists(pluginDataDirectory))
            {
                Tracer.Error("Plugin data is not created. Cannot continue installation");
                return false;
            }

            // Now we put a template configuration file from resources
            var filePath = System.IO.Path.Combine(pluginDataDirectory, this.ClientConfigurationFilePath);

            using (var streamWriter = new System.IO.StreamWriter(filePath))
            {
                // Write data
                this.Assembly.ExtractFile("VSCode-client-template.txt", this.ClientConfigurationFilePath);
            }

            return true;
        }
    }
}
