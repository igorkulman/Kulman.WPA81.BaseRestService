using System;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class DeserializationException : DataServiceException
    {
        public DeserializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
