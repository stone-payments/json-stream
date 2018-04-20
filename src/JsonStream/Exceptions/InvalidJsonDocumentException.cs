using System;
using System.Runtime.Serialization;

namespace StoneCo.Utils.IO.Exceptions
{
    public class InvalidJsonDocumentException : Exception
    {
        /// <summary>
        /// The position in stream where the exception was thrown.
        /// </summary>
        public long Position { get; set; }

        public InvalidJsonDocumentException(string message, long position = 0) : base(message)
        {
            this.Position = position;
        }

    }
}
