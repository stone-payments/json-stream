using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace StoneCo.Utils.IO
{
    public interface IJsonStream : IDisposable
    {

        /// <summary>
        /// The number of bytes to representing the size of the json document.
        /// </summary>
        int DocumentSizeLengthInBytes { get; set; }

        /// <summary>
        /// The pointer position in the Stream.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Returns the bytes of the document and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document.</returns>
        byte[] ReadBytes();

        /// <summary>
        /// Returns the bytes of the document and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document.</returns>
        Task<byte[]> ReadBytesAsync();

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        JArray ReadJArray();

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        Task<JArray> ReadJArrayAsync();

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        JObject ReadJObject();

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        Task<JObject> ReadJObjectAsync();

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        JToken ReadJToken();

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        Task<JToken> ReadJTokenAsync();

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        T ReadObject<T>();

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        Task<T> ReadObjectAsync<T>();

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        string ReadString();

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        Task<string> ReadStringAsync();

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        void WriteBytes(byte[] bytes, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        Task WriteBytesAsync(byte[] bytes, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        void WriteString(string jString, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        Task WriteStringAsync(string jString, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        void WriteObject(object instance);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        Task WriteObjectAsync(object instance);
    }
}
