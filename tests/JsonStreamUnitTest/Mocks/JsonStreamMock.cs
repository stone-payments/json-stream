using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StoneCo.Utils.IO.JsonStreamUnitTest.Mocks
{
    public class JsonStreamMock : JsonStream
    {
        public JsonStreamMock(Stream stream, int documentSizeLengthInBytes = 8) : base(stream, documentSizeLengthInBytes)
        {
        }

        public int GetNextDocumentSize()
        {
            return base.GetNextDocumentSizeAsync().GetAwaiter().GetResult();
        }

        public new void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
