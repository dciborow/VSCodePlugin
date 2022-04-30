# VSCode Plugin

VSCode plugin for Loupedeck software, based on [Loupedeck Plugin SDK](https://developer.loupedeck.com/docs). See [Developing plugins for Loupedeck - Process overview](https://developer.loupedeck.com/docs/Process-overview)

It is recommended to use Loupedeck software version 5.0 or newer.

## Restrictions for VSCode Plugin
Loupedeck software comes with an "internal" VSCode plugin that does not use SDK Plugin API. [VSCode SDK](https://github.com/Loupedeck/VSCodePlugin) solution uses SDK Plugin API and contains the same functionality as the "internal" plugin.

🛑 It is not possible to use both "internal" and SDK VSCode plugin versions at the same time. Select only VSCode SDK from Manage Plugins.  

<p align="center">
  <img src="images/ManagePlugins.png" width="300" title="hover text">
</p>

The reason for this is that currently SDK plugins cannot use the same dll as Loupedeck application is using. In this case e.g., a modified version of [VSCodeAPI-NET](https://github.com/Loupedeck/VSCodeAPI-NET) which is used to communicate with VSCode Web API. SDK Plugin must reference the [VSCodeAPI-NET](https://github.com/Loupedeck/VSCodeAPI-NET) dlls within Loupedeck installation:
```csharp
<INSTALLATION PATH>\Loupedeck\Loupedeck2\VSCodeAPI.Web.dll
<INSTALLATION PATH>\Loupedeck\Loupedeck2\VSCodeAPI.Web.Auth.dll
```
Hence it's not possible to upgrade this plugin to use latest version of [VSCodeAPI-NET](http://johnnycrazy.github.io/VSCodeAPI-NET).  

## Solution description
Solution was created using the [Plugin Generator tool](https://github.com/Loupedeck/Loupedeck4PluginSdkCopy/wiki/Creating-the-project-files-for-the-plugin)

```
<TOOL LOCATION>\LoupedeckPluginTool.exe g -name=VSCode
```

Repository contains solution for Windows, **VSCodePluginWin.sln**. 

Main functionality is in VSCodePlugin.cs and partial classes. Actions are under folders
```
Adjustments
CommandFolders
Commands
ParameterizedCommands
```
<p align="center">
  <img src="images/VS_Solution.png" width="250" title="hover text">
</p>

DLLs referencing Loupedeck installation, see [Restrictions for VSCode plugin usage](#Restrictions-for-VSCode-plugin) 
```
VSCodeAPI.Web
VSCodeAPI.Web.Auth
```
<p align="center">
  <img src="images/LD_Installation_DLL_References.png" width="250" title="hover text">
</p>

### VS Build Configuration for Debugging

VSCodePlug.csproj
```
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    ...
    <OutputPath>$(LocalAppData)\Loupedeck\Plugins\</OutputPath>
    ...
  </PropertyGroup>
  ```

  <p align="center">
  <img src="images/VS_Debug.png" width="600" title="hover text">
</p>

### VSCode Client Configuration
To access [VSCode Web API](https://developer.VSCode.com/documentation/web-api/quick-start/), first create a simple server-side application. Then add the Client Id and Secret, and port to the configuration file:
```
%LOCALAPPDATA%/Loupedeck/PluginData/VSCode/VSCode-client.txt
```
See example project file **VSCode-client-template.txt**

Port(s) must correspond to that on the VSCode developers's configuration.


🛑 IMPORTANT! This plugin uses VSCode Client Id and Secret that are read from text file. DO NOT DISTRIBUTE THIS FILE.


For creating distributable VSCode plugin, use [Authorization code flow with PKCE](https://developer.VSCode.com/documentation/general/guides/authorization/code-flow/). This concept is currently untested, but e.g.,
1. Upgrade to latest [VSCodeAPI-NET](http://johnnycrazy.github.io/VSCodeAPI-NET) version and implement Authorization code flow with PKCE.
2. Add automation to disable/remove *VSCodePlugin.dll*, *VSCodeAPI.Web.dll*, *VSCodeAPI.Web.Auth.dll* from Loupedeck software installation folder.


## Creating Installation Package for Loupedeck software 5.0 or newer
Loupedeck 5.0 comes with Loupedeck Plugin Package Installer (LoupedeckPluginPackageInstaller.exe) that can install and uninstall plugins.

The input for Loupedeck Plugin Package Installer is a ZIP archive with .lplug4 extension. Douple click the .lplug4 file will start plugin install with Loupedeck software 5.0 or newer.

ZIP archive must contain a LoupedeckPackage.yaml file with plugin manifest in YAML format.

Recommended archive structure:
```
/LoupedeckPackage.yaml   -- plugin manifest
/bin/win/                -- binaries for Windows version
/bin/mac/                -- (binaries for Mac version)
```
The YAML manifest has the following format (user modifiable fields are in <> brackets)
```
type: plugin4
name: <Name of the plugin>
displayName: <Display name of the plugin>
version: <version string>
author: <author id>
copyright: <copyright>

supportedDevices: <Note if you support only one, remove another>
    - LoupedeckCt  
    - LoupedeckLive

pluginFileName: <Plugin file name>
pluginFolderWin: <Folder for windows binaries within zip file>
pluginFolderMac: <Folder for mac binaries within zip file>
```

Example LoupedeckPackage.yaml for VSCode SDK:
```
type: plugin4
name: VSCode
displayName: VSCode SDK
version: 1.0
author: Loupedeck
copyright: Loupedeck

supportedDevices:
    - LoupedeckCt  
    - LoupedeckLive

pluginFileName: VSCodePlugin.dll
pluginFolderWin: bin\win\
pluginFolderMac: bin\mac\
```

## Plugin usage with Loupedeck software and devices
To use plugin, first select it in Loupedeck software. See  [Restrictions for VSCode plugin usage](#Restrictions-for-VSCode-plugin) 

Add action **Login to VSCode** or any other action. All will open browser for user to login with VSCode. Note that user must have VSCode subscription.

  <p align="center">
  <img src="images/LD_Live_with_VSCode_Icons.png" width="600" title="hover text">
</p>

Possible problems with plugin/VSCode API will display warning (yellow dot in plugin icon) and error (red dot) messages.

  <p align="center">
  <img src="images/API_warning.png" width="300" title="hover text">
</p>
