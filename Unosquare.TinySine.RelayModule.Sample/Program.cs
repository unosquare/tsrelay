namespace Unosquare.TinySine.RelayModule.Sample
{
    using Swan;
    using Swan.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This program is a simple console test that  
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            using (var relayBoard = new RelayController(s => s.Trace(), (e) => e.Error()))
            {
                var isOpen = false;
                var password = RelayController.DefaultPassword;
                const string serialPort = "COM6";

                while (isOpen == false)
                {
                    try
                    {
                        $"Opening port {serialPort}".Info();

                        relayBoard.Open(serialPort, password);
                        isOpen = true;
                    }
                    catch (Exception ex)
                    {
                        ex.Log(nameof(Main));
                        password = PromptForPassword();
                    }
                }

                while (true)
                {
                    var selectedOption = Terminal.ReadPrompt("Select an option", ActionOptions, "Exit this program");
                    if (selectedOption.Key == ConsoleKey.Q)
                    {
                        $"Board: Model: {relayBoard.BoardModel}, Version: {relayBoard.BoardVersion}, FW: {relayBoard.FirmwareVersion}, Channels: {relayBoard.RelayChannelCount}, Mode: {relayBoard.RelayOperatingMode}".Info();
                    }
                    else if (selectedOption.Key == ConsoleKey.W)
                    {
                        DumpRelaysState(relayBoard);
                    }
                    else if (selectedOption.Key == ConsoleKey.E)
                    {
                        var newOpModeKey = Terminal.ReadKey("Enter Operating Mode (L for Latching, anything else, Momentary): ", false);
                        relayBoard.RelayOperatingMode = newOpModeKey.Key == ConsoleKey.L ? RelayOperatingMode.Latching : RelayOperatingMode.Momentary;

                        $"Relay Operating Mode is now: {relayBoard.RelayOperatingMode}".Info();
                    }
                    else if (selectedOption.Key == ConsoleKey.R)
                    {
                        $"Relay Board Working Voltage: {relayBoard.WorkingVoltage}".Info();
                        $"Relay Board Temperature    : {relayBoard.Temperature}".Info();
                    }
                    else if (selectedOption.Key == ConsoleKey.T)
                    {
                        relayBoard.ChangePassword(PromptForPassword());
                    }
                    else if (selectedOption.Key == ConsoleKey.A)
                    {
                        relayBoard.SetRelayStateAll(true);
                    }
                    else if (selectedOption.Key == ConsoleKey.S)
                    {
                        relayBoard.SetRelayStateAll(false);
                    }
                    else if (selectedOption.Key == ConsoleKey.D)
                    {
                        var relayNumber = (RelayNumber)Terminal.ReadNumber("Enter a relay number from 1 to 8", 1);
                        if ((int)relayNumber >= 1 && (int)relayNumber <= 8)
                        {
                            relayBoard[relayNumber] = true;
                        }
                        else
                        {
                            "Bad relay number. Argument must be between 1 and 8".Error();
                        }
                    }
                    else if (selectedOption.Key == ConsoleKey.F)
                    {
                        var relayNumber = (RelayNumber)Terminal.ReadNumber("Enter a relay number from 1 to 8", 1);
                        if ((int)relayNumber >= 1 && (int)relayNumber <= 8)
                        {
                            relayBoard[relayNumber] = false;
                        }
                        else
                        {
                            "Bad relay number. Argument must be between 1 and 8".Error();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

            }

            Terminal.ReadKey("Press any key to continue . . .");
        }

        /// <summary>
        /// Prompts for password.
        /// </summary>
        /// <returns></returns>
        private static string PromptForPassword()
        {
            var enteredSixDigitPassword = false;
            var password = string.Empty;

            while (!enteredSixDigitPassword)
            {
                var passwordNumber = Terminal.ReadNumber("Enter the 6-digit password", -1);

                if (passwordNumber >= 0 && passwordNumber <= 999999)
                {
                    enteredSixDigitPassword = true;
                    password = passwordNumber.ToString("000000");
                }
                else
                {
                    "The number must be between 0 and 999999".Error();
                }
            }

            return password;
        }

        /// <summary>
        /// Dumps the state of the relays.
        /// </summary>
        /// <param name="relayBoard">The relay board.</param>
        private static void DumpRelaysState(RelayController relayBoard)
        {
            var states = relayBoard.GetRelaysStateDictionary();

            "STATUS DUMP".Info();
            "RELAY:     \t8\t7\t6\t5\t4\t3\t2\t1".Info();
            ($"STATE:     \t" +
                $"{(states[RelayNumber.Relay08] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay07] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay06] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay05] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay04] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay03] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay02] ? 1 : 0)}\t" +
                $"{(states[RelayNumber.Relay01] ? 1 : 0)}\t").Info();
        }

        #region Action Options

        /// <summary>
        /// The action options
        /// </summary>
        private static readonly Dictionary<ConsoleKey, string> ActionOptions = new Dictionary<ConsoleKey, string>
        {
            // Module COntrol Items
            { ConsoleKey.Q, "MODULE   - Show Board Info" },
            { ConsoleKey.W, "MODULE   - Show Relays States" },
            { ConsoleKey.E, "MODULE   - Change Relay Operating Mode" },
            { ConsoleKey.R, "MODULE   - Show Voltage and Temperature" },
            { ConsoleKey.T, "MODULE   - Change Password" },

            { ConsoleKey.A, "RELAYS   - Set All Relays to High" },
            { ConsoleKey.S, "RELAYS   - Set All Relays to Low" },
            { ConsoleKey.D, "RELAYS   - Set the State of a Single Relay to High" },
            { ConsoleKey.F, "RELAYS   - Set the State of a Single Relay to Low" },
        };

        #endregion
    }
}