using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StoneCo.Utils.IO.Exceptions;

namespace StoneCo.Utils.IO
{
    /// <summary>
    /// A Stream to read and write json documents.
    /// </summary>
    public class JsonStream : IJsonStream
    {
        #region Constants

        /// <summary>
        /// The default number of bytes to representing the size of the json document.
        /// </summary>
        public const int DEFAULT_DOCUMENT_SIZE_LENGTH = 8;

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
        public int DocumentSizeLengthInBytes { get; set; }

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
        public JsonStream(Stream stream, int documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH)
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

            JsonConvert.DefaultSettings = () => Serialization.Settings;
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Dispose pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the size in bytes of the next document or zero if the end of the stream was reached.
        /// </summary>
        /// <returns>The size in bytes of the next document or zero if the end of the stream was reached.</returns>
        protected async Task<int> GetNextDocumentSizeAsync()
        {
            byte[] documentLengthDescriptor = new byte[DocumentSizeLengthInBytes];
            int readBytes = await Stream.ReadAsync(documentLengthDescriptor, 0, DocumentSizeLengthInBytes);

            if(readBytes == 0)
            {
                return readBytes;
            }

            int documentSize;
            if(readBytes != this.DocumentSizeLengthInBytes)
            {
                throw new InvalidDocumentSizeLengthException(DocumentSizeLengthInBytes, readBytes);                
            }

            try
            {
                documentSize = int.Parse(System.Text.Encoding.UTF8.GetString(documentLengthDescriptor));
            }catch(Exception ex)
            {
                throw new InvalidDocumentSizeLengthException("Error interpreting document size.", ex);
            }
            
            return documentSize;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the bytes of the document or null if the end of the stream was reached and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document or null if the end of the stream was reached.</returns>
        public byte[] ReadBytes()
        {
            return ReadBytesAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the bytes of the document or null if the end of the stream was reached and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document or null if the end of the stream was reached.</returns>
        public async Task<byte[]> ReadBytesAsync()
        {
            int nextDocumentSize = await GetNextDocumentSizeAsync();

            if(nextDocumentSize == 0)
            {
                return null;
            }

            byte[] documentBytes = new byte[nextDocumentSize];

            int readBytes = await Stream.ReadAsync(documentBytes, 0, nextDocumentSize);

            if(readBytes != nextDocumentSize)
            {
                throw new InvalidJsonDocumentException($"Cant't read all bytes of the json document at position {Position}.", this.Position);
            }            
            
            return documentBytes;            
        }

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        public JArray ReadJArray()
        {
            return ReadJArrayAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        public async Task<JArray> ReadJArrayAsync()
        {
            return await ReadObjectAsync<JArray>();
        }

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        public JObject ReadJObject()
        {
            return ReadJsonAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        public async Task<JObject> ReadJsonAsync()
        {
            return await ReadObjectAsync<JObject>();
        }

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        public JToken ReadJToken()
        {
            return ReadJTokenAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        public async Task<JToken> ReadJTokenAsync()
        {
            return await ReadObjectAsync<JToken>();
        }

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        public T ReadObject<T>()
        {
            return ReadObjectAsync<T>().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        public async Task<T> ReadObjectAsync<T>()
        {
            string strJson = await ReadStringAsync();
            if(strJson == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(strJson);
        }

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        public string ReadString()
        {
            return ReadStringAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        public async Task<string> ReadStringAsync()
        {
            byte[] readBytes = await ReadBytesAsync();
            if(readBytes == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(readBytes);
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public void WriteBytes(byte[] bytes, bool validate = true)
        {
            this.WriteBytesAsync(bytes, validate).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public async Task WriteBytesAsync(byte[] bytes, bool validate = true)
        {
            if(bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            int size = bytes.Length;
            if (size == 0)
            {
                return;
            }

            if (validate)
            {
                JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
            }

            byte[] lenghDescriptor = Encoding.UTF8.GetBytes(bytes.Length.ToString().PadLeft(DocumentSizeLengthInBytes));
            await Stream.WriteAsync(lenghDescriptor, 0, DocumentSizeLengthInBytes);
            await Stream.WriteAsync(bytes, 0, size);            
        }


        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public void WriteString(string jString, bool validate = true)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jString);
            WriteBytes(bytes, validate);
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public async Task WriteStringAsync(string jString, bool validate = true)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jString);
            await WriteBytesAsync(bytes, validate);
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance of T to write.</param>
        public void WriteObject(object instance)
        {
            WriteObjectAsync(instance).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance of T to write.</param>
        public async Task WriteObjectAsync(object instance)
        {
            string strObject = JsonConvert.SerializeObject(instance);
            await WriteStringAsync(strObject, false);
        }

        #endregion
    }
}
