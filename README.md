# GodSharp.SerialPort
An easy-to-use .NET SerialPort class.

[![AppVeyor build status](https://img.shields.io/appveyor/ci/seayxu/godsharp-serialport.svg?label=appveyor&style=flat-square)](https://ci.appveyor.com/project/seayxu/godsharp-serialport/) [![NuGet](https://img.shields.io/nuget/v/GodSharp.SerialPort.svg?label=nuget&style=flat-square)](https://www.nuget.org/packages/GodSharp.SerialPort/) [![MyGet](https://img.shields.io/myget/seay/v/GodSharp.SerialPort.svg?label=myget&style=flat-square)](https://www.myget.org/Package/Details/seay?packageType=nuget&packageId=GodSharp.SerialPort)


# Requirement
.NET Framework >= 3.5

# Getting Started

1. New instance GodSerialPort.

```
GodSerialPort serial = new GodSerialPort("COM1", 9600,0);
```

2. Use `DataReceived` event with received data action: `Action<byte[]>`.

**Notice**:*This is not need when you read data by read method.*
```
serial.UseDataReceived(true,(sp,bytes)=>{});
```

3. Open SerialPort object.

```
serial.Open();
```

4. Write/Send data.

```
byte[] bytes = new byte[]{31,32,33,34};
serial.Write(bytes);
serial.Write(bytes,offset:1,count:2);
serial.WriteHexString("7E 48 53 44");
serial.WriteAsciiString("ascii string");
```

5. Read data.
```
byte[] bytes = serial.Read();
string stringAsciiOrHex = serial.ReadString();
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

        GodSerialPort gsp = new GodSerialPort("COM"+num, 9600,0);
        gsp.UseDataReceived(true,(sp,bytes) => {
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

# Notes

## 1.2.0
- 1.support .NET Core 2.0.

## 1.1.2
- 1.Add GodSerialPort to event action as signature param for initial a list.

## 1.1.1
- 1.Add constructor and change the constructor signature.
- 2.Add `PortUtil` class.
      
## 1.1.0
- 1.Add UseDataReceived method use to trigger DataReceived event.
- 2.The read metnod can be used to end character.
- 3.Add sleep time when try read data.

## 1.0.1
- 1.Fix ctor and comments.

## 1.0.0
- 1.The first version release.