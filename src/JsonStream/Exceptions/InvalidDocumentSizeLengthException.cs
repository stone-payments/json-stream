using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.Utils.IO.Exceptions
{
    public class InvalidDocumentSizeLengthException : Exception
    {
        public int ExpectedDocumentSizeLength { get; set; }

        public int ReadDocumentSizeLength { get; set; }

        public InvalidDocumentSizeLengthException(string message) : base(message)
        {

        }

        public InvalidDocumentSizeLengthException(string message, Exception ex) : base(message, ex)
        {

        }

        public InvalidDocumentSizeLengthException(string message, int expectedDocumentSizeLength, int readDocumentSizeLength) : base(message)
        {
            this.ExpectedDocumentSizeLength = expectedDocumentSizeLength;
            this.ReadDocumentSizeLength = readDocumentSizeLength;
        }

        public InvalidDocumentSizeLengthException(int expectedDocumentSizeLength, int readDocumentSizeLength) : base($"The expected DocumentSizeLength value {expectedDocumentSizeLength} is different from the read value {readDocumentSizeLength}")
        {
            this.ExpectedDocumentSizeLength = expectedDocumentSizeLength;
            this.ReadDocumentSizeLength = readDocumentSizeLength;
        }
    }
}
