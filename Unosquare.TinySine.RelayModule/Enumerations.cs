using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unosquare.TinySine.RelayModule
{
    public enum RelayOperatingMode : byte
    {
        Latching = 0x41,
        Momentary = 0x42,
    }

    public enum RelayNumber : byte
    {
        Relay01 = 1,
        Relay02 = 2,
        Relay03 = 3,
        Relay04 = 4,
        Relay05 = 5,
        Relay06 = 6,
        Relay07 = 7,
        Relay08 = 8,
    }

    internal enum OperationCode : byte
    {
        GetBoardModel = 0x3C,
        GetBoardVersion = 0x3D,
        GetPassword = 0x3E,
        VerifyPassword = 0x3F,
        SetPassword = 0x40,
        SetRelayModeLatching = 0x41,
        SetRelayModeMomentary = 0x42,
        GetRelayMode = 0x43,
        GetStatus = 0x44,
        GetFirmwareVersion = 0x5A,
        GetRelayState = 0x5B,
        GetWorkingVoltage = 0x5D,
        GetTemperatureRaw = 0x61,
        GetTemperature = 0x62,
        SetRelayStateAllHigh = 0x64,
        SetRelayState01High = 0x65,
        SetRelayState02High = 0x66,
        SetRelayState03High = 0x67,
        SetRelayState04High = 0x68,
        SetRelayState05High = 0x69,
        SetRelayState06High = 0x6A,
        SetRelayState07High = 0x6B,
        SetRelayState08High = 0x6C,

        SetRelayStateAllLow = 0x6E,
        SetRelayState01Low = 0x6F,
        SetRelayState02Low = 0x70,
        SetRelayState03Low = 0x71,
        SetRelayState04Low = 0x72,
        SetRelayState05Low = 0x73,
        SetRelayState06Low = 0x74,
        SetRelayState07Low = 0x75,
        SetRelayState08Low = 0x76,
    }
}
