using System;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class DeserializationException : DataServiceException
    {
        public string JsonData { get; private set; }

        public DeserializationException(string message, Exception innerException, string data)
            : base(message, innerException)
        {
            JsonData = data;
        }
    }
}
