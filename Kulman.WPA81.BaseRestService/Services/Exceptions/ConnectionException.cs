using System;
using Windows.Web.Http;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class ConnectionException : DataServiceException
    {
        /// <summary>
        /// HTTP status code from the server
        /// </summary>
        [NotNull]
        public HttpStatusCode Status { get; private set; }

        public ConnectionException([NotNull] string message, [NotNull] Exception innerException, [NotNull] HttpStatusCode status)
            : base(message, innerException)
        {
            Status = status;
        }
    }
}
