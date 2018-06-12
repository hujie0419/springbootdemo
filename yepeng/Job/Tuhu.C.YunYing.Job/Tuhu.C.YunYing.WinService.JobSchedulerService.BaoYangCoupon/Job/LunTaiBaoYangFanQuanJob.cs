using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>双11轮胎返保养券 huhengxing 2016-11-8 </summary>
    [DisallowConcurrentExecution]
    public class LunTaiBaoYangFanQuanJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LunTaiBaoYangFanQuanJob));

        public const string LunTaiBaoYangFanQuanName = "双11轮胎返保养券活动";
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < Convert.ToDateTime("2016-11-14 12:00:00"))
                {
                    Logger.Info("2016-11-14 12:00开始发放双11轮胎返保养券");
                    return;
                }

                Logger.Info("启动任务");
                //完成轮胎订单轮胎数量超过发券上限
                var dt = DalCoupon.GetAllEndLunTaiOrderList();
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), Num = x["Num"].ToString() }).ToList();
                Logger.Info($"完成轮胎订单轮胎数量超过发券上限{dic?.Count}");

                foreach (var i in dic)
                {
                    CreatePromotionByOrder(StringToGuid(i.UserID), i.Num);
                }

                Thread.Sleep(5000);

                //完成轮胎订单的轮胎数量未达到发券要求的上限
                var orderList = DalCoupon.GetHasEndLunTaiOrderList();
                if (orderList != null && orderList.Any())
                {
                    //已完成订单
                    var endOrderList = orderList.Where(x => (x.Status == "3Installed" && x.InstallStatus == "2Installed" && x.InstallShopId > 0 && x.InstallType == "1ShopInstall") || (x.Status == "2Shipped" && x.DeliveryStatus == "3.5Signed" && x.InstallShopId == 0)).ToList();
                    if (endOrderList != null && endOrderList.Any())
                    {
                        //按用户分组
                        var groupOrderList = endOrderList.GroupBy(x => x.UserId);
                        //按用户相加轮胎数量
                        var selectOrderList = groupOrderList.Select(x => new
                        {
                            UserID = x.First().UserId,
                            Num = x.Sum(s => s.Num)
                        });

                        foreach (var order in selectOrderList)
                        {
                            //当前用户未完成轮胎订单
                            var noEndOrderList = orderList.Where(x => x.UserId == order.UserID && (!(x.Status == "3Installed" && x.InstallStatus == "2Installed" && x.InstallShopId > 0 && x.InstallType == "1ShopInstall") && !(x.Status == "2Shipped" && x.DeliveryStatus == "3.5Signed" && x.InstallShopId == 0))).ToList();
                            if (noEndOrderList != null && noEndOrderList.Any())
                            {
                                continue;
                            }
                            CreatePromotionByOrder(order.UserID,order.Num.ToString());
                        }
                    }

                }

                Logger.Info("结束任务");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static void CreatePromotionByOrder(Guid userId, string orderNum)
        {
            if (userId == Guid.Empty)
            {
                return;
            }
            int num = StringToInt(orderNum);
            num = num > 4 ? 4 : num;
            if (num > 0)
            {
                int discount = num * 25;
                int result = CreateOrYzPromotion(userId, 23, discount);
                if (result < 1)
                {
                    Logger.Error(new Exception("双11轮胎返保养券活动发券失败"));
                }
            }
        }

        public static int CreateOrYzPromotion(Guid userId, int ruleId, int discount)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "双11买轮胎送保养券";
                promotionCodeModel.Discount = discount;
                promotionCodeModel.MinMoney = discount;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(30).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = userId;
                promotionCodeModel.CodeChannel = LunTaiBaoYangFanQuanName;
                promotionCodeModel.BatchId = null;
                promotionCodeModel.RuleId = ruleId;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreateMultiplePromotionCode(promotionCodeModel);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static Guid StringToGuid(string userId)
        {
            try
            {
                return  new Guid(userId);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        public static int StringToInt(string num)
        {
            try
            {
                return int.Parse(num);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }


}
