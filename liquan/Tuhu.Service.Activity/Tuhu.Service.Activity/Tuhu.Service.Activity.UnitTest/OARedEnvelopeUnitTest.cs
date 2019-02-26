using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    ///     公众号领红包 - 测试类
    /// </summary>
    [TestClass]
    public class OARedEnvelopeUnitTest
    {
        private OARedEnvelopeClient _client;

        public OARedEnvelopeUnitTest()
        {
            _client = new OARedEnvelopeClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }

        /// <summary>
        ///     测试：公众号领红包活动详情
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeActivityInfoAsync()
        {
            var result = _client.OARedEnvelopeActivityInfoNoCache();

            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     测试：公众号领红包 - 每日数据初始化
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeDailyDataInit()
        {
            var result = _client.OARedEnvelopeDailyDataInit(new Models.Requests.OARedEnvelopeDailyDataInitRequest()
            {
                Date = DateTime.Now,

            });


            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///     测试：公众号领红包 - 删除用户数据
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeUserReceiveDelete()
        {
            var result = _client.OARedEnvelopeUserReceiveDelete(Guid.Parse("a5e65a7a-0cef-4fc3-b364-291182034b24"));


            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     测试：公众号领红包 - 用户领取
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeUserReceive()
        {
            var result = _client.OARedEnvelopeUserReceive(new Models.Requests.OARedEnvelopeUserReceiveRequest()
            {
                OfficialAccountType = 1,
                OpenId = "0021313",
                WXNickName = "啦啦啦啦啦啦啦啦啦啦啦啦",
                WXHeadPicUrl = "https://s1.ax2x.com/2018/09/14/5FrwN3.jpg",
                UserId = Guid.Parse("a5e65a7a-0cef-4fc3-b364-291182034b24")
            });

            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///     测试：公众号领红包 - 用户领取 100人
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeUserReceive100()
        {
            for (int i = 0; i < 100; i++)
            {
                var result = _client.OARedEnvelopeUserReceive(new Models.Requests.OARedEnvelopeUserReceiveRequest()
                {
                    OfficialAccountType = 1,
                    OpenId = "0021313",
                    WXNickName = "啦啦啦啦啦啦啦啦啦啦啦啦",
                    WXHeadPicUrl = "https://s1.ax2x.com/2018/09/14/5FrwN3.jpg",
                    UserId = Guid.NewGuid()
                });

                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
            }

        }

        /// <summary>
        ///     测试：公众号领红包 - 同步统计
        /// </summary>
        [TestMethod]
        public void TestOARedEnvelopeStatisticsUpdate()
        {
            var result = _client.OARedEnvelopeStatisticsUpdate(new OARedEnvelopeStatisticsUpdateRequest()
            {
                StatisticsDate = DateTime.Now
            });

            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     测试：公众号领红包 - 返回生成的红包数据
        /// </summary>
        [TestMethod]
        public void TestGetAllOARedEnvelopeDailyData()
        {
            var result = _client.GetAllOARedEnvelopeDailyData(new GetAllOARedEnvelopeDailyDataRequest()
            {
                 Date  = DateTime.Now,
                 OfficialAccountType = 1
            });

            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
    }
}
