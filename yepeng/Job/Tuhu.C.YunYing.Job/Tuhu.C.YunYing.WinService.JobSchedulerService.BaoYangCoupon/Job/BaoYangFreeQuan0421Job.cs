using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Linq;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    /// <summary>
    /// 已过期
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaoYangFreeQuan0421Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0421Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 4, 21) || DateTime.Now > new DateTime(2017, 6, 16))
                {
                    //                    需要再帮忙安排一个自动返券任务，本周内开始跑即可，任务需持续至6.15

                    //返券条件：
                    //使用了(优惠券规则编号：3421 / 优惠券规则Guid：4f789f41 - bdf2 - 4344 - b6c0 - 84ebeabd78d9)的优惠券的订单

                    //          订单状态：已安装
                    //          发券时间：订单安装完成后5天
                    //          每天跑一次，给符合要求的用户塞券，同时发送短信通知，短信不要夜里发送
                    //          短信内容：感谢您参与出光品牌保养，机油买一送一活动，活动返券已发至您的账户，可下载手机APP查看 dwz.cn / Beul5

                    //发券要求：
                    //名称：出光机油抵用券
                    //说明：机油买一送一活动返券，出光品牌到店保养专用
                    //优惠券RuleID：11144
                    //有效期：2017.5.1 - 2017.10.31
                    //面值：与原订单内所有机油产品实付金额相同
                    //最低使用金额：与面值相同
                    Logger.Info("2017-4-21~2017-6-15发放出光机油抵用券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务机油买一送一券" + id);
                var dt = DalCoupon.Get0421BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), OrderID = x["OrderID"].ToString(), InstallDatetime = Convert.ToDateTime(x["InstallDatetime"]) }).OrderBy(O => O.InstallDatetime).Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                using (var orderClient = new OrderQueryClient())
                {
                    dic.ForEach(f =>
                    {
                        var orderIds = orderClient.GetRelatedSplitOrderIDsAsync(Convert.ToInt32(f.OrderID), SplitQueryType.Full);
                        if (orderIds != null && orderIds.Result != null && orderIds.Result.Result != null)
                        {
                            var orderList = DalCoupon.SelectOrderListByOrderIdFor0421(orderIds.Result.Result);
                            if (orderList != null && orderList.Any())
                            {
                                int money = Convert.ToInt32(orderList.Sum(s => s.TotalPrice));
                                Logger.Info(id + $"塞券订单{f.OrderID}-关联订单总金额{money}");
                                int result = CreateOrYzPromotion(f.UserID, money, f.OrderID);
                                if (result > 0)
                                {
                                    string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(f.UserID));
                                    BLL.Business.SendMarketingSms(userTel, "出光品牌保养，机油买一送一");
                                }
                            }
                            else
                            {
                                Logger.Error("OrderID:" + f.OrderID + "查询订单列表数据出错,数据为空");
                            }
                        }
                        else
                        {
                            Logger.Error("OrderID:" + f.OrderID + "查询拆单订单集合出错,数据为空");
                        }
                    });
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
                promotionCodeModel.Description = "机油买一送一活动返券，出光品牌到店保养专用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = new DateTime(2017, 10, 31, 23, 59, 59);
                promotionCodeModel.StartTime = new DateTime(2017, 05, 01);
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "出光机油抵用券";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 11144;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0421Job");
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