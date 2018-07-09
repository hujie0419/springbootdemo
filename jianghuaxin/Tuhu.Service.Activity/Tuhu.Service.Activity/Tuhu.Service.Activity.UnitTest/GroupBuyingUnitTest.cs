using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class GroupBuyingUnitTest
    {
        private GroupBuyingClient _client;
        private readonly Guid _ownerId = new Guid("{f20cfbec-38f7-43df-8824-8473f7eb2b68}");
        private Guid _userId = new Guid("{bee3bfd3-368c-4a1a-8151-fdc37d8d86f0}");
        private readonly Guid _groupId = new Guid("664B42CF-BE28-4969-95E4-A4F6C8066275");

        [TestInitialize]
        public void TestInitialize()
        {
            _client = new GroupBuyingClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }

        [TestMethod]
        public void GetGroupBuyingProduct()
        {
            var result = _client.GetGroupBuyingProductList(1, 10, false, "WXAPP", true);
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
            var result2 = _client.SelectProductGroupInfo(result.Result.ToList());
            result2.ThrowIfException();
            Assert.IsNull(result2.Exception);
            Assert.IsNotNull(result2.Result);
        }

        [TestMethod]
        public void SelectProductGroupInfo()
        {
            var result = _client.SelectProductGroupInfo(new List<string> { "PT27875554" });
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void SelectGroupBuyingProductsById()
        {
            var result = _client.SelectGroupBuyingProductsById("PT36899178");
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
            var result2 = _client.SelectProductInfoByPid("PT19408240", result.Result[0]);
            result2.ThrowIfException();
            Assert.IsNull(result2.Exception);
            Assert.IsNotNull(result2.Result);
        }

        [TestMethod]
        public void RefreshCache()
        {
            var result = _client.RefreshCache();
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void CreateGroupBuying()
        {
            var result = _client.CreateGroupBuying(_ownerId, "PT36899178", "AP-TAWA-DC|1", 1882014);
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void ChangeGroupBuyingStatus()
        {
            var result = _client.ChangeGroupBuyingStatus(_groupId, 1882014);
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            var result2 = _client.ChangeUserStatus(_groupId, _ownerId, 1882014);
            result2.ThrowIfException();
            Assert.IsNull(result2.Exception);
        }


        [TestMethod]
        public void TakePartInTest()
        {
            var orderid = 18800005;
            _userId = Guid.NewGuid();
            var result = _client.TakePartInGroupBuying(_userId, _groupId, "XU-PTCJ-XX|1", orderid);
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            var result2 = _client.ChangeUserStatus(_groupId, _userId, orderid);
            result2.ThrowIfException();
            Assert.IsNull(result2.Exception);
        }

        [TestMethod]
        public void GetUserGroupInfoByUserId()
        {
            var result = _client.GetUserGroupInfoByUserId(new GroupInfoRequest
            {
                PageIndex = 1,
                PageSize = 10,
                Type = 3,
                UserId = new Guid("A6D35679-A38B-4781-8DA9-1727E390F033")
            });
            result.ThrowIfException();

            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void GetUserGroupCountByUserId()
        {
            var result = _client.GetUserGroupCountByUserId(new Guid("A6D35679-A38B-4781-8DA9-1727E390F033"));
            result.ThrowIfException();

            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void SelectGroupMemberByGroupId()
        {
            var result = _client.SelectGroupMemberByGroupId(new Guid("50DAEC47-8ECF-4E66-BAAA-2440DF41FC25"));
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void FetchGroupInfoByGroupId()
        {
            var result = _client.FetchGroupInfoByGroupId(_groupId);
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void SelectGroupInfoByProductGroupId()
        {
            var id = new Guid("D33969C4-0752-4512-8272-9E361FC12B1F");
            var result = _client.SelectGroupInfoByProductGroupId("PT83694034", id, 100);
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void CheckGroupInfoByUserId()
        {
            var id = new Guid("7f73364b-0307-4245-9e95-b5489fe54eef");
            var groupid = new Guid("21C9A69B-D82B-4567-8E77-CB11C7C95E43");
            var result = _client.CheckGroupInfoByUserId(Guid.Empty, id, "PT15546382", "AP-XK-T10|1");
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void FetchUserOrderInfo()
        {
            var result = _client.FetchUserOrderInfo(_groupId, Guid.NewGuid());
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void FetchProductGroupDetail()
        {
            var result = _client.SelectProductGroupDetail("Tuhu002");
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void FetchGroupInfoByOrderId()
        {
            var result = _client.FetchGroupInfoByOrderId(163785);
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void FetchProductGroupInfoById()
        {
            var result = _client.SelectProductGroupInfo(new List<string> { "TH10244512" });
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void PushPinTuanMessage()
        {
            var result = _client.PushPinTuanMessage(_groupId, 1650);
            Assert.IsNull(result.Exception);
        }

        [TestMethod]

        public void GetProductGroupInfoByPId()
        {
            var result = _client.GetProductGroupInfoByPId("AP-VICTION-T");
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void SelectProductListByPids()
        {
            var result = _client.SelectProductListByPids(new List<GroupBuyingProductRequest>
            {
                new GroupBuyingProductRequest
                {
                    ProductGroupId = "PT-6595762",
                    PId = "AP-BOTNY-B1999-12TH|1"
                },
                new GroupBuyingProductRequest
                {
                    ProductGroupId = "PT-6595762",
                    PId = "AP-BOTNY-B1999-8TH|1"
                }
                //new GroupBuyingProductRequest()
            });
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void GetLotteryRuleUnt()
        {
            var result = _client.GetLotteryRule("PT-1949354");
            Assert.IsNull(result.Exception);
        }
        [TestMethod]
        public void GetWinnerUnt()
        {
            var result = _client.GetWinnerList("PT-1949354");
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void CheckLotteryResult()
        {
            var result = _client.CheckUserLotteryResult(new Guid("81A4EE18-98FC-4B6F-981E-0CD387AF9B24"), "PT-1949354", 18848567);
            Assert.IsNull(result.Exception);
        }
        [TestMethod]
        public void GetUserLotteryHistory()

        {
            var result = _client.GetUserLotteryHistory(new Guid("81A4EE18-98FC-4B6F-981E-0CD387AF9B24"), null);
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void GetActivityProductGroup()
        {
            var result = _client.GetActivityProductGroup(new ActivityGroupRequest { Type = 3, PageIndex = 1, PageSize = 10 });
            Assert.IsNull(result.Exception);
        }
        [TestMethod]
        public void GetUserFreeCoupon()
        {
            var result = _client.GetUserFreeCoupon(new Guid("50F8E1EE-C20E-45E5-8382-6AA213601207"));
            Assert.IsNull(result.Exception);
        }

        [TestMethod]
        public void UpdateGroupBuyingInfo()
        {
            var result = _client.GetGroupBuyingProductList(1, 1000, false, "");
            result.ThrowIfException();
            Assert.IsNull(result.Exception);
            Assert.IsNotNull(result.Result);
            var result2 = _client.UpdateGroupBuyingInfo(result.Result.Source.ToList());
            result2.ThrowIfException();
            Assert.IsNull(result2.Exception);
            Assert.IsNotNull(result2.Result);
        }

        [TestMethod]
        public void GetGroupBuyingCategoryInfo()
        {
            var result = _client.GetGroupBuyingCategoryInfo();
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void GetGroupBuyingProductListNew()
        {
            var result = _client.GetGroupBuyingProductListNew(new GroupBuyingQueryRequest
            {
                PageIndex = 1,
                PageSize = 102,
                SortType = 1,
                Channel = "kH5",
                NewCategoryCode = 12,
                IsOldUser = false,
                KeyWord = "米奇林"
            });
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void SelectGroupBuyingListNew()
        {
            var result = _client.SelectGroupBuyingListNew(new GroupBuyingQueryRequest
            {
                PageIndex = 1,
                PageSize = 102,
                SortType = 1,
                Channel = "kH5",
                NewCategoryCode = 12,
                IsOldUser = false,
                KeyWord = "米奇林"
            });
            Assert.IsNotNull(result.Result);
        }
        [TestMethod]
        public void GetUserBuyLimitInfo()
        {
            var result = _client.GetUserBuyLimitInfo(new GroupBuyingBuyLimitRequest
            {
                ProductGroupId = "PT15546382",
                PID = "AP-XK-T10|1",
                UserId = _ownerId
            });
            Assert.IsNotNull(result.Result);
        }
    }
}
