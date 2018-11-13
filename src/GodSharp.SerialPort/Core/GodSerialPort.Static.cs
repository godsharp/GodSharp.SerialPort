using System.Collections.Generic;

// ReSharper disable All
namespace GodSharp.SerialPort
{
    /// <summary>
    /// SerialPort util.
    /// </summary>
    public partial class GodSerialPort
    {

        /// <summary>
        /// Get an array of serialport name for current computer.
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames() => System.IO.Ports.SerialPort.GetPortNames();
        
        /// <summary>
        /// Gets the baudrate dictionary.
        /// </summary>
        /// <value>The baudrate dictionary.</value>
        public static Dictionary<string, int> BaudRateDictionary { get; }

        /// <summary>
        /// Gets the parity dictionary.
        /// </summary>
        /// <value>The parity dictionary.</value>
        public static Dictionary<string, int> ParityDictionary { get; }

        /// <summary>
        /// Gets the stop bit dictionary.
        /// </summary>
        /// <value>The stop bit dictionary.</value>
        public static Dictionary<string, int> StopBitDictionary { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, int> HandshakeDictionary { get; }
    
    
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

            ParityDictionary = new Dictionary<string, int>
            {
                {"None", 0},
                {"Odd", 1},
                {"Even", 2},
                {"Mark", 3},
                {"Space", 4}
            };

            StopBitDictionary = new Dictionary<string, int>
            {
                {"None", 0},
                {"1", 1},
                {"2", 2},
                {"1.5", 3}
            };
            
            HandshakeDictionary = new Dictionary<string, int>
            {
                {"None", 0},
                {"Software", 1},
                {"Hardware", 2},
                {"Both", 3}
            };
        }
    }
}
