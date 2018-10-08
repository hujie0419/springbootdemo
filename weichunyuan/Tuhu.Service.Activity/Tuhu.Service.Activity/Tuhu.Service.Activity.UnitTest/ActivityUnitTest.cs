using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class ActivityUnitTest
    {
        [TestMethod]
        public void GetActivityConfigForDownloadApp()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetActivityConfigForDownloadApp(9);
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void CleanActivityConfigForDownloadAppCache()
        {
            using (var client = new ActivityClient())
            {
                var result = client.CleanActivityConfigForDownloadAppCache(9);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectTireActivity()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectTireActivity("VE-DDRAM", "275/60R20");
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void SelectTireActivityPids()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectTireActivityPids(new Guid("66AB4218-6EBD-4D54-8CC5-0F52C55CF302"));
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void UpdateActivityPidsCache()
        {
            using (var client = new ActivityClient())
            {
                var result = client.UpdateActivityPidsCache(new Guid("4E8D5BC6-0658-49C8-B5AC-0834FE3CB6E8"));
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void SelectTireActivityByActivityId()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectTireActivityByActivityId(new Guid("4E8D5BC6-0658-49C8-B5AC-0834FE3CB6E8"));
                Assert.IsTrue(result.Success);
            }
        }


        [TestMethod]
        public void SelectVehicleAaptTireAsync()
        {
            using (var client = new ActivityClient())
            {
                var request = new VehicleAdaptTireRequestModel()
                {
                    VehicleId = "VE-A4LYSDZAD",
                    TireSizes = null
                };
                var result = client.SelectVehicleAaptTires(request);

                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectCarTagCouponConfigsAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectCarTagCouponConfigs();
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectVehicleAaptBaoyangsAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectVehicleAaptBaoyangs("VE-A6LYSDZAD");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectVehicleAdaptChepinsAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectVehicleAdaptChepins("VE-A4LYSDZAD");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectVehicleSortedTireSizesAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectVehicleSortedTireSizes("VE-A6LYSDZAD");
                Assert.IsNotNull(result.Result);
            }
        }


        [TestMethod]
        public void GetGuidAndInsertUserShareInfoAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetGuidAndInsertUserShareInfo("LG-HG-HG5009-15|1",
                    new Guid("1FC3C31B-F603-4ECE-A08F-DBE4D5A83F51"), new Guid("36420da7-91ac-4e1e-8cd8-bba08bc8dcc2"));
                Assert.IsNotNull(result.Result);
            }
        }


        [TestMethod]
        public void GetActivityUserShareInfoAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetActivityUserShareInfo(new Guid("F4C17F5D-5A45-485B-BF71-20A56D597703"));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectPromotionPacketHistoryAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectPromotionPacketHistory(new Guid("3FA9D1D4-561F-42A8-8EB5-609CD4E177BA"),
                    new Guid("8FC0EACB-5FA8-43F3-8F51-A544E40D3570"));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetGuidAndInsertUserForShareAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetGuidAndInsertUserForShare(new Guid("0E36AB59-E736-4292-A96D-262A21897B19"),
                    new Guid("36420da7-91ac-4e1e-8cd8-bba08bc8dcc2"));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void FetchRecommendGetGiftConfigAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.FetchRecommendGetGiftConfig(null, new Guid("DFA7EC09-F4B6-4653-8982-47132EA660EC"));
                result.ThrowIfException(true);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetActivityUrl()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetRegionActivityPageUrl("自贡市", "1");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectTireActivityNewAsync()
        {
            using (var client = new ActivityClient())
            {
                var result =
                    client.SelectTireActivityNew(new TireActivityRequest()
                    {
                        VehicleId = "VE-AUDI07AF",
                        TireSize = "235/55R18",
                        CityID = 1
                    });
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetLuckyWheelUserlotteryCountAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetLuckyWheelUserlotteryCount(new Guid("743C2104-B8C1-4849-9407-0C8A6440CED9"),
                    new Guid("2d017ec6-015c-40f6-bea0-18bbcf1cb3bd"));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void UpdateLuckyWheelUserlotteryCountAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.UpdateLuckyWheelUserlotteryCount(new Guid("743c2104-b8c1-4849-9407-0c8a6440ced9"),
                    new Guid("2d017ec6-015c-40f6-bea0-18bbcf1cb3bd"));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetActivePageListModel()
        {
            using (var client = new ActivityClient())
            {
                var request = new ActivtyPageRequest()
                {
                    HashKey = "3E8593F0",
                    Channel = "wap",
                    OrderChannel = "kH5",
                    UserId = new Guid("1ab0fa62-91bb-4fe7-8cf2-eb57367463d4"),
                    Width = "205",
                    AspectRatio = "55",
                    Rim = "16",
                    CityId = "1",
                    VehiclId = "VE-JEJ7YSDZ",
                    Tid = "108627"
                };
                var result = client.GetActivePageListModel(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void RefreshActivePageListCache()
        {
            using (var client = new ActivityClient())
            {
                var request = new ActivtyPageRequest()
                {
                    HashKey = "F3AC38EC",

                    Channel = "wap",
                    OrderChannel = "kH5"

                };
                var result = client.RefreshActivePageListModelCache(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void VerificationByTires()
        {
            using (var client = new ActivityClient())
            {
                var result = client.VerificationByTires(new VerificationTiresRequestModel()
                {
                    AddressPhone = "13162362352",
                    DeviceId = "",
                    Number = 10,
                    UserIp = "127.0.0.1",
                    UserPhone = "13162362352"
                });
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void InsertTiresOrderRecord()
        {
            using (var client = new ActivityClient())
            {
                var result = client.InsertTiresOrderRecord(new TiresOrderRecordRequestModel()
                {
                    AddressPhone = "18801790000",
                    DeviceId = null,
                    Number = 10,
                    UserIp = "127.0.0.1",
                    UserPhone = "13162362352",
                    OrderId = 1
                });
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void RevokeTiresOrderRecord()
        {
            using (var client = new ActivityClient())
            {
                var result = client.RevokeTiresOrderRecord(1635525);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectTiresOrderRecord()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectTiresOrderRecord(new TiresOrderRecordRequestModel()
                {
                    AddressPhone = "13162362352",
                    DeviceId = "",
                    UserIp = "127.0.0.1",
                    UserPhone = "13162362352",
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void RedisTiresOrderRecordReconStruction()
        {
            using (var client = new ActivityClient())
            {
                var result = client.RedisTiresOrderRecordReconStruction(new TiresOrderRecordRequestModel()
                {
                    AddressPhone = "13162362352",
                    DeviceId = "",
                    Number = 10,
                    UserIp = "127.0.0.1",
                    UserPhone = "13162362352"
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void SelectShareActivityProductByIdTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectShareActivityProductById("LG-HG-HG5009-15|1");
                Assert.IsNull(result.Result);
            }
        }

        [TestMethod]
        public void BaoyangActivityTest1()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectBaoYangActivitySetting("e0500795-b313-4d73-a99c-8147d4394853");
                Assert.IsNotNull(result);
            }
        }
        [TestMethod]
        public void BaoyangActivityTest2()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectCouponActivityConfig("e0500795-b313-4d73-a99c-8147d4394853", 1);
                Assert.IsNotNull(result);
            }
        }


        [TestMethod]
        public void GetActivityBuildWithSelKeyAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetActivityBuildWithSelKey("辣眼睛");
                Assert.IsTrue(result.Success);
            }
        }
        [TestMethod]
        public void RecordActivityTypeLogAsync()
        {
            var request = new ActivityTypeRequest()
            {
                ActivityId = new Guid("db5f63c4-1084-48ee-b1c7-459922403118"),
                Type = 9,
                Status = 1,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now
            };
            using (var client = new ActivityClient())
            {
                var result = client.RecordActivityTypeLog(request);
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void GetFixedPriceStatus()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetFixedPriceActivityStatus(Guid.Parse("b7b8fe14-c887-40b5-bd72-673bcaf56786"), Guid.Empty, 0);
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void GetFixedPriceActivityRound()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetFixedPriceActivityRound(Guid.Parse("b7b8fe14-c887-40b5-bd72-673bcaf56786"));
                Assert.IsTrue(result.Success);
            }
        }

        [TestMethod]
        public void UpdateBaoYangPurchaseCount()
        {
            using (var client = new ActivityClient())
            {
                var result = client.UpdateBaoYangPurchaseCount(Guid.Parse("b7b8fe14-c887-40b5-bd72-673bcaf56786"));
                Assert.IsTrue(result.Success);
            }
        }
        #region 分车型分地区活动页配置
        [TestMethod]
        public void GetRegionVehicleIdTargetActivityId()
        {
            using (var client = new ActivityClient())
            {
                var result1 = client.GetRegionVehicleIdActivityUrl(Guid.Empty, 0, null);
                Assert.AreEqual(result1.ErrorMessage, "参数错误");
                var result3 = client.GetRegionVehicleIdActivityUrlNew(Guid.Parse("ed88f838-e265-4f9b-8e36-a3fac309980e"), 0, "VE-AUDI07AF", "kH5");
                var result4 = client.GetRegionVehicleIdActivityUrlNew(Guid.Parse("e3881d87-8e3f-4f8a-814d-5b98b190273c"), 0, "VE2-A4LYSDZAD", "kH5");
                var result5 = client.GetRegionVehicleIdActivityUrlNew(Guid.Parse("92abb43f-238d-4e07-9e43-565280443313"), 0, "VE-AUDI07AF", "WXAPP");
            }
        }

        [TestMethod]
        public void RefreshRegionVehicleIdActivityUrlCache()
        {
            using (var client = new ActivityClient())
            {
                var result1 = client.RefreshRegionVehicleIdActivityUrlCache(Guid.Empty);
                Assert.AreEqual(result1.ErrorMessage, "参数错误");
                var result2 = client.RefreshRegionVehicleIdActivityUrlCache(Guid.Parse("e3881d87-8e3f-4f8a-814d-5b98b190273c"));
            }
        }
        #endregion
        [TestMethod]
        public void GetActivityTypeLogAsync()
        {

            using (var client = new ActivityClient())
            {
                var result = client.SelectActivityTypeByActivityIds(new List<Guid> { new Guid("516eaa5a-98aa-428c-bef6-3d177753b07b") });
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void FetchRegionTiresActivity()
        {
            var request = new FlashSaleTiresActivityRequest()
            {
                ActivityId = Guid.Parse("0b4dd1c7-f897-4c33-8cdc-901320d8d19d"),
                RegionId = 1
            };
            using (var client = new ActivityClient())
            {
                var result = client.FetchRegionTiresActivity(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void RefreshRegionTiresActivityCache()
        {
            var activityId = Guid.Parse("0b4dd1c7-f897-4c33-8cdc-901320d8d19d");
            var regionId = 1;

            using (var client = new ActivityClient())
            {
                var result = client.RefreshRegionTiresActivityCache(activityId, regionId);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectTireChangedActivity()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectTireChangedActivity(new TireActivityRequest()
                {
                    VehicleId = "VE-AUDI07AF",
                    TireSize = "255/45R18"
                });
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void RecordActivityProductUserRemindLogAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.RecordActivityProductUserRemindLog(new ActivityProductUserRemindRequest()
                {
                    //UserId = Guid.NewGuid(),
                    ActivityId = "testhuodong",
                    ActivityName = "满一万百万减一个亿",
                    Pid = "TR-HK-K415|6",
                    PorductName = "飞驰的轮胎"
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void RemoveRedisCacheKeyAsync()
        {
            using (var client = new CacheClient())
            {
                var result = client.RemoveRedisCacheKey(
                    "ActivityDeafult",
                    "TireChangedActivity/VE-AUDI07AF/255/45R18",
                    "TireChangedActivity/{VehicleId}/{TireSize}");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void InsertRebateApplyRecordAsync()
        {
            var request = new RebateApplyRequest()
            {
                OrderId = 1667745,
                UserPhone = "231631555",
                QRCodeImg = "https://img1.tuhu.org/activity/image/82be/1e6d/2ac146a6e939f27e79af1fc3_w800_h600.jpg",
                Remarks = "",
                ImgList = new List<string> { "123", "456" }
            };
            using (var client = new ActivityClient())
            {
                var result = client.InsertRebateApplyRecordNew(request);
                Assert.IsNotNull(result.Result);
            }
        }
        #region 白名单
        [TestMethod]
        public void InsertOrUpdateActivityPageWhiteListRecordsAsync()
        {
            #region 参数
            var requests = new List<ActivityPageWhiteListRequest>
            {
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "13817116040",
                Status = 1
                },
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "18817922971",
                    Status = 1
                },
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "17717365811",
                    Status = 1
                },
               new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "18217712774",
                    Status = 1
                },
               new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "15221013729",
                    Status = 1
                },
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "18721357587",
                    Status = 1
                },
               new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "13758253717",
                    Status = 1
                },
                 new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "13651646449",
                    Status = 1
                },
               new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "18516814350",
                    Status = 1
                },
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "15900494151",
                    Status = 1
                },
               new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "18018674975",
                    Status = 1
                },
                new ActivityPageWhiteListRequest()
                {
                    PhoneNum = "13636557021",
                    Status = 1
                },
            };
            #endregion
            using (var client = new ActivityClient())
            {
                var result = client.InsertOrUpdateActivityPageWhiteListRecords(requests);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetActivityPageWhiteListByUserIdAsync()
        {

            using (var client = new ActivityClient())
            {
                var result = client.GetActivityPageWhiteListByUserId(new Guid("21FA116E-5AAF-4DD8-9EE0-C2D93CB026C7"));
                Assert.IsNotNull(result.Result);
            }
        }
        #endregion

        [TestMethod]
        public void PutUserRewardApplicationAsync()
        {
            var request = new UserRewardApplicationRequest
            {
                ApplicationName = "Ray",
                Phone = "18721970953",
                ImageUrl1 = "https://img1.tuhu.org/activity/image/44ee/27eb/8745ed44d455d9a971300ab6_w751_h448.png"
            };
            using (var client = new ActivityClient())
            {
                var result = client.PutUserRewardApplication(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void PutApplyCompensateRecordAsync()
        {
            var request = new ApplyCompensateRequest()
            {
                UserName = "Ray",
                PhoneNumber = "13671714524",
                OrderId = "TH16670088",
                Images = "https://img1.tuhu.org/activity/image/44ee/27eb/8745ed44d455d9a971300ab6_w751_h448.png"
            };
            using (var client = new ActivityClient())
            {
                var result = client.PutApplyCompensateRecord(request);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetActivtyValidityResponses()
        {
            var request = new ActivtyValidityRequest()
            {
                Channel = "wap",
                HashKeys = new List<string>()
                {
                    "fdsfdsfds",
                    "6E5237BC"
                }
            };
            using (var client = new ActivityClient())
            {
                var result = client.GetActivtyValidityResponses(request);
                Assert.IsNotNull(result.Result);
            }
        }

        #region vipcard
        [TestMethod]
        public void GetVipCardSaleConfigDetailsAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetVipCardSaleConfigDetails("f32f72a0-b685-4a0a-b828-be3cf3be94bb");
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void VipCardCheckStockAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.VipCardCheckStock(new List<string>
                {
                    "3004",
                    "111102"
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void PutVipCardRecordAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.PutVipCardRecord(new VipCardRecordRequest
                {
                    ActivityId = "21A07A84-5171-49BC-BE9C-6FF02776363E",
                    OrderId = 778899,
                    UserId = new Guid("21fa116e-5aaf-4dd8-9ee0-c2d93cb026c6"),
                    UserPhone = "13671714538",
                    Batches = new List<VipCardBatchModel>()
                   {
                       new VipCardBatchModel()
                       {
                           BatchId = "3004",
                           CardNum = 2
                       }
                   }
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void BindVipCardAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.BindVipCard(778899);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void RefreshVipCardAsync()
        {
            using (var client = new CacheClient())
            {
                var result = client.RefreshVipCardCacheByActivityId("f32f72a0-b685-4a0a-b828-be3cf3be94bb");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void ModifyVipCardRecordByOrderIdAsync()
        {
            using (var client = new ActivityClient())
            {
                var result = client.ModifyVipCardRecordByOrderId(1671266);
                Assert.IsNotNull(result.Result);
            }
        }
        #endregion
        [TestMethod]
        public void SelectRebateApplyPageConfig()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectRebateApplyPageConfig();
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void InsertRebateApplyRecordV2()
        {
            var request = new RebateApplyRequest()
            {
                OrderId = 1671372,
                UserPhone = "231631555",
                WXId = "8GKDGDJG",
                WXName = "123",
                UnionId = "oPKZWtwJvGvdPBIJrpp8t-mIMU6c",
                OpenId = "o7XCEtz2mIgNRAVmpaoHW8YJF2H4",
                QRCodeImg = "https://img1.tuhu.org/activity/image/82be/1e6d/2ac146a6e939f27e79af1fc3_w800_h600.jpg",
                Remarks = "",
                ImgList = new List<string> { "123", "456" }
            };
            using (var client = new ActivityClient())
            {
                var result = client.InsertRebateApplyRecordV2(request);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectRebateApplyByOpenId()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SelectRebateApplyByOpenId("o7XCEtz2mIgNRAVmpaoHW8YJF2H4");
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        ///     测试：获取2018世界杯活动对象和积分规则信息
        /// </summary>
        [TestMethod]
        public void TestGetWorldCup2018Activity()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetWorldCup2018Activity();
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：通过活动类型 获取 活动
        /// </summary>
        [TestMethod]
        public void TestGetActivityByActivityTypeId()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetActivityInfoByType(1);
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        ///     测试：活动分享赠送积分
        /// </summary>
        [TestMethod]
        public void TestActivityShare()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.ActivityShare(new ActivityShareDetailRequest()
                {
                    ActivityId = activity.Result.PKID,
                    UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                    ShareName = "2018世界杯-分享送积分",
                    IntegralRuleID = activity.Result.ShareIntegralRuleID
                });


                var result1 = client.ActivityShare(new ActivityShareDetailRequest()
                {
                    ActivityId = activity.Result.PKID,
                    UserId = Guid.Parse("a3136282-251e-462a-af0c-627843f5c649"),
                    ShareName = "2018世界杯-分享送积分",
                    IntegralRuleID = activity.Result.ShareIntegralRuleID
                });

                var result2 = client.ActivityShare(new ActivityShareDetailRequest()
                {
                    ActivityId = activity.Result.PKID,
                    UserId = Guid.Parse("5171B31F-B541-45B9-B9D9-7DC2DBA597ED"),
                    ShareName = "2018世界杯-分享送积分",
                    IntegralRuleID = activity.Result.ShareIntegralRuleID
                });
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：获取用户兑换券
        /// </summary>
        [TestMethod]
        public void TestSearchCouponCountByUserId()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.GetCouponCountByUserId(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID);
                Assert.IsNotNull(coupon.Result);
            }
        }

        /// <summary>
        ///     测试：获取今日竞猜题目
        /// </summary>
        [TestMethod]
        public void TestSearchQuestion()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.SearchQuestion(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：提交用户竞猜
        /// </summary>
        [TestMethod]
        public void TestSubmitQuestionAnswer()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();

                //积分不足
                var result = client.SubmitQuestionAnswer(new SubmitQuestionAnswerRequest()
                {
                    ActivityId = activity.Result.PKID,
                    IntegralRuleID = activity.Result.UserSelectionIntegralRuleID,
                    OptionId = 212,
                    UserId = Guid.Parse("00000000-0000-0000-0000-000000000001")
                });
                Assert.IsFalse(result.Success);

                //积分充足
                result = client.SubmitQuestionAnswer(new SubmitQuestionAnswerRequest()
                {
                    ActivityId = activity.Result.PKID,
                    IntegralRuleID = activity.Result.UserSelectionIntegralRuleID,
                    OptionId = 588,
                    UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24")
                });
                Assert.IsTrue(result.Success);


                //问题时间
            }
        }

        /// <summary>
        ///     测试：返回用户答题历史
        /// </summary>
        [TestMethod]
        public void TestSearchQuestionAnswerHistoryByUserId()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result =
                    client.SearchQuestionAnswerHistoryByUserId(new SearchQuestionAnswerHistoryRequest()
                    {
                        ActivityId = activity.Result.PKID,
                        PageIndex = 1,
                        PageSize = 20,
                        UserId = Guid.Parse("A3136282-251E-462A-AF0C-627843F5C649")
                    });


                result =
                    client.SearchQuestionAnswerHistoryByUserId(new SearchQuestionAnswerHistoryRequest()
                    {
                        ActivityId = activity.Result.PKID,
                        PageIndex = 1,
                        PageSize = 20,
                        ShowFlag = 1,
                        UserId = Guid.Parse("A3136282-251E-462A-AF0C-627843F5C649")
                    });
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：返回用户胜利次数和胜利称号
        /// </summary>
        [TestMethod]
        public void TestGetVictoryInfo()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result =
                    client.GetVictoryInfo(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                        activity.Result.PKID);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：获取兑换券排行榜
        /// </summary>
        [TestMethod]
        public void TestSearchCouponRank()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.SearchCouponRank(activity.Result.PKID, 1, 10);
                Assert.IsNotNull(coupon.Result);
            }
        }

        /// <summary>
        ///     测试：获取用户排名信息
        /// </summary>
        [TestMethod]
        public void TestGetUserCouponRank()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.GetUserCouponRank(Guid.Parse("00000000-0000-0000-0000-000000000561"), activity.Result.PKID);
                Assert.IsNotNull(coupon.Result);
            }
        }

        /// <summary>
        ///     测试：用户可兑换奖品列表
        /// </summary>
        [TestMethod]
        public void TestSearchPrizeListByUserId()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.SearchPrizeList(new SearchPrizeListRequest()
                {
                    ActivityId = activity.Result.PKID,
                    ShowCanPay = false,
                    PageIndex = 1,
                    PageSize = 20,
                    UserId = Guid.Parse("A3136282-251E-462A-AF0C-627843F5C649")
                });

                var coupon1 = client.SearchPrizeList(new SearchPrizeListRequest()
                {
                    ActivityId = activity.Result.PKID,
                    ShowCanPay = true,
                    PageIndex = 1,
                    PageSize = 20,
                    UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24")
                });
                Assert.IsNotNull(coupon.Result);
            }
        }
        /// <summary>
        ///     测试：用户兑换奖品
        /// </summary>
        [TestMethod]
        public void TestUserRedeemPrizes()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.UserRedeemPrizes(Guid.Parse("37706970-D1D0-4548-8B37-AD8465EFCAA6"), 75, activity.Result.PKID);

                var result = client.UserRedeemPrizes(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), 47, activity.Result.PKID);

                Assert.IsNotNull(coupon.Result);
            }
        }

        /// <summary>
        ///     测试：用户已兑换商品列表
        /// </summary>
        [TestMethod]
        public void TestSearchPrizeOrderDetailListByUserId()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var coupon = client.SearchPrizeOrderDetailListByUserId(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID, 1, 20);
                Assert.IsNotNull(coupon.Result);
            }
        }

        [TestMethod]
        public void GetOrSetActivityPageSortedPidsAsync()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetOrSetActivityPageSortedPids(new SortedPidsRequest
                {
                    Brand = "龟牌/Turtle Wax",
                    DicActivityId = new KeyValuePair<string, ActivityIdType>("8e518f61-6da1-4487-a86f-069096243106", ActivityIdType.FlashSaleActivity),
                    ProductType = ProductType.AutoProduct,
                    NeedUpdatePkid = 4601
                });

                Assert.IsNotNull(activity.Result);
            }
        }

        /// <summary>
        ///     测试：今日是否已经活动分享了
        /// </summary>
        [TestMethod]
        public void TestActivityTodayAlreadyShare()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.ActivityTodayAlreadyShare(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                    activity.Result.PKID);


                var result1 = client.ActivityTodayAlreadyShare(Guid.Parse("a3136282-251e-462a-af0c-627843f5c649"),
                    activity.Result.PKID);

                var result2 = client.ActivityTodayAlreadyShare(Guid.Parse("5171B31F-B541-45B9-B9D9-7DC2DBA597ED"),
                    activity.Result.PKID);

                var result3 = client.ActivityTodayAlreadyShare(Guid.Parse("0384C35B-7199-4FEC-B275-0AD4C805E93E"),
                    activity.Result.PKID);

                Assert.IsNotNull(result.Result);
            }
        }



        /// <summary>
        ///     测试：添加用户兑换券假数据
        /// </summary>
        [TestMethod]
        public void TestInsertActivityUserConpon()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var couponName = "单元测试";

                //client.ActivityCouponUpsert(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID, 1);

                client.ModifyActivityCoupon(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID, -101, couponName);

                client.ModifyActivityCoupon(Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"), activity.Result.PKID, 1, couponName, DateTime.Parse("2017-01-01"));


                client.ModifyActivityCoupon(Guid.NewGuid(), activity.Result.PKID, 1, couponName);


                var i = 1;
                while (i <= 1000000)
                {
                    var guid = Guid.Parse("00000000-0000-0000-0000-00000" + i.ToString().PadLeft(7, '0'));
                    client.ModifyActivityCoupon(guid, activity.Result.PKID, i, couponName);
                    i++;
                }

                Assert.IsNotNull(true);
            }

        }



        /// <summary>
        ///     测试：清理活动问题缓存 和  清理活动兑换物缓存
        /// </summary>
        [TestMethod]
        public void TestActivityClearCache()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result = client.RefreshActivityQuestionCache(activity.Result.PKID);
                var result1 = client.RefreshActivityPrizeCache(activity.Result.PKID);

                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：保存用户答题数据到数据库
        /// </summary>
        [TestMethod]
        public void TestSubmitQuestionUserAnswer()
        {
            using (var client = new ActivityClient())
            {
                var result = client.SubmitQuestionUserAnswer(new SubmitActivityQuestionUserAnswerRequest()
                {
                    AnswerDate = DateTime.Now,
                    AnswerOptionContent = "测测",
                    AnswerOptionID = -555,
                    AnswerText = "否",
                    ObjectID = 0,
                    QuestionID = -44,
                    QuestionnaireID = -6,
                    QuestionnaireName = "测试问卷",
                    QuestionName = "测试",
                    QuestionScore = 0,
                    QuestionType = 0,
                    UseIntegral = 100,
                    UserId = Guid.Parse("A5E65A7A-0CEF-4FC3-B364-291182034B24"),
                    WinCouponCount = 10
                });


                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        ///     测试：更新用户答题结果状态
        /// </summary>
        [TestMethod]
        public void TestModifyQuestionUserAnswerResult()
        {
            using (var client = new ActivityClient())
            {
                var activity = client.GetWorldCup2018Activity();
                var result =
                    client.ModifyQuestionUserAnswerResult(new ModifyQuestionUserAnswerResultRequest()
                    {
                        ResultId = 205989,
                        AnswerResultStatus = 2,
                        WinCouponCount = 0
                    });

                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        ///     测试：添加途虎星级认证门店信息
        /// </summary>
        [TestMethod]
        public void TestAddStarRatingStore()
        {
            using (var client = new ActivityClient())
            {
                var model = new AddStarRatingStoreRequest()
                {
                    Phone = "1608938839",
                    UserName = "瑟西",
                    StoreName = "美容店",
                    Duty = "店长",
                    ProvinceID = 4,
                    CityID = 4,
                    DistrictID = 5,
                    ProvinceName = "上海",
                    CityName = "上海",
                    DistrictName = "李子园",
                    StoreAddress = "荣和",
                    StoreArea = 222.34M,
                    StoreNum = 4,
                    WorkPositionNum = 4,
                    MaintainQualification = "二类",
                    Storefront = "美容",
                    //StorefrontDesc="",
                    StoreLocation = "小区里",
                    IsAgree = true
                };
                var result = client.AddStarRatingStore(model);
                Assert.IsNotNull(result.Result);
            }
        }

        /// 蓄电池/加油卡活动配置
        /// </summary>
        [TestMethod]
        public void SelectCouponActivityConfigNew()
        {
            using (var client = new ActivityClient())
            {
                var request = new CouponActivityConfigRequest()
                {
                    ActivityNum = "d1e5ef83-ba3b-4726-8f02-6da64624f000",
                    Channel = "JIOS",
                    Type = 2
                };
                var result = client.SelectCouponActivityConfigNew(request);
                Assert.IsNotNull(result);
            }
        }

        /// <summary>
        /// 测试：分页获取报名参加活动的用户列表
        /// </summary>
        [TestMethod]
        public void GetUserRegistrationsByAreaTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetUserRegistrationsByArea(string.Empty).Result;
                Assert.AreEqual(35, result.Pager.Total);
            }
        }

        /// <summary>
        /// 测试：获取用户活动列表
        /// </summary>
        [TestMethod]
        public void GetUserActivitiesTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetUserActivities().Result;
                Assert.AreEqual("2018世界杯", result[0].Content);
            }
        }

        /// <summary>
        /// 测试：获取用户报名信息
        /// </summary>
        [TestMethod]
        public void GetUserRegistrationByIdTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.GetUserRegistrationById(3).Result;
                Assert.AreEqual("张三", result.Name);
            }
        }

        /// <summary>
        /// 测试：新增用户报名信息
        /// </summary>
        [TestMethod]
        public void AddUserRegistrationTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.AddUserRegistration(
                    new UserRegistrationRequest
                    {
                        Name = "王五",
                        Phone = "15555555555",
                        Area = "上海市闵行区",
                        ActivityId = 1
                    })
                    .Result;

                Assert.IsTrue(result);
            }
        }

        /// <summary>
        /// 测试：修改用户报名信息
        /// </summary>
        [TestMethod]
        public void ModifyUserRegistrationTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.ModifyUserRegistration(
                    new UserRegistrationRequest
                    {
                        PKID = 10,
                        Name = "赵六",
                        Phone = "16666666666",
                        Area = "浙江省杭州市",
                        ActivityId = 2
                    })
                    .Result;

                Assert.IsTrue(result);
            }
        }

        /// <summary>
        /// 测试：更新用户报名状态
        /// </summary>
        [TestMethod]
        public void UpdateUserRegistrationStatusTest()
        {
            using (var client = new ActivityClient())
            {
                var result = client.UpdateUserRegistrationStatus(
                    "未知地区", RegistrationStatus.Failed)
                    .Result;

                Assert.IsTrue(result);
            }
        }
    }
}
