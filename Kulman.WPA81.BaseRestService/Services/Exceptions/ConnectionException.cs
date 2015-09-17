using System;
using Windows.Web.Http;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    /// <summary>
    /// Connection exception
    /// </summary>
    public class ConnectionException : DataServiceException
    {
        /// <summary>
        /// HTTP status code from the server
        /// </summary>
        [NotNull]
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="status">HTTP status code</param>
        public ConnectionException([NotNull] string message, [NotNull] Exception innerException, [NotNull] HttpStatusCode status)
            : base(message, innerException)
        {
            Status = status;
        }
    }
}
