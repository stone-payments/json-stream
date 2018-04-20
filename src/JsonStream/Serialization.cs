using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.Utils.IO
{
    /// <summary>
    /// Class with serialization settings.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Serializer used by this class.
        /// </summary>
        public static JsonSerializer Serializer { get; set; }

        /// <summary>
        /// Json serializer settings.
        /// </summary>
        public static JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Serialization()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateParseHandling = DateParseHandling.None
            };

            Serializer = JsonSerializer.Create(Settings);
        }

    }
}
