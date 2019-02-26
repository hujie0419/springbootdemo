using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    ///     七龙珠 单元测试
    /// </summary>
    [TestClass]
    public class DragonBallUnitTest
    {
        private DragonBallClient _client;



        [TestInitialize]
        public void TestInitialize()
        {
            _client = new DragonBallClient();

        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }


        /// <summary>
        ///    用户当前龙珠总数/兑换次数
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallUserInfo()
        {
            var result = _client.DragonBallUserInfo(new Models.Requests.DragonBallUserInfoRequest()
            {
                UserId = Guid.Parse("46b5fea0-6035-48f0-8efb-d3f0e9fcbc77")
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    获奖轮播
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DragonBallBroadcast()
        {
            var result = _client.DragonBallBroadcast(10);
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    我的战利品
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void DragonBallUserLootList()
        {
            var result = _client.DragonBallUserLootList(new DragonBallUserLootListRequest()
            {
                UserId = Guid.Empty
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    用户任务列表
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallUserMissionList()
        {
            var result = _client.DragonBallUserMissionList(new DragonBallUserMissionListRequest()
            {
                UserId = Guid.Parse("f4148ea6-9274-4c4e-bf00-320caa49f22d")
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    领取奖励
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallUserMissionReward()
        {
            var result = _client.DragonBallUserMissionReward(new Models.Requests.DragonBallUserMissionRewardRequest()
            {
                UserId = Guid.Parse("3b4b3ec6-326f-4d1b-bfbf-54f1d337d819"),
                MissionId = 3
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    召唤神龙啦
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallSummon()
        {
            var result = _client.DragonBallSummon(new DragonBallSummonRequest()
            {
                UserId = Guid.Parse("a5e65a7a-0cef-4fc3-b364-291182034b24")
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    获取活动状态
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallActivityInfo()
        {
            var result = _client.DragonBallActivityInfo();
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    分享完成任务
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallUserShare()
        {
            var result = _client.DragonBallUserShare(new DragonBallUserShareRequest()
            {
                UserId = Guid.Empty
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    创建一个用户任务
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void TestDragonBallCreateUserMissionDetail()
        {
            var result = _client.DragonBallCreateUserMissionDetail(new DragonBallCreateUserMissionDetailRequest()
            {
                UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                MissionId = 5
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///    完成任务历史
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void RefreshRedisCacheCustomerSetting()
        {
            var result = _client.DragonBallUserMissionHistoryList(new DragonBallUserMissionHistoryListRequest()
            {
                UserId = Guid.Empty
            });
            result.ThrowIfException(true);
            Assert.IsNotNull(result.Result);
        }
    }
}
