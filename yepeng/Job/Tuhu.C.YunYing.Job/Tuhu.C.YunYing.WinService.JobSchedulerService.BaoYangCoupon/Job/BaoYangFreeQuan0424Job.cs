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
    internal class BaoYangFreeQuan0424Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(BaoYangFreeQuan0424Job));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 4, 25) || DateTime.Now > new DateTime(2017, 7, 6))
                {
                    //                    4.25内开始跑即可，任务需持续至6.15，延长至7.5

                    //返券条件：
                    //①使用了(优惠券规则编号：3524 / 优惠券规则Guid：3ed65eed - 3064 - 42f8 - bee4 - cd666fd2864c)的优惠券的订单

                    //②销售渠道：u门店，订单内包含胜牌 / 海湾 / 出光任意品牌机油的保养订单 下单时间：4.19 - 5.12

                    //满足以上任一条件即需返券

                    //订单状态：已安装
                    //发券时间：订单安装完成后5天
                    //要求：每天跑一次，给符合要求的用户塞券（每个用户只返一次，返券金额以第一个符合返券条件的已安装完成订单为准），同时发送短信通知，短信不要夜里发送
                    //短信内容：感谢您参与途虎指定品牌保养，机油买一送一活动，活动返券已发至您的账户，可下载手机APP查看 dwz.cn / Beul5

                    //发券要求：
                    //名称：机油抵用券
                    //说明：机油买一送一活动返券，胜牌 / 海湾 / 出光到店保养专用
                    //优惠券RuleID：2006
                    //有效期：2017.5.1 - 2017.10.31
                    //面值：与原订单内所有机油产品实付金额相同
                    //最低使用金额：与面值相同
                    Logger.Info("2017-4-25~2017-7-5发放机油买一送一券");
                    return;
                }
                Guid id = Guid.NewGuid();
                Logger.Info("启动任务机油买一送一券" + id);
                var dt = DalCoupon.Get0424BaoYangFreeOrderList();

                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), OrderID = x["OrderID"].ToString(), InstallDatetime = Convert.ToDateTime(x["InstallDatetime"]) }).OrderBy(O => O.InstallDatetime).Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                using (var orderClient = new OrderQueryClient())
                {
                    dic.GroupBy(g => g.UserID).ForEach(f =>
                    {
                        var temp = f.FirstOrDefault();
                        if (!DalCoupon.GetHasGetPromotionCodeFor0424(new Guid(temp.UserID)))
                        {
                            var orderIds = orderClient.GetRelatedSplitOrderIDsAsync(Convert.ToInt32(temp.OrderID), SplitQueryType.Full);
                            if (orderIds != null && orderIds.Result != null && orderIds.Result.Result != null)
                            {
                                var orderList = DalCoupon.SelectOrderListByOrderIdFor0424(orderIds.Result.Result);
                                if (orderList != null && orderList.Any())
                                {
                                    int money = Convert.ToInt32(orderList.Sum(s => s.TotalPrice));
                                    Logger.Info(id + $"塞券订单{temp.OrderID}-关联订单总金额{money}");
                                    int result = CreateOrYzPromotion(temp.UserID, money, temp.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(temp.UserID));
                                        BLL.Business.SendMarketingSms(userTel, "途虎指定品牌保养，机油买一送一");
                                    }
                                }
                                else
                                {
                                    Logger.Error("OrderID:" + temp.OrderID + "查询订单列表数据出错,数据为空");
                                }
                            }
                            else
                            {
                                Logger.Error("OrderID:" + temp.OrderID + "查询拆单订单集合出错,数据为空");
                            }
                        }
                        else
                        {
                            Logger.Info($"OrderID:{temp.OrderID}不返券；用户{temp.UserID}已经获取过");
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
                promotionCodeModel.Description = "机油买一送一活动返券，胜牌 / 海湾 / 出光到店保养专用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = new DateTime(2017, 10, 31, 23, 59, 59);
                promotionCodeModel.StartTime = new DateTime(2017, 05, 01);
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "20170425机油买一送一券3524";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 2006;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0424Job");
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