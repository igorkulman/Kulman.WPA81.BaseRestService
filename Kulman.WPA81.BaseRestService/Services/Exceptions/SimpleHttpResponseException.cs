using System;
using Windows.Web.Http;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    /// <summary>
    /// Simple HttpResponse exception
    /// </summary>
    public class SimpleHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="statusCode">Http status code</param>
        /// <param name="content">Response content</param>
        public SimpleHttpResponseException(HttpStatusCode statusCode, [NotNull] string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}