using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kulman.WPA81.BaseRestService.Code;
using Kulman.WPA81.BaseRestService.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kulman.WPA81.BaseRestService.Services.Abstract
{
    public abstract class BaseRestService
    {
        protected abstract string GetBaseUrl();

        protected virtual Task OnBeforeRequest()
        {
            return Task.FromResult(1);
        }

        protected abstract Dictionary<string, string> GetRequestHeaders();

        protected Task<T> Get<T>(string url)
        {
            return GetResponse<T>(url, HttpMethod.Get, null);
        }

        protected Task Delete(string url)
        {
            return GetResponse(url, HttpMethod.Delete, null);
        }

        protected Task<T> Put<T>(string url, object request)
        {
            return GetResponse<T>(url, HttpMethod.Put, request);
        }

        protected Task<T> Post<T>(string url, object request)
        {
            return GetResponse<T>(url, HttpMethod.Post, request);
        }

        protected Task<T> Patch<T>(string url, object request)
        {
            return GetResponse<T>(url, HttpMethod.Trace, request);
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);
            var headers = GetRequestHeaders();
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

        private async Task GetResponse(string url, HttpMethod method, object request)
        {
            await OnBeforeRequest();

            //string json = string.Empty;
            HttpResponseMessage data = null;

            try
            {
                var client = CreateHttpClient();
                if (method == HttpMethod.Get)
                {
                    data = await client.GetAsync(GetBaseUrl() + url);
                }
                else if (method == HttpMethod.Delete)
                {
                    data = await client.DeleteAsync(GetBaseUrl() + url);
                }
                else if (method == HttpMethod.Post)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PostAsync(new Uri(GetBaseUrl() + url), content);
                }
                else if (method == HttpMethod.Put)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PutAsync(new Uri(GetBaseUrl() + url), content);
                }
                else if (method == HttpMethod.Trace)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PatchAsync(new Uri(GetBaseUrl() + url), content);
                }
                else
                {
                    throw new NotImplementedException();
                }
                //json = await data.Content.ReadAsStringAsync();
                // *******************
                // DEBUG INFO
                // *******************
                Debug.WriteLine("RESPONSE:" + url);
                Debug.WriteLine(data.StatusCode);
                // *******************
                //data.EnsureSuccessStatusCode();
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

        private async Task<T> GetResponse<T>(string url, HttpMethod method, object request)
        {
            string json = string.Empty;
            HttpResponseMessage data = null;

            try
            {
                var client = CreateHttpClient();
                if (method == HttpMethod.Get)
                {
                    data = await client.GetAsync(GetBaseUrl() + url);
                }
                else if (method == HttpMethod.Delete)
                {
                    data = await client.DeleteAsync(GetBaseUrl() + url);
                }
                else if (method == HttpMethod.Post)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PostAsync(new Uri(GetBaseUrl() + url), content);
                }
                else if (method == HttpMethod.Put)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PutAsync(new Uri(GetBaseUrl() + url), content);
                }
                else if (method == HttpMethod.Trace)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    data = await client.PatchAsync(new Uri(GetBaseUrl() + url), content);
                }
                else
                {
                    throw new NotImplementedException();
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
                throw new DeserializationException("Error while processing response. See the inner exception for details.", ex);
            }

            return result;
        }
    }
}
