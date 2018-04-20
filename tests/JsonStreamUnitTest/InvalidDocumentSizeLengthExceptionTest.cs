using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoneCo.Utils.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.Utils.IO.JsonStreamUnitTest
{
    [TestClass]
    public class InvalidDocumentSizeLengthExceptionTest
    {
        [TestMethod]
        public void Constructor_with_message()
        {
            InvalidDocumentSizeLengthException exception = new InvalidDocumentSizeLengthException("Message");
            Assert.AreEqual("Message", exception.Message);
            Assert.AreEqual(default(int), exception.ReadDocumentSizeLength);
            Assert.AreEqual(default(int), exception.ExpectedDocumentSizeLength);
        }

        [TestMethod]
        public void Constructor_with_message_expected_and_read()
        {
            int expectedDocumentSizeLength = 1;
            int readDocumentSizeLength = 2;
            InvalidDocumentSizeLengthException exception = new InvalidDocumentSizeLengthException("Message", expectedDocumentSizeLength, readDocumentSizeLength);
            Assert.AreEqual("Message", exception.Message);
            Assert.AreEqual(readDocumentSizeLength, exception.ReadDocumentSizeLength);
            Assert.AreEqual(expectedDocumentSizeLength, exception.ExpectedDocumentSizeLength);
        }

        [TestMethod]
        public void Constructor_with_message_and_exception()
        {
            Exception baseException = new Exception();
            InvalidDocumentSizeLengthException exception = new InvalidDocumentSizeLengthException("Message", baseException);
            Assert.AreEqual("Message", exception.Message);
            Assert.AreSame(baseException, exception.InnerException);
            Assert.AreEqual(default(int), exception.ReadDocumentSizeLength);
            Assert.AreEqual(default(int), exception.ExpectedDocumentSizeLength);
        }

        [TestMethod]
        public void Constructor_with_expected_and_read()
        {
            int expectedDocumentSizeLength = 1;
            int readDocumentSizeLength = 2;
            InvalidDocumentSizeLengthException exception = new InvalidDocumentSizeLengthException(expectedDocumentSizeLength, readDocumentSizeLength);
            Assert.AreEqual($"The expected DocumentSizeLength value {expectedDocumentSizeLength} is different from the read value {readDocumentSizeLength}", exception.Message);
            Assert.AreEqual(readDocumentSizeLength, exception.ReadDocumentSizeLength);
            Assert.AreEqual(expectedDocumentSizeLength, exception.ExpectedDocumentSizeLength);
        }
    }
}
