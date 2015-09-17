using System;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    /// <summary>
    /// Deserialization exception
    /// </summary>
    public class DeserializationException : DataServiceException
    {
        /// <summary>
        /// Original JSON data
        /// </summary>
        [NotNull]
        public string JsonData { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="data">JSON data</param>
        public DeserializationException([NotNull] string message, [NotNull] Exception innerException, [NotNull] string data)
            : base(message, innerException)
        {
            JsonData = data;
        }
    }
}
