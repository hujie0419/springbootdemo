using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Data;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>保养免单券 huhengxing 2016-12-8 </summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan1211Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan1211Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2016, 12, 11))
                {
                    Logger.Info("2016-12-11开始发放保养免单券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务" + id);
                var dt = DalCoupon.Get1211BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), RuleID = x["RuleID"].ToString(), OrderID = x["OrderID"].ToString() }).ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                foreach (var i in dic)
                {
                    if (!string.IsNullOrWhiteSpace(i.SumMoney))
                    {
                        Logger.Info(id + $"塞券{i.OrderID}-订单总金额{i.SumMoney}");
                        var items = DalCoupon.SelectOrderListByOrderId(i.OrderID);
                        try
                        {
                            var oil = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
                            var other = items.Where(x => x.Pid.LastIndexOf("OL-") != 0).ToList();
                            var oilMoney = oil.Select(x => x.TotalPrice).AsQueryable().Sum();
                            if (oilMoney > 300)
                                oilMoney = 300;

                            var otherMoney = other.Select(x => x.TotalPrice).AsQueryable().Sum();
                            var m = Convert.ToInt32((otherMoney + oilMoney).ToString("0"));
                            if (m > 500)
                                m = 500;

                            int result = CreateOrYzPromotion(i.UserID, m, i.OrderID);
                            if (result > 0)
                            {
                                string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(i.UserID));
                                BLL.Business.SendMarketingSms(userTel, "途虎养车双12保养免单");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("OrderID:" + i.OrderID + "查询订单列表数据出错" + JsonConvert.SerializeObject(items), ex);
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
                promotionCodeModel.Description = "仅限雪佛龙/海湾/胜牌/3M四大品牌机油到店保养使用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "双12保养免单活动";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 1073;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan1211Job");
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