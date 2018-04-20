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
                    IJsonStream jsonStream = new JsonStream(stream, 0);
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
                    IJsonStream jsonStream = new JsonStream(stream, 0);
                }                    
            });
            Assert.AreEqual("documentSizeLengthInBytes", exception.ParamName);
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
                int size = jsonStream.GetNextDocumentSize();
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
                int size = jsonStream.GetNextDocumentSize();
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
                    int size = jsonStream.GetNextDocumentSize();
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

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, 9))
            {
                int size = jsonStream.GetNextDocumentSize();
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

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, 9))
            {
                int size = jsonStream.GetNextDocumentSize();
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

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, 9))
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
        public void GetNexDocumentSizeAsync_when_size_descriptor_is_invalid()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("AsWedbUp"), 0, 8);
            stream.Position = 0;

            using (JsonStreamMock jsonStream = new JsonStreamMock(stream, 8))
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
                jsonStream.WriteBytes(bytes, false);

                Assert.AreEqual(0, stream.Length);
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
    }
}
