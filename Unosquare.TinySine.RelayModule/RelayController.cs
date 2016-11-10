using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unosquare.TinySine.RelayModule
{
    /// <summary>
    /// Represents a TinySine TOSR1 Relay Controller providing up to 8 relay channels
    /// Reference: http://www.tinyosshop.com/datasheet/TOSR14x%20User%20Manual.pdf
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class RelayController : IDisposable
    {
        #region Private Declarations

        /// <summary>
        /// The read buffer length of the serial port
        /// </summary>
        private const int ReadBufferLength = 1024; // 1kb

        /// <summary>
        /// The default baud rate
        /// </summary>
        private const int DefaultBaudRate = 9600;

        /// <summary>
        /// The default timeout
        /// </summary>
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The default password
        /// </summary>
        private const string DefaultPassword = "123456";


        private SerialPort SerialPort;
        private bool IsDisposing;
        private static readonly ManualResetEventSlim SerialPortDone = new ManualResetEventSlim(true);
        private string m_Password = DefaultPassword;

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="RelayController"/> class.
        /// </summary>
        public RelayController()
        {
            IsAuthenticated = false;
        }

        #region Methods

        private void InitializeProperties()
        {
            Password = DefaultPassword;
        }

        private bool VerifyPassword(string sixDigitPassword)
        {
            throw new NotImplementedException();
        }

        private void SetPassword(string sixDigitPassword)
        {
            throw new NotImplementedException();
        }

        private bool GetRelayState(RelayNumber relayNumber)
        {
            throw new NotImplementedException();
        }

        private bool SetRelayState(RelayNumber relayNumber, bool state)
        {
            throw new NotImplementedException();
        }

        private RelayOperatingMode GetRelayOperatingMode()
        {
            throw new NotImplementedException();
        }

        private void SetRelayOperatingMode(RelayOperatingMode operatingMode)
        {
            throw new NotImplementedException();
        }

        private decimal GetVoltage()
        {
            throw new NotImplementedException();
        }

        private decimal GetTemperature()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Read-Once Properties

        /// <summary>
        /// Gets the board model.
        /// </summary>
        public string BoardModel { get; private set; }

        /// <summary>
        /// Gets the board version.
        /// </summary>
        public string BoardVersion { get; private set; }

        /// <summary>
        /// Gets the relay channel count.
        /// </summary>
        public int RelayChannelCount { get; private set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return m_Password; }
            set
            {
                // TODO: Make sure password is EXACTLY 6 digits.

                if (IsAuthenticated)
                {
                    // We are already authenticated. We need to change the password and verify it.
                    SetPassword(value);
                    m_Password = value;
                    IsAuthenticated = VerifyPassword(m_Password);
                }
                else
                {
                    // We are not yet authenticated. Set the IsAuthenticated state variable accordingly
                    m_Password = value;
                    IsAuthenticated = VerifyPassword(m_Password);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a correct Password has been set
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        #endregion

        #region Operating Status Properties

        /// <summary>
        /// Gets the temperature.
        /// </summary>
        public decimal Temperature { get { return GetVoltage(); } }

        /// <summary>
        /// Gets the working voltage.
        /// </summary>
        public decimal WorkingVoltage { get { return GetTemperature(); } }

        /// <summary>
        /// Gets or sets the relay operating mode.
        /// </summary>
        public RelayOperatingMode RelayOperatingMode { get { return GetRelayOperatingMode(); } set { SetRelayOperatingMode(value); } }

        #endregion

        #region Relay State Properties

        public bool this[RelayNumber relayNumber]
        {
            get { return GetRelayState(relayNumber); }
            set { SetRelayState(relayNumber, value); }
        }

        public bool RelayState01 { get { return this[RelayNumber.Relay01]; } set { this[RelayNumber.Relay01] = value; } }
        public bool RelayState02 { get { return this[RelayNumber.Relay02]; } set { this[RelayNumber.Relay02] = value; } }
        public bool RelayState03 { get { return this[RelayNumber.Relay03]; } set { this[RelayNumber.Relay03] = value; } }
        public bool RelayState04 { get { return this[RelayNumber.Relay04]; } set { this[RelayNumber.Relay04] = value; } }
        public bool RelayState05 { get { return this[RelayNumber.Relay05]; } set { this[RelayNumber.Relay05] = value; } }
        public bool RelayState06 { get { return this[RelayNumber.Relay06]; } set { this[RelayNumber.Relay06] = value; } }
        public bool RelayState07 { get { return this[RelayNumber.Relay07]; } set { this[RelayNumber.Relay07] = value; } }
        public bool RelayState08 { get { return this[RelayNumber.Relay08]; } set { this[RelayNumber.Relay08] = value; } }

        #endregion

        #region Serial Port Operations

        /// <summary>
        /// Opens device communication on the specified port name.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <exception cref="InvalidOperationException">Device is already open. Call the Close method first.</exception>
        public void Open(string portName)
        {
            if (SerialPort != null)
                throw new InvalidOperationException($"Device is already open. Call the {nameof(Close)} method first.");

            SerialPortDone.Wait();
            SerialPortDone.Reset();

            try
            {
                SerialPort = new SerialPort(portName, DefaultBaudRate, Parity.None, 8, StopBits.One);
                SerialPort.ReadBufferSize = ReadBufferLength;
                SerialPort.Open();
                InitializeProperties();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SerialPortDone.Set();
            }
        }

        /// <summary>
        /// Writes data to the serial port asynchronously
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public async Task WriteAsync(byte[] payload)
        {
            if (SerialPort == null || SerialPort.IsOpen == false)
                throw new InvalidOperationException($"Call the {nameof(Open)} method befor attempting communication");

            SerialPortDone.Wait();
            SerialPortDone.Reset();

            try
            {
                await SerialPort.BaseStream.WriteAsync(payload, 0, payload.Length);
                await SerialPort.BaseStream.FlushAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SerialPortDone.Set();
            }
        }

        /// <summary>
        /// Reads data from the serial port asynchronously with the default timeout and no expected packet size
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReadAsync()
        {
            return await ReadAsync(DefaultTimeout, 0);
        }

        /// <summary>
        /// Flushes the serial port read data discarding all bytes in the read buffer
        /// </summary>
        /// <returns></returns>
        private async Task<int> FlushReadBufferAsync()
        {
            if (SerialPort == null || SerialPort.IsOpen == false)
                return 0;

            SerialPortDone.Wait();
            SerialPortDone.Reset();

            try
            {
                var count = 0;
                var buffer = new byte[1024];
                while (SerialPort.BytesToRead > 0)
                {
                    count += await SerialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                }

                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SerialPortDone.Set();
            }



        }

        /// <summary>
        /// Reads bytes from the serial port.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="expectedBytes">The expected bytes. Use a value of zero or less to read until no more bytes are available</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Open</exception>
        /// <exception cref="System.InvalidOperationException">Open</exception>
        public async Task<byte[]> ReadAsync(TimeSpan timeout, int expectedBytes)
        {
            if (SerialPort == null || SerialPort.IsOpen == false)
                throw new InvalidOperationException($"Call the {nameof(Open)} method befor attempting communication");

            SerialPortDone.Wait();
            SerialPortDone.Reset();

            try
            {
                var response = new List<byte>(1024);
                var remainingBytes = expectedBytes;
                var startTime = DateTime.UtcNow;

                var buffer = new byte[SerialPort.ReadBufferSize];

                while (SerialPort.IsOpen && (expectedBytes <= 0 || response.Count < expectedBytes)
                {
                    if (SerialPort.BytesToRead > 0)
                    {
                        var readBytes = await SerialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                        if (readBytes > 0)
                        {
                            response.AddRange(buffer.Skip(0).Take(readBytes));
                            remainingBytes = expectedBytes - response.Count;
                            startTime = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        await Task.Delay(10);
                    }

                    if (DateTime.UtcNow.Subtract(startTime) > timeout)
                    {
                        Log.Error($"RX: Did not receive enough bytes. Received: {response.Count}  Expected: {expectedBytes}");
                        Log.Error($"RX: {BitConverter.ToString(response.ToArray()).Replace("-", " ")}");
                        return null;
                    }

                }

                return response.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SerialPortDone.Set();
            }


        }

        /// <summary>
        /// Closes serial port communication -- if open.
        /// </summary>
        private void Close()
        {
            if (SerialPort == null)
                return;

            try
            {
                if (SerialPort.IsOpen)
                    SerialPort.Close();
            }
            finally
            {
                IsAuthenticated = false;
                m_Password = DefaultPassword;
                SerialPort.Dispose();
                SerialPort = null;

            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposing) return;
            IsDisposing = true;
            Close();
        }

        #endregion

    }
}
