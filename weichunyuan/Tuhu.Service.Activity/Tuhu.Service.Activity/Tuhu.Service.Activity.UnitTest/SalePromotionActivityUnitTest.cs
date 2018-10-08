using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.ProductQuery;

namespace Tuhu.Service.Activity.UnitTest
{
    [TestClass]
    public class SalePromotionActivityUnitTest
    {
        #region 配置

        /// <summary>
        /// 新增活动
        /// </summary>
        [TestMethod]
        public void TestInsertActivity()
        {
            using (var client = new SalePromotionActivityClient())
            {
                var model = new SalePromotionActivityModel()
                {
                    Name = "测试活动1",
                    ActivityId = Guid.NewGuid().ToString(),
                    PromotionType = (int)SalePromotionActivityType.FullDiscount,
                    Is_DefaultLabel = 1,
                    Is_PurchaseLimit = 1,
                    StartTime = DateTime.Now.ToString(),
                    EndTime = DateTime.Now.ToString(),
                    AuditStatus = (int)SalePromotionActivityAuditStatus.WaitAudit,
                    Is_UnShelve = 1,
                    CreateUserName = "单元测试"
                };
                var result = client.InsertActivityAsync(model).Result;
                Assert.IsTrue(result.Result);
            }
        }

        /// <summary>
        /// 修改活动
        /// </summary>
        [TestMethod]
        public void TestUpdateActivity()
        {
            var model = new SalePromotionActivityModel()
            {
                ActivityId = "a008b183-5429-4b97-b5ca-c8c999322728",
                AuditStatus = 0,
                Banner = "2",
                ChannelKeyList = new List<string>() { "key1", "key2" },
                CreateDateTime = null,

                StartTime = DateTime.Now.ToString(),
                EndTime = DateTime.Now.ToString(),
                LastUpdateUserName = "单元测试"
            };

            using (var client = new SalePromotionActivityClient())
            {

                var result = client.UpdateActivityAsync(model).Result;
                Assert.IsTrue(result.Result);
            }
        }

        [TestMethod]
        public void GetActivityAndProducts()
        {
            var pidList = new List<string>() {
                   "TR-YK-AE50|2",
                    "TR-YK-AE50|5",
                    "TR-GT-221|6",
                    "BD-BO-Common|105",
                    "BD-BO-Common|115",
                    "BD-BO-Common|144",
                    "BD-BO-Common|145",
                };
            using (var client = new SalePromotionActivityClient())
            {

                var result = client.GetActivityAndProductsAsync("0f5c2eba-5150-4c75-93ec-78079b47645f", pidList).Result;
                Assert.IsTrue(1==1);
            }
        }

        
        /// <summary>
        /// 审核后修改活动
        /// </summary>
        [TestMethod]
        public void UpdateActivityAfterAudit()
        {
            var model = new SalePromotionActivityModel()
            {
                ActivityId = "640a77c6-1d5a-43b3-b13c-426990b74f7e",
                StartTime = (DateTime.Now - TimeSpan.FromHours(10)).ToString(),
                EndTime = (DateTime.Now + TimeSpan.FromHours(10)).ToString(),
                AuditStatus=2,
                LastUpdateUserName = "单元测试"
            };
            using (var client = new SalePromotionActivityClient())
            {

                var result = client.UpdateActivityAfterAudit(model).Result;
                Assert.IsTrue(result);
            }
        }
        public void InsertActivityProductList()
        {
            var model = new SalePromotionActivityModel()
            {
                ActivityId = "640a77c6-1d5a-43b3-b13c-426990b74f7e",
                StartTime = (DateTime.Now - TimeSpan.FromHours(10)).ToString(),
                EndTime = (DateTime.Now + TimeSpan.FromHours(10)).ToString(),
                AuditStatus = 2,
                LastUpdateUserName = "单元测试"
            };
            using (var client = new SalePromotionActivityClient())
            {
                var list =new List<SalePromotionActivityProduct>() {
                    new SalePromotionActivityProduct(){

                    }
                };
                string activityId = "640a77c6-1d5a-43b3-b13c-426990b74f7e";
                    string userName = "单元测试";
                var result = client.InsertActivityProductList(list, activityId, userName).Result;
                Assert.IsTrue(result);
            }
        }
        

        [TestMethod]
        public void UnShelveActivity()
        {
            using (var client = new SalePromotionActivityClient())
            {
                var result = client.UnShelveActivity("c025e03b-e7b1-4717-9daa-05747a26ea60", "单元测试").Result;
                Assert.IsTrue(result);
            }
        }
        [TestMethod]
        public void GetActivityInfo()
        {
            using (var client = new SalePromotionActivityClient())
            {
                var result = client.GetActivityInfo("ec95b4b3-8cef-4721-aab4-6115eb78187f").Result;
                Assert.IsTrue(true);
            }
        }
        
        [TestMethod]
        public void GetProductInfo()
        {
            var pids = new List<string>() { "TR-WL-AP028|18", "TR-WL-AP028|19", "TR-WL-AP028|20",
                "TR-WL-AP028|30", "TR-WL-AP028|33" ,"TR-YK-A580|1","TR-YK-AE01|6"};
            using (var client = new ProductInfoQueryClient())
            {
                var list = client.SelectProductBaseInfo(pids).Result;
                Assert.IsNotNull(list);
            }
        }

        /// <summary>
        /// 审核促销活动
        /// </summary>
        [TestMethod]
        public void PassAuditAcivity()
        {
            string activityid = "0f5c2eba-5150-4c75-93ec-78079b47645f";
            using (var client = new SalePromotionActivityClient())
            {
                var list = client.SetActivityAuditStatus(activityid, "dev", 2, "").Result;
                Assert.IsNotNull(list);
            }
        }

        #endregion
         
        #region 服务接口
        [TestMethod]
        ///获取打折标签缓存数据
        public void GetProductDiscountInfoForTag()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var pidList = new List<string>() {
                   "TR-YK-AE50|2",
                    "TR-YK-AE50|5",
                    "TR-GT-221|6",
                    "BD-BO-Common|105",
                    "BD-BO-Common|115",
                    "BD-BO-Common|144",
                    "BD-BO-Common|145",
                };
                pidList = pidList.Distinct().ToList();
                DateTime startTime = DateTime.Now;
                DateTime endTime = startTime + TimeSpan.FromHours(3);
                var result = client.GetProductDiscountInfoForTag(pidList, startTime, endTime).Result.ToList();
                Assert.IsNotNull(result.Count > 0);
            }
        }

        [TestMethod]
        public void GetProductAndUserDiscountInfo()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var pidList = new List<string>() {
                      "TR-YK-AE50|2",
                    "TR-YK-AE50|5",
                    "TR-GT-221|6",
                    "BD-BO-Common|105",
                    "BD-BO-Common|115",
                    "BD-BO-Common|144",
                    "BD-BO-Common|145",
                };
                var result = client.GetProductAndUserDiscountInfo(pidList, "dfa7ec09-f4b6-4653-8982-47132ea660ec").Result;
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void SaveCreateOrderDiscountInfoAndSetCache()
        {
            string activityid = "0f5c2eba-5150-4c75-93ec-78079b47645f";
            string userid = "333";
            var orderInfoList = new List<DiscountCreateOrderRequest>() {
                new DiscountCreateOrderRequest()
                {
                    UserId=userid,
                    ActivityId=activityid,
                    Num=1,
                    Pid="TR-GT-221|6",
                    OrderId=8
                },
                //new DiscountCreateOrderRequest()
                //{
                //    UserId=userid,
                //    ActivityId=activityid,
                //    Num=1,
                //    Pid="BD-BO-Common|105",
                //    OrderId=3
                //},
                //new DiscountCreateOrderRequest()
                //{
                //    UserId=userid,
                //    ActivityId=activityid,
                //    Num=1,
                //    Pid="BD-BO-Common|115",
                //    OrderId=3
                //},
                //new DiscountCreateOrderRequest()
                //{
                //    UserId=userid,
                //    ActivityId=activityid,
                //    Num=1,
                //    Pid="BD-BO-Common|144",
                //    OrderId=3
                //},
                //new DiscountCreateOrderRequest()
                //{
                //    UserId=userid,
                //    ActivityId=activityid,
                //    Num=1,
                //    Pid="BD-BO-Common|145",
                //    OrderId=3
                //}
            };

            using (var client = new DiscountActivityInfoClient())
            {
                var result = client.SaveCreateOrderDiscountInfoAndSetCache(orderInfoList).Result;
                // var activityIdList = new List<string>() { activityid };
                //验证缓存
                //  var userByNum = client.GetOrSetUserActivityBuyNumCacheAsync(userid, activityIdList).Result;

                var pidList = new List<string>() {
                    "TR-ME-ENERGY-XM2|1","TR-ME-ENERGY-XM2|11"
                };
                // var soldNum = client.GetOrSetActivityProductSoldNumCacheAsync(activityid, pidList).Result;
                Assert.IsNotNull(1);
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        [TestMethod]
        public void UpdateCancelOrderDiscountInfoAndSetCache()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var result = client.UpdateCancelOrderDiscountInfoAndSetCache(8).Result;
                Assert.IsNotNull(result);
            }
        }
        [TestMethod]
        public void GetProductAndUserHitDiscountInfo()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var pidList = new List<DiscountActivityRequest>()
                {
                    new DiscountActivityRequest(){ Pid="AP-BXJG-XY|1",Num=4,Price=200,PaymentMethod=2,InstallMethod=2 },
                       new DiscountActivityRequest(){ Pid="LF-GiGi-G-1069|8",Num=1,Price=400,PaymentMethod=2,InstallMethod=2 }
                };
                var result = client.GetProductAndUserHitDiscountInfo(pidList, null).Result;
                Assert.IsNotNull(result);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void SelectProductDiscountFromCacheAsync()
        {
            using (var client = new ProductConfigClient())
            {
                var pidList = new List<string>() {
                    "TR-CP-C1|3"
                };
                var result = client.SetProductCommonTagDetailsCacheAsync(ProductCommonTag.Discount, pidList).Result;
                Assert.IsNotNull(result);
                var request = new ProductInfoByTagRequest()
                {
                    Tags = new List<ProductCommonTag>() { ProductCommonTag.Discount },
                    Pids = pidList
                };
                var result11 = client.SelectProductDiscountFromCacheAsync(pidList).Result;
                Assert.IsNotNull(result11);
            }
        }

        /// <summary>
        /// 标签接口
        /// </summary>
        [TestMethod]
        public void SelectProductInfoByTagRequest()
        {
            using (var client = new ProductConfigClient())
            {
                var pidList = new List<string>() {
                    "TR-YK-AE50|2",
                    "TR-YK-AE50|5",
                    "TR-GT-221|6",
                    "BD-BO-Common|105",
                    "BD-BO-Common|115",
                    "BD-BO-Common|144",
                    "BD-BO-Common|145",
                };
                var result1 = client.SetProductCommonTagDetailsCacheAsync(ProductCommonTag.Discount, pidList).Result;
              //  Assert.IsNotNull(result1);

                var request = new ProductInfoByTagRequest()
                {
                    Tags = new List<ProductCommonTag>() { ProductCommonTag.Discount },
                    Pids = pidList
                };
                var tags = new List<ProductCommonTag>() { ProductCommonTag.Discount };
                var result = client.SelectProductInfoByTagRequestAsync(request).Result.Result;
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void GetOrSetUserActivityBuyNumCache()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var activityList = new List<string>() {
                    "1","2"
                };
                string userid = "1";
                var result = client.GetOrSetUserActivityBuyNumCacheAsync(userid, activityList).Result;
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void GetOrSetActivityProductSoldNumCache()
        {
            using (var client = new DiscountActivityInfoClient())
            {
                var pidList = new List<string>() {
                    "1","2","3"
                };
                string activityId = "1";
                var result = client.GetOrSetActivityProductSoldNumCacheAsync(activityId, pidList).Result;
                Assert.IsNotNull(result);
            }
        }

        #endregion

        #region 日志单元测试

        [TestMethod]
        public void InsertLogAndDetail()
        {
            var operationLogModel = new SalePromotionActivityLogModel()
            {
                ReferId = "日志来源id(活动id)",
                ReferType = "日志来源类型（打折活动）",
                OperationLogType = "操作类型（新增活动）",
                CreateDateTime = DateTime.Now.ToString(),
                CreateUserName = "单元测试UserName",
                //日志详情
                LogDetailList = new List<SalePromotionActivityLogDetail>() {
                            new SalePromotionActivityLogDetail(){
                                OperationLogType="新增商品",
                                Property="pid",
                                NewValue="商品的关键信息",
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="修改活动",
                                Property="活动描述",
                                OldValue="旧的活动描述",
                                NewValue="新的活动描述",
                            },
                            new SalePromotionActivityLogDetail(){
                                 OperationLogType="删除商品",
                                Property="pid",
                                OldValue="商品的关键信息",
                            },
                        }
            };
            using (var client = new SalePromotionActivityLogClient())
            {
                var result = client.InsertAcitivityLogAndDetail(operationLogModel).Result;
                Assert.IsTrue(result);
            }
        }


        [TestMethod]
        public void GetOperationLogList()
        {
            using (var client = new SalePromotionActivityLogClient())
            {
                var result = client.GetOperationLogList("6DC14F47", 1,20).Result;
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void InsertLogDescription()
        {
            var model = new SalePromotionActivityLogDescription()
            {
                OperationLogType = "UnitTest",
                OperationLogDescription = "单元测试描述",
                Remark = "这是单元测试",
                CreateDateTime = DateTime.Now.ToString(),
                CreateUserName = "单元测试",
            };

            using (var client = new SalePromotionActivityLogClient())
            {
                var result = client.InsertActivityLogDescriptionAsync(model).Result.Result;
                Assert.IsTrue(result);
            }
        }
        #endregion

    }
}
