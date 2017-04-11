# GodSharp.SerialPort
Easy to use SerialPort class.

[![AppVeyor build status](https://img.shields.io/appveyor/ci/seayxu/godsharp-serialport.svg?label=appveyor&style=flat-square)](https://ci.appveyor.com/project/seayxu/godsharp-serialport/) [![NuGet](https://img.shields.io/nuget/v/GodSharp.SerialPort.svg?label=nuget&style=flat-square)](https://www.nuget.org/packages/GodSharp.SerialPort/) [![MyGet](https://img.shields.io/myget/seay/v/GodSharp.SerialPort.svg?label=myget&style=flat-square)](https://www.myget.org/Package/Details/seay?packageType=nuget&packageId=GodSharp.SerialPort)

# Getting Started

1. New instance GodSerialPort.

```
GodSerialPort serial = new GodSerialPort("COM1", 9600);
```

2. Initialize the GodSerialPort instance with received data action: `Action<byte[]>`.

```
serial.Init((bytes)=>{});
```

3. Open SerialPort object.

```
serial.Open();
```

4. Write/Send data.

```
byte[] bytes = new byte[]{31,32,33,34};
serial.Write(bytes);
serial.WriteAsciiString("ascii string");
serial.WriteHexString("7E 48 53 44");
```


# Sample

```
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

        GodSerialPort gsp = new GodSerialPort("COM"+num, 9600);
        gsp.Init((bytes) => {
             string buffer = string.Join(" ", bytes);
             Console.WriteLine("receive data:" + buffer);
        });
        flag = gsp.Open();

        if (!flag)
        {
            Exit();
        }

        Console.WriteLine("serialport opend");

        Console.WriteLine("press any thing as data to send,press key 'q' to quit.");

        string data = null;
        while (data == null || data.ToLower()!="q")
        {
            if (!string.IsNullOrEmpty(data))
            {
                Console.WriteLine("send data:"+data);
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
```
