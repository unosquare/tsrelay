[![Build status](https://ci.appveyor.com/api/projects/status/8frmfyb5wo751aop/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/tsrelay/branch/master)[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/tsrelay/)](https://github.com/igrigorik/ga-beacon)

# TinySine USB/Wireless Relay Module TOSR1x - Interfacing Library for .NET

* Product Page: http://www.tinyosshop.com/index.php?route=product/product&path=141_142&product_id=947

## Features
* All documented commands are implemented (2016-11-06)
* No dependencies
* Nice sample application included for testing
* MIT License

## NuGet Installation [![NuGet version](https://badge.fury.io/nu/Unosquare.TinySine.RelayModule.svg)](https://badge.fury.io/nu/Unosquare.TinySine.RelayModule)

```
PM> Install-Package Unosquare.TinySine.RelayModule
```

## Usage

```csharp
using (var controller = new RelayController())
{
    controller.open("COM4", RelayController.DefaultPassword);
    
    controller[RelayNumber.Relay01] = true;
    controller[RelayNumber.Relay02] = false;
    controller.RelayOperatingMode = RelayOperatingMode.Momentary;
    Console.WriteLine($"Board: Model: {controller.BoardModel}, Version: {controller.BoardVersion}, FW: {controller.FirmwareVersion}, Channels: {controller.RelayChannelCount}, Mode: {controller.RelayOperatingMode}");
    controller.SetRelayStateAll(false);
}
```

## .NET Core 2 for Raspberry Pi 3

Implementing .NET Core 2 in your projects running on [Ubuntu Classic Server 16.04](https://ubuntu-pi-flavour-maker.org/download/) for Raspberry PI 3.

This example uses the Sample project

You need to have:

- [.NET Core SDK](https://www.microsoft.com/net/core#windowscmd) installed on your machine.
- [Visual Studio 2017](https://www.visualstudio.com)
- [Ubuntu Classic Server 16.04](https://ubuntu-pi-flavour-maker.org/download/) for Raspberry PI 3.
- [7zip](http://www.7-zip.org/)
- [Win32DiskImage](https://sourceforge.net/projects/win32diskimager/)
- Highly recommend that you use a Class 6 or Class 10 microSD HC card.

### Configuring Ubuntu on the Raspberry

If you already have installed all you need:

- Extract and Write Ubuntu Classic Server into your SD Card
- Plugin your SD Card into your Raspberry and start it
    - It's going to ask you for login and password, the defaults are ubuntu/ubuntu
- Install the  [Wiring Pi](http://wiringpi.com/download-and-install/) dependency (Needed it to access the serial port).

#### Installing .NET Core

Run the fallowing commands in your Raspberry Pi

```
# Update Ubuntu 16.04
sudo apt-get -y update

# Install the packages necessary for .NET Core
sudo apt-get -y install libunwind8 libunwind8-dev gettext libicu-dev liblttng-ust-dev libcurl4-openssl-dev libssl-dev uuid-dev

# Download the latest binaries for .NET Core 2 
wget https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz

# Make a directory for .NET Core to live in
mkdir /home/ubuntu/dotnet

# Unzip the binaries into the directory you just created
tar -xvf dotnet-runtime-latest-linux-arm.tar.gz -C /home/ubuntu/dotnet

# Now add the path to the dotnet executable to the environment path
# This ensures the next time you log in, the dotnet exe is on your path
echo "PATH=\$PATH:/home/ubuntu/dotnet" >> dotnetcore.sh
sudo mv dotnetcore.sh /etc/profile.d

# Then run the command below to add the path to the dotnet executable to the current session
PATH=$PATH:/home/ubuntu/dotnet
```

After that, you can reboot the raspberry. To check if dotnet is installed just run "dotnet" and a message should show.

```
ubuntu@ubuntu:~$ dotnet

Usage: dotnet [options]
Usage: dotnet [path-to-application]

Options:
  -h|--help            Display help.
  --version         Display version.

path-to-application:
  The path to an application .dll file to execute.
```

### Configuring Windows and Visual Studio 2017

Once you have all you need:

- Open the Sample Project
- You need to edit your csproj to run .NET Core 2, you need to check if the `TargetFramework` is correct
    ```xml
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <AssemblyTitle>Unosquare.TinySine.RelayModule.Sample</AssemblyTitle>
        <AssemblyName>Unosquare.TinySine.RelayModule.Sample</AssemblyName>
        <PackageId>Unosquare.TinySine.RelayModule.Sample</PackageId>
        <TargetFramework>netcoreapp2.0</TargetFramework>
    </PropertyGroup>
    ```
- Rebuild the Sample project
- Open the Package Manager Console
- Type
    ```
    // If you have more than one project, you need to specify the project to restore the packages
    PM> dotnet restore .\Unosquare.TinySine.RelayModule.Sample
    ```
    ```
    // If you have more than one project, you need to specify the project to publish the packages
    PM> dotnet publish -r ubuntu.16.04-arm .\Unosquare.TinySine.RelayModule.Sample
    ```
- Find the publish files, usually located in 
    - C:\\~\Unosquare.TinySine.RelayModule.Sample\bin\Debug\netcoreapp2.0\ubuntu.16.04-arm\publish

### Running the project

Once you have published the project you need to pass the publish folder to the Raspberry Pi, you can use ssh or an usb to do that, and if you want, you can rename the folder. We just pass the publish folder.

In the Raspberry, you need to do:

- Navigate to the projects folder
    ```
    ubuntu@ubuntu:~$ cd publish
    ubuntu@ubuntu:~/publish$

    ```
- Give permissions to run the project
    ```
    ubuntu@ubuntu:~/publish$ sudo chmod u+x *
    ```
- Run the project
    ```
    ubuntu@ubuntu:~/publish$ ./Unosquare.TinySine.RelayModule.Sample
    ```

## Missing Stuff
* Some more testing is needed
