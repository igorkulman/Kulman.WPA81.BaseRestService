using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kulman.WPA81.BaseRestService.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kulman.WPA81.BaseRestService.Services.Abstract
{
    /// <summary>
    /// Base class for JSON based REST services
    /// </summary>
    public abstract class BaseRestService
    {
        /// <summary>
        /// Must be overriden to set the Base URL
        /// </summary>
        /// <returns>Base URL</returns>
        protected abstract string GetBaseUrl();

        /// <summary>
        /// Executed before every request
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected virtual Task OnBeforeRequest(string url)
        {
            return Task.FromResult(1);
        }

        /// <summary>
        /// Must be overriden to set the default request headers
        /// </summary>
        /// <returns>Dictionary containing default request headers</returns>
        protected abstract Dictionary<string, string> GetRequestHeaders(string requestUrl);        

        /// <summary>
        /// REST Get
        /// </summary>        
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected Task<T> Get<T>(string url)
        {
            return GetResponse<T>(url, HttpMethod.Get, null);
        }

        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected Task Delete(string url)
        {
            return GetResponse(url, HttpMethod.Delete, null);
        }

        /// <summary>
        /// REST Put
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON)</param>
        /// <returns>Task</returns>
        protected Task<T> Put<T>(string url, object request)
        {
            return GetResponse<T>(url, HttpMethod.Put, request);
        }

        /// <summary>
        /// REST Post
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON)</param>
        /// <returns>Task</returns>
        protected Task<T> Post<T>(string url, object request)
        {
            return GetResponse<T>(url, HttpMethod.Post, request);
        }

        /// <summary>
        /// REST Patch
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON)</param>
        /// <returns>Task</returns>
        protected Task<T> Patch<T>(string url, object request)
        {
            return GetResponse<T>(url, new HttpMethod("PATCH"), request);
        }

        /// <summary>
        /// Override if you need custom HttpClientHandler
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        protected virtual HttpClientHandler CreateHttpClientHandler()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            return handler;
        }

        /// <summary>
        /// Creates a HTTP Client instance
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <returns>HttpClient</returns>
        private HttpClient CreateHttpClient(string requestUrl)
        {
            var client = new HttpClient(CreateHttpClientHandler());
            var headers = GetRequestHeaders(requestUrl);
            foreach (var key in headers.Keys)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(key, headers[key]);
            }

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            return client;
        }

        //TODO: merge with the typed version
        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <returns>Task</returns>
        private async Task GetResponse(string url, HttpMethod method, object request)
        {
            await GetResponse<Object>(url, method, request, true);
        }

        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="noOutput">Output will not be proceed when true, method return default(T)</param>
        /// <returns>Task</returns>
        private async Task<T> GetResponse<T>(string url, HttpMethod method, object request, bool noOutput = false)
        {
            await OnBeforeRequest(url);

            string json = string.Empty;
            HttpResponseMessage data = null;

            try
            {
                var client = CreateHttpClient(GetBaseUrl() + url);

                var requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(GetBaseUrl() + url),
                    Content = request != null ? new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json") : null,
                };

                data = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                if (noOutput)
                {
                    return default(T);
                }

                json = await data.Content.ReadAsStringAsync();
                // *******************
                // DEBUG INFO
                // *******************
                Debug.WriteLine("RESPONSE:" + url);
                Debug.WriteLine(json.Length < 1000 ? json : json.Substring(0, Math.Min(json.Length, 1000)));
                // *******************
                data.EnsureSuccessStatusCode();
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, data != null ? data.StatusCode : HttpStatusCode.ExpectationFailed, json);
            }

            T result;

            try
            {
                //deserialization and creation of the result
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new DeserializationException("Error while processing response. See the inner exception for details.", ex, json);
            }

            return result;
        }

        /// <summary>
        /// REST Head
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Dictionary with headers</returns>
        public async Task<Dictionary<string, IEnumerable<string>>> Head(string url)
        {
            await OnBeforeRequest(url);

            HttpResponseMessage data = null;

            try
            {
                var client = CreateHttpClient(GetBaseUrl() + url);
                var request = new HttpRequestMessage(HttpMethod.Head, GetBaseUrl() + url);
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                return response.Headers.ToDictionary(headerItem => headerItem.Key, headerItem => headerItem.Value);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, data != null ? data.StatusCode : HttpStatusCode.ExpectationFailed, string.Empty);
            }
        }
    }
}
