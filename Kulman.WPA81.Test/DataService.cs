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

        public Task<IPResponse> Get()
        {
            return Get<IPResponse>("http://ip.jsontest.com");
        }

        public Task<PostUserLoginResponse> Post(PostUserLoginRequest request)
        {
            return Post<PostUserLoginResponse>("https://private-anon-36c13ade2-timdorr.apiary-mock.com/login", request);
        }
    }
}
