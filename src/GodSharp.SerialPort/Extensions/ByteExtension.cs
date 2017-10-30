using System.Collections.Generic;
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace GodSharp.SerialPort.Extensions
{
    /// <summary>
    /// Byte extension methods class.
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// Bytes to hexadecimal.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="separator">The separator,default is space</param>
        /// <returns>String of hex.</returns>
        public static string ToHexString(this byte[] bytes,string separator=" ")
        {
            if (bytes==null||bytes.Length<1)
            {
                return null;
            }

            List<string> list = new List<string>();
            foreach (byte b in bytes)
            {
                list.Add(b.ToString("X2"));
            }

            return string.Join(separator, list.ToArray());
        }
    }
}
