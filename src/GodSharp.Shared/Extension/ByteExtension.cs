using System.Collections.Generic;

namespace GodSharp.Extension
{
    /// <summary>
    /// Byte extension methods class.
    /// </summary>
    public static class ByteExtension
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
