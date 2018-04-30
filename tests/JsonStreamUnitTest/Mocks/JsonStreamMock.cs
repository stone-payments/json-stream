using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StoneCo.Utils.IO.JsonStreamUnitTest.Mocks
{
    public class JsonStreamMock : JsonStream
    {
        public JsonStreamMock(Stream stream, Modes mode = Modes.ReadAndWrite, int documentSizeLengthInBytes = 8) : base(stream, mode, documentSizeLengthInBytes)
        {
        }

        public JsonStreamMock(Modes mode, string filePath, int documentSizeLengthInBytes = 8, int bufferSize = 524288) : base(mode, filePath, documentSizeLengthInBytes, bufferSize)
        {
        }

        /// <summary>
        /// The stream to use.
        /// </summary>
        public new Stream Stream { get { return base.Stream; } }

        /// <summary>
        /// Used to write bytes on a file.
        /// </summary>
        public new BinaryWriter BinaryWriter { get { return base.BinaryWriter; } }

        /// <summary>
        /// Used to read bytes from a file.
        /// </summary>
        public new BinaryReader BinaryReader { get { return base.BinaryReader; } }


        public new int GetNextDocumentSize()
        {
            return base.GetNextDocumentSize();
        }

        public new async Task<int> GetNextDocumentSizeAsync()
        {
            return await base.GetNextDocumentSizeAsync();
        }

        public new void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
