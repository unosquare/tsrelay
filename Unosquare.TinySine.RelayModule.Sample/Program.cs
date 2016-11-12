namespace Unosquare.TinySine.RelayModule.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var relayBoard = new RelayController())
            {
                relayBoard.Open("COM5", RelayController.DefaultPassword);

                relayBoard.RelayOperatingMode = RelayOperatingMode.Latching;
                relayBoard.ChangePassword(RelayController.DefaultPassword);

                Log.Info($"Board: Model: {relayBoard.BoardModel}, Version: {relayBoard.BoardVersion}, FW: {relayBoard.FirmwareVersion}, Channels: {relayBoard.RelayChannelCount}, Mode: {relayBoard.RelayOperatingMode}");
                DumpRelayState(relayBoard);

                Log.ReadKey("Press any key to set all the relay statuses to high", true);
                relayBoard.SetRelayStateAll(true);
                DumpRelayState(relayBoard);

                Log.ReadKey("Press any key to set all the relay statuses to low", true);
                relayBoard.SetRelayStateAll(false);
                DumpRelayState(relayBoard);

                relayBoard.RelayOperatingMode = RelayOperatingMode.Momentary;

                Log.ReadKey("Press any key to set relay 1 to high", true);
                relayBoard[RelayNumber.Relay01] = true;
                DumpRelayState(relayBoard);

                Log.ReadKey("Press any key to set relay 2 to high", true);
                relayBoard[RelayNumber.Relay02] = true;
                DumpRelayState(relayBoard);

                Log.ReadKey($"The temperature is {relayBoard.Temperature}", true);
                Log.ReadKey($"The voltage is {relayBoard.WorkingVoltage}", true);
            }

            Log.ReadKey("Press any key to continue . . .", true);
        }

        static private void DumpRelayState(RelayController relayBoard)
        {
            //return;
            var states = relayBoard.GetRelaysStateDictionary();

            Log.Info("STATUS DUMP");
            Log.Info($"Relay : 8 7 6 5 4 3 2 1");
            Log.Info($"Status: " + 
                $"{(states[RelayNumber.Relay08] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay07] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay06] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay05] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay04] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay03] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay02] ? 1 : 0)} " +
                $"{(states[RelayNumber.Relay01] ? 1 : 0)} ");
        }
    }
}
