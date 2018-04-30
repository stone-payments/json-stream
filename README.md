# Json Stream
JsonStream is a thread-safe class library to make easy to read and to write json structures from/to a stream.

## Introduction
This class library is built to have a high performance when is reading or writing sequential files.

If you are reading or writing a file in sequential mode (one object after another) prefer to use the optimized constructors (that where you pass the file name).

If you are reading or writing another kind of stream like a MemoryStream or NetworkStream, you can use the constructor receiving the stream.

## Examples

### Writing an object


```C#

class Human
{
	public int Age { get; set; }
	
	public string Name { get; set; }
	
	public Gender Gender { get; set; }
}

enum Gender {
	Male,
	Female
}

// Choose the WriteOnly mode and inform the file name.
using (IJsonStream writeJsonStream = new JsonStream(Modes.WriteOnly, "objects.json"))
{
	Human human = new Human
	{
		Age = 35,
		Gender = Gender.Male,
		Name = "My name"
	};
	
	writeJsonStream.WriteObject(human);                    
}

```

### Reading the whole file

```C#

// Just inform the mode and the file name.
using (IJsonStream readJsonStream = new JsonStream(Modes.ReadOnly, "objects.json"))
{
	Human human;
	while ((human = readJsonStream.ReadObject<Human>()) != null)
	{
		Console.WriteLine($"Name {human.Name}, Gender: {human.Gender}, Age: {human.Age}");
	}	
}


```