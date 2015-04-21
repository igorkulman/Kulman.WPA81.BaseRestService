using System.Threading.Tasks;
using Kulman.WPA81.BaseRestService.Tests.Code;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Kulman.WPA81.BaseRestService.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        private readonly DataService _dataService;

        public DataServiceTests()
        {
            _dataService = new DataService();
        }

        [TestMethod]
        public async Task TestGet()
        {
            var res = await _dataService.TestGet();
            Assert.IsNotNull(res);
            Assert.AreEqual("two", res.One);
            Assert.AreEqual("value", res.Key);
        }
    }
}
