using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GodSharp.Extension;
using GodSharp.Util;

// ReSharper disable TooWideLocalVariableScope
// ReSharper disable ArrangeThisQualifier

namespace GodSharp
{
    /// <summary>
    /// GodSerialPort Util Class.
    /// </summary>
    /// <example>
    /// GodSerialPort serial= new GodSerialPort("COM1",9600);
    /// serial.UseDataReceived((sp,bytes)=>{});
    /// serial.Open();
    /// </example>
    public class GodSerialPort
    {
        #region Propertys
        /// <summary>
        /// Gets the baudrate dictionary.
        /// </summary>
        /// <value>The baudrate dictionary.</value>
        public static Dictionary<string, int> BaudRateDictionary { get; }

        /// <summary>
        /// Gets the parity dictionary.
        /// </summary>
        /// <value>The parity dictionary.</value>
        public static Dictionary<string, string> ParityDictionary { get; }

        /// <summary>
        /// Gets the stop bit dictionary.
        /// </summary>
        /// <value>The stop bit dictionary.</value>
        public static Dictionary<string, string> StopBitDictionary { get; }

        /// <summary>
        /// The method of execution that data has been received through a port represented by the SerialPort object.
        /// </summary>
        private Action<GodSerialPort,byte[]> onData;

        /// <summary>
        /// The method of execution that an error has occurred with a port represented by a SerialPort object.
        /// </summary>
        private Action<GodSerialPort,SerialError> onError;

        /// <summary>
        /// The method of execution that a non-data signal event has occurred on the port represented by the SerialPort object.
        /// </summary>
        private Action<GodSerialPort,SerialPinChange> onPinChange;

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
            BaudRateDictionary = new Dictionary<string, int>
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
                {"Mark", "m"},
                {"Space", "s"}
            };

            StopBitDictionary = new Dictionary<string, string>
            {
                {"None", "n"},
                {"1", "o"},
                {"1.5", "o"},
                {"2", "f"}
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        private GodSerialPort()
        {
            this.tryCountOfReceive = 10;
            this.parity = Parity.None;
            this.stopBits = StopBits.One;
            this.handshake = Handshake.None;
            this.serialPort = new SerialPort();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600)
            : this(portName,baudRate,8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, int dataBits = 8)
            : this()
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;

            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="parity">The string parity,default is none,Parity.None.
        /// <para>Parity.None：0|n|none</para>
        /// <para>Parity.Odd：1|o|odd</para>
        /// <para>Parity.Even：2|e|even</para>
        /// <para>Parity.Mark：3|m|mark</para>
        /// <para>Parity.Space：4|s|space</para>
        /// </param>
        /// <param name="stopBits">The string stopbits,default is one,StopBits.One.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// <para>StopBits.OnePointFive：3|1.5|f|of|opf</para>
        /// </param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, string parity = null, int dataBits = 8,
            string stopBits = null)
            : this(portName,baudRate,parity,dataBits,stopBits,null)
        {
            
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="parity">The int parity,default is 0,Parity.None.
        /// <para>Parity.None：0</para>
        /// <para>Parity.Odd：1</para>
        /// <para>Parity.Even：2</para>
        /// <para>Parity.Mark：3</para>
        /// <para>Parity.Space：4</para>
        /// </param>
        /// <param name="stopBits">The int stopbits,default is 1,StopBits.One.
        /// <para>StopBits.None：0</para>
        /// <para>StopBits.One：1</para>
        /// <para>StopBits.Two：2</para>
        /// <para>StopBits.OnePointFive：3</para>
        /// </param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, int parity = 0, int dataBits = 8,
            int stopBits = 0)
            : this(portName, baudRate, parity, dataBits, stopBits, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="parity">The int parity,default is Parity.None.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="stopBits">The int stopbits,default is StopBits.One.</param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8,StopBits stopBits = StopBits.None)
            : this(portName, baudRate, parity, dataBits, stopBits, Handshake.None)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="parity">The string parity,default is none,Parity.None.
        /// <para>Parity.None：0|n|none</para>
        /// <para>Parity.Odd：1|o|odd</para>
        /// <para>Parity.Even：2|e|even</para>
        /// <para>Parity.Mark：3|m|mark</para>
        /// <para>Parity.Space：4|s|space</para>
        /// </param>
        /// <param name="stopBits">The string stopbits,default is one,StopBits.One.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// <para>StopBits.OnePointFive：3|1.5|f|of|opf</para>
        /// </param>
        /// <param name="handshake">The string handshake,default is none,Handshake.None.
        /// <para>Handshake.None：0|n|none</para>
        /// <para>Handshake.XOnXOff：1|x|xoxo</para>
        /// <para>Handshake.RequestToSend：2|r|rst</para>
        /// <para>Handshake.RequestToSendXOnXOff：3|rx|rtsxx</para>
        /// </param>
        public GodSerialPort(string portName="COM1", int baudRate=9600, string parity = null, int dataBits=8,
            string stopBits=null, string handshake=null)
        : this()
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;

            if (!string.IsNullOrEmpty(parity))
            {
                this.parity = PortUtil.GetParity(parity); 
            }
            if (!string.IsNullOrEmpty(stopBits))
            {
                this.stopBits = PortUtil.GetStopBits(stopBits); 
            }
            if (!string.IsNullOrEmpty(handshake))
            {
                this.handshake = PortUtil.GetHandshake(handshake); 
            }

            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="parity">The int parity,default is 0,Parity.None.
        /// <para>Parity.None：0</para>
        /// <para>Parity.Odd：1</para>
        /// <para>Parity.Even：2</para>
        /// <para>Parity.Mark：3</para>
        /// <para>Parity.Space：4</para>
        /// </param>
        /// <param name="stopBits">The int stopbits,default is 1,StopBits.One.
        /// <para>StopBits.None：0</para>
        /// <para>StopBits.One：1</para>
        /// <para>StopBits.Two：2</para>
        /// <para>StopBits.OnePointFive：3</para>
        /// </param>
        /// <param name="handshake">The int handshake,default is 0,Handshake.None.
        /// <para>Handshake.None：0</para>
        /// <para>Handshake.XOnXOff：1</para>
        /// <para>Handshake.RequestToSend：2</para>
        /// <para>Handshake.RequestToSendXOnXOff：3</para>
        /// </param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, int parity = 0, int dataBits = 8,
             int stopBits = 1, int handshake = 0)
            : this()
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;

            if (parity>0)
            {
                this.parity = PortUtil.GetParity(parity);
            }

            this.stopBits = PortUtil.GetStopBits(stopBits);

            if (handshake > 0)
            {
                this.handshake = PortUtil.GetHandshake(handshake);
            }

            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GodSerialPort"/> class.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="parity">The int parity,default is Parity.None.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="stopBits">The int stopbits,default is StopBits.One.</param>
        /// <param name="handshake">The int handshake,default is Handshake.None.</param>
        public GodSerialPort(string portName = "COM1", int baudRate = 9600, Parity parity =  Parity.None, int dataBits = 8,
            StopBits stopBits = StopBits.None, Handshake handshake = Handshake.None)
            : this()
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.dataBits = dataBits;

            this.parity = parity;
            this.stopBits = stopBits;
            this.handshake = handshake;

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
                    this.onData?.Invoke(this, bytes);
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
                this.onError?.Invoke(this, e.EventType);
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
                this.onPinChange?.Invoke(this, e.EventType);
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
        public void UseDataReceived(Action<GodSerialPort, byte[]> action)
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
#if DEBUG
                Console.WriteLine("Init SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message); 
#endif
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
#if DEBUG
                    Console.WriteLine("the port is opened!"); 
#endif
                    return true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Open SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message); 
#endif
            }

            if (serialPort.IsOpen)
            {
#if DEBUG
                Console.WriteLine("successed to open the port!"); 
#endif
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
        public void OnError(Action<GodSerialPort, SerialError> action)
        {
            this.onError = action;
        }
        #endregion

        #region Set the method when pin changed
        /// <summary>
        /// Set the method when [pin changed].
        /// </summary>
        /// <param name="action">The action.</param>
        public void OnPinChange(Action<GodSerialPort, SerialPinChange> action)
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
#if DEBUG
                    Console.WriteLine("the port is already closed!"); 
#endif
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
#if DEBUG
                Console.WriteLine("Close SerialPort Exception:" + PortName + "\r\nMessage:" + ex.Message + "\r\nStackTrace:" + ex.StackTrace); 
#endif
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
                            str = bytes.ToHexString();
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
                ending = EndCharOfHex.HexToByte();
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

                byte[] bytes = str.HexToByte();

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
    }
}