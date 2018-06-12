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
    /// <summary>
    /// 已过期
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan0310Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0310Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 3, 14))
                {
                    Logger.Info("2017-3-14开始发放开春保养免单券");
                    return;
                }
                if (DateTime.Now > new DateTime(2017, 6, 1))
                {
                    Logger.Info("2017-6-1结束发放开春保养免单券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务开春保养免单券" + id);
                var dt = DalCoupon.Get0310BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), IsMenDian = Convert.ToInt32(x["IsMenDian"]), OrderID = x["OrderID"].ToString() }).Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                using (var orderClient = new OrderQueryClient())
                {
                    foreach (var i in dic)
                    {
                        if (!string.IsNullOrWhiteSpace(i.SumMoney))
                        {
                            List<string> orderIds = new List<string> { i.OrderID };
                            if (i.IsMenDian == 0)//使用优惠券
                            {
                                //获取拆单数据
                                int orderid = Convert.ToInt32(i.OrderID);
                                var splitOrderIds = orderClient.GetRelatedSplitOrderIDsAsync(orderid, Service.Order.Enum.SplitQueryType.Full);
                                if (splitOrderIds != null && splitOrderIds.Result != null && splitOrderIds.Result.Result != null && splitOrderIds.Result.Result.Any())
                                {
                                    var splitOrdersString = string.Join(",", splitOrderIds.Result.Result);
                                    Logger.Info(id + $"塞券订单{i.OrderID}调用订单服务获取拆单数据：{splitOrdersString}");
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
                                Logger.Info(id + $"塞券订单{i.OrderID}调用订单服务未获取到拆单数据");
                            }

                            Logger.Info(id + $"塞券订单{i.OrderID}-订单总金额{i.SumMoney}");
                            var items = DalCoupon.SelectOrderListByOrderIds(orderIds);
                            try
                            {
                                if (items != null && items.Any())
                                {
                                    Logger.Info(id + $"塞券关联订单{string.Join(",", orderIds)}-关联订单总金额{items.Sum(s => s.TotalPrice)}");
                                    var m = DalCoupon.GetOrderPromotionMoneyFor0310(items.ToList());
                                    int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                        BLL.Business.SendMarketingSms(userTel, "途虎工场店出光保养买一送一");
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
                promotionCodeModel.Description = "深圳、东莞工场店专用 出光机油抵用券";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "途虎工场店出光保养买一送一活动";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 29627;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0310Job");
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