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
            return "http://ip.jsontest.com";
        }

        protected override Dictionary<string, string> GetRequestHeaders()
        {
            return new Dictionary<string, string>();
        }

        public Task<IPResponse> Get()
        {
            return Get<IPResponse>("/");
        }
    }
}
