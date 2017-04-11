using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GodSharp.Tests
{
    [TestClass()]
    public class SerialClassTests
    {
        [TestMethod()]
        public void HexToByteTest()
        {
            byte[] bytes;
            bytes = GodSerialPort.HexToByte("58 16 37");
            foreach (var item in bytes)
            {
                System.Console.WriteLine(item);
            }
            bytes = GodSerialPort.HexToByte("581637");
            System.Console.WriteLine("no space");
            foreach (var item in bytes)
            {
                System.Console.WriteLine(item);
            }
        }
    }
}