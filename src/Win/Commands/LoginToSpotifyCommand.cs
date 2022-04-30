// Copyright(c) Loupedeck.All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;
    using Loupedeck;

    internal class LoginToVSCodeCommand : PluginDynamicCommand
    {
        public LoginToVSCodeCommand()
            : base(
                  "Login to VSCode",
                  " user login to VSCode API",
                  "Login")
        {
        }

        protected override void RunCommand(String actionParameter) => (this.Plugin as VSCodePlugin).LoginToVSCode();
    }
}
