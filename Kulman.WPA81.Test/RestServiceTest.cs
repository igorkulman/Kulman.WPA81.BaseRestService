using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kulman.WPA81.Test.Data;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Kulman.WPA81.Test
{
    [TestClass]
    public class RestServiceTest
    {
        private DataService _dataService;

        [TestInitialize]
        public void Init()
        {
            _dataService=new DataService();
        }

        [TestMethod]
        public async Task TestGet()
        {
            var data = await _dataService.Get();
            Assert.IsNotNull(data);
            Assert.IsFalse(String.IsNullOrEmpty(data.Ip));
        }

        [TestMethod]
        public async Task TestPost()
        {
            var data = await _dataService.Post(new PostUserLoginRequest());
            Assert.IsNotNull(data);
        }
    }
}
