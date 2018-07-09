using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    /// ActivityPageUnitTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ActivityPageUnitTest
    {
        private ActivityClient _client;
        private ActivityPageInfoModuleBaseRequest request;

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [TestInitialize]
        public void TestInitialize()
        {
            _client = new ActivityClient();
            request = new ActivityPageInfoModuleBaseRequest()
            {
                HashKey = "0C96F72A",
                Channel = "wap"
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }

        [TestMethod]
        public void Refresh()
        {
            using (var cacheClient = new CacheClient())
            {
                var result = cacheClient.RefreshRedisCachePrefixForCommon(new RefreshCachePrefixRequest()
                {
                    Prefix = "activeinfo",
                    ClientName = "ActivityPageV2",
                    Expiration = TimeSpan.FromDays(1)
                });
                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void TestImage()
        {
            var result = _client.GetActivityPageInfoRowImages(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestSeckill()
        {
            var result = _client.GetActivityPageInfoRowSeckills(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestLink()
        {
            var result = _client.GetActivityPageInfoRowLinks(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestProduct()
        {
            var result = _client.GetActivityPageInfoRowProducts(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestCoupons()
        {
            var result = _client.GetActivityPageInfoRowCoupons(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestBys()
        {
            var result = _client.GetActivityPageInfoRowBys(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestRules()
        {
            var result = _client.GetActivityPageInfoRowRules(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestOthers()
        {
            var result = _client.GetActivityPageInfoRowOthers(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestOtherActivitys()
        {
            var result = _client.GetActivityPageInfoRowOtherActivitys(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestVideos()
        {
            var result = _client.GetActivityPageInfoRowVideos(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestPintuans()
        {
            var result = _client.GetActivityPageInfoRowPintuans(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestJsons()
        {
            var result = _client.GetActivityPageInfoRowJsons(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestActivityTexts()
        {
            var result = _client.GetActivityPageInfoRowActivityTexts(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestCountDowns()
        {
            var result = _client.GetActivityPageInfoRowCountDowns(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestNewPools()
        {
            var requestPool = new ActivityPageInfoModuleNewProductPoolRequest()
            {
                Channel = "wap",
                HashKey = "0C96F72A",
                VehicleId = "VE-AUDF99AB",
                Width = "205",
                AspectRatio = "55",
                Rim = "16",
                Tid = "102429"
            };
            var result = _client.GetActivityPageInfoRowNewPools(requestPool);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestPools()
        {
            var requestPool = new ActivityPageInfoModuleProductPoolRequest()
            {
                Channel = "wap",
                HashKey = "0C96F72A",
                VehicleId = "VE-AUDF99AB",
                Width = "205",
                AspectRatio = "55",
                Rim = "16",
                CityId = "1",
                Tid = "102429"
            };
            var result = _client.GetActivityPageInfoRowNewPools(requestPool);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestMenus()
        {
            var result = _client.GetActivityPageInfoRowMenus(request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestTireRecommends()
        {
            var recommend=new ActivityPageInfoModuleRecommendRequest()
            {
                Channel = "wap",
                HashKey = "0C96F72A",
                VehicleId = "VE-AUDF99AB",
                Width = "205",
                AspectRatio = "55",
                Rim = "16",
                CityId = "1",
                Tid = "102429",
                RecommendCount = 5
            };
            var result = _client.GetActivityPageInfoTireRecommends(recommend);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestCpRecommends()
        {
            var recommend = new ActivityPageInfoModuleRecommendRequest()
            {
                Channel = "wap",
                HashKey = "0C96F72A",
                VehicleId = "VE-AUDF99AB",
                Width = "205",
                AspectRatio = "55",
                Rim = "16",
                CityId = "1",
                Tid = "102429",
                RecommendCount = 5
            };
            var result = _client.GetActivityPageInfoCpRecommends(recommend);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestActivityPage()
        {
            var requestpage = new ActivityPageInfoRequest
            {
                HashKey = "0C96F72A",
                Channel = "wap",
                UserId = new Guid("21fa116e-5aaf-4dd8-9ee0-c2d93cb026c6")
            };
            var result = _client.GetActivityPageInfoConfigModel(requestpage);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestVehicleBanner()
        {
            var bannerRequest = new ActivityPageInfoModuleVehicleBannerRequest()
            {
                Channel = "wap",
                HashKey = "0C96F72A",
                VehicleId = "VE-AUDF99AB"
            };
            var result = _client.GetActivityPageInfoRowVehicleBanners(bannerRequest);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
    }
}
