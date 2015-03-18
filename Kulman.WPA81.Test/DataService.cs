using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kulman.WPA81.Test.Data;

namespace Kulman.WPA81.Test
{
    public class DataService: BaseRestService.Services.Abstract.BaseRestService
    {
        protected override string GetBaseUrl()
        {
            return "";
        }

        protected override Dictionary<string, string> GetRequestHeaders(string url)
        {
            return new Dictionary<string, string>();
        }

        public Task<IPResponse> Get()
        {
            return Get<IPResponse>("http://ip.jsontest.com");
        }

        public Task<PostUserLoginResponse> Post(PostUserLoginRequest request)
        {
            return Post<PostUserLoginResponse>("https://private-34396-gooddata.apiary-mock.com/gdc/account/login", request);
        }
    }
}
