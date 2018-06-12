using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>买轮胎返保养券 huhengxing 2016-12-8 </summary>
    [DisallowConcurrentExecution]
    public class LunTaiBaoYangFanQuan1211Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LunTaiBaoYangFanQuan1211Job));

        public const string LunTaiBaoYangFanQuanName1211 = "买轮胎送保养活动返券1211";
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < Convert.ToDateTime("2016-12-11 10:00:00"))
                {
                    Logger.Info("2016-12-11 10:00开始发放买轮胎送保养活动返券");
                    return;
                }

                Logger.Info("启动任务");
                //完成轮胎订单轮胎数量超过发券上限4
                var dt = DalCoupon.Get1211AllEndLunTaiOrderList();
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), Num = x["Num"].ToString() }).ToList();
                Logger.Info($"完成轮胎订单轮胎数量超过发券上限{dic?.Count}");

                foreach (var i in dic)
                {
                    CreatePromotionByOrder(StringToGuid(i.UserID), i.Num);
                }

                Thread.Sleep(5000);

                //完成轮胎订单的轮胎数量未达到发券要求的上限
                var orderList = DalCoupon.GetHas1211EndLunTaiOrderList();
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
                            CreatePromotionByOrder(order.UserID, order.Num.ToString());
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
                int result = CreateOrYzPromotion(userId, 1073, num);
                if (result < 1)
                {
                    Logger.Error(new Exception("买轮胎送保养活动返券发券失败"));
                }
            }
        }

        public static int CreateOrYzPromotion(Guid userId, int ruleId, int num)
        {
            try
            {
                PromotionCodeModel promotionCodeModel1 = new PromotionCodeModel();
                PromotionCodeModel promotionCodeModel2 = new PromotionCodeModel();
                switch (num)
                {
                    case 1: promotionCodeModel1 = GetPromotionCodeModel("买轮胎送保养活动返券", 50, userId, ruleId);
                        break;
                    case 2:
                        promotionCodeModel1 = GetPromotionCodeModel("买轮胎送保养活动返券", 100, userId, ruleId);
                        break;
                    case 3:
                        promotionCodeModel1 = GetPromotionCodeModel("买轮胎送保养活动返券", 50, userId, ruleId);
                        promotionCodeModel2 = GetPromotionCodeModel("买轮胎送保养活动返券", 100, userId, ruleId);
                        break;
                    case 4:
                        promotionCodeModel1 = GetPromotionCodeModel("买轮胎送保养活动返券", 100, userId, ruleId);
                        promotionCodeModel2 = GetPromotionCodeModel("买轮胎送保养活动返券", 100, userId, ruleId);
                        break;
                    default:
                        break;
                }

                var result = DalCoupon.Create1211MultiplePromotionCode(promotionCodeModel1, promotionCodeModel2,num);
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
                return new Guid(userId);
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

        public static PromotionCodeModel GetPromotionCodeModel(string description,int discount,Guid userId,int ruleId)
        {
            var promotionCodeModel = new PromotionCodeModel();
            promotionCodeModel.OrderId = 0;
            promotionCodeModel.Status = 0;
            promotionCodeModel.Description = "买轮胎送保养活动返券";
            promotionCodeModel.Discount = discount;
            promotionCodeModel.MinMoney = discount;
            promotionCodeModel.EndTime = DateTime.Now.AddDays(90).Date;
            promotionCodeModel.StartTime = DateTime.Now.Date;
            promotionCodeModel.UserId = userId;
            promotionCodeModel.CodeChannel = LunTaiBaoYangFanQuanName1211;
            promotionCodeModel.BatchId = null;
            promotionCodeModel.RuleId = ruleId;
            promotionCodeModel.Number = 1;
            return promotionCodeModel;
        }
    }
}
