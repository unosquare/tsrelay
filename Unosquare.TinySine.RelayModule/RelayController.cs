namespace Unosquare.TinySine.RelayModule
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a TinySine TOSR1 Relay Controller providing up to 8 relay channels
    /// Product Page: http://www.tinyosshop.com/index.php?route=product/product&path=141_142&product_id=947
    /// Reference: http://www.tinyosshop.com/datasheet/TOSR14x%20User%20Manual.pdf
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class RelayController : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayController"/> class.
        /// </summary>
        public RelayController()
        {
            Uninitialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verifies the password on the device.
        /// </summary>
        /// <returns></returns>
        public bool? VerifyPassword()
        {
            return VerifyPassword(Password);
        }

        /// <summary>
        /// Changes the password on the device.
        /// </summary>
        /// <param name="sixDigitPassword">The six digit password.</param>
        /// <returns></returns>
        public bool ChangePassword(string sixDigitPassword)
        {
            var verifyResult = VerifyPassword();
            if (verifyResult.HasValue && verifyResult.Value)
            {
                if (SetPassword(sixDigitPassword))
                {
                    var result = VerifyPassword(sixDigitPassword);
                    Password = sixDigitPassword;
                    return result.HasValue && result.Value;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the state of the relays as a dictionary of relays and their corresponding state.
        /// </summary>
        /// <returns></returns>
        public Dictionary<RelayNumber, bool> GetRelaysStateDictionary()
        {
            var statesByte = GetRelaysStateAll();
            var relayNumbers = Enum.GetValues(typeof(RelayNumber)).Cast<RelayNumber>().ToArray();
            var result = new Dictionary<RelayNumber, bool>();
            foreach (var relay in relayNumbers)
            {
                result[relay] = GetRelayState(statesByte, relay);
            }

            return result;
        }

        #endregion

        #region Read-Once Properties

        /// <summary>
        /// Gets the board model.
        /// </summary>
        public int BoardModel { get; private set; }

        /// <summary>
        /// Gets the board version.
        /// </summary>
        public int BoardVersion { get; private set; }

        /// <summary>
        /// Gets the relay channel count.
        /// </summary>
        public int RelayChannelCount { get; private set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        public int FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the current password
        /// </summary>
        public string Password { get; private set; }

        #endregion

        #region Operating Status Properties

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        public decimal Temperature
        {
            get
            {
                var tempRaw = GetTemperatureRawData();
                var result = (255M * tempRaw[0] + tempRaw[1]) / 16M;
                return result;
            }
        }

        /// <summary>
        /// Gets the working voltage.
        /// </summary>
        public byte WorkingVoltage { get { return GetWorkingVoltage(); } }

        /// <summary>
        /// Gets or sets the relay operating mode.
        /// Wehn setting the operating mode the relays are reset by setting all highs and then setting all lows.
        /// </summary>
        public RelayOperatingMode RelayOperatingMode
        {
            get { return GetOperatingMode(); }
            set
            {
                SetOperatingMode(value);
            }
        }

        #endregion

        #region Relay State Properties

        /// <summary>
        /// Gets or sets the state as a <see cref="System.Boolean"/> value given the relay number.
        /// </summary>
        /// <value>
        /// The <see cref="System.Boolean"/>.
        /// </value>
        /// <param name="relayNumber">The relay number.</param>
        /// <returns></returns>
        public bool this[RelayNumber relayNumber]
        {
            get { return GetRelayState(relayNumber); }
            set { SetRelayState(relayNumber, value); }
        }

        /// <summary>
        /// Gets or sets the state of the relay 1
        /// </summary>
        public bool RelayState01 { get { return this[RelayNumber.Relay01]; } set { this[RelayNumber.Relay01] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 2
        /// </summary>
        public bool RelayState02 { get { return this[RelayNumber.Relay02]; } set { this[RelayNumber.Relay02] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 3
        /// </summary>
        public bool RelayState03 { get { return this[RelayNumber.Relay03]; } set { this[RelayNumber.Relay03] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 4
        /// </summary>
        public bool RelayState04 { get { return this[RelayNumber.Relay04]; } set { this[RelayNumber.Relay04] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 5
        /// </summary>
        public bool RelayState05 { get { return this[RelayNumber.Relay05]; } set { this[RelayNumber.Relay05] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 6
        /// </summary>
        public bool RelayState06 { get { return this[RelayNumber.Relay06]; } set { this[RelayNumber.Relay06] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 7
        /// </summary>
        public bool RelayState07 { get { return this[RelayNumber.Relay07]; } set { this[RelayNumber.Relay07] = value; } }
        /// <summary>
        /// Gets or sets the state of the relay 8
        /// </summary>
        public bool RelayState08 { get { return this[RelayNumber.Relay08]; } set { this[RelayNumber.Relay08] = value; } }

        #endregion

    }
}
