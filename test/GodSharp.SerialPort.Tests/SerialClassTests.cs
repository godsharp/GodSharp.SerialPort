using System;
using System.IO.Ports;
using GodSharp.Extension;
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
            bytes = "58 16 37".HexToByte();
            foreach (var item in bytes)
            {
                Console.WriteLine(item);
            }
            bytes ="581637".HexToByte();
            Console.WriteLine("no space");
            foreach (var item in bytes)
            {
                Console.WriteLine(item);
            }
        }

        [TestMethod()]
        public void SerialValueTest()
        {
            SerialPort port = new SerialPort();
            Console.WriteLine("PortName:" + port.PortName);
            Console.WriteLine("BaudRate:" + port.BaudRate);
            Console.WriteLine("Parity:" + port.Parity);
            Console.WriteLine("DataBits:" + port.DataBits);
            Console.WriteLine("StopBits:" + port.StopBits);
            Console.WriteLine("Handshake:" + port.Handshake);
            Console.WriteLine("ReadBufferSize:" + port.ReadBufferSize);
            Console.WriteLine("WriteBufferSize:" + port.WriteBufferSize);
        }
    }
}