using System;

namespace Kulman.WPA81.BaseRestService.Services.Exceptions
{
    public class DataServiceException : Exception
    {
        /// <summary>
        /// User-friendly error description, suited to be displayed to the user.
        /// </summary>
        public string Description { get; protected set; }

        protected DataServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
#if DEBUG
            Description = innerException != null ? innerException.Message : message;
#else
			//Description = AppResources.GenericCommunicationErrorMessage;
#endif
        }
    }
}
