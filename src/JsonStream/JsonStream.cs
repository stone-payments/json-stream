using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StoneCo.Utils.IO
{
    public class JsonStream : IJsonStream, IDisposable
    {
        #region Constants

        /// <summary>
        /// The default number of bytes to representing the size of the json document.
        /// </summary>
        public const uint DEFAULT_DOCUMENT_SIZE_LENGTH = 8;

        #endregion

        #region Protected properties

        /// <summary>
        /// The stream to use.
        /// </summary>
        protected Stream Stream;

        #endregion

        #region Public properties

        /// <summary>
        /// The number of bytes to representing the size of the json document.
        /// </summary>
        public uint DocumentSizeLengthInBytes { get; set; }

        /// <summary>
        /// The pointer position in the Stream.
        /// </summary>
        public long Position { get { return this.Stream.Position; } }

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor receiving a Stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        public JsonStream(Stream stream, uint documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH)
        {
            #region Validations

            if(stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if(documentSizeLengthInBytes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(documentSizeLengthInBytes), "Please reserve at least one byte to represent the size of the document.");
            }

            #endregion

            this.Stream = stream;
            this.DocumentSizeLengthInBytes = documentSizeLengthInBytes;            
        }

        /// <summary>
        /// The constructor receiving a FileStream.
        /// </summary>
        /// <param name="fileStream">The FileStream.</param>
        public JsonStream(FileStream fileStream, uint documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH)
        {
            #region Validations

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (documentSizeLengthInBytes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(documentSizeLengthInBytes), "Please reserve at least one byte to represent the size of the document.");
            }

            #endregion

            this.Stream = fileStream;
            this.DocumentSizeLengthInBytes = documentSizeLengthInBytes;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Stream.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the bytes of the document and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document.</returns>
        public byte[] ReadBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the bytes of the document and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document.</returns>
        public async Task<byte[]> ReadBytesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        public JArray ReadJArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        public async Task<JArray> ReadJArrayAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        public JObject ReadJson()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        public async Task<JObject> ReadJsonAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        public JToken ReadJToken()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        public async Task<JToken> ReadJTokenAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        public T ReadObject<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        public async Task<T> ReadObjectAsync<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        public string ReadString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        public async Task<string> ReadStringAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public void WriteBytes(byte[] bytes, bool validate = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public async Task WriteBytesAsync(byte[] bytes, bool validate = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jArray">The JArray to write.</param>
        public void WriteJArray(JArray jArray)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jArray">The JArray to write.</param>
        public async Task WriteJArrayAsync(JArray jArray)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jObject">The JObject to write.</param>
        public void WriteJson(JObject jObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jObject">The JObject to write.</param>
        public async Task WriteJsonAsync(JObject jObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jToken">The JToken to write.</param>
        public void WriteJToken(JToken jToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jToken">The JToken to write.</param>
        public async Task WriteJTokenAsync(JToken jToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public void WriteString(string jString, bool validate = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public async Task WriteStringAsync(string jString, bool validate = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance of T to write.</param>
        public void WriteObject<T>(T instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance of T to write.</param>
        public async Task WriteObjectAsync<T>(T instance)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
