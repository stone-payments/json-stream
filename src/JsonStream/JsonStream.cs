using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// The default buffer size to pass as argument on creating a FileStream.
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 2 << 18;

        #endregion

        #region Private field

        private Object LockObject = new Object();

        #endregion

        #region Protected fields

        /// <summary>
        /// The stream to use.
        /// </summary>
        protected Stream Stream;

        /// <summary>
        /// Used to write bytes on a file.
        /// </summary>
        protected BinaryWriter BinaryWriter;

        /// <summary>
        /// Used to read bytes from a file.
        /// </summary>
        protected BinaryReader BinaryReader;

        #endregion

        #region Public properties

        /// <summary>
        /// The number of bytes to representing the size of the json document.
        /// </summary>
        public int DocumentSizeLengthInBytes { get; set; }

        /// <summary>
        /// The choosed mode.
        /// </summary>
        public Modes Mode { get; set; }

        /// <summary>
        /// The pointer position in the Stream.
        /// </summary>
        public long Position { get { return this.Stream.Position; } set { this.Stream.Position = value; } }

        /// <summary>
        /// Flag to verify if the instance was created using one of the optimized constructors.
        /// </summary>
        public bool IsUsingOptimizedConstructor { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor receiving a Stream. Do not use this constructor and do not change the default values if you don't know exactly what are you doing.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="mode">ReadOnly, WriteOnly or ReadAndWrite.</param>
        /// <param name="documentSizeLengthInBytes">The length of the document size descriptor.</param>
        public JsonStream(Stream stream, Modes mode = Modes.ReadAndWrite, int documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH)
        {
            Mode = mode;
            this.Initialize(stream, documentSizeLengthInBytes);
        }

        /// <summary>
        /// Optimized constructor to read or write a sequential json file. Do not change the default values if you don't know exactly what are you doing.
        /// </summary>
        /// <param name="mode">ReadOnly, WriteOnly or ReadAndWrite.</param>
        /// <param name="filePath">The filePath.</param>
        /// <param name="documentSizeLengthInBytes">The length of the document size descriptor.</param>
        /// <param name="bufferSize">The buffer size.</param>
        public JsonStream(Modes mode, string filePath, int documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH, int bufferSize = DEFAULT_BUFFER_SIZE)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            FileMode fileMode;
            FileAccess fileAccess;
            FileShare fileShare;
            FileOptions fileOptions;

            Mode = mode;
            if (Mode == Modes.WriteOnly)
            {
                fileMode = FileMode.Append;
                fileAccess = FileAccess.Write;
                fileShare = FileShare.ReadWrite;
                fileOptions = FileOptions.SequentialScan;
            }
            else if(Mode == Modes.ReadOnly)
            {
                fileMode = FileMode.Open;
                fileAccess = FileAccess.Read;
                fileShare = FileShare.ReadWrite;
                fileOptions = FileOptions.SequentialScan;
            }
            else
            {
                fileMode = FileMode.OpenOrCreate;
                fileAccess = FileAccess.ReadWrite;
                fileShare = FileShare.ReadWrite;
                fileOptions = FileOptions.RandomAccess;
            }           

            FileStream stream = new FileStream(filePath, fileMode, fileAccess, fileShare, bufferSize, fileOptions);
            this.Initialize(stream, documentSizeLengthInBytes);
            this.IsUsingOptimizedConstructor = true;
        }

        #endregion

        #region IDisposable Support

        ~JsonStream()
        {
            Dispose(false);
        }

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
                    this.Stream.Flush();
                    this.Stream.Dispose();
                    this.Stream = null;
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
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected void Initialize(Stream stream, int documentSizeLengthInBytes = DEFAULT_DOCUMENT_SIZE_LENGTH)
        {
            #region Validations

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (documentSizeLengthInBytes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(documentSizeLengthInBytes), "Please reserve at least one byte to represent the size of the document.");
            }

            #endregion

            this.Stream = stream;

            if (this.Stream.CanWrite)
            {
                this.BinaryWriter = new BinaryWriter(Stream);
            }

            if (this.Stream.CanRead)
            {
                this.BinaryReader = new BinaryReader(Stream);
            }

            this.DocumentSizeLengthInBytes = documentSizeLengthInBytes;

            JsonConvert.DefaultSettings = () => Serialization.Settings;
        }

        /// <summary>
        /// Returns the size in bytes of the next document or zero if the end of the stream was reached.
        /// </summary>
        /// <returns>The size in bytes of the next document or zero if the end of the stream was reached.</returns>
        protected async Task<int> GetNextDocumentSizeAsync()
        {            
            byte[] documentLengthDescriptor = new byte[DocumentSizeLengthInBytes];
            int readBytes = await Stream.ReadAsync(documentLengthDescriptor, 0, DocumentSizeLengthInBytes);

            return this.CalculateNextDocumentSize(readBytes, documentLengthDescriptor);
        }

        /// <summary>
        /// Returns the size in bytes of the next document or zero if the end of the stream was reached.
        /// </summary>
        /// <returns>The size in bytes of the next document or zero if the end of the stream was reached.</returns>
        protected int GetNextDocumentSize()
        {
            byte[] documentLengthDescriptor = new byte[DocumentSizeLengthInBytes];
            int readBytes = this.BinaryReader.Read(documentLengthDescriptor, 0, DocumentSizeLengthInBytes);

            return this.CalculateNextDocumentSize(readBytes, documentLengthDescriptor);
        }

        /// <summary>
        /// Calculate the next document size.
        /// </summary>
        /// <param name="readBytes">The length of the last read bytes from the stream.</param>
        /// <param name="documentSizeDescriptor">The last read bytes from the stream.</param>
        /// <returns>The next document size.</returns>
        protected int CalculateNextDocumentSize(int readBytes, byte[] documentSizeDescriptor)
        {
            if (readBytes == 0)
            {
                return readBytes;
            }

            int documentSize;
            if (readBytes != this.DocumentSizeLengthInBytes)
            {
                throw new InvalidDocumentSizeLengthException(this.DocumentSizeLengthInBytes, readBytes);
            }

            try
            {
                documentSize = int.Parse(System.Text.Encoding.UTF8.GetString(documentSizeDescriptor));
            }
            catch (Exception ex)
            {
                throw new InvalidDocumentSizeLengthException("Error interpreting document size.", ex);
            }

            return documentSize;
        }

        /// <summary>
        /// Run the validations and returns the document size descriptor.
        /// </summary>
        /// <param name="bytes">Bytes to write.</param>
        /// <param name="validate">If true performs a Json validation and throws an exception if is invalid.</param>
        /// <returns>The document size descriptor.</returns>
        protected byte[] PrepareWrite(byte[] bytes, bool validate = true)
        {
            #region Validate

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if(bytes.Length == 0)
            {
                throw new ArgumentException("Can't write an empty byte array.", nameof(bytes));
            }

            #endregion

            if (validate)
            {
                JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
            }

            return Encoding.UTF8.GetBytes(bytes.Length.ToString().PadLeft(DocumentSizeLengthInBytes));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Writes the in memory buffer to disk.
        /// </summary>
        public void Flush() => Stream.Flush();

        /// <summary>
        /// Writes the in memory buffer to disk asynchronously.
        /// </summary>
        public async Task FlushAsync() => await Stream.FlushAsync();

        #region ReadMethods

        /// <summary>
        /// Returns the bytes of the document or null if the end of the stream was reached and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document or null if the end of the stream was reached.</returns>
        public byte[] ReadBytes()
        {
            if (Mode == Modes.WriteOnly)
            {
                throw new ForbiddenOperationException("Can't read in WriteOnly mode.");
            }

            lock (LockObject)
            {
                int nextDocumentSize = GetNextDocumentSize();

                if (nextDocumentSize == 0)
                {
                    return null;
                }

                byte[] documentBytes = new byte[nextDocumentSize];
                int readBytes = this.BinaryReader.Read(documentBytes, 0, nextDocumentSize);

                if (readBytes != nextDocumentSize)
                {
                    throw new InvalidJsonDocumentException($"Cant't read all bytes of the json document at position {Position}.", this.Position);
                }

                return documentBytes;
            }
        }

        /// <summary>
        /// Returns the bytes of the document or null if the end of the stream was reached and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document or null if the end of the stream was reached.</returns>
        public async Task<byte[]> ReadBytesAsync()
        {
            if (this.IsUsingOptimizedConstructor)
            {
                throw new ForbiddenOperationException("Do not call any async method when using optimized constructor.");
            }

            if(Mode == Modes.WriteOnly)
            {
                throw new ForbiddenOperationException("Can't read in WriteOnly mode.");
            }

            int nextDocumentSize = GetNextDocumentSize();

            if (nextDocumentSize == 0)
            {
                return null;
            }

            byte[] documentBytes = new byte[nextDocumentSize];
            int readBytes = await this.Stream.ReadAsync(documentBytes, 0, nextDocumentSize);

            if (readBytes != nextDocumentSize)
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
            return ReadObject<JArray>();
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
            return ReadObject<JObject>();
        }

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        public async Task<JObject> ReadJObjectAsync()
        {
            return await ReadObjectAsync<JObject>();
        }

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        public JToken ReadJToken()
        {
            return ReadObject<JToken>();
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
            string strJson = ReadString();
            if (strJson == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(strJson);
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
            byte[] readBytes = ReadBytes();
            if (readBytes == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(readBytes);
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

        #endregion

        #region Write methods

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public void WriteBytes(byte[] bytes, bool validate = true)
        {
            if (Mode == Modes.ReadOnly)
            {
                throw new ForbiddenOperationException("Can't write in ReadOnly mode.");
            }

            lock (LockObject)
            {
                byte[] lengthDescriptor = PrepareWrite(bytes, validate);

                this.BinaryWriter.Write(lengthDescriptor, 0, lengthDescriptor.Length);
                this.BinaryWriter.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        public async Task WriteBytesAsync(byte[] bytes, bool validate = true)
        {
            if (this.IsUsingOptimizedConstructor)
            {
                throw new ForbiddenOperationException("Do not call any async method when using optimized constructor.");
            }

            if (Mode == Modes.ReadOnly)
            {
                throw new ForbiddenOperationException("Can't write in ReadOnly mode.");
            }
            
            byte[] lengthDescriptor = PrepareWrite(bytes, validate);

            await Stream.WriteAsync(lengthDescriptor, 0, lengthDescriptor.Length);
            await Stream.WriteAsync(bytes, 0, bytes.Length);
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
            string strObject = JsonConvert.SerializeObject(instance);
            WriteString(strObject, false);
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

        #endregion
    }
}
