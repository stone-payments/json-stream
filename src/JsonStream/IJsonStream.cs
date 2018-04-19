using Newtonsoft.Json.Linq;

namespace StoneCo.Utils.IO
{
    public interface IJsonStream
    {
        /// <summary>
        /// Returns the bytes of the document and move to the next document.
        /// </summary>
        /// <returns>The bytes of the document.</returns>
        byte[] ReadBytes();

        /// <summary>
        /// Returns a JArray and move to the next document.
        /// </summary>
        /// <returns>The JArray.</returns>
        JArray ReadJArray();

        /// <summary>
        /// Returns a JObject and move to the next document.
        /// </summary>
        /// <returns>The JObject.</returns>
        JObject ReadJson();

        /// <summary>
        /// Returns a JToken and move to the next document.
        /// </summary>
        /// <returns>The JToken.</returns>
        JToken ReadJToken();

        /// <summary>
        /// Returns an instance of T and move to the next document.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <returns>The T instance.</returns>
        T ReadObject<T>();

        /// <summary>
        /// Returns the string of the document and move to the next document.
        /// </summary>
        /// <returns>The string of the document.</returns>
        string ReadString();

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="bytes">The bytes of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        void WriteBytes(byte[] bytes, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jArray">The JArray to write.</param>
        void WriteJArray(JArray jArray);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jObject">The JObject to write.</param>
        void WriteJson(JObject jObject);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jToken">The JToken to write.</param>
        void WriteJToken(JToken jToken);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <param name="jString">The string of the document.</param>
        /// <param name="validate">Throws an exception if the bytes is not a valid Json document.</param>
        void WriteString(string jString, bool validate = true);

        /// <summary>
        /// Writes a document and move the pointer to the end of file.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance of T to write.</param>
        void WriteObject<T>(T instance);
    }
}
