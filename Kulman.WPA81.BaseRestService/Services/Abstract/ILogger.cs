using System;

namespace Kulman.WPA81.BaseRestService.Services.Abstract
{
    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">message</param>
        void Info(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">message</param>
        void Error(string message);

        /// <summary>
        /// Logs a debug message with exception
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="e">Exception</param>
        void Error(string message, Exception e);
    }
}
