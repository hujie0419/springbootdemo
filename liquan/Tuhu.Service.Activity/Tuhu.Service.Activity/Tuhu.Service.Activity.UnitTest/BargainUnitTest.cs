using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        #region 砍价标准版

        /// <summary>
        /// 用户发起砍价并自砍一刀
        /// </summary>
        [TestMethod]
        public void CreateBargainAndCutSelf()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new CreateBargainAndCutSelfRequest()
                {
                    UserId = new Guid("47b2c84f-1132-427e-9b53-b78c738fd313"),
                    ActivityProductId = 409,
                    Pid = "AP-DGQ-C8|1",
                    IsPush = false,
                    Mobile = "12348883333",
                };

                var result = client.CreateBargainAndCutSelf(request).Result;
                Assert.IsTrue(result.Code == 1);
            }
        }

        /// <summary>
        /// 帮砍
        /// </summary>
        [TestMethod]
        public void HelpCut()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new HelpCutRequest()
                {
                    UserId = new Guid("c2f64f1b-24c5-4004-8d9f-634104eb334a"),
                    ActivityProductId = 409,
                    //7070231b-2357-458e-8cc2-081d0abec004 c14a058f-2123-4106-a876-c0d4ea33b8b1 a9526330-8661-4695-b2af-42dac1467558
                    //b110c02c-7690-4281-b481-67c3c330e4aa  002b8e49-bee9-435d-abb8-f80cce9e7997
                    IdKey = new Guid("da3eb369-0ebf-44ad-b9e0-aa6ee6726f10"),
                };

                var result = client.HelpCut(request).Result;
                Assert.IsTrue(result.Code == 1);
            }
        }

        /// <summary>
        /// 检查砍价商品是否可购买
        /// </summary>
        [TestMethod]
        public void CheckBargainProductBuyStatus()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new CheckBargainProductBuyStatusRequest()
                {
                    ActivityProductId = 357,
                    OwnerId = new Guid("47b2c83f-1132-417e-9b53-b78c738fda11"),
                    Pid = "AP-3M-PN38816|1",
                    DeviceId = "123",
                    Mobile = "12388883333",
                };
                var result = client.CheckBargainProductBuyStatus(request);
                Assert.IsNull(result.Exception);
            }
        }

        /// <summary>
        /// 领券砍价优惠券
        /// </summary>
        [TestMethod]
        public void ReceiveBargainCoupon()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new ReceiveBargainCouponRequest()
                {
                    OwnerId = new Guid("47B2C83F-1132-417E-9B53-B78C738FDA11"),
                    ActivityProductId = 357,
                    Pid = "AP-3M-PN38816|1",
                    Mobile = "12388883333",
                };
                var result = client.ReceiveBargainCoupon(request).Result;
                Assert.IsTrue(result.Code == 1);
            }
        }

        /// <summary>
        /// 验证砍价黑名单
        /// </summary>
        [TestMethod]
        public void CheckBargainBlackList()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new BargainBlackListRequest()
                {
                    Mobile = "12388883333",
                    Ip = "123.34.2.2"
                };
                var result = client.CheckBargainBlackList(request).Result;
                Assert.IsTrue(result.Code == 1);
            }
        }

        /// <summary>
        ///  获取砍价商品信息：配置、参与人数 详情图片
        /// </summary>
        [TestMethod]
        public void GetShareBargainProductInfo()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new GetShareBargainProductInfoRequest()
                {
                    ActivitiProductID = 362
                };
                var result = client.GetShareBargainProductInfoAsync(request).Result;
                Assert.IsTrue(result.Success);
            }
        }

        /// <summary>
        /// 获取砍价商品配置信息
        /// </summary>
        [TestMethod]
        public void GetShareBargainSettingInfo()
        {
            using (var client = new ShareBargainClient())
            {
                var request = new GetShareBargainSettingInfoRequest()
                {
                    ActivitiProductID = 362
                };
                var result = client.GetShareBargainSettingInfo(request).Result;
                Assert.IsTrue(result.PKID > 0);
            }
        }


        #endregion

        /// <summary>
        ///     测试：帮忙砍价
        /// </summary>
        [TestMethod]
        public void AddBargainActionTest()
        {
            using (var client = new ShareBargainClient())
            {
                var idkey = new Guid("832E9BC2-71FC-4505-9DA3-23E5B5980E8D");
                var userId = new Guid("569D820D-68E9-44C7-8C46-B02137B2B8F3");
                int activityProductId = 357;
                var result = client.AddBargainAction(idkey, userId, activityProductId);
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
                var result = client.GetInviteeBargainInfo(new Guid("B98B9629-FEA0-4349-AE8D-6FC248A5A7D0"), new Guid("c3f64f1b-24c5-4004-8d9f-634104ebe34a"));
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
                var result = client.MarkUserReceiveCoupon(new Guid("cb3ee07d-9d47-480e-8cc7-489dbf284555"), 308,
                    "2ca7ad48-7017-4efb-883d-06eae5ce0b59", "b1666b68-af70-4792-a276-df8808c1b745");
                Assert.IsNull(result.Exception);
            }
        }


        [TestMethod]
        public void GetValidBargainOwnerActionsByApidsAsync()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.GetValidBargainOwnerActionsByApids(603, DateTime.Now, DateTime.Now, 1, 0);
                Assert.IsNull(result.Exception);
            }
        }

        [TestMethod]
        public void FetchBargainProductItemByShareId()
        {
            using (var client = new ShareBargainClient())
            {
                var result = client.FetchBargainProductItemByShareId(new Guid("587109BC-A8AE-4D49-9461-F3597448C17C"));
                Assert.IsNull(result.Exception);
            }
        }


        [TestMethod]
        public void SelectBargainProductList()
        {
            using (var client = new ShareBargainClient())
            {
                //var result = client.SelectBargainProductList(1, 20);
                //.IsNull(result.Exception);
                var result2 = client.GetBargsinProductDetail(new Guid("E025B1D3-271E-6977-59B3-7FB41DCA4CC5"),
                    new List<BargainProductItem>
                    {
                        new BargainProductItem {PID = "AP-DW-MFJ500-2|1", ActivityProductId = 333}
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
                var userid = new Guid("cb3ee07d-9d47-480e-8cc7-489dbf284555");
                int appid = 308;
                string pid = "2ca7ad48-7017-4efb-883d-06eae5ce0b59";
                var result = client.CreateserBargain(userid, appid, pid);
                Assert.IsNull(result.Exception);
            }
        }


    }
}
