using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
