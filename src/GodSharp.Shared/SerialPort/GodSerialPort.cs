using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
// ReSharper disable TooWideLocalVariableScope
// ReSharper disable ArrangeThisQualifier

namespace GodSharp
{
    /// <summary>
    /// GodSerialPort Util Class.
    /// </summary>
    /// <example>
    /// GodSerialPort serial= new GodSerialPort("COM1",9600);
    /// serial.UseDataReceived((bytes)=>{});
    /// serial.Open();
    /// </example>
    public class GodSerialPort
    {
        #region Propertys
        /// <summary>
        /// Gets the baudrate dictionary.
        /// </summary>
        /// <value>The baudrate dictionary.</value>
        public static Dictionary<string, uint> BaudRateDictionary { get; }

        /// <summary>
        /// Gets the parity dictionary.
        /// </summary>
        /// <value>The parity dictionary.</value>
        public static Dictionary<string, string> ParityDictionary { get; }

        /// <summary>
        /// Gets the stop bit dictionary.
        /// </summary>
        /// <value>The stop bit dictionary.</value>
        public static Dictionary<string, double> StopBitDictionary { get; }

        /// <summary>
        /// The method of execution that data has been received through a port represented by the SerialPort object.
        /// </summary>
        private Action<byte[]> onData;

        /// <summary>
        /// The method of execution that an error has occurred with a port represented by a SerialPort object.
        /// </summary>
        private Action<SerialError> onError;

        /// <summary>
        /// The method of execution that a non-data signal event has occurred on the port represented by the SerialPort object.
        /// </summary>
        private Action<SerialPinChange> onPinChange;

        /// <summary>
        /// Gets or sets the data format.
        /// </summary>
        /// <value>The data format.</value>
        public SerialPortDataFormat DataFormat { get; set; } = SerialPortDataFormat.Hex;
        
        private int tryCountOfReceive;
        /// <summary>
        /// Gets or sets the try count of receive.
        /// </summary>
        /// <value>The try count of receive,default is 10.</value>
        public int TryCountOfReceive
        {
            get => this.tryCountOfReceive;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(TryCountOfReceive), "TryCountOfReceive must be equal or greater than 1.");
                }
                tryCountOfReceive = value;
            }
        }


        /// <summary>
        /// Gets or sets the try sleep time of receive,unit is ms.
        /// </summary>
        /// <value>The try sleep time of receive,default is 10.</value>
        public int TrySleepTimeOfReceive { get; set; } = 10;

        /// <summary>
        /// Gets or sets the end character.
        /// </summary>
        /// <value>The end character.</value>
        public string EndCharOfHex { get; set; } = null;

        /// <summary>
        /// The serial port
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// SerialPort对象
        /// </summary>
        public SerialPort SerialPort => serialPort;

        /// <summary>
        /// Determines whether this instance is open.
        /// </summary>
        /// <returns><c>true</c> if this serialport is open; otherwise, <c>false</c>.</returns>
        public bool IsOpen => serialPort != null && serialPort.IsOpen;

        string portName;

        /// <summary>
        /// Gets or sets the name of the port.
        /// </summary>
        /// <value>The name of the port.</value>
        public string PortName
        {
            get => serialPort.PortName;
            set => serialPort.PortName = value;
        }

        int baudRate;

        /// <summary>
        /// Gets or sets the baudrate.
        /// </summary>
        /// <value>The baudrate.</value>
        public int BaudRate
        {
            get => serialPort.BaudRate;
            set => serialPort.BaudRate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [break state].
        /// </summary>
        /// <value><c>true</c> if [break state]; otherwise, <c>false</c>.</value>
        public bool BreakState
        {
            get => serialPort.BreakState;
            set => serialPort.BreakState = value;
        }

        int dataBits;
        /// <summary>
        /// Gets or sets the databits.
        /// </summary>
        /// <value>The databits.</value>
        public int DataBits
        {
            get => serialPort.DataBits;
            set => serialPort.DataBits = value;
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get => serialPort.Encoding;
            set => serialPort.Encoding = value;
        }

        Handshake handshake;

        /// <summary>
        /// Gets or sets the handshake.
        /// </summary>
        /// <value>The handshake.</value>
        public Handshake Handshake
        {
            get => serialPort.Handshake;
            set => serialPort.Handshake = value;
        }

        Parity parity;

        /// <summary>
        /// Gets or sets the parity.
        /// </summary>
        /// <value>The parity.</value>
        public Parity Parity
        {
            get => serialPort.Parity;
            set => serialPort.Parity = value;
        }

        StopBits stopBits;
        /// <summary>
        /// Gets or sets the stopbits.
        /// </summary>
        /// <value>The stopbits.</value>
        public StopBits StopBits
        {
            get => serialPort.StopBits;
            set => serialPort.StopBits = value;
        }

        /// <summary>
        /// Gets or sets the read timeout.
        /// </summary>
        /// <value>The read timeout.</value>
        public int ReadTimeout
        {
            get => serialPort.ReadTimeout;
            set => serialPort.ReadTimeout = value;
        }

        /// <summary>
        /// Gets or sets the write timeout.
        /// </summary>
        /// <value>The write timeout.</value>
        public int WriteTimeout
        {
            get => serialPort.WriteTimeout;
            set => serialPort.WriteTimeout = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [DTR enable].
        /// </summary>
        /// <value><c>true</c> if [DTR enable]; otherwise, <c>false</c>.</value>
        public bool DtrEnable
        {
            get => serialPort.DtrEnable;
            set => serialPort.DtrEnable = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [RTS enable].
        /// </summary>
        /// <value><c>true</c> if [RTS enable]; otherwise, <c>false</c>.</value>
        public bool RtsEnable
        {
            get => serialPort.RtsEnable;
            set => serialPort.RtsEnable = value;
        }

        /// <summary>
        /// Gets a value indicating whether [CTS holding].
        /// </summary>
        /// <value><c>true</c> if [CTS holding]; otherwise, <c>false</c>.</value>
        public bool CtsHolding => serialPort.CtsHolding;

        /// <summary>
        /// Gets a value indicating whether [DSR holding].
        /// </summary>
        /// <value><c>true</c> if [DSR holding]; otherwise, <c>false</c>.</value>
        public bool DsrHolding => serialPort.DsrHolding;

        /// <summary>
        /// Gets a value indicating whether [cd holding].
        /// </summary>
        /// <value><c>true</c> if [cd holding]; otherwise, <c>false</c>.</value>
        public bool CdHolding => serialPort.CDHolding;

        /// <summary>
        /// Gets or sets a value indicating whether [discard null].
        /// </summary>
        /// <value><c>true</c> if [discard null]; otherwise, <c>false</c>.</value>
        public bool DiscardNull
        {
            get => serialPort.DiscardNull;
            set => serialPort.DiscardNull = value;
        }

        /// <summary>
        /// Gets or sets the size of the read buffer.
        /// </summary>
        /// <value>The size of the read buffer.</value>
        public int ReadBufferSize
        {
            get => serialPort.ReadBufferSize;
            set => serialPort.ReadBufferSize = value;
        }

        /// <summary>
        /// Gets or sets the parity replace.
        /// </summary>
        /// <value>The parity replace.</value>
        public byte ParityReplace
        {
            get => serialPort.ParityReplace;
            set => serialPort.ParityReplace = value;
        }

        /// <summary>
        /// Gets or sets the received bytes threshold.
        /// </summary>
        /// <value>The received bytes threshold.</value>
        public int ReceivedBytesThreshold
        {
            get => serialPort.ReceivedBytesThreshold;
            set => serialPort.ReceivedBytesThreshold = value;
        }

        /// <summary>
        /// Gets or sets the size of the write buffer.
        /// </summary>
        /// <value>The size of the write buffer.</value>
        public int WriteBufferSize
        {
            get => serialPort.WriteBufferSize;
            set => serialPort.WriteBufferSize = value;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="GodSerialPort"/> class.
        /// </summary>
        static GodSerialPort()
        {
            BaudRateDictionary = new Dictionary<string, uint>
            {
                {"110", 110},
                {"300", 300},
                {"600", 600},
                {"1200", 1200},
                {"2400", 2400},
                {"4800", 4800},
                {"9600", 9600},
                {"14400", 14400},
                {"19200", 19200},
                {"38400", 38400},
                {"56000", 56000},
                {"57600", 57600},
                {"115200", 115200}
            };

            ParityDictionary = new Dictionary<string, string>
            {
                {"None", "n"},
                {"Odd", "o"},
                {"Even", "e"},
                {"Space", "s"},
                {"Mark", "m"}
            };

            StopBitDictionary = new Dictionary<string, double>
            {
                {"None", 0},
                {"1", 1},
                {"1.5", 1.5},
                {"2", 2}
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        private GodSerialPort()
        {
            this.tryCountOfReceive = 10;
            this.handshake = Handshake.None;
            this.parity = Parity.None;
            this.stopBits = StopBits.One;
            this.serialPort = new SerialPort();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="parity">The parity,default is none,Parity.None.
        /// <para>Parity.Space：0|s|space</para>
        /// <para>Parity.Mark：1|m|mark</para>
        /// <para>Parity.Even：2|e|even</para>
        /// <para>Parity.Odd：3|o|odd</para>
        /// <para>Parity.None：4|n|none</para>
        /// </param>
        /// <param name="stopBits">The stopbits,default is none,StopBits.None.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.OnePointFive：3|opf|of|f</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// </param>
        /// <param name="handshake">The handshake,default is none,Handshake.None.
        /// <para>Handshake.None：0|n|none</para>
        /// <para>Handshake.RequestToSend：1|r|rst</para>
        /// <para>Handshake.RequestToSendXOnXOff：2|rtsxx|rsxx|rtsx|rsx|rx</para>
        /// <para>Handshake.XOnXOff：3|x|xx</para>
        /// </param>
        public GodSerialPort(string portName="COM1", int baudRate=9600, int dataBits=8,
            string parity=null, string stopBits=null, string handshake=null)
            : this()
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;

            if (!string.IsNullOrEmpty(parity))
            {
                this.parity = GetParity(parity); 
            }
            if (!string.IsNullOrEmpty(stopBits))
            {
                this.stopBits = GetStopBits(stopBits); 
            }
            if (!string.IsNullOrEmpty(handshake))
            {
                this.handshake = GetHandshake(handshake); 
            }

            this.Init();
        }

        #endregion

        #region DataReceived event
        /// <summary>
        /// Handles the DataReceived event of the SerialPort.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    byte[] bytes = this.TryRead();
                    this.onData?.Invoke(bytes);
                    this.DiscardOutBuffer();
                    this.DiscardInBuffer();
                }
                else
                {
                    serialPort.Open();
                    SerialPort_DataReceived(sender, e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region ErrorReceived event
        /// <summary>
        /// Handles the ErrorReceived event of the SerialPort.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SerialErrorReceivedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                this.onError?.Invoke(e.EventType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region PinChanged event
        /// <summary>
        /// Handles the PinChanged event of the SerialPort.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SerialPinChangedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            try
            {
                this.onPinChange?.Invoke(e.EventType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Use DataReceived event with data received action.
        /// </summary>
        /// <param name="action">The action which process data.</param>
        public void UseDataReceived(Action<byte[]> action)
        {
            onData = action;
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        #region Initializes the SerialPort method
        /// <summary>
        /// Initializes the <see cref="SerialPort"/> with the action of data receive.
        /// </summary>
        public void Init()
        {
            try
            {
                portName = portName?.Trim() ?? throw new ArgumentNullException(nameof(portName));

                Regex regx = new Regex("^COM([0-9]{1,})$", RegexOptions.IgnoreCase);
                if (!regx.IsMatch(portName))
                {
                    throw new ArgumentException("port name must be COMxx.", nameof(portName));
                }

                Match match = regx.Match(portName);
                bool b = int.TryParse(match.Groups[1].Value, out int v);

                if (!b || v < 1)
                {
                    throw new ArgumentException("port name must be COMxx.", nameof(portName));
                }

                serialPort.PortName = portName;
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = dataBits;
                serialPort.Handshake = handshake;
                serialPort.Parity = parity;
                serialPort.StopBits = stopBits;
                serialPort.PinChanged += SerialPort_PinChanged;
                serialPort.ErrorReceived += SerialPort_ErrorReceived;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Init SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region Open SerialPort method
        /// <summary>
        /// Open the <see cref="SerialPort"/>.
        /// </summary>
        /// <returns><c>true</c> if opend, <c>false</c> otherwise.</returns>
        public bool Open()
        {

            bool rst = false;
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                else
                {
                    Console.WriteLine("the port is opened!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message);
            }

            if (serialPort.IsOpen)
            {
                Console.WriteLine("successed to open the port!");
                rst = true;
            }
            return rst;
        }

        #endregion

        #region Set the method when error
        /// <summary>
        /// Set the method when [error].
        /// </summary>
        /// <param name="action">The action.</param>
        public void OnError(Action<SerialError> action)
        {
            this.onError = action;
        }
        #endregion

        #region Set the method when pin changed
        /// <summary>
        /// Set the method when [pin changed].
        /// </summary>
        /// <param name="action">The action.</param>
        public void OnPinChange(Action<SerialPinChange> action)
        {
            this.onPinChange = action;
        }
        #endregion

        #region Close SerialPort method
        /// <summary>
        /// Close the <see cref="SerialPort"/>.
        /// </summary>
        /// <returns><c>true</c> if closed, <c>false</c> otherwise.</returns>
        public bool Close()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    Console.WriteLine("the port is already closed!");
                    return true;
                }
                else
                {
                    serialPort.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Close SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message +
                                  "\r\nStackTrace:" + ex.StackTrace);
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region Reads method
        /// <summary>
        /// Reads data from the input buffer.
        /// </summary>
        /// <returns>System.String,hex or ascii format.</returns>
        public string ReadString()
        {
            try
            {
                string str = null;

                byte[] bytes = this.TryRead();

                if (bytes != null && bytes.Length > 0)
                {
                    switch (DataFormat)
                    {
                        case SerialPortDataFormat.Char:
                            str = serialPort.Encoding.GetString(bytes);
                            break;
                        case SerialPortDataFormat.Hex:
                            str = ByteToHex(bytes);
                            break;
                    }
                }

                return str;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Reads data from the input buffer.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] Read()
        {
            try
            {
                return this.TryRead();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region //Try Read Data
        /// <summary>
        /// Try Read Data
        /// </summary>
        /// <returns>The byte array.</returns>
        private byte[] TryRead()
        {
            int tryCount = 0;
            int endingLength = 0;
            bool found = false;
            List<byte> list = new List<byte>();
            byte[] bytes;
            byte[] ending = null;
            byte[] currentEnding;

            if (EndCharOfHex != null)
            {
                ending = HexToByte(EndCharOfHex);
                endingLength = ending.Length;
            }

            int dataLength;
            while ((serialPort.BytesToRead > 0 || !found) && tryCount < tryCountOfReceive)
            {
                dataLength = serialPort.BytesToRead < serialPort.ReadBufferSize
                    ? serialPort.BytesToRead
                    : serialPort.ReadBufferSize;
                bytes = new byte[dataLength];
                serialPort.Read(bytes, 0, bytes.Length);
                list.AddRange(bytes);

                if (ending != null && endingLength > 0)
                {
                    currentEnding = new byte[endingLength];

                    if (bytes.Length >= endingLength)
                    {
                        Buffer.BlockCopy(bytes, bytes.Length - endingLength, currentEnding, 0, endingLength);
                    }
                    else if (list.ToArray().Length >= endingLength)
                    {
                        byte[] temp = list.ToArray();
                        Buffer.BlockCopy(temp, temp.Length - endingLength, currentEnding, 0, endingLength);
                    }
                    else
                    {
                        continue;
                    }

                    found = ending.Length > 0 && currentEnding.SequenceEqual(ending);
                }

                if (TrySleepTimeOfReceive>0)
                {
                    Thread.Sleep(TrySleepTimeOfReceive); 
                }

                tryCount++;
            }

            return list.Count > 0 ? list.ToArray() : null;
        } 
        #endregion

        #region Writes method
        /// <summary>
        /// Writes the specified hex string.
        /// </summary>
        /// <param name="str">The hex string.</param>
        public void WriteHexString(string str)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }

                byte[] bytes = HexToByte(str);

                serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified ascii string.
        /// </summary>
        /// <param name="str">The ascii string.</param>
        public void WriteAsciiString(string str)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }

                byte[] bytes = serialPort.Encoding.GetBytes(str);

                serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the specified string.
        /// </summary>
        /// <param name="str"></param>
        // ReSharper disable once UnusedMember.Local
        private void Write(string str)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                serialPort.Write(str);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Writes the byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the byte array with offset.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="offset">The number of offset.</param>
        /// <param name="count">The length of write.</param>
        public void Write(byte[] bytes, int offset, int count)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
                serialPort.Write(bytes, offset, count);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region SerialPort other method
        /// <summary>
        /// Discards the input buffer.
        /// </summary>
        public void DiscardInBuffer() => serialPort.DiscardInBuffer();

        /// <summary>
        /// Discards the output buffer.
        /// </summary>
        public void DiscardOutBuffer() => serialPort.DiscardOutBuffer();

        /// <summary>
        /// Get an array of serialport name for current computer.
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames() => SerialPort.GetPortNames();
        #endregion

        #region Gets the parity method
        /// <summary>
        /// Gets the parity by string.
        /// </summary>
        /// <param name="parityVal">The parity.
        /// Parity.Space：0|s|space
        /// Parity.Mark：1|m|mark
        /// Parity.Even：2|e|even
        /// Parity.Odd：3|o|odd
        /// Parity.None：4|n|none
        /// </param>
        /// <returns>Parity.</returns>
        private Parity GetParity(string parityVal)
        {
            Parity patityBit;
            switch (parityVal.ToLower())
            {
                case "0":
                case "s":
                case "space":
                    patityBit = Parity.Space;
                    break;
                case "1":
                case "m":
                case "mark":
                    patityBit = Parity.Mark;
                    break;
                case "2":
                case "e":
                case "even":
                    patityBit = Parity.Even;
                    break;
                case "3":
                case "o":
                case "odd":
                    patityBit = Parity.Odd;
                    break;
                case "4":
                case "n":
                case "none":
                    patityBit = Parity.None;
                    break;
                default:
                    patityBit = Parity.None;
                    break;
            }
            return patityBit;
        }

        #endregion

        #region Gets the stopbits method
        /// <summary>
        /// Gets the stopbits by string.
        /// </summary>
        /// <param name="stopBitsVal">The stop bits.
        /// StopBits.None：0|n|none
        /// StopBits.One：1|o|one
        /// StopBits.OnePointFive：3|opf|of|f
        /// StopBits.Two：2|t|two
        /// </param>
        /// <returns>StopBits.</returns>
        private StopBits GetStopBits(string stopBitsVal)
        {
            StopBits stopBit;
            switch (stopBitsVal.ToLower())
            {
                case "0":
                case "n":
                case "none":
                    stopBit = StopBits.None;
                    break;
                case "1":
                case "o":
                case "one":
                    stopBit = StopBits.One;
                    break;
                case "1.5":
                case "opf":
                case "of":
                case "f":
                case "onepointfive":
                    stopBit = StopBits.OnePointFive;
                    break;
                case "2":
                case "t":
                case "two":
                    stopBit = StopBits.Two;
                    break;
                default:
                    stopBit = StopBits.None;
                    break;
            }
            return stopBit;
        }

        #endregion

        #region Gets the handshake by string
        /// <summary>
        /// Gets the handshake by string.
        /// </summary>
        /// <param name="shake">The shake.
        /// Handshake.None：0|n|none
        /// Handshake.RequestToSend：1|r|rst
        /// Handshake.RequestToSendXOnXOff：2|rtsxx|rsxx|rtsx|rsx|rx
        /// Handshake.XOnXOff：3|x|xx
        /// </param>
        /// <returns>Handshake.</returns>
        private Handshake GetHandshake(string shake)
        {
            Handshake handShake;
            switch (shake.ToLower())
            {
                case "0":
                case "n":
                case "none":
                    handShake = Handshake.None;
                    break;
                case "1":
                case "r":
                case "rs":
                case "rts":
                    handShake = Handshake.RequestToSend;
                    break;
                case "2":
                case "rtsxx":
                case "rsxx":
                case "rtsx":
                case "rsx":
                case "rx":
                    handShake = Handshake.RequestToSendXOnXOff;
                    break;
                case "3":
                case "x":
                case "xx":
                    handShake = Handshake.XOnXOff;
                    break;
                default:
                    handShake = Handshake.None;
                    break;
            }
            return handShake;
        }
        #endregion

        #region Convert method
        /// <summary>
        /// Hexadecimal string to an byte array.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>An byte array.</returns>
        public static byte[] HexToByte(string hex)
        {
            // remove space
            hex = hex.Replace(" ", "");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// Bytes to hexadecimal.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.String.</returns>
        public static string ByteToHex(byte[] bytes)
        {
            string[] array = { };

            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    array[i] = bytes[i].ToString("X2");
                }
            }

            return string.Join(" ", array);
        }
        #endregion
    }
}