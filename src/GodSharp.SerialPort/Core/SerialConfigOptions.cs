using System;
using System.IO.Ports;

namespace GodSharp.SerialPort
{
    /// <summary>
    /// SerialConfigOptions
    /// </summary>
    public class SerialConfigOptions
    {
        /// <summary>
        /// Gets or sets the name of the port.
        /// </summary>
        /// <value>
        /// The name of the port.
        /// </value>
        public string PortName { get; set; }

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        /// <value>
        /// The baud rate.
        /// </value>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// Gets or sets the data bits.
        /// </summary>
        /// <value>
        /// The data bits.
        /// </value>
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// Gets or sets the stop bits.
        /// </summary>
        /// <value>
        /// The stop bits.
        /// </value>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// Gets or sets the parity bits.
        /// </summary>
        /// <value>
        /// The parity bits.
        /// </value>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// Gets or sets the handshake.
        /// </summary>
        /// <value>
        /// The handshake.
        /// </value>
        public Handshake Handshake { get; set; } = Handshake.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConfigOptions"/> class.
        /// </summary>
        public SerialConfigOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConfigOptions"/> class.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <param name="baudRate">The baud rate.</param>
        /// <param name="dataBits">The data bits.</param>
        /// <param name="stopBits">The stop bits.</param>
        /// <param name="parity">The parity.</param>
        /// <param name="handshake">The handshake.</param>
        /// <exception cref="ArgumentNullException">portName</exception>
        public SerialConfigOptions(string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None, Handshake handshake = Handshake.None)
        {
            PortName = portName ?? throw new ArgumentNullException(nameof(portName));
            if (dataBits < 7 || dataBits > 8) throw new ArgumentNullException(nameof(dataBits), "only 7,8");
            BaudRate = baudRate;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            Handshake = handshake;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConfigOptions"/> class.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="stopBits">The int stopbits,default is 1,StopBits.One.
        /// <para>StopBits.None：0</para>
        /// <para>StopBits.One：1</para>
        /// <para>StopBits.Two：2</para>
        /// <para>StopBits.OnePointFive：3</para>
        /// </param>
        /// <param name="parity">The int parity,default is 0,Parity.None.
        /// <para>Parity.None：0</para>
        /// <para>Parity.Odd：1</para>
        /// <para>Parity.Even：2</para>
        /// <para>Parity.Mark：3</para>
        /// <para>Parity.Space：4</para>
        /// </param>
        /// <param name="handshake">The int handshake,default is 0,Handshake.None.
        /// <para>Handshake.None：0</para>
        /// <para>Handshake.XOnXOff：1</para>
        /// <para>Handshake.RequestToSend：2</para>
        /// <para>Handshake.RequestToSendXOnXOff：3</para>
        /// </param>
        public SerialConfigOptions(string portName, int baudRate = 9600, int dataBits = 8, int stopBits = 1, int parity = 0, int handshake = 0) : this(portName, baudRate, dataBits, GetStopBits(stopBits), GetParity(parity), GetHandshake(handshake)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialConfigOptions"/> class.
        /// </summary>
        /// <param name="portName">Name of the port.</param>
        /// <param name="baudRate">The baudrate,default is 9600.</param>
        /// <param name="dataBits">The databits,default is 8.</param>
        /// <param name="stopBits">The string stopbits,default is one,StopBits.One.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// <para>StopBits.OnePointFive：3|1.5|f|of|opf|onepointfive</para>
        /// </param>
        /// <param name="parity">The string parity,default is none,Parity.None.
        /// <para>Parity.None：0|n|none</para>
        /// <para>Parity.Odd：1|o|odd</para>
        /// <para>Parity.Even：2|e|even</para>
        /// <para>Parity.Mark：3|m|mark</para>
        /// <para>Parity.Space：4|s|space</para>
        /// </param>
        /// <param name="handshake">The string handshake,default is none,Handshake.None.
        /// <para>Handshake.None：0|n|none</para>
        /// <para>Handshake.XOnXOff：1|x|xoxo</para>
        /// <para>Handshake.RequestToSend：2|r|rst</para>
        /// <para>Handshake.RequestToSendXOnXOff：3|rx|rtsxx</para>
        /// </param>
        public SerialConfigOptions(string portName, int baudRate = 9600, int dataBits = 8, string stopBits = "one", string parity = "none", string handshake = "none") : this(portName, baudRate, dataBits, GetStopBits(stopBits), GetParity(parity), GetHandshake(handshake)) { }
        
        #region Gets the parity method
        /// <summary>
        /// Gets the parity by string.
        /// </summary>
        /// <param name="parityVal">The string parity value.
        /// <para>Parity.None：0|n|none</para>
        /// <para>Parity.Odd：1|o|odd</para>
        /// <para>Parity.Even：2|e|even</para>
        /// <para>Parity.Mark：3|m|mark</para>
        /// <para>Parity.Space：4|s|space</para>
        /// </param>
        /// <returns>The <see cref="Parity"/>.</returns>
        public static Parity GetParity(string parityVal)
        {
            Parity patityBit;
            switch (parityVal?.ToLower())
            {
                case "0":
                case "n":
                case "none":
                    patityBit = Parity.None;
                    break;
                case "1":
                case "o":
                case "odd":
                    patityBit = Parity.Odd;
                    break;
                case "2":
                case "e":
                case "even":
                    patityBit = Parity.Even;
                    break;
                case "3":
                case "m":
                case "mark":
                    patityBit = Parity.Mark;
                    break;
                case "4":
                case "s":
                case "space":
                    patityBit = Parity.Space;
                    break;
                default:
                    patityBit = Parity.None;
                    break;
            }
            return patityBit;
        }

        /// <summary>
        /// Gets the parity by int.
        /// </summary>
        /// <param name="parityVal">The int parity value.
        /// <para>Parity.None：0</para>
        /// <para>Parity.Odd：1</para>
        /// <para>Parity.Even：2</para>
        /// <para>Parity.Mark：3</para>
        /// <para>Parity.Space：4</para>
        /// </param>
        /// <returns>The <see cref="Parity"/>.</returns>
        public static Parity GetParity(int parityVal) => GetParity(parityVal.ToString());
        #endregion

        #region Gets the stopbits method
        /// <summary>
        /// Gets the stopbits by string.
        /// </summary>
        /// <param name="stopBitsVal">The string stop bits.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// <para>StopBits.OnePointFive：3|1.5|f|of|opf|onepointfive</para>
        /// </param>
        /// <returns>The <see cref="StopBits"/>.</returns>
        public static StopBits GetStopBits(string stopBitsVal)
        {
            StopBits stopBit;
            switch (stopBitsVal?.ToLower())
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
                case "2":
                case "t":
                case "two":
                    stopBit = StopBits.Two;
                    break;
                case "3":
                case "1.5":
                case "f":
                case "of":
                case "opf":
                case "onepointfive":
                    stopBit = StopBits.OnePointFive;
                    break;
                default:
                    stopBit = StopBits.None;
                    break;
            }

            return stopBit;
        }

        /// <summary>
        /// Gets the stopbits by int.
        /// </summary>
        /// <param name="stopBitsVal">The int stop bits.
        /// <para>StopBits.None：0</para>
        /// <para>StopBits.One：1</para>
        /// <para>StopBits.Two：2</para>
        /// <para>StopBits.OnePointFive：3</para>
        /// </param>
        /// <returns>The <see cref="StopBits"/>.</returns>
        public static StopBits GetStopBits(int stopBitsVal) => GetStopBits(stopBitsVal.ToString());
        #endregion

        #region Gets the handshake by string
        /// <summary>
        /// Gets the handshake by string.
        /// </summary>
        /// <param name="shake">The string handshake value.
        /// <para>Handshake.None：0|n|none</para>
        /// <para>Handshake.XOnXOff：1|x|xoxo</para>
        /// <para>Handshake.RequestToSend：2|r|rst</para>
        /// <para>Handshake.RequestToSendXOnXOff：3|rx|rtsxx</para>
        /// </param>
        /// <returns>Handshake.</returns>
        public static Handshake GetHandshake(string shake)
        {
            Handshake handShake;
            switch (shake?.ToLower())
            {
                case "0":
                case "n":
                case "none":
                    handShake = Handshake.None;
                    break;
                case "1":
                case "x":
                case "xoxo":
                case "xonxoff":
                case "software":
                    handShake = Handshake.XOnXOff;
                    break;
                case "2":
                case "r":
                case "rts":
                case "requesttosend":
                case "hardware":
                    handShake = Handshake.RequestToSend;
                    break;
                case "3":
                case "rx":
                case "rtsxx":
                case "requesttosendxonxoff":
                case "both":
                    handShake = Handshake.RequestToSendXOnXOff;
                    break;
                default:
                    handShake = Handshake.None;
                    break;
            }
            return handShake;
        }

        /// <summary>
        /// Gets the handshake by string.
        /// </summary>
        /// <param name="shake">The int handshake value.
        /// <para>Handshake.None：0</para>
        /// <para>Handshake.XOnXOff：1</para>
        /// <para>Handshake.RequestToSend：2</para>
        /// <para>Handshake.RequestToSendXOnXOff：3</para>
        /// </param>
        /// <returns>Handshake.</returns>
        public static Handshake GetHandshake(int shake) => GetHandshake(shake.ToString());
        #endregion
    }
}
