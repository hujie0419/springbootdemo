using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    internal class BaoYangFreeQuan0630Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<BaoYangFreeQuan0630Job>();

        public void Execute(IJobExecutionContext context)
        {
            #region 返券条件

            //            返券条件：
            //使用了(优惠券规则编号：5081 / 优惠券规则Guid：9c594813 - a486 - 4880 - 91a9 - 94ac0dfc4e4d)的优惠券的订单

            //          * 本需求无需考虑U门店渠道的订单，直接排除这部分订单即可

            //           订单状态：已安装

            //           发券时间：7.5凌晨

            //           要求：给符合要求的用户塞券，同时发送短信通知，短信不要夜里发送

            //           短信内容：感谢您参与途虎618保养买一送一活动，活动返券已发至您的账户，可下载手机APP查看 dwz.cn / Beul5

            //发券要求：

            //名称：途虎全网保养抵用券

            //说明：途虎618活动返券。仅限用于实物产品。保养套餐 / 年卡不可用

            //优惠券RuleID：332

            //有效期：2017.7.5 - 2017.10.31

            //面值：订单内机油部分最高返还300元；整单最高返还500元；安装费--服务费、运费不计入返券金额

            //最低使用金额：与面值相同

            #endregion 返券条件

            if (DateTime.Now < new DateTime(2017, 07, 05) || DateTime.Now > new DateTime(2017, 07, 06))
            {
                Logger.Info($"BaoYangFreeQuan0630Job保养返券七月五日凌晨执行一次");
                return;
            }

            Logger.Info($"BaoYangFreeQuan0630Job保养返券开始");
            try
            {
                Guid id = Guid.NewGuid();
                var dt = DalCoupon.Get0630BaoYangFreeOrderList();
                Logger.Info(id + "查出订单数量为" + dt.Rows.Count);
                var dic = dt.Rows.OfType<DataRow>().Select(x => new { UserID = x["UserID"].ToString(), SumMoney = x["SumMoney"].ToString(), UserTel = x["UserTel"].ToString(), OrderID = x["OrderID"].ToString(), ShippingMoney = x["ShippingMoney"].ToString() }).Distinct().ToList();
                Logger.Info(id + "执行订单数量为" + dic.Count);
                using (var orderClient = new OrderQueryClient())
                {
                    dic.ForEach(f =>
                    {
                        var orderIds = orderClient.GetRelatedSplitOrderIDsAsync(Convert.ToInt32(f.OrderID), SplitQueryType.Full);
                        if (orderIds != null && orderIds.Result != null && orderIds.Result.Result != null)
                        {
                            var orderList = DalCoupon.SelectOrderListByOrderIdFor0630(orderIds.Result.Result);
                            if (orderList != null && orderList.Any())
                            {
                                int money = GetBackMonty(orderList, Convert.ToDecimal(f.ShippingMoney));
                                if (money > 0)
                                {
                                    Logger.Info(id + $"塞券订单{f.OrderID}-关联订单总金额{money}");
                                    int result = CreateOrYzPromotion(f.UserID, money, f.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(f.UserID));
                                        BLL.Business.SendMarketingSms(userTel, "途虎618保养买一送一");
                                    }
                                }
                                else
                                {
                                    Logger.Error($"OrderID:{f.OrderID}计算订单返券金额异常,数据为{money}");
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
            }
            catch (Exception ex)
            {
                Logger.Info($"BaoYangFreeQuan0630Job保养返券异常：{ex}");
            }
            Logger.Info($"BaoYangFreeQuan0630Job保养返券结束");
        }

        private int GetBackMonty(IEnumerable<OrderItem> orderList, decimal shippingMoney)
        {
            //面值：订单内机油部分最高返还300元；整单最高返还500元；安装费、运费不计入返券金额
            decimal other = 0, oil = 0;
            if (orderList == null || !orderList.Any())
                return 0;
            orderList.ForEach(f =>
            {
                if (f.Pid.StartsWith("OL-"))
                {
                    oil += f.TotalPrice;
                }
                else
                {
                    other += f.TotalPrice;
                }
            });
            var result = (other + (oil > 300 ? 300 : oil) - shippingMoney) > 500 ? 500 : (other + (oil > 300 ? 300 : oil) - shippingMoney);
            return Convert.ToInt32(result);
        }

        private static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "途虎618活动返券。仅限用于实物产品。保养套餐 / 年卡不可用";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = new DateTime(2017, 10, 31, 23, 59, 59);
                promotionCodeModel.StartTime = new DateTime(2017, 07, 05);
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "20170705机油买一送一券5081";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 332;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFreeQuan0630Job");
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