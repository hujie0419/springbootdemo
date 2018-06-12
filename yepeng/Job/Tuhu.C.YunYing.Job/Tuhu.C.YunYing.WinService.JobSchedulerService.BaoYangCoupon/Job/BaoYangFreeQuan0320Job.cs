using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan0320Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0320Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 3, 20))
                {
                    Logger.Info("2017-3-20开始发放轮胎节送保养返券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务轮胎节送保养返券" + id);
                var tupleList = DalCoupon.Get0320TireOrders();
                if (tupleList == null || !tupleList.Any())
                {
                    Logger.Info("不存在符合要求的订单");
                    return;
                }
                Logger.Info(id + "查出订单数量为" + tupleList.Count);
                var tupleListDistinct = tupleList.Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + tupleListDistinct.Count);
                foreach (var item in tupleListDistinct)
                {
                    Logger.Info(id + $"塞券订单{item.Item3}");
                    //查看用户已经 获得几张券
                    var hasCount = DalCoupon.GetHasGetPromotionCodeCount(item.Item1);
                    if (hasCount < 4)
                    {
                        int canGetCount = 4 - hasCount;
                        int result = CreateOrYzPromotion(item.Item1.ToString(), 100, item.Item3.ToString(), item.Item2 <= canGetCount ? item.Item2 : canGetCount);
                        if (result > 0)
                        {
                            string userTel = DalCoupon.GetUserPhoneByUserId(item.Item1);
                            BLL.Business.SendMarketingSms(userTel, "途虎轮胎节");
                        }
                    }
                    else
                    {
                        Logger.Info(id + $"塞券订单{item.Item3}用户{item.Item1}已经获取4张该劵，故不发放");
                    }
                }
                Logger.Info("结束任务" + id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public static int CreateOrYzPromotion(string userId, int money, string orderID, int num)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "仅限出光、海湾、胜牌三个品牌机油到店保养使用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "轮胎节送保养返券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 2006;
                promotionCodeModel.Number = num;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0320Job");
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