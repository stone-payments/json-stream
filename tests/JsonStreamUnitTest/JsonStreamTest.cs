using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StoneCo.Utils.IO.Exceptions;
using StoneCo.Utils.IO.JsonStreamUnitTest.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoneCo.Utils.IO.JsonStreamUnitTest
{
    public enum Gender { Male, Female };

    public class Human
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }
    }

    [TestClass]
    public class JsonStreamTest
    {
        #region Public properties

        public TestContext TestContext { get; set; }

        #endregion

        #region Private methods

        private string GetTestFileName()
        {            
            return $"{TestContext.TestName}.txt";
        }

        #endregion

        #region TestInitialize

        [TestInitialize]
        public void TestInitialize()
        {            
            if (File.Exists(GetTestFileName()))
            {
                File.Delete(GetTestFileName());
            }
        }

        #endregion

        #region TestCleanup

        [TestCleanup]
        public void TestCleanUp()
        {
            if (File.Exists(GetTestFileName()))
            {
                File.Delete(GetTestFileName());
            }
        }
        #endregion

        #region Dispose

        [TestMethod]
        public void Dispose_true()
        {
            MemoryStream memoryStream = new MemoryStream();
            JsonStreamMock jsonStream = new JsonStreamMock(memoryStream);
            jsonStream.Dispose(true);
            Assert.IsFalse(memoryStream.CanRead);
            Assert.IsFalse(memoryStream.CanWrite);
            Assert.IsFalse(memoryStream.CanSeek);
        }

        [TestMethod]
        public void Dispose_true_twice()
        {
            MemoryStream memoryStream = new MemoryStream();
            JsonStreamMock jsonStream = new JsonStreamMock(memoryStream);
            jsonStream.Dispose(true);
            Assert.IsFalse(memoryStream.CanRead);
            Assert.IsFalse(memoryStream.CanWrite);
            Assert.IsFalse(memoryStream.CanSeek);
            jsonStream.Dispose(true);
            Assert.IsFalse(memoryStream.CanRead);
            Assert.IsFalse(memoryStream.CanWrite);
            Assert.IsFalse(memoryStream.CanSeek);
        }

        [TestMethod]
        public void Dispose_false()
        {
            MemoryStream memoryStream = new MemoryStream();
            JsonStreamMock jsonStream = new JsonStreamMock(memoryStream);
            jsonStream.Dispose(false);
            Assert.IsTrue(memoryStream.CanRead);
            Assert.IsTrue(memoryStream.CanWrite);
            Assert.IsTrue(memoryStream.CanSeek);
        }

        #endregion

        #region Constructors

        [TestMethod]
        public void Constructor_with_stream_given_a_null_stream()
        {            
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(()=>
            {
                MemoryStream stream = null;
                IJsonStream jsonStream = new JsonStream(stream);
            });
            Assert.AreEqual("stream", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_with_stream_given_the_size_as_zero()
        {
            ArgumentOutOfRangeException exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    IJsonStream jsonStream = new JsonStream(stream, Modes.ReadAndWrite, 0);
                }                    
            });
            Assert.AreEqual("documentSizeLengthInBytes", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_with_filestream_given_a_null_filestream()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                FileStream fileStream = null;
                IJsonStream jsonStream = new JsonStream(fileStream);
            });
            Assert.AreEqual("stream", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_with_filestream_given_the_size_as_zero()
        {
            ArgumentOutOfRangeException exception = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                using (FileStream stream = File.Create(GetTestFileName()))
                {
                    IJsonStream jsonStream = new JsonStream(stream, Modes.ReadAndWrite, 0);
                }                    
            });
            Assert.AreEqual("documentSizeLengthInBytes", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_with_stream_pass()
        {            
            using (JsonStreamMock jsonStream = new JsonStreamMock(new MemoryStream()))
            {
                Assert.IsInstanceOfType(jsonStream.Stream, typeof(MemoryStream));
                Assert.IsTrue(jsonStream.Stream.CanWrite);
                Assert.IsTrue(jsonStream.Stream.CanRead);
                Assert.IsNotNull(jsonStream.BinaryWriter);
                Assert.IsNotNull(jsonStream.BinaryReader);
                Assert.IsFalse(jsonStream.IsUsingOptimizedConstructor);
                Assert.AreEqual(Modes.ReadAndWrite, jsonStream.Mode);
            }
        }

        [TestMethod]
        public void Constructor_with_filePath_when_filePath_is_null()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                IJsonStream jsonStream = new JsonStream(Modes.ReadAndWrite, "");
            });
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [TestMethod]
        public void Constructor_with_filePath_when_mode_is_WriteOnly()
        {
            using (var fs = File.OpenWrite(GetTestFileName())) { }
            using (JsonStreamMock jsonStream = new JsonStreamMock(Modes.WriteOnly, GetTestFileName()))
            {
                Assert.IsInstanceOfType(jsonStream.Stream, typeof(FileStream));
                Assert.IsTrue(jsonStream.Stream.CanWrite);
                Assert.IsFalse(jsonStream.Stream.CanRead);
                Assert.IsNotNull(jsonStream.BinaryWriter);
                Assert.IsNull(jsonStream.BinaryReader);
                Assert.IsTrue(jsonStream.IsUsingOptimizedConstructor);
                Assert.AreEqual(Modes.WriteOnly, jsonStream.Mode);
            }
        }

        [TestMethod]
        public void Constructor_with_filePath_when_mode_is_ReadOnly()
        {
            using (var fs = File.OpenWrite(GetTestFileName())) { }
            using (JsonStreamMock jsonStream = new JsonStreamMock(Modes.ReadOnly, GetTestFileName()))
            {
                Assert.IsInstanceOfType(jsonStream.Stream, typeof(FileStream));
                Assert.IsFalse(jsonStream.Stream.CanWrite);
                Assert.IsTrue(jsonStream.Stream.CanRead);
                Assert.IsNull(jsonStream.BinaryWriter);
                Assert.IsNotNull(jsonStream.BinaryReader);
                Assert.IsTrue(jsonStream.IsUsingOptimizedConstructor);
                Assert.AreEqual(Modes.ReadOnly, jsonStream.Mode);
            }
        }

        [TestMethod]
        public void Constructor_with_filePath_when_mode_is_ReadWrite()
        {
            using (var fs = File.OpenWrite(GetTestFileName())) { }
            using (JsonStreamMock jsonStream = new JsonStreamMock(Modes.ReadAndWrite, GetTestFileName()))
            {
                Assert.IsInstanceOfType(jsonStream.Stream, typeof(FileStream));
                Assert.IsTrue(jsonStream.Stream.CanWrite);
                Assert.IsTrue(jsonStream.Stream.CanRead);
                Assert.IsNotNull(jsonStream.BinaryWriter);
                Assert.IsNotNull(jsonStream.BinaryReader);
                Assert.IsTrue(jsonStream.IsUsingOptimizedConstructor);
                Assert.AreEqual(Modes.ReadAndWrite, jsonStream.Mode);
            }
        }

        #endregion

        #region GetString

        [TestMethod]
        public void GetString()
        {
            Human human = new Human
            {
                Name = "Marcus",
                Age = 35,
                Gender = Gender.Male
            };

            string expectedString = "{\"Name\":\"Marcus\",\"Age\":35,\"Gender\":\"Male\"}";            
            string humanString = JsonStream.GetString(human);            

            Assert.AreEqual(expectedString, humanString);
        }

        #endregion

        #region GetBytes

        [TestMethod]
        public void GetBytes()
        {
            Human human = new Human
            {
                Name = "Marcus",
                Age = 35,
                Gender = Gender.Male
            };
            string expectedString = "{\"Name\":\"Marcus\",\"Age\":35,\"Gender\":\"Male\"}";
            byte[] expectedBytes = System.Text.Encoding.UTF8.GetBytes(expectedString);            
            byte[] humanBytes = JsonStream.GetBytes(human);
            string humanString = System.Text.Encoding.UTF8.GetString(humanBytes);

            Assert.AreEqual(expectedString, humanString);
        }

        #endregion

        #region GetNextDocumentSizeAsync

        [TestMethod]
        public void GetNextDocumentSizeAsync_with_default_value_pass()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"),0,8);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                long position = stream.Position;

                Assert.AreEqual(1024, size);
                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
            }

        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_with_default_value_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = stream.Length;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                long position = stream.Position;

                Assert.AreEqual(0, size);
                Assert.AreEqual(stream.Length, position);
            }

        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_with_default_value_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = 1;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                });

                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
                Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
                Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
            }
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_value_pass()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                long position = stream.Position;

                Assert.AreEqual(1024, size);
                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
            }
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = stream.Length;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                long position = stream.Position;

                Assert.AreEqual(0, size);
                Assert.AreEqual(stream.Length, position);
            }
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 1;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                });

                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
                Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
                Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
            }
        }

        [TestMethod]
        public void GetNexDocumentSizeAsync_when_size_descriptor_is_invalid()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("AsWedbUp"), 0, 8);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 8))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
                });

                Assert.AreEqual(default(int), exception.ExpectedDocumentSizeLength);
                Assert.AreEqual(default(int), exception.ReadDocumentSizeLength);
                Assert.AreEqual("Error interpreting document size.", exception.Message);
            }
        }

        #endregion

        #region GetNextDocumentSize

        [TestMethod]
        public void GetNextDocumentSize_with_default_value_pass()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                int size = jsonStream.GetNextDocumentSize();
                long position = stream.Position;

                Assert.AreEqual(1024, size);
                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
            }

        }

        [TestMethod]
        public void GetNextDocumentSize_with_default_value_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = stream.Length;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                int size = jsonStream.GetNextDocumentSize();
                long position = stream.Position;

                Assert.AreEqual(0, size);
                Assert.AreEqual(stream.Length, position);
            }

        }

        [TestMethod]
        public void GetNextDocumentSize_with_default_value_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = 1;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSize();
                });

                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
                Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
                Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
            }
        }

        [TestMethod]
        public void GetNextDocumentSize_changing_default_length_value_pass()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                int size = jsonStream.GetNextDocumentSize();
                long position = stream.Position;

                Assert.AreEqual(1024, size);
                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
            }
        }

        [TestMethod]
        public void GetNextDocumentSize_changing_default_length_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = stream.Length;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                int size = jsonStream.GetNextDocumentSize();
                long position = stream.Position;

                Assert.AreEqual(0, size);
                Assert.AreEqual(stream.Length, position);
            }
        }

        [TestMethod]
        public void GetNextDocumentSize_changing_default_length_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 1;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 9))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSize();
                });

                Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
                Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
                Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
            }
        }

        [TestMethod]
        public void GetNexDocumentSize_when_size_descriptor_is_invalid()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("AsWedbUp"), 0, 8);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, Modes.ReadAndWrite, 8))
            {
                InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
                {
                    int size = jsonStream.GetNextDocumentSize();
                });

                Assert.AreEqual(default(int), exception.ExpectedDocumentSizeLength);
                Assert.AreEqual(default(int), exception.ReadDocumentSizeLength);
                Assert.AreEqual("Error interpreting document size.", exception.Message);
            }
        }

        #endregion

        #region ReadJObject

        [TestMethod]
        public void ReadJObject_pass()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation readJObject = jsonStream.ReadJObject().ToObject<JObjectValidation>();
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region ReadJArray

        [TestMethod]
        public void ReadJArray_pass()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };

            JObjectValidation[] list = new JObjectValidation[1] { jObject };

            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JArray readArray = jsonStream.ReadJArray();
                Assert.AreEqual(1, readArray.Count);

                JObjectValidation readJObject = readArray.First.ToObject<JObjectValidation>();

                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region ReadJToken

        [TestMethod]
        public void ReadJToken_is_end_of_stream()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = stream.Length;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JToken readJToken = jsonStream.ReadJToken();
                Assert.IsNull(readJToken);
            }
        }

        #endregion

        #region ReadObject

        [TestMethod]
        public void ReadObject_cant_read_entire_document()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes((jObjectBytes.Length + 1).ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                InvalidJsonDocumentException ex = Assert.ThrowsException<InvalidJsonDocumentException>(() =>
                {
                    JObjectValidation readObject = jsonStream.ReadObject<JObjectValidation>();
                });
                Assert.IsInstanceOfType(ex, typeof(InvalidJsonDocumentException));
                Assert.IsTrue(ex.Message.Contains("Cant't read all bytes of the json document at position"));
            }
        }

        [TestMethod]
        public void ReadObject_pass()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation readJObject = jsonStream.ReadObject<JObjectValidation>();
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region ReadBytes

        [TestMethod]
        public void ReadBytes_pass()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation readJObject = JsonConvert.DeserializeObject<JObjectValidation>(Encoding.UTF8.GetString(jsonStream.ReadBytes()));
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        [TestMethod]
        public void ReadBytes_next_document_size_equal_zero()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = stream.Length;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                byte[] bytes = jsonStream.ReadBytes();
                Assert.IsNull(bytes);
            }
        }

        [TestMethod]
        public void ReadBytes_cant_read_entire_next_document()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes((jObjectBytes.Length+1).ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            InvalidJsonDocumentException exception = Assert.ThrowsException<InvalidJsonDocumentException>(() =>
            {
                using (IJsonStream jsonStream = new JsonStream(stream))
                {
                    byte[] bytes = jsonStream.ReadBytes();                    
                }
            });

            Assert.IsTrue(exception.Message.Contains("Cant't read all bytes of the json document at position"));
        }

        [TestMethod]
        public void ReadBytes_using_WriteOnly_mode()
        {
            Stream stream = new MemoryStream();

            ForbiddenOperationException exception = Assert.ThrowsException<ForbiddenOperationException>(() =>
            {
                using (IJsonStream jsonStream = new JsonStream(stream, Modes.WriteOnly))
                {
                    byte[] bytes = jsonStream.ReadBytes();
                }
            });

            Assert.AreEqual("Can't read in WriteOnly mode.", exception.Message);
        }

        #endregion

        #region ReadBytesAsync

        [TestMethod]
        public void ReadBytesAsync_using_optimized_constructor()
        {
            using (JsonStreamMock jsonStream = new JsonStreamMock(Modes.ReadAndWrite, GetTestFileName()))
            {
                ForbiddenOperationException exception = Assert.ThrowsException<ForbiddenOperationException>(() =>
                {
                    jsonStream.ReadBytesAsync().GetAwaiter().GetResult();
                });

                Assert.AreEqual("Do not call any async method when using optimized constructor.", exception.Message);
            }            
        }

        [TestMethod]
        public void ReadBytesAsync_in_write_only_mode()
        {
            using (JsonStreamMock jsonStream = new JsonStreamMock(new MemoryStream(), Modes.WriteOnly))
            {
                ForbiddenOperationException exception = Assert.ThrowsException<ForbiddenOperationException>(() =>
                {
                    jsonStream.ReadBytesAsync().GetAwaiter().GetResult();
                });

                Assert.AreEqual("Can't read in WriteOnly mode.", exception.Message);
            }
        }

        #endregion

        #region ReadString

        [TestMethod]
        public void ReadString_pass()
        {
            JObjectValidation jObject = new JObjectValidation
            {
                BooleanField = true,
                ByteField = 1,
                CharField = 'a',
                DateField = "2016-08-24T18:30:32.2387069+00:00",
                DoubleField = 10.5,
                IntField = 10,
                ListField = new object[2] { new { }, new { } },
                ObjectField = new { },
                StringField = "string",
                NullField = null
            };
            byte[] jObjectBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));
            byte[] length = Encoding.UTF8.GetBytes(jObjectBytes.Length.ToString().PadLeft(8));

            Stream stream = new MemoryStream();
            stream.Write(length, 0, 8);
            stream.Write(jObjectBytes, 0, jObjectBytes.Length);
            stream.Position = 0;

            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation readJObject = JsonConvert.DeserializeObject<JObjectValidation>(jsonStream.ReadString());
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region WriteObject

        [TestMethod]
        public void WriteObject_pass()
        {
            Stream stream = new MemoryStream();
            using(IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation jObject = new JObjectValidation
                {
                    BooleanField = true,
                    ByteField = 1,
                    CharField = 'a',
                    DateField = "2016-08-24T18:30:32.2387069+00:00",
                    DoubleField = 10.5,
                    IntField = 10,
                    ListField = new object[2] { new { }, new { } },
                    ObjectField = new { },
                    StringField = "string",
                    NullField = null
                };

                jsonStream.WriteObject(jObject);
                stream.Position = 0;
                JObjectValidation readJObject = jsonStream.ReadObject<JObjectValidation>();
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region WriteString

        [TestMethod]
        public void WriteString_pass()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation jObject = new JObjectValidation
                {
                    BooleanField = true,
                    ByteField = 1,
                    CharField = 'a',
                    DateField = "2016-08-24T18:30:32.2387069+00:00",
                    DoubleField = 10.5,
                    IntField = 10,
                    ListField = new object[2] { new { }, new { } },
                    ObjectField = new { },
                    StringField = "string",
                    NullField = null
                };
                string jObjectSerialized = JsonConvert.SerializeObject(jObject);
                jsonStream.WriteString(jObjectSerialized);
                stream.Position = 0;
                JObjectValidation readJObject = jsonStream.ReadObject<JObjectValidation>();
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        #endregion

        #region WriteBytes

        [TestMethod]
        public void WriteBytes_when_bytes_length_is_zero()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                byte[] bytes = new byte[0];
                ArgumentException ex = Assert.ThrowsException<ArgumentException>(() =>
                {
                    jsonStream.WriteBytes(bytes, false);
                });
                Assert.AreEqual("bytes", ex.ParamName);
                Assert.IsTrue(ex.Message.Contains("Can't write an empty byte array."));
            }
        }

        [TestMethod]
        public void WriteBytes_when_bytes_is_null()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                byte[] bytes = null;

                ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(()=>
                {
                    jsonStream.WriteBytes(bytes, false);
                });
                Assert.AreEqual("bytes", ex.ParamName);
                Assert.AreEqual(0, stream.Length);
            }
        }

        [TestMethod]
        public void WriteBytes_when_validate_is_true_pass()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                JObjectValidation jObject = new JObjectValidation
                {
                    BooleanField = true,
                    ByteField = 1,
                    CharField = 'a',
                    DateField = "2016-08-24T18:30:32.2387069+00:00",
                    DoubleField = 10.5,
                    IntField = 10,
                    ListField = new object[2] { new { }, new { } },
                    ObjectField = new { },
                    StringField = "string",
                    NullField = null
                };

                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jObject));

                jsonStream.WriteBytes(bytes, true);
                stream.Position = 0;
                JObjectValidation readJObject = jsonStream.ReadObject<JObjectValidation>();
                Assert.AreEqual(jObject.BooleanField, readJObject.BooleanField);
                Assert.AreEqual(jObject.ByteField, readJObject.ByteField);
                Assert.AreEqual(jObject.CharField, readJObject.CharField);
                Assert.AreEqual(jObject.DateField, readJObject.DateField);
                Assert.AreEqual(jObject.DoubleField, readJObject.DoubleField);
                Assert.AreEqual(jObject.IntField, readJObject.IntField);
                Assert.AreEqual(jObject.ListField.Length, readJObject.ListField.Length);
                Assert.AreEqual(jObject.StringField, readJObject.StringField);
                Assert.IsNotNull(readJObject.ObjectField);
                Assert.IsNull(readJObject.NullField);
            }
        }

        [TestMethod]
        public void WriteBytes_using_ReadOnly_mode()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream, Modes.ReadOnly))
            {
                byte[] bytes = null;
                ForbiddenOperationException ex = Assert.ThrowsException<ForbiddenOperationException>(() =>
                {
                    jsonStream.WriteBytes(bytes, false);
                });

                Assert.AreEqual("Can't write in ReadOnly mode.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteBytes_when_validate_is_true_not_passing()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream))
            {
                byte[] bytes = Encoding.UTF8.GetBytes("NOT PASS");

                JsonReaderException ex = Assert.ThrowsException<JsonReaderException>(() =>
                {
                    jsonStream.WriteBytes(bytes, true);
                });
                Assert.IsInstanceOfType(ex, typeof(JsonReaderException));
                Assert.AreEqual(0, stream.Length);
            }
        }

        #endregion

        #region WriteBytesAsync

        [TestMethod]
        public void WriteBytesAsync_using_ReadOnly_mode()
        {
            Stream stream = new MemoryStream();
            using (IJsonStream jsonStream = new JsonStream(stream, Modes.ReadOnly))
            {
                byte[] bytes = null;
                ForbiddenOperationException ex = Assert.ThrowsException<ForbiddenOperationException>(() =>
                {
                    jsonStream.WriteBytesAsync(bytes, false).GetAwaiter().GetResult();
                });

                Assert.AreEqual("Can't write in ReadOnly mode.", ex.Message);
            }
        }

        [TestMethod]
        public void WriteBytesAsync_using_optimized_constructor()
        {
            using (File.Create(GetTestFileName())) { }
            using (IJsonStream jsonStream = new JsonStream(Modes.ReadAndWrite, GetTestFileName()))
            {
                byte[] bytes = null;
                ForbiddenOperationException ex = Assert.ThrowsException<ForbiddenOperationException>(() =>
                {
                    jsonStream.WriteBytesAsync(bytes, false).GetAwaiter().GetResult();
                });

                Assert.AreEqual("Do not call any async method when using optimized constructor.", ex.Message);
            }
        }

        #endregion
    }
}
