using System;
using Windows.Web.Http;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    /// <summary>
    /// Exception thrown when response from the server indicates an error
    /// </summary>
    public class ConnectionException : DataServiceException
    {
        /// <summary>
        /// HTTP status code from the server
        /// </summary>
        [NotNull]
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// HttpResponseMessage content
        /// </summary>
        [CanBeNull]
        public string ResponseContent { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="status">HTTP status code</param>
        /// <param name="responseContent">HttpResponseMessage content</param>
        public ConnectionException([NotNull] string message, [NotNull] Exception innerException, [NotNull] HttpStatusCode status, [CanBeNull] string responseContent)
            : base(message, innerException)
        {
            Status = status;
            ResponseContent = responseContent;
        }
    }
}
