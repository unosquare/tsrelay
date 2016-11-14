namespace Unosquare.TinySine.RelayModule
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    partial class RelayController
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
        public const string DefaultPassword = "123456";

        private SerialPort SerialPort;
        private bool IsDisposing;
        private static readonly ManualResetEventSlim SerialPortDone = new ManualResetEventSlim(true);
#if DEBUG
        private const bool IsDebugBuild = true;
#else
        private const bool IsDebugBuild = false;
#endif

        #endregion

        #region Helper Methods

        /// <summary>
        /// Encodes given 6 digit password into a 3 byte little endian array
        /// </summary>
        /// <param name="sixDigitPassword">The six digit password.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// sixDigitPassword
        /// or
        /// sixDigitPassword
        /// or
        /// sixDigitPassword
        /// </exception>
        private static byte[] EncodePassword(string sixDigitPassword)
        {
            if (string.IsNullOrWhiteSpace(sixDigitPassword) || sixDigitPassword.Length != 6)
                throw new ArgumentException(nameof(sixDigitPassword));

            if (sixDigitPassword.Any(c => char.IsDigit(c) == false))
                throw new ArgumentException(nameof(sixDigitPassword));

            uint passwordUint = 0;
            if (uint.TryParse(sixDigitPassword, out passwordUint) == false)
                throw new ArgumentException(nameof(sixDigitPassword));

            var passwordBytes = BitConverter.GetBytes(passwordUint);
            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(passwordBytes);
                passwordBytes = passwordBytes.Skip(1).Take(3).ToArray();
            }
            else
            {
                passwordBytes = passwordBytes.Skip(0).Take(3).ToArray();
            }


            return passwordBytes;
        }

        /// <summary>
        /// Initializes read-once properties of the relay board.
        /// </summary>
        private void Initialize()
        {
            BoardModel = GetBoardModel();
            BoardVersion = GetBoardVersion();
            RelayChannelCount = int.Parse(BitConverter.ToString(new byte[] { (byte)BoardModel })[0].ToString());
            FirmwareVersion = GetFirmwareVersion();
        }

        /// <summary>
        /// Clears read-once properties of the relay board
        /// </summary>
        private void Uninitialize()
        {
            Password = DefaultPassword;
            BoardModel = 0;
            BoardVersion = 0;
            RelayChannelCount = 0;
            FirmwareVersion = 0;
        }

        #endregion

        #region Serial Port Operations

        /// <summary>
        /// Opens device communication on the specified port name.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <param name="sixDigitPassword">The six digit password.</param>
        /// <exception cref="System.InvalidOperationException">Close</exception>
        /// <exception cref="System.Security.SecurityException">Invalid password or failed synchronization.</exception>
        /// <exception cref="InvalidOperationException">Device is already open. Call the Close method first.</exception>
        public void Open(string portName, string sixDigitPassword = DefaultPassword)
        {
            if (SerialPort != null)
                throw new InvalidOperationException($"Device is already open. Call the {nameof(Close)} method first.");

            try
            {
                SerialPort = new SerialPort(portName, DefaultBaudRate, Parity.None, 8, StopBits.One);
                SerialPort.ReadBufferSize = ReadBufferLength;
                SerialPort.Open();
                Password = sixDigitPassword;
                if (SynchronizeCommunication() == false)
                    throw new SecurityException("Invalid password or failed syncrhonization.");

                Initialize();
            }
            catch (Exception ex)
            {
                Close();
                throw ex;
            }
        }

        /// <summary>
        /// Writes the specified payload synchronously.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public void Write(byte[] payload)
        {
            WriteAsync(payload).GetAwaiter().GetResult();
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
                //Log.Debug($"TX: {BitConverter.ToString(payload).Replace("-", "")}");
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
        private int FlushReadBuffer()
        {
            return FlushReadBufferAsync().GetAwaiter().GetResult();
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
        /// Reads the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="expectedBytes">The expected bytes.</param>
        /// <returns></returns>
        public byte[] Read(TimeSpan timeout, int expectedBytes)
        {
            return ReadAsync(timeout, expectedBytes).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads data from the serial port asynchronously with the default timeout and no expected packet size
        /// </summary>
        /// <returns></returns>
        public byte[] Read()
        {
            return Read(DefaultTimeout, 0);
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

                while (SerialPort.IsOpen && (expectedBytes <= 0 || response.Count < expectedBytes))
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
                        if (expectedBytes <= 0 && response.Count > 0)
                            break;

                        if (IsDebugBuild)
                        {
                            Log.Error($"RX: Did not receive enough bytes. Received: {response.Count}  Expected: {expectedBytes}");
                            Log.Error($"RX: {BitConverter.ToString(response.ToArray()).Replace("-", " ")}");
                        }

                        return null;
                    }

                }

                var responseBytes = response.ToArray();
                //Log.Info($"RX: {BitConverter.ToString(responseBytes).Replace("-", "")}");
                return responseBytes;
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
                SerialPort.Dispose();
                SerialPort = null;
                Uninitialize();
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
