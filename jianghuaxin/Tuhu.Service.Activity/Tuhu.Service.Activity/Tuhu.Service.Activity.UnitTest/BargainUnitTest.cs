using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    /// <summary>
    ///     分享砍价 单元测试
    /// </summary>
    [TestClass]
    public class BargainUnitTest
    {
        private readonly Guid userId = new Guid("9903b770-f484-4361-879d-d91e524397e9");

        /// <summary>
        ///     测试：获取分享砍价列表（已废弃）
        /// </summary>
        [TestMethod]
        public void GetBargainPaoductList()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetBargainPaoductList(new GetBargainproductListRequest
                {
                    PageIndex = 1,
                    PageSize = 100
                });
                Assert.IsNull(result.Exception);
            }
        }

        /// <summary>
        ///     测试：用户新增砍价（已废弃）
        /// </summary>
        [TestMethod]
        public void AddShareBargain()
        {
            using (var client = new ShareBargainClient())
            {
                //var guid = Guid.NewGuid();
                var guid = new Guid("5a3a8b58-0098-7ae6-fb0a-655c3b8c8fb1");
                var result = client.AddShareBargain(guid, 115, "AP-BXJG-TF-LP-XC-01|1");
                Assert.IsNull(result.Exception);
            }
        }
        /// <summary>
        ///     测试：受邀人获取分享产品信息
        /// </summary>
        [TestMethod]
        public void FetchShareBargainInfoTest()
        {
            using (var client = new ShareBargainClient())
            {
                var fuid = new Guid("3FE94A59-8843-40B5-BE20-9C58D14CAE42");
                var result = client.FetchShareBargainInfo(fuid, new Guid());
                Assert.IsNull(result.Exception);
            }
        }

        /// <summary>
        ///     测试：帮忙砍价
        /// </summary>
        [TestMethod]
        public void AddBargainActionTest()
        {
            using (var client = new ShareBargainClient())
            {
                for (var i = 0; i < 3; i++)
                {
                    var idkey = new Guid("04BA925E-130B-4227-B2BD-27F08A4B59FB");
                    //var guid=new Guid("49545CFD-CB2E-4691-ACEA-D3F3E26031C2");
                    var userId = i == 0 ? new Guid("F46C3A58-995A-4607-8F15-A7DAF11F8FA3") : Guid.NewGuid();
                    var result = client.AddBargainAction(idkey, userId, 189);
                    Assert.IsNull(result.Exception);
                }
            }
        }

        [TestMethod]
        public void CheckBargainProductStatusTest()
        {
            using (var client = new ShareBargainClient())
            {
                var uid = new Guid("7d2d8c1e-886b-47ca-a654-4a26359140c7");
                var result = client.CheckBargainProductStatus(uid, 249, "aad82014-d234-442d-8795-2ab22a4e9d5a",
                    "b1666b68-af70-4792-a276-df8808c1b745");
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void RefreshCache()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.RefreshShareBargainCache();
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void History()
        {
            using (var client = new ShareBargainClient())
            {
                var uid = new Guid("49545CFD-CB2E-4691-ACEA-D3F3E26031C2");
                var result = client.FetchBargainProductHistory(uid, 18, "AP-OJLONG-Long-09|1");
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void FetchActivityProductPrice()
        {
            using (var client = new FlashSaleCreateOrderClient())
            {
                var result = client.FetchActivityProductPrice(new ActivityPriceRequest
                {
                    UserId = new Guid("4f79e798-502c-1891-afe5-156b452bf4b2"),
                    //GroupId=new Guid("d4f0483f-d212-417d-8775-d92ecf24b2b2"),
                    Items = new List<ActivityPriceItem>
                    {
                        new ActivityPriceItem
                        {
                            ActicityId = new Guid("2E917E8A-5DE5-448D-810E-B5899C8AA8F2"),
                            PID = "AP-BOTNY-B1999-1|"
                        }
                    }
                });

                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void SelectBargainProductItems()
        {
            using (var client = new ShareBargainClient())
            {
                var guid = new Guid("9994744F-697E-44DC-8EB5-632742C65B0E");
                var result = client.SelectBargainProductItems(guid, 1, 100);
                Assert.IsNull(result.Exception);
                //var result2 = client.SelectBargainProductById(guid, guid, result.Result.Source.ToList());
                //Assert.IsNull(result2.Exception);
            }
        }

        [TestMethod]
        public void FetchBargainProductItemByShareId()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.FetchBargainProductItemByShareId(new Guid("a6162a09-4302-4aad-b367-770b677d50fd"));
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void SelectBargainProductById()
        {
            var ownerId = new Guid("3c6a9e0c-e86e-4c80-860b-90e4d4587c11");
            var userId = new Guid("EA80503D-C912-43E0-87F9-34ACC1988880");

            var list = new List<BargainProductItem>
            {
                new BargainProductItem
                {
                    ActivityProductId = 177,
                    PID = "XU-YHQ-LT|1"
                }
            };
            using (var client = new ShareBargainClient())
            {
                var res = client.SelectBargainProductById(Guid.Empty, Guid.Empty, list);

                res = client.SelectBargainProductById(ownerId, ownerId, list);

                res = client.SelectBargainProductById(ownerId, userId, list);

                res = client.SelectBargainProductById(ownerId, Guid.Empty, list);
            }
        }

        [TestMethod]
        public void SetShareBargainStatus()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.SetShareBargainStatus(new Guid("5BA71C7F-1574-4CF5-A974-31A65A9D32F9"));
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void GetBargainProductForIndex()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetBargainProductForIndex();
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void SelectBargainProductList()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.SelectBargainProductList(1, 20);
                Assert.IsNull(result.Exception);
                var result2 = client.GetBargsinProductDetail(new Guid("E025B1D3-271E-6977-59B3-7FB41DCA4CC5"),
                    new List<BargainProductItem>
                    {
                        new BargainProductItem {PID = "AP-TW-169|1", ActivityProductId = 203}
                    });
                Assert.IsNull(result2.Exception);
            }
        }

        /// <summary>
        ///     测试：创建砍价 并且 砍一刀
        /// </summary>
        [TestMethod]
        public void CreateserBargain()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.CreateserBargain(userId, 190, "f1f40778-9a3b-4861-91bb-b82b5f0f7568");
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void SelectBargainHistory()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.SelectBargainHistory(1, 20, userId);
                Assert.IsNull(result.Exception);
            }
        }
        /// <summary>
        ///     测试：受邀人获取砍价结果
        /// </summary>
        [TestMethod]
        public void GetInviteeBargainInfo()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetInviteeBargainInfo(new Guid("8E05587E-DD10-4510-96EF-59F9D8D870A2"), Guid.Empty);
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void GetShareBargainConfig()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetShareBargainConfig();
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void GetSliceShowInfo()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetSliceShowInfo(100);
                Assert.IsNull(result.Exception);
            }
        }
        /// <summary>
        ///     测试：砍价流程完成，领取优惠券
        /// </summary>
        [TestMethod]
        public void MarkUserReceiveCoupon()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.MarkUserReceiveCoupon(new Guid("7d2d8c1e-886b-47ca-a654-4a26359140c7"), 249,
                    "aad82014-d234-442d-8795-2ab22a4e9d5a", "b1666b68-af70-4792-a276-df8808c1b745");
                Assert.IsNull(result.Exception);
            }
        }
    }
}
