using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Service.Order;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>保养免单券 huhengxing 2017-2-8 </summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan0208Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0208Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 2, 13))
                {
                    Logger.Info("2017-2-13开始发放开春保养免单券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务开春保养免单券" + id);
                var dt = DalCoupon.Get0208BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                using (var orderClient = new OrderQueryClient())
                {
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            List<string> orderIds = new List<string> { i.OrderID };
                            //获取拆单数据
                            int orderid = Convert.ToInt32(i.OrderID);
                            var splitOrderIds = orderClient.GetRelatedSplitOrderIDsAsync(orderid, Service.Order.Enum.SplitQueryType.Full);
                            if (splitOrderIds != null && splitOrderIds.Result != null && splitOrderIds.Result.Result != null && splitOrderIds.Result.Result.Any())
                            {
                                //属于拆单订单
                                var splitOrders = DalCoupon.SelectSplitOrderListByOrderIds(splitOrderIds.Result.Result);
                                if (splitOrders != null && splitOrders.Any())
                                {
                                    //一个订单已经安装了，另外一个还未安装------等两个订单都安装完成后返券
                                    if (splitOrders.Where(s => s.Status != "7Canceled" && s.Status != "3Installed").FirstOrDefault() != null)
                                    {
                                        Logger.Info(id + $"订单{i.OrderID}还存在其他未完成安装的关联订单，暂不返券");
                                        continue;
                                    }
                                    else
                                    {
                                        orderIds.AddRange(splitOrders.Where(s => s.Status == "3Installed").Select(s => s.PKID.ToString()));
                                    }
                                    //拆的单如果取消了怎么办-----那就只返没取消并且已安装的那个订单的金额
                                }
                            }

                            Logger.Info(id + $"塞券{i.OrderID}-订单总金额{i.SumMoney}");
                            var items = DalCoupon.SelectNoFuOrderListByOrderId(orderIds);
                            try
                            {
                                if (items != null && items.Any())
                                {
                                    Logger.Info(id + $"塞券关联订单{string.Join(",", orderIds)}-关联订单总金额{items.Sum(l => l.TotalPrice)}");
                                    //获取优惠券金额 机油上限300，总额上限400
                                    var m = DalCoupon.GetOrderPromotionMoney(items.ToList(), 300, 400);
                                    int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                        BLL.Business.SendMarketingSms(userTel, "途虎养车开春保养买一送一");
                                    }
                                }
                                else
                                {
                                    Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错,数据为空");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错,数据:" + JsonConvert.SerializeObject(items), ex);
                            }
                        }
                    }
                }
                Logger.Info("结束任务" + id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "指定品牌专享 年卡/套餐/安装费不可用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "开春保养买一送一返券活动";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 10604;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0208Job");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }
    }
}