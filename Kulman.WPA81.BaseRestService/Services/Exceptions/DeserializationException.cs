using System;
using JetBrains.Annotations;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class DeserializationException : DataServiceException
    {
        [NotNull]
        public string JsonData { get; private set; }

        public DeserializationException([NotNull] string message, [NotNull] Exception innerException, [NotNull] string data)
            : base(message, innerException)
        {
            JsonData = data;
        }
    }
}
