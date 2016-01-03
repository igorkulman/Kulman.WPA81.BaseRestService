using System;
using System.Threading.Tasks;
using Windows.Web.Http;
using Kulman.WPA81.BaseRestService.Services.Exceptions;

namespace Kulman.WPA81.BaseRestService.Services.Extensions
{
    /// <summary>
    /// HttpResponseMessageExtensions class
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Ensure the Success status code
        /// </summary>
        /// <param name="response">Http response message</param>
        /// <returns>Task</returns>
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            response.Content?.Dispose();

            throw new SimpleHttpResponseException(response.StatusCode, content);
        }
    }
}