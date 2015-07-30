using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using JetBrains.Annotations;
using Kulman.WPA81.BaseRestService.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kulman.WPA81.BaseRestService.Services.Abstract
{
    /// <summary>
    /// Base class for JSON based REST services
    /// </summary>
    public abstract class BaseRestService
    {
        /// <summary>
        /// Http filter
        /// </summary>
        private readonly HttpBaseProtocolFilter _filter;

        /// <summary>
        /// Cookie manager
        /// </summary>
        protected HttpCookieManager CookieManager
        {
            get { return _filter.CookieManager; }
        }

        /// <summary>
        /// Ctor, creates Http filter
        /// </summary>
        protected BaseRestService()
        {
            _filter = CreateHttpFilter();
        }

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
        protected virtual Task OnBeforeRequest([NotNull] string url)
        {
            return Task.FromResult(1);
        }

        /// <summary>
        /// Can be overriden to set the default request headers
        /// </summary>
        /// <returns>Dictionary containing default request headers</returns>
        protected virtual Dictionary<string, string> GetRequestHeaders([NotNull] string requestUrl)
        {
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// REST Get
        /// </summary>        
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected Task<T> Get<T>([NotNull] string url)
        {
            return GetResponse<T>(url, HttpMethod.Get, null);
        }

        /// <summary>
        /// REST Get (RAW)
        /// </summary>        
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected Task<HttpResponseMessage> Get([NotNull] string url)
        {
            return GetRawResponse(url, HttpMethod.Get, null);
        }

        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        protected Task Delete([NotNull] string url)
        {
            return GetResponse(url, HttpMethod.Delete, null);
        }

        /// <summary>
        /// REST Put
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<T> Put<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, HttpMethod.Put, request);
        }

        /// <summary>
        /// REST Put (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<HttpResponseMessage> Put([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, HttpMethod.Put, request);
        }

        /// <summary>
        /// REST Post
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<T> Post<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, HttpMethod.Post, request);
        }

        /// <summary>
        /// REST Post (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<HttpResponseMessage> Post([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, HttpMethod.Post, request);
        }

        /// <summary>
        /// REST Patch
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<T> Patch<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, new HttpMethod("PATCH"), request);
        }

        /// <summary>
        /// REST Patch (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Task</returns>
        protected Task<HttpResponseMessage> Patch([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, new HttpMethod("PATCH"), request);
        }

        /// <summary>
        /// Override if you need custom HttpClientHandler
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        protected virtual HttpBaseProtocolFilter CreateHttpFilter()
        {
            var handler = new HttpBaseProtocolFilter { AutomaticDecompression = true };
            return handler;
        }

        /// <summary>
        /// Creates a HTTP Client instance
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <returns>HttpClient</returns>
        private HttpClient CreateHttpClient([NotNull] string requestUrl)
        {
            var client = new HttpClient(_filter);
            var headers = GetRequestHeaders(requestUrl);
            foreach (var key in headers.Keys)
            {
                client.DefaultRequestHeaders.TryAppendWithoutValidation(key, headers[key]);
            }

            return client;
        }

        /// <summary>
        /// Creates JSON serializer settings. Can be overridden.
        /// </summary>
        /// <returns>JSON serializer settings</returns>
        protected virtual JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }

        //TODO: merge with the typed version
        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <returns>Task</returns>
        private async Task GetResponse([NotNull] string url, [NotNull] HttpMethod method, [CanBeNull] object request)
        {
            await GetResponse<Object>(url, method, request, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets raw HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="noOutput">Output will not be proceed when true, method return default(T)</param>
        /// <returns>Task</returns>
        private async Task<HttpResponseMessage> GetRawResponse([NotNull] string url, [NotNull]  HttpMethod method, [CanBeNull] object request, bool noOutput = false)
        {
            await OnBeforeRequest(url).ConfigureAwait(false);

            HttpResponseMessage data = null;

            HttpStringContent requestcontent = null;

            var content = request as string;
            if (content != null)
            {
                requestcontent = new HttpStringContent(content);
            }
            else if (request != null)
            {
                requestcontent = new HttpStringContent(JsonConvert.SerializeObject(request, CreateJsonSerializerSettings()), UnicodeEncoding.Utf8, "application/json");
            }

            try
            {
                var fullUrl = (new[] { "http://", "https://" }).Any(url.StartsWith) ? url : GetBaseUrl() + url;

                var client = CreateHttpClient(fullUrl);

                var requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(fullUrl),
                    Content = requestcontent,
                };

                data = await client.SendRequestAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                return data;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, data?.StatusCode ?? HttpStatusCode.ExpectationFailed);
            }
        }

        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="noOutput">Output will not be proceed when true, method return default(T)</param>
        /// <returns>Task</returns>
        private async Task<T> GetResponse<T>([NotNull] string url, [NotNull]  HttpMethod method, [CanBeNull] object request, bool noOutput = false)
        {
            T result;
            var data = await GetRawResponse(url, method, request, noOutput);

            //deserialization and creation of the result
            using (var s = await data.Content.ReadAsInputStreamAsync())
            {
                using (var sr = new StreamReader(s.AsStreamForRead()))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        try
                        {
                            var serializer = new JsonSerializer();
                            result = serializer.Deserialize<T>(reader);
                        }
                        catch (Exception ex)
                        {
                            throw new DeserializationException("Error while processing response. See the inner exception for details.", ex, reader.ReadAsString());
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// REST Head
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Dictionary with headers</returns>
        public async Task<Dictionary<string, string>> Head([NotNull] string url)
        {
            await OnBeforeRequest(url).ConfigureAwait(false);

            try
            {
                var client = CreateHttpClient(GetBaseUrl() + url);
                var request = new HttpRequestMessage(HttpMethod.Head, new Uri(GetBaseUrl() + url));
                var response = await client.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);

                return response.Headers.ToDictionary(headerItem => headerItem.Key, headerItem => headerItem.Value);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, HttpStatusCode.ExpectationFailed);
            }
        }
    }
}
