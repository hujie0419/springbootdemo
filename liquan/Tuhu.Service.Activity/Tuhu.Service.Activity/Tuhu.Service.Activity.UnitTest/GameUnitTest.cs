using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    ///     游戏单元测试
    /// </summary>
    [TestClass]
    public class GameUnitTest
    {

        private IGameClient _client;

        [TestInitialize]
        public void TestInitialize()
        {
            _client = new GameClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }

        /// <summary>
        ///     单元测试：获取游戏信息
        /// </summary>
        [TestMethod]
        public void TestGetGameInfo()
        {
            var result = _client.GetGameInfo(new GetGameInfoRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }



        /// <summary>
        ///     单元测试：获取 里程碑信息
        /// </summary>
        [TestMethod]
        public void TestGetGameMilepostInfo()
        {
            var result = _client.GetGameMilepostInfo(new GetGameMilepostInfoRequest()
            {
                GameVersion = GameVersionEnum.MAPAI,
                UserId = Guid.Parse("a5e65a7a-0cef-4fc3-b364-291182034b24")
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：获取 当前用户信息
        /// </summary>
        [TestMethod]
        public void TestGetGameUserInfo()
        {
            var result = _client.GetGameUserInfo(new GetGameUserInfoRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserId = Guid.Parse("CB3EE07D-9D47-480E-8CC7-489DBF284555")
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///     单元测试：用户分享
        /// </summary>
        [TestMethod]
        public void TestGameUserShare()
        {
            var result = _client.GameUserShare(new GameUserShareRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserId = Guid.Parse("6AA49464-8996-49AC-BA23-AFD9976B2D33")
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///     单元测试：用户分享 100round
        /// </summary>
        [TestMethod]
        public void TestGameUserShare100round()
        {
            var guid = Guid.NewGuid();
            for (int i = 0; i < 100; i++)
            {
                var result = _client.GameUserShare(new GameUserShareRequest()
                {
                    GameVersion = GameVersionEnum.MAPAI,
                    UserId = guid
                });
                result.ThrowIfException();
            }
            Assert.IsNotNull(new object());
        }

        /// <summary>
        ///     单元测试：用户兑换奖品
        /// </summary>
        [TestMethod]
        public void TestGameUserLoot()
        {
            var result = _client.GameUserLoot(new GameUserLootRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserId = new Guid("88B4300A-31FC-4805-A0B1-88C3FC6245BE"),
                PrizeId = 1
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：获取 用户好友助力信息
        /// </summary>
        [TestMethod]
        public void TestGetGameUserFriendSupport()
        {
            var result = _client.GetGameUserFriendSupport(new GetGameUserFriendSupportRequest()
            {
                GameVersion = GameVersionEnum.MAPAI,
                UserId = Guid.Parse("a5e65a7a-0cef-4fc3-b364-291182034b24"),

            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：获取 用户里程收支明细
        /// </summary>
        [TestMethod]
        public void TestGetGameUserDistanceInfo()
        {
            var result = _client.GetGameUserDistanceInfo(new GetGameUserDistanceInfoRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserId = Guid.Parse("CEF8AB29-CE2D-4EF6-B70B-85EBF5E54980"),

            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：获取 奖励滚动
        /// </summary>
        [TestMethod]
        public void TestGetGameUserLootBroadcast()
        {
            var result = _client.GetGameUserLootBroadcast(new GetGameUserLootBroadcastRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                StartTime = DateTime.Now.AddDays(-1),
                PageIndex = 1,
                PageSize = 50
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：获取 用户助力信息【剩余助力次数】
        /// </summary>
        [TestMethod]
        public void TestGetGameUserSupportInfo()
        {
            var result = _client.GetGameUserSupportInfo(new GetGameUserSupportInfoRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                OpenId = "213123214"
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        /// <summary>
        ///     单元测试：帮助助力
        /// </summary>
        [TestMethod]
        public void TestGameUserFriendSupport()
        {
            var result = _client.GameUserFriendSupport(new GameUserFriendSupportRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                OpenId = "213123217",
                TargetUserId = Guid.Parse("A70CA5FA-5A82-40C4-8BDA-8B4FFC5DC07B"),
                WechatNickName = "blablabla",
                WechatHeadImg = "imgimgimg"
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：测试订单
        /// </summary>
        [TestMethod]
        public void TestGameOrderTracking()
        {
            var result = _client.GameOrderTracking(new GameOrderTackingRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                OrderId = 123500632,

            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


        /// <summary>
        ///     单元测试：刪除用户信息
        /// </summary>
        [TestMethod]
        public void TestDeleteGameUserData()
        {
            var result = _client.DeleteGameUserData(new DeleteGameUserDataRequest()
            {
                GameVersion = GameVersionEnum.MAPAI,
                UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                OpenId = ""

            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestUserParticipateGame()
        {
            var result = _client.UserParticipateGame(new UserParticipateGameRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserID = Guid.Parse("CB3EE07D-9D47-480E-8CC7-489DBF284555"),
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestGetRankList()
        {
            var result = _client.GetRankList(new GetRankListAsyncRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                UserID = Guid.Parse("CEF8AB29-CE2D-4EF6-B70B-85EBF5E54980"),
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void TestGetRankListBeforeDay()
        {
            var result = _client.GetRankListBeforeDay(new GetRankListBeforeDayRequest()
            {
                GameVersion = GameVersionEnum.BUMBLEBEE,
                DayTime = DateTime.Now
            });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }


    }


}
