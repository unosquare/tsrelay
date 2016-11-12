namespace Unosquare.TinySine.RelayModule
{
    using System;
    using System.Globalization;
    using System.Security;
    using System.Text;

    partial class RelayController
    {
        #region Command Methods

        private byte GetBoardModel()
        {
            var payload = new byte[] { (byte)OperationCode.GetBoardModel };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            return response[0];
        }

        private byte GetBoardVersion()
        {
            var payload = new byte[] { (byte)OperationCode.GetBoardVersion };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            return response[0];
        }

        private string GetPassword()
        {
            var payload = new byte[] { (byte)OperationCode.GetPassword };
            Write(payload);
            var response = Read(DefaultTimeout, 4);
            var encodedPassword = new byte[] { response[0], response[1], response[2], 0 };
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(encodedPassword);

            return BitConverter.ToUInt32(encodedPassword, 0).ToString(CultureInfo.InvariantCulture);
        }

        private bool VerifyPassword(string sixDigitPassword)
        {
            var payload = new byte[] { (byte)OperationCode.VerifyPassword, 0, 0, 0 };
            Array.Copy(EncodePassword(sixDigitPassword), 0, payload, 1, 3);
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            var result = response[0] == 0 ? false : true;
            return result;
        }

        private bool SetPassword(string sixDigitPassword)
        {
            var encodedPassword = EncodePassword(sixDigitPassword);
            var payload = new byte[] { (byte)OperationCode.SetPassword, encodedPassword[0], encodedPassword[1], encodedPassword[2] };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            var result = response[0] == 0 ? false : true;
            return result;
        }

        private RelayOperatingMode SetOperatingMode(RelayOperatingMode mode)
        {
            var payload = new byte[] { (byte)(mode == RelayOperatingMode.Latching ?
                OperationCode.SetRelayModeLatching : OperationCode.SetRelayModeMomentary) };
            Write(payload);

            return GetOperatingMode();
        }

        private RelayOperatingMode GetOperatingMode()
        {
            var payload = new byte[] { (byte)OperationCode.GetRelayMode };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            return (RelayOperatingMode)response[0];
        }

        private byte[] GetStatus()
        {
            var payload = new byte[] { (byte)OperationCode.GetStatus };
            Write(payload);
            var response = Read(DefaultTimeout, 4);
            return response;
        }

        private int GetFirmwareVersion()
        {
            var payload = new byte[] { (byte)OperationCode.GetFirmwareVersion };
            Write(payload);
            var response = Read(DefaultTimeout, 2);
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(response);

            return Convert.ToInt32(BitConverter.ToUInt16(response, 0));
        }

        private byte GetRelaysStateAll()
        {
            var payload = new byte[] { (byte)OperationCode.GetRelayState };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            return response[0];
        }

        private bool GetRelayState(RelayNumber relayNumber)
        {
            return GetRelayState(GetRelaysStateAll(), relayNumber);
        }

        private bool GetRelayState(byte relayStates, RelayNumber relayNumber)
        {
            var exponent = (int)(relayNumber - 1);

            byte mask = (byte)(Math.Pow(2, exponent));
            return (relayStates & mask) != 0;
        }

        private byte GetWorkingVoltage()
        {
            var payload = new byte[] { (byte)OperationCode.GetWorkingVoltage };
            Write(payload);
            var response = Read(DefaultTimeout, 1);
            return response[0];
        }

        private byte[] GetTemperatureRawData()
        {
            var payload = new byte[] { (byte)OperationCode.GetTemperatureRaw };
            Write(payload);
            var response = Read(DefaultTimeout, 2);
            return response;
        }

        private decimal GetTemperature()
        {
            var payload = new byte[] { (byte)OperationCode.GetTemperature };
            Write(payload);
            var response = Read(DefaultTimeout, 0);
            var temperatureStr = Encoding.ASCII.GetString(response).Trim().Replace("\r\n", "");
            decimal result = 0.0M;
            if (decimal.TryParse(temperatureStr, out result))
                return result;

            return 0;
        }

        public bool SetRelayStateAll(bool state)
        {
            var payload = new byte[] { (byte)(state ? OperationCode.SetRelayStateAllHigh : OperationCode.SetRelayStateAllLow) };
            Write(payload);
            return true;
        }

        private byte SetRelayState(RelayNumber relayNumber, bool state)
        {
            var opCode = OperationCode.SetRelayState01High;
            if (state)
            {
                switch (relayNumber)
                {
                    case RelayNumber.Relay01: opCode = OperationCode.SetRelayState01High; break;
                    case RelayNumber.Relay02: opCode = OperationCode.SetRelayState02High; break;
                    case RelayNumber.Relay03: opCode = OperationCode.SetRelayState03High; break;
                    case RelayNumber.Relay04: opCode = OperationCode.SetRelayState04High; break;
                    case RelayNumber.Relay05: opCode = OperationCode.SetRelayState05High; break;
                    case RelayNumber.Relay06: opCode = OperationCode.SetRelayState06High; break;
                    case RelayNumber.Relay07: opCode = OperationCode.SetRelayState07High; break;
                    case RelayNumber.Relay08: opCode = OperationCode.SetRelayState08High; break;
                }
            }
            else
            {
                switch (relayNumber)
                {
                    case RelayNumber.Relay01: opCode = OperationCode.SetRelayState01Low; break;
                    case RelayNumber.Relay02: opCode = OperationCode.SetRelayState02Low; break;
                    case RelayNumber.Relay03: opCode = OperationCode.SetRelayState03Low; break;
                    case RelayNumber.Relay04: opCode = OperationCode.SetRelayState04Low; break;
                    case RelayNumber.Relay05: opCode = OperationCode.SetRelayState05Low; break;
                    case RelayNumber.Relay06: opCode = OperationCode.SetRelayState06Low; break;
                    case RelayNumber.Relay07: opCode = OperationCode.SetRelayState07Low; break;
                    case RelayNumber.Relay08: opCode = OperationCode.SetRelayState08Low; break;
                }
            }


            var payload = new byte[] { (byte)opCode };
            Write(payload);
            return GetRelaysStateAll();
        }

        #endregion
    }
}
