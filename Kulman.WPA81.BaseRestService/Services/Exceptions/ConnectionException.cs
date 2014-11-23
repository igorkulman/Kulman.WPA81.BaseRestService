using System;
using System.Net;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class ConnectionException : DataServiceException
    {
        /// <summary>
        /// HTTP status code from the server
        /// </summary>
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// JSON response from the server
        /// </summary>
        public string Response { get; set; }

        public ConnectionException(string message, Exception innerException, HttpStatusCode status, string response)
            : base(message, innerException)
        {
            Status = status;
            Response = response;
        }
    }
}
