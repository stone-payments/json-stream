using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            JsonStreamMock jsonStream = new JsonStreamMock(stream);
            int size = jsonStream.GetNextDocumentSize();
            long position = stream.Position;

            Assert.AreEqual(1024, size);
            Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_with_default_value_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = stream.Length;

            JsonStreamMock jsonStream = new JsonStreamMock(stream);
            int size = jsonStream.GetNextDocumentSize();
            long position = stream.Position;

            Assert.AreEqual(0, size);
            Assert.AreEqual(stream.Length, position);
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_with_default_value_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("00001024"), 0, 8);
            stream.Position = 1;

            JsonStreamMock jsonStream = new JsonStreamMock(stream);

            InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(()=>
            {
                int size = jsonStream.GetNextDocumentSize();
            });

            Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
            Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
            Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_value_pass()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 0;

            JsonStreamMock jsonStream = new JsonStreamMock(stream, 9);
            int size = jsonStream.GetNextDocumentSize();
            long position = stream.Position;

            Assert.AreEqual(1024, size);
            Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, position);
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_when_end_of_stream_was_reached()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = stream.Length;

            JsonStreamMock jsonStream = new JsonStreamMock(stream, 9);
            int size = jsonStream.GetNextDocumentSize();
            long position = stream.Position;

            Assert.AreEqual(0, size);
            Assert.AreEqual(stream.Length, position);
        }

        [TestMethod]
        public void GetNextDocumentSizeAsync_changing_default_length_when_cant_read_all_expected_buffer()
        {
            Stream stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes("000001024"), 0, 9);
            stream.Position = 1;

            JsonStreamMock jsonStream = new JsonStreamMock(stream, 9);

            InvalidDocumentSizeLengthException exception = Assert.ThrowsException<InvalidDocumentSizeLengthException>(() =>
            {
                int size = jsonStream.GetNextDocumentSize();
            });

            Assert.AreEqual(jsonStream.DocumentSizeLengthInBytes, exception.ExpectedDocumentSizeLength);
            Assert.AreNotEqual(exception.ReadDocumentSizeLength, exception.ExpectedDocumentSizeLength);
            Assert.IsTrue(exception.ReadDocumentSizeLength < exception.ExpectedDocumentSizeLength);
        }

        #endregion
    }
}
