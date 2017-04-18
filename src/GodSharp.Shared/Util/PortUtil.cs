using System.IO.Ports;

namespace GodSharp.Util
{
    /// <summary>
    /// SerialPort util.
    /// </summary>
    public class PortUtil
    {
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
            switch (parityVal.ToLower())
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
        public static Parity GetParity(int parityVal)
        {
            Parity patityBit;
            switch (parityVal)
            {
                case 0:
                    patityBit = Parity.None;
                    break;
                case 1:
                    patityBit = Parity.Odd;
                    break;
                case 2:
                    patityBit = Parity.Even;
                    break;
                case 3:
                    patityBit = Parity.Mark;
                    break;
                case 4:
                    patityBit = Parity.Space;
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
        /// <param name="stopBitsVal">The string stop bits.
        /// <para>StopBits.None：0|n|none</para>
        /// <para>StopBits.One：1|o|one</para>
        /// <para>StopBits.Two：2|t|two</para>
        /// <para>StopBits.OnePointFive：3|1.5|f|of|opf</para>
        /// </param>
        /// <returns>The <see cref="StopBits"/>.</returns>
        public static StopBits GetStopBits(string stopBitsVal)
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
        public static StopBits GetStopBits(int stopBitsVal)
        {
            StopBits stopBit;
            switch (stopBitsVal)
            {
                case 0:
                    stopBit = StopBits.None;
                    break;
                case 1:
                    stopBit = StopBits.One;
                    break;
                case 2:
                    stopBit = StopBits.Two;
                    break;
                case 3:
                    stopBit = StopBits.OnePointFive;
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
            switch (shake.ToLower())
            {
                case "0":
                case "n":
                case "none":
                    handShake = Handshake.None;
                    break;
                case "1":
                case "x":
                case "xoxo":
                    handShake = Handshake.XOnXOff;
                    break;
                case "2":
                case "r":
                case "rts":
                    handShake = Handshake.RequestToSend;
                    break;
                case "3":
                case "rx":
                case "rtsxx":
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
        public static Handshake GetHandshake(int shake)
        {
            Handshake handShake;
            switch (shake)
            {
                case 0:
                    handShake = Handshake.None;
                    break;
                case 1:
                    handShake = Handshake.XOnXOff;
                    break;
                case 2:
                    handShake = Handshake.RequestToSend;
                    break;
                case 3:
                    handShake = Handshake.RequestToSendXOnXOff;
                    break;
                default:
                    handShake = Handshake.None;
                    break;
            }
            return handShake;
        }
        #endregion
    }
}
