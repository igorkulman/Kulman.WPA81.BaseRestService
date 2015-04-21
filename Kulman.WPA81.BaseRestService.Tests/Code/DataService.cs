using System.Threading.Tasks;

namespace Kulman.WPA81.BaseRestService.Tests.Code
{
    public class DataService: Services.Abstract.BaseRestService
    {
        public class Response
        {
            public string One { get; set; }
            public string Key { get; set; }
        }

        protected override string GetBaseUrl()
        {
            return "http://echo.jsontest.com/";
        }

        public Task<Response> TestGet()
        {
            return Get<Response>("/key/value/one/two");
        }
    }
}
