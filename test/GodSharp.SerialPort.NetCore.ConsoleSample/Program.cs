using System;
using GodSharp.SerialPort;

namespace GodSharp.NetCore.ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("input serialport number(only 0-9):");
            string read = Console.ReadLine();
            bool flag = uint.TryParse(read, out uint num);
            if (!flag)
            {
                Exit();
            }

            GodSerialPort gsp = new GodSerialPort("COM" + num, 9600);
            gsp.UseDataReceived((sp, bytes) => {
                if (bytes != null && bytes.Length > 0)
                {
                    string buffer = string.Join(" ", bytes);
                    Console.WriteLine("receive data:" + buffer);
                }
            });
            flag = gsp.Open();

            if (!flag)
            {
                Exit();
            }

            Console.WriteLine("serialport opend");

            Console.WriteLine("press any thing as data to send,press key 'q' to quit.");

            string data = null;
            while (data == null || data.ToLower() != "q")
            {
                if (!string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("send data:" + data);
                    gsp.WriteAsciiString(data);
                }
                data = Console.ReadLine();
            }
        }

        static void Exit()
        {
            Console.WriteLine("press any key to quit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
