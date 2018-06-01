# StoneCo.JsonStream
JsonStream is a thread-safe dotNet Standard Class Library to make easy to read and to write json structures from/to a stream.

* [Code of Conduct](codeOfConduct/README.md)
* [CONTRIBUTING.md](contributing/README.md)
* [LICENSE.md](license/README.md)

## Dependences

1. NETStandard.Library v2.0.0
1. Newtonsoft.Json v11.0.2

## Introduction
This class library was built to have a high performance when is reading or writing sequential files.

If you are reading or writing a file in sequential mode (one object after another) prefer to use the optimized constructor (that where you pass the file name).

If you are reading or writing another kind of stream like a MemoryStream or NetworkStream, you can use the other constructor passing this stream.

**If you used the optimized constructor to instantiate the JsonStream, you can't use the async methods.**
 

## How it works?
Before write, JsonStream calculates the length of the buffer and writes this buffer length into the stream before writes the buffer. With default configuration, each buffer uses 8 bytes to describe his length.

When reading, JsonStream firstly reads the length of the next buffer and than reads the next *n* bytes of the buffer. When reader reaches the end of the stream, the method returns the default value of the type.

When the Dispose method is called, the JsonStream calls the Dispose method of the given stream or the inner stream if you used the optimized constructor.

### About the constructors
When you use the optimized constructor (the one with the filePath argument), the class will create internally one FileStream.
See the arguments used in the FileStream constructor depending of the choosed mode.

#### ReadOnly
* **FileMode** - Open
* **FileAccess** - Read
* **FileShare** - ReadWrite
* **FileOptions** - SequentialScan

#### WriteOnly
* **FileMode** - Append
* **FileAccess** - Write
* **FileShare** - Read
* **FileOptions** - SequentialScan

#### ReadAndWrite
* **FileMode** - OpenOrCreate
* **FileAccess** - ReadWrite
* **FileShare** - ReadWrite
* **FileOptions** - RandomAccess

### Default Values

#### DEFAULT\_BUFFER\_SIZE = 4096 bytes
The buffer size passed to the FileStream constructor.


#### DEFAULT\_DOCUMENT\_SIZE\_LENGTH = 8 bytes
The length of the document size descriptor as explained above.

## Examples

### Base code

```C#

class Human
{
	public int Age { get; set; }
	
	public string Name { get; set; }
	
	public Gender Gender { get; set; }
}

enum Gender
{
	Male,
	Female
}

```

### Writing an object


```C#

// Choose the WriteOnly mode and inform the file path.
using (IJsonStream writeJsonStream = new JsonStream(Modes.WriteOnly, "objects.json"))
{
	Human human = new Human
	{
		Age = 5,
		Gender = Gender.Female,
		Name = "Stone"
	};
	
	writeJsonStream.WriteObject(human);                    
}

```

### Reading the whole file

```C#

// Choose the ReadOnly mode and inform the file path.
using (IJsonStream readJsonStream = new JsonStream(Modes.ReadOnly, "objects.json"))
{
	Human human;
	while ((human = readJsonStream.ReadObject<Human>()) != null)
	{
		Console.WriteLine($"Name {human.Name}, Gender: {human.Gender}, Age: {human.Age}");
	}	
}


```

### Using the constructor receiving the stream

```C#

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

NetworkStream networkStream = new NetworkStream(socket);
using (IJsonStream jsonStream = new JsonStream(networkStream))
{
	jsonStream.WriteObjectAsync(human);
}

```

## License
This library is distributed under the MIT license.