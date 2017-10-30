using System;
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace GodSharp.SerialPort.Extensions
{
	/// <summary>
	/// String extension methods class.
	/// </summary>
	public partial class Extension
	{
		/// <summary>
		/// Hexadecimal string to an byte array.
		/// </summary>
		/// <param name="hex">The hex string.</param>
		/// <returns>An byte array.</returns>
		public static byte[] HexToByte(this string hex)
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
	}
}
