[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/tsrelay/)](https://github.com/igrigorik/ga-beacon)

# TinySine USB/Wireless Relay Module TOSR1x - Interfacing Library for .NET

* Wiki: http://www.waveshare.com/wiki/UART_Fingerprint_Reader
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

## Missing Stuff
* Some more testing is needed
