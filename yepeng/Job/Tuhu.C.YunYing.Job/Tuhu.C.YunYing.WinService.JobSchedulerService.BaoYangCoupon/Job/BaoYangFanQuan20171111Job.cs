using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangFanQuan20171111Job:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<BaoYangFanQuan20171111Job>();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now < new DateTime(2017, 11, 6) || DateTime.Now > new DateTime(2017, 12, 20))
                {
                    //返券条件：
                    //使用了(优惠券规则编号：8395 / 优惠券规则Guid：38d0b92a - 498c - 4c64 - 8173 - a0b56f8cb347)的优惠券的订单

                    //    * 无需考虑U门店渠道的订单，直接排除这部分订单即可

                    //    订单状态：已安装

                    //    任务持续时间：11.6 - 12.20

                    //要求：给完成安装后5天的用户塞券，同时发送短信通知，短信不要夜里发送

                    //    短信内容：感谢您参与途虎双11机油买一送一活动，活动返券已发至您的账户，可下载手机APP查看 dwz.cn / Beul5




                    //发券要求：

                    //名称：指定机油到店保养券

                    //    说明：途虎双11机油买一送一活动返券，仅限胜牌 / 海湾 / 出光品牌机油到店保养使用

                    //优惠券RuleID：2006

                    //有效期：2017.12.1 - 2018.4.30

                    //面值：订单内机油部分，最高返还485元；除机油外其他实物产品、安装费、运费皆不计入返券金额

                    //    最低使用金额：与面值相同
                    Logger.Info("2017-11-6到2017-11-20发放机油买一送一券");
                    return;
                }
                string id = DateTime.Now.ToString("yyyyMMddHHMMss");

                Logger.Info("启动任务机油买一送一券" + id);
                var dt = DalCoupon.Get20171111BaoYangFanQuanOrderList();

                Logger.Info($"{id}查出订单数量为" + dt.Rows.Count);

                var dic = dt.Rows.OfType<DataRow>().Where(x => x["UserTel"] != DBNull.Value && x["InstallDatetime"]!=DBNull.Value)
                    .Select(x => new
                    {
                        UserID = x["UserID"].ToString(),
                        SumMoney = x["SumMoney"].ToString(),
                        UserTel = x["UserTel"].ToString(),
                        OrderID = x["OrderID"].ToString(),
                        InstallDatetime = Convert.ToDateTime(x["InstallDatetime"])
                    }).OrderBy(O => O.InstallDatetime).Distinct().ToList();

                Logger.Info(id + "执行订单数量为" + dic.Count);

                using (var orderClient = new OrderQueryClient())
                {
                    dic.GroupBy(g => g.UserID).ForEach(f =>
                    {
                        var temp = f.FirstOrDefault();
                        if (!DalCoupon.GetHasGetPromotionCodeFor20171111(new Guid(temp.UserID)))
                        {
                            var orderIds = orderClient.GetRelatedSplitOrderIDsAsync(Convert.ToInt32(temp.OrderID), SplitQueryType.Full).GetAwaiter().GetResult();
                            if (orderIds != null && orderIds.Result != null)
                            {
                                var orderList = DalCoupon.SelectOrderListByOrderIdFor20171111(orderIds.Result);
                                if (orderList != null && orderList.Any())
                                {
                                    int money = Convert.ToInt32(orderList.Sum(s => s.TotalPrice));
                                    Logger.Info(id + $"塞券订单{temp.OrderID}-关联订单总金额{money}");
                                    if (money > 485) //最高返还485元
                                        money = 485;
                                    Logger.Info(id + $"塞券订单{temp.OrderID}-实际塞券金额{money}");
                                    int result = CreateOrYzPromotion(temp.UserID, money, temp.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(temp.UserID));
                                        BLL.Business.SendMarketingSms(userTel, "途虎双11机油买一送一");
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

                Logger.Info($"{id}结束任务");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }
        public static int CreateOrYzPromotion(string userId, int money, string orderID)
        {
            try
            {
                var promotionCodeModel = new PromotionCodeModel();
                promotionCodeModel.OrderId = 0;
                promotionCodeModel.Status = 0;
                promotionCodeModel.Description = "途虎双11机油买一送一活动返券，仅限胜牌/海湾/出光品牌机油到店保养使用";
                promotionCodeModel.PromtionName = "指定机油到店保养券";
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = new DateTime(2018, 4, 30, 23, 59, 59);
                promotionCodeModel.StartTime = new DateTime(2017, 12, 01);
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = "20171111机油买一送一券8395";
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = 2006;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFanQuan20171111Job");
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
