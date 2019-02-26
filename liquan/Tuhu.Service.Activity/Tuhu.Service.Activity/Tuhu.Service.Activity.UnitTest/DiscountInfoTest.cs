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
    public class DiscountInfoTest
    {
        [TestMethod]
        public void GetProductDiscountInfoForTag()
        {
            //验证情况：传入的pid都是有活动的pid

            //准备参数
            string userId = "333";
            string activityId = "7c709b97-88cf-4a42-a364-35893721da4e";
            int orderId = 0;//338;
            var hitRequestList = new List<DiscountActivityRequest>()//下单验证
                {
                    new DiscountActivityRequest(){ Pid="TR-ME-ENERGY-XM2|17",Num=5,Price=818},
                      // new DiscountActivityRequest(){ Pid="LF-GiGi-G-1069|8",Num=1,Price=400,PaymentMethod=2,InstallMethod=2 }
                };
            var pidList = new List<string>() {
                    "TR-ME-ENERGY-XM2|17", 
                };
            var orderInfoList = new List<DiscountCreateOrderRequest>() {
                new DiscountCreateOrderRequest()
                {
                    UserId=userId,
                    ActivityId=activityId,
                    Num=2,
                    Pid="TR-ME-ENERGY-XM2|17",
                    OrderId=orderId
                },
                //new DiscountCreateOrderRequest()
                //{
                //    UserId=userid,
                //    ActivityId=activityid,
                //    Num=1,
                //    Pid="BD-BO-Common|105",
                //    OrderId=orderId
                //},
            };
            var pidCount = pidList.Distinct().Count();
            //1.验证打折标签
            using (var client = new ProductConfigClient())
            {
                //刷新标签缓存
                //var result1 = client.SetProductCommonTagDetailsCacheAsync(ProductCommonTag.Discount, pidList).Result;
                var request = new ProductInfoByTagRequest()
                {
                    Tags = new List<ProductCommonTag>() { ProductCommonTag.Discount },
                    Pids = pidList
                };
                var tags = new List<ProductCommonTag>() { ProductCommonTag.Discount };
                var result = client.SelectProductInfoByTagRequestAsync(request).Result.Result;
                var descList = result.Where(a=>a.Value.Count>0).Select(a => a.Value[0].TagDescription);
                var hasDesc = descList.Where(a => !string.IsNullOrWhiteSpace(a)).Any();
                //验证是否有标签
                Assert.IsNotNull(hasDesc);
            }
            //2.供标签缓存使用的接口
            using (var client = new DiscountActivityInfoClient())
            {
                pidList = pidList.Distinct().ToList();
                DateTime startTime = DateTime.Now;
                DateTime endTime = startTime + TimeSpan.FromHours(12);
                var result = client.GetProductDiscountInfoForTag(pidList, startTime, endTime).Result.ToList();
                 
                Assert.IsTrue(result.Count == pidCount);
            }

            //3.验证详情页是否有打折信息和用户限购
            using (var client = new DiscountActivityInfoClient())
            {
                var result = client.GetProductAndUserDiscountInfo(pidList, userId).Result;
                var resultList = result.Where(a=>a.HasDiscountActivity);
                Assert.IsTrue(resultList.Count()== pidCount); 
            }

            //3.验证是否可以下单
            using (var client = new DiscountActivityInfoClient())
            {
                var result = client.GetProductAndUserHitDiscountInfo(hitRequestList, null).Result;
                var resultList = result.Where(a => a.HasDiscountActivity);
                Assert.IsTrue(resultList?.Count()==pidCount);
            }
            if (orderId > 0)
            {
                //4.验证下单是否成功
                using (var client = new DiscountActivityInfoClient())
                {
                    var result = client.SaveCreateOrderDiscountInfoAndSetCache(orderInfoList).Result;
                    Assert.IsTrue(result);
                }
            }

            using (var client = new DiscountActivityInfoClient())
            {
                //验证活动用户已购数缓存
                var userByNum = client.GetOrSetUserActivityBuyNumCacheAsync(userId, new List<string>() { activityId }).Result.Result;
                var userByNumList = userByNum.Where(a=>a.BuyNum>0);
                Assert.IsTrue(userByNumList?.Count() > 0);
                //验证活动商品已售数量缓存
                var soldNum = client.GetOrSetActivityProductSoldNumCacheAsync(activityId, pidList).Result.Result;
                var soldNumList = soldNum.Where(a => a.SoldNum > 0);
                Assert.IsTrue(soldNumList?.Count() ==pidCount);
            }
            if (orderId > 0)
            {
                //5.取消订单
                using (var client = new DiscountActivityInfoClient())
                {
                    var result = client.UpdateCancelOrderDiscountInfoAndSetCache(orderId).Result;
                    Assert.IsTrue(result);
                }
            }

            //取消订单后验证已购数量和已售数量
            using (var client = new DiscountActivityInfoClient())
            {
                //验证活动用户已购数缓存
                var userByNum = client.GetOrSetUserActivityBuyNumCacheAsync(userId, new List<string>() { activityId }).Result.Result;
                var userByNumList = userByNum.Where(a => a.BuyNum == 0);
                Assert.IsTrue(userByNumList?.Count() > 0);
                //验证活动商品已售数量缓存
                var soldNum = client.GetOrSetActivityProductSoldNumCacheAsync(activityId, pidList).Result.Result;
                var soldNumList = soldNum.Where(a => a.SoldNum == 0);
                Assert.IsTrue(soldNumList?.Count() == pidCount);
            }
        }
    }
}
