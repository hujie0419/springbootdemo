using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuhu.Service.Activity.Models;
using System.Collections.Generic;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class FlashSaleUnitTest
    {
        [TestMethod]
        public void SelectFlashSaleDataByActivityIDAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.SelectFlashSaleDataByActivityID(new Guid("505be422-0fcc-40de-8c8e-5dfc2d486841"));

                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetFlashSaleList()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetFlashSaleList(new Guid[] { Guid.Parse("971832e8-5b79-4612-8960-e9bac9e862be") });

                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void UpdateFlashSaleDataToCouchBaseByActivityIDAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.UpdateFlashSaleDataToCouchBaseByActivityID(Guid.Parse("3066c016-e2ab-4ebc-a0f1-c9d23f3969fd"));

                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetUserCanBuyFlashSaleItemCountAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetUserCanBuyFlashSaleItemCount(Guid.Parse("743c2104-b8c1-4849-9407-0c8a6440ced9"), Guid.Parse("9d2b149d-3306-4458-8af9-88071c86972d"), "AP-JBL-C100SI|3");

                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void CheckCanBuyFlashSaleOrderAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.CheckCanBuyFlashSaleOrder(
                    new Models.Requests.FlashSaleOrderRequest()
                    {
                        UseTel = "15856315315",
                        UserId = Guid.Parse("744A690F-BEF3-4694-BC55-8A47DDC891DE"),
                        Products = new List<Models.Requests.OrderItems>()
                        {
                            new Models.Requests.OrderItems()
                            {
                                ActivityId=Guid.Parse("328c3cbe-d3f8-42fb-9fd2-4e6f0d763f39"),
                                PID="WP-BO-NFENGYI|14",
                                Num=1
                            }
                        }

                    }
                    );

                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void CheckFlashSaleProductBuyLimit()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.CheckFlashSaleProductBuyLimit(new Models.CheckFlashSaleProductRequest()
                {
                    DeviceID = null,
                    UserID = Guid.Parse("D37F50BA-07E8-4AC8-96B3-F64B94257058"),
                    ActivityProducts = new List<FlashSaleProductBuyLimitModel>() {
                       new FlashSaleProductBuyLimitModel() {
                           ActivityID=Guid.Parse("315dd457-1713-432f-a22f-fe2a36c62b94"),
                           PID="AP-WINDEK-RCP-C43L|2",
                           Num=2
                       },
                        new FlashSaleProductBuyLimitModel() {
                           ActivityID=Guid.Parse("315dd457-1713-432f-a22f-fe2a36c62b94"),
                           PID="AP-YLX-Q3H-3A|3",
                           Num=1
                       },
                         new FlashSaleProductBuyLimitModel() {
                           ActivityID=Guid.Parse("315dd457-1713-432f-a22f-fe2a36c62b94"),
                           PID="LG-FT-101-15-6001X|2",
                           Num=6
                       },
                         new FlashSaleProductBuyLimitModel() {
                           ActivityID=Guid.Parse("7684d759-3e00-4c43-a876-622f45d9e789"),
                           PID="LI-TUYA-FRSCG|2",
                           Num=4
                       },
                         new FlashSaleProductBuyLimitModel() {
                           ActivityID=Guid.Parse("7684d759-3e00-4c43-a876-622f45d9e789"),
                           PID="AP-KUST-KLZ-SCD|2",
                           Num=1
                       },
                   }
                });
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void FetchProductDetailForFlashSaleAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.FetchProductDetailForFlashSale("TR-ME-ENERGY-XM2", "17",
                    "0B56A0C0-08DB-484A-9414-653453CD2DCA", "app", "21fa116e-5aaf-4dd8-9ee0-c2d93cb026c6");
                Assert.IsNotNull(result.Result);
                // result.ThrowIfException(true);
            }
        }

        [TestMethod]
        public void SelectSecondKillTodayDataAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.SelectSecondKillTodayData(1, DateTime.Parse("2018-10-17"), true, true);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void OrderCancerMaintenanceFlashSaleData()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.OrderCancerMaintenanceFlashSaleData(1677872);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void RefreshFlashSaleHashCache()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.RefreshFlashSaleHashCount(new List<string> { "05bb176-ffd8-4c84-b790-95e3d73c18ce" }, false);
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void GetFlashSaleList2()
        {
            try
            {
                using (var client = new FlashSaleClient())
                {
                    var result = client.GetFlashSaleWithoutProductsList(new List<Guid>() { new Guid() });
                    result.ThrowIfException();
                    Assert.IsNotNull(result.Result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void CacheGetTest()
        {
            try
            {
                var request = new OrderCountCacheRequest
                {
                    ActivityId = "89a02e1a-f560-4460-875a-98839c945954",
                    UserId = "1AB0FA62-91BB-4FE7-8CF2-EB57367463D4",
                    UserTel = "18890495250",
                    Pid = "LG-FT-504-16-7001X|39",
                    DeviceId = "8805a4237859e2f86d130e3933eb0ed0"
                };
                using (var client = new FlashSaleClient())
                {
                    var result = client.GetUserCreateFlashOrderCountCache(request);
                    result.ThrowIfException();
                    Assert.IsNotNull(result.Result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [TestMethod]
        public void CacheSetTest()
        {
            try
            {
                var request = new OrderCountCacheRequest
                {
                    ActivityId = "dde07654-f666-4648-9a26-bdfadb0166e5",
                    UserId = "21fa116e-5aaf-4dd8-9ee0-c2d93cb026c7",
                    UserTel = "13671714523",
                    Pid = "LG-FT-503|11",
                    PerSonalNum = 4,
                    PlaceNum = 4
                };
                using (var client = new FlashSaleClient())
                {
                    var result = client.SetUserCreateFlashOrderCountCache(request);
                    result.ThrowIfException();
                    Assert.IsNotNull(result.Result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void SelectFlashSaleWrongCacheAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.SelectFlashSaleWrongCache();
                result.ThrowIfException();
                Assert.IsNotNull(result.Result);
            }
        }
        [TestMethod]
        public void OrderCreateMaintenanceFlashSaleDbDataAsync()
        {
            var request = new FlashSaleOrderRequest()
            {
                DeviceId = "123456",
                OrderId = 7788,
                UserId = new Guid("21fa116e-5aaf-4dd8-9ee0-c2d93cb026c7"),
                UseTel = "13671714523",
                Products = new List<OrderItems>
                {
                    new OrderItems()
                    {
                        Num = 1,
                        PID = "LG-FT-503|11",
                        ActivityId = new Guid("dde07654-f666-4648-9a26-bdfadb0166e5")
                    }

                }
            };
            using (var client = new FlashSaleClient())
            {
                var result = client.OrderCreateMaintenanceFlashSaleDbData(request);
                result.ThrowIfException();
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void UpdateConfigSaleoutQuantityFromLogAsync()
        {
            var request = new UpdateConfigSaleoutQuantityRequest
            {
                RefreshType = RefreshType.RefreshByActivityId,
                ProductModels = new List<UpdateQuantityProductModel>()
                {
                    new UpdateQuantityProductModel()
                    {
                        ActivityId = new Guid("0E01F706-9B2E-4C04-875F-0AE7DC398A37")

                    },
                    new UpdateQuantityProductModel()
                    {
                        ActivityId = new Guid("8312FE9D-6DCE-4CD5-87D5-99CE064336A3")

                    }
                }

            };
            using (var client = new FlashSaleClient())
            {
                var result = client.UpdateConfigSaleoutQuantityFromLog(request);
                result.ThrowIfException();
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void TestInsertEveryDaySeckillUserInfo()
        {
            var model = new EveryDaySeckillUserInfo()
            {
                UserId = new Guid("95cc0733-c9a7-4504-a29b-26c28072737f"),
                MobilePhone = "15921252821",
                FlashActivityGuid = new Guid("f4e155be-702c-4117-8d06-89c4800f2164"),
                Pid = "AP-BOTNY-B1999-8TH|1"
            };
            using (var client = new FlashSaleClient())
            {
                var response = client.InsertEveryDaySeckillUserInfo(model);
            }
        }

        [TestMethod]
        public void TestGetUserReminderInfo()
        {
            var model = new EveryDaySeckillUserInfo()
            {
                UserId = new Guid("F1D59B13-FFFD-62B9-C416-0A452AD112B0"),
                MobilePhone = "13621840035",
                FlashActivityGuid = new Guid("F4E155BE-702C-4117-8D06-89C4800F2164"),
                Pid = "AP-BXJG-JZDMJ|1"
            };
            using (var client = new FlashSaleClient())
            {
                var response = client.GetUserReminderInfo(model);
            }
        }
        [TestMethod]
        public void RefreshSeckillDefaultDataBySchedule()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.RefreshSeckillDefaultDataBySchedule("20点场");
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetSeckillScheduleInfo()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetSeckillScheduleInfo(new List<string>
            {
                "LF-GiGi-G-1069|6"
            }, DateTime.Now.AddMinutes(-30), DateTime.Now.AddHours(2));
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetSeckillAvailableStockResponseAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetSeckillAvailableStockResponse(new List<SeckillAvailableStockInfoRequest>
            {
                    new SeckillAvailableStockInfoRequest()
                        {
                        ActivityId =new Guid("505be422-0fcc-40de-8c8e-5dfc2d486841"),
                         Pid="LF-GiGi-G-1069|6"
                        },
                new SeckillAvailableStockInfoRequest()
                    {
                        ActivityId =new Guid("785dcdd4-d55a-482d-b9da-78215362f7f2"),
                         Pid="AP-TH-CZSHZJ|8"
                }
            });
                Assert.IsNotNull(result.Result);
            }
        }


        [TestMethod]
        public void GetSecondsBoysAsync()
        {
            using (var client = new FlashSaleClient())
            {
                DateTime startTime = Convert.ToDateTime("2018-05-31 23:01:00");
                DateTime endTime = Convert.ToDateTime("2018-06-13 10:00:00");
                var result = client.GetSecondsBoysAsync(3, startTime, endTime);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void SelectFlashSaleDataByActivityIdsAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var listFlashSaleDataByActivityRequest = new List<FlashSaleDataByActivityRequest>();

                //var flashSaleDataByActivityRequest = new FlashSaleDataByActivityRequest
                //{
                //    activityID = new Guid("8312FE9D-6DCE-4CD5-87D5-99CE064336A3"),
                //    pids = new List<string>() { "AP-BOTNY-B1866|", "AP-SL-DZG|3", "AP-WRC-22380|1", "LF-CLEAR-CARLAS-C82|" }
                //};
                //listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest);

                var flashSaleDataByActivityRequest2 = new FlashSaleDataByActivityRequest
                {
                    activityID = new Guid("663543D4-5B62-4133-935D-F0A4C68751F8"),
                    pids = new List<string>() { "AP-Babyfirst-HWDJD|2", "AP-LTL-AQZY-LLXB|2" }
                };
                listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest2);


                var flashSaleDataByActivityRequest3 = new FlashSaleDataByActivityRequest
                {
                    activityID = new Guid("EEDC44CD-35F5-4152-990B-5F22FD039EDB")
                };
                listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest3);

                var result = client.SelectFlashSaleDataByActivityIdsAsync(listFlashSaleDataByActivityRequest).Result;
                Assert.IsNotNull(result);
            }
        }


        /// <summary>
        /// 天天秒杀首页
        /// </summary>
        [TestMethod]
        public void SelectHomeSeckillDataAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var request = new SelectHomeSecKillRequest
                {
                    //scheduleDate = DateTime.Parse("2018-09-18")
                };
                var result = client.SelectHomeSeckillData(request);
                Assert.IsNotNull(result.Result);
            }
        }

        /// <summary>
        /// 天天秒杀列表
        /// </summary>
        [TestMethod]
        public void SelectSeckillDataByActivityId()
        {
            using (var client = new FlashSaleClient())
            {
                var request = new SelectSecKillByIdRequest
                {
                    activityId = new Guid("e6a5aa96-618b-4d12-a758-704740fde7f0"),
                    pageIndex = 1,
                    pageSize = 10
                };
                var result = client.SelectSeckillDataByActivityId(request);
                Assert.IsNotNull(result.Result);
            }
        }


        [TestMethod]
        public void SpikeListRefreshAsyncTest()
        {
            using (var client = new FlashSaleClient())
            {
                var activityId = new Guid("");
                var result = client.SpikeListRefreshAsync(activityId);
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        /// 活动页秒杀测试
        /// </summary>
        [TestMethod]
        public void GetActivePageSecondKillTest()
        {
            using (var client = new FlashSaleClient())
            {
                var result = client.GetActivePageSecondKillAsync(5, true);
                Assert.IsNotNull(result.Result);
            }
        }


        /// <summary>
        /// 活动页秒杀测试
        /// </summary>
        [TestMethod]
        public void GenerateKeyPrefixTest()
        {
            using (var client = new CacheClient())
            {
                RefreshCachePrefixRequest refreshCachePrefixRequest = new RefreshCachePrefixRequest();
                refreshCachePrefixRequest.ClientName = "FlashSale";
                refreshCachePrefixRequest.Key = "ActivePageSecondKill";
                refreshCachePrefixRequest.Prefix = "ActivePageSecondKill";
                var result = client.RefreshRedisCachePrefixForCommonAsync(refreshCachePrefixRequest);
                Assert.IsNotNull(result.Result);
            }
        }

        [TestMethod]
        public void GetFlashSaleDataActivityPageByIdsAsync()
        {
            using (var client = new FlashSaleClient())
            {
                var listFlashSaleDataByActivityRequest = new List<FlashSaleActivityPageRequest>();

                //var flashSaleDataByActivityRequest = new FlashSaleDataByActivityRequest
                //{
                //    activityID = new Guid("8312FE9D-6DCE-4CD5-87D5-99CE064336A3"),
                //    pids = new List<string>() { "AP-BOTNY-B1866|", "AP-SL-DZG|3", "AP-WRC-22380|1", "LF-CLEAR-CARLAS-C82|" }
                //};
                //listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest);

                var flashSaleDataByActivityRequest2 = new FlashSaleActivityPageRequest
                {
                    activityID = new Guid("663543D4-5B62-4133-935D-F0A4C68751F8"),
                    pids = new List<string>() { "AP-Babyfirst-HWDJD|2", "AP-LTL-AQZY-LLXB|2" }
                };
                listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest2);


                var flashSaleDataByActivityRequest3 = new FlashSaleActivityPageRequest
                {
                    activityID = new Guid("EEDC44CD-35F5-4152-990B-5F22FD039EDB")
                };
                listFlashSaleDataByActivityRequest.Add(flashSaleDataByActivityRequest3);

                var result = client.GetFlashSaleDataActivityPageByIdsAsync(listFlashSaleDataByActivityRequest).Result;
                Assert.IsNotNull(result);
            }
        }
    }
}
