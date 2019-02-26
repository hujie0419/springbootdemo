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
        private ActivityPageInfoModuleBaseRequest _request;

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
            _request = new ActivityPageInfoModuleBaseRequest()
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
            var requestImage = new ActivityPageInfoModuleBaseRequest()
            {
                HashKey = "0C96F72A",
                Channel = "wap",
                //RowNums = new List<string>
                //{
                //    "20180516102214773",
                //    "20180516102409139",
                //    "20180516102551813"
                //}
            };
            var result = _client.GetActivityPageInfoRowImages(requestImage);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestSeckill()
        {
            var result = _client.GetActivityPageInfoRowSeckills(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestLink()
        {
            var result = _client.GetActivityPageInfoRowLinks(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestProduct()
        {
            var result = _client.GetActivityPageInfoRowProducts(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestCoupons()
        {
            var result = _client.GetActivityPageInfoRowCoupons(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestBys()
        {
            var result = _client.GetActivityPageInfoRowBys(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestRules()
        {
            var result = _client.GetActivityPageInfoRowRules(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestOthers()
        {
            var result = _client.GetActivityPageInfoRowOthers(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestOtherActivitys()
        {
            var result = _client.GetActivityPageInfoRowOtherActivitys(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestVideos()
        {
            var result = _client.GetActivityPageInfoRowVideos(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestPintuans()
        {
            var result = _client.GetActivityPageInfoRowPintuans(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestJsons()
        {
            var result = _client.GetActivityPageInfoRowJsons(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void TestActivityTexts()
        {
            var result = _client.GetActivityPageInfoRowActivityTexts(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestCountDowns()
        {
            var result = _client.GetActivityPageInfoRowCountDowns(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestNewPools()
        {
            var requestPool = new ActivityPageInfoModuleNewProductPoolRequest()
            {
                Channel = "wap",
                HashKey = "F3AC38EC",
                VehicleId = "VE-JEJ7YSDZ",
                Width = "205",
                AspectRatio = "55",
                Rim = "16",
                Tid = "108627"
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
            var result = _client.GetActivityPageInfoRowMenus(_request);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestTireRecommends()
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



        /// <summary>
        /// 根据活动编号查询配置信息测试
        /// </summary>
        [TestMethod]
        public void TestGetCustomerSettingInfo()
        {
            string activityExclusiveId = "B79402E5E00937B4903FCFF9DF84EF70";
            var result = _client.GetCustomerSettingInfo(activityExclusiveId);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        /// 查询用户绑定的券码测试
        /// </summary>
        [TestMethod]
        public void TestGetUserCouponCode()
        {
            string activityExclusiveId = "B79402E5E00937B4903FCFF9DF84EF70";
            string userid = "99f4ae1e-a4c7-48b4-b79c-bbeeb78bb675";
            var result = _client.GetUserCouponCode(activityExclusiveId, userid);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        /// 用户券码绑定测试
        /// </summary>
        [TestMethod]
        public void TestCouponCodeBound()
        {
            var activityCustomerCouponRequests = new ActivityCustomerCouponRequests()
            {
                ActivityExclusiveId = "B79402E5E00937B4903FCFF9DF84EF70",
                CouponCode = "QBI3",
                UserId = new Guid("99f4ae1e-a4c7-48b4-b79c-bbeeb78bb271"),
                UserName = "devteam@tuhu.work",
                Phone = "15221617501"

            };

            var result = _client.CouponCodeBound(activityCustomerCouponRequests);
            //result.ThrowIfException(true);
            //Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///客户专享活动下单验证（锦湖员工轮胎）测试
        /// </summary>
        [TestMethod]
        public void TestActiveOrderVerify()
        {
            var activityOrderVerifyRequests = new ActivityOrderVerifyRequests()
            {
                UserId = new Guid("99f4ae1e-a4c7-48b4-b79c-bbeeb78bb671"),
                ActivityId = "5bb4a4e3-c2b4-4f67-9570-72bbfaaeff15"
            };

            var result = _client.ActiveOrderVerify(activityOrderVerifyRequests);
            //result.ThrowIfException(true);
            //Assert.IsNotNull(result.Result);
        }


        /// <summary>
        /// 刷新大客户活动配置缓存
        /// </summary>
        /// <param name="activityExclusiveId"></param>
        /// <returns></returns>
        [TestMethod]
        public void RefreshRedisCacheCustomerSetting()
        {
            using (var client = new CacheClient())
            {
                string activityExclusiveId = "1AEB59084518F994CD33C0BEC18CAE17";
                var getResult = client.RefreshRedisCacheCustomerSetting(activityExclusiveId);
                getResult.ThrowIfException(true);
                Assert.IsNotNull(getResult.Result);
            }
        }

        /// <summary>
        /// 根据限时抢购ID查询配置信息测试
        /// </summary>
        [TestMethod]
        public void TestGetVipCustomerSettingInfoByActivityId()
        {
            string activityId = "5bb4a4e3-c2b4-4f67-9570-72bbfaaeff55";
            var result = _client.GetVipCustomerSettingInfoByActivityId(activityId);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
    }
}
