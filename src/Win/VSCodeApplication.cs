// Copyright (c) Loupedeck. All rights reserved.

namespace Loupedeck.VSCodePlugin
{
    using System;

    /// <summary>
    /// Target application - process name for Windows, bundle name for macOS
    /// </summary>
    public class VSCodeApplication : ClientApplication
    {
        public VSCodeApplication()
        {
        }

        protected override String GetProcessName() => "VSCode";

        protected override String GetBundleName() => "com.VSCode.client";
    }
}