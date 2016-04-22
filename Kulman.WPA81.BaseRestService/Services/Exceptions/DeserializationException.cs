using System;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    /// <summary>
    /// Exception that occurs when the JSON parser fails to deserialize the data from the server. The incorrect data is included in this exception.
    /// </summary>
    public class DeserializationException : DataServiceException
    {
        /// <summary>
        /// Original JSON data
        /// </summary>
        [CanBeNull]
        public string JsonData { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="data">JSON data</param>
        public DeserializationException([NotNull] string message, [NotNull] Exception innerException, [CanBeNull] string data): base(message, innerException)
        {
            JsonData = data;
        }
    }
}
