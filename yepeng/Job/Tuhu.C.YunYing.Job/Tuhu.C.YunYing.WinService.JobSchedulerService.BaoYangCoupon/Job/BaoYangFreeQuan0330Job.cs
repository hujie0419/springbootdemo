using Common.Logging;
using Quartz;
using System;
using System.Linq;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    internal class BaoYangFreeQuan0330Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0330Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 3, 31))
                {
                    //返券查找条件：
                    //下单时间：3月30日—4月12日
                    //平台：自有渠道
                    //订单状态：已完成
                    //返券规则：①单个订单购买1条任意轮胎就送1张100元保养现金券。
                    //          ②单个订单购买2条任意轮胎就送2张100元保养现金券。
                    //          ③单个订单购买3条任意轮胎就送3张100元保养现金券。
                    //          ④单个订单购买4条及以上任意轮胎就送4张100元保养现金券。
                    //          ⑤每个用户最多可得4张100元保养现金券。
                    //要求：每天跑一次，给符合要求的用户塞券，同时发送短信通知，短信不要夜里发送
                    //短信内容：感谢您参与途虎养车焕新去踏青活动，活动返券已发至您的账户，可下载手机APP查看http://dwz.cn/Beul5。

                    //发券要求：
                    //名称：焕新去踏青活动返券
                    //说明：焕新活动 海湾&胜牌 & 出光到店保养专用（不适用安装费 / 套餐 / 年卡）
                    //优惠券RuleID：2006
                    //有效期：自领取后6个月
                    Logger.Info("2017-3-31开始发放焕新去踏青活动返券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务焕新去踏青活动返券" + id);
                var tupleList = DalCoupon.Get0330TireOrders();
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
                    var hasCount = DalCoupon.GetHasGetPromotionCodeCountFor0330(item.Item1);
                    if (hasCount < 4)
                    {
                        int canGetCount = 4 - hasCount;
                        int result = CreateOrYzPromotion(item.Item1.ToString(), 100, item.Item3.ToString(), item.Item2 <= canGetCount ? item.Item2 : canGetCount);
                        if (result > 0)
                        {
                            string userTel = DalCoupon.GetUserPhoneByUserId(item.Item1);
                            BLL.Business.SendMarketingSms(userTel, "途虎养车焕新去踏青");
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
                promotionCodeModel.Description = "焕新活动 海湾&胜牌&出光到店保养专用（不适用安装费/套餐/年卡)";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = DateTime.Now.AddDays(180).Date;
                promotionCodeModel.StartTime = DateTime.Now.Date;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "焕新去踏青活动返券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 2006;
                promotionCodeModel.Number = num;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0330Job");
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