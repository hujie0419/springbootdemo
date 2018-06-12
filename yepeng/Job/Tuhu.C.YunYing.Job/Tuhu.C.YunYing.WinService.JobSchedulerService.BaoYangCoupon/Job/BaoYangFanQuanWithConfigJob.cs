using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangFanQuanWithConfigJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<BaoYangFanQuanWithConfigJob>();
        private static BaoYangFanQuanConfig config;
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                string configStr = dataMap.GetString("Config");
                config = JsonConvert.DeserializeObject<BaoYangFanQuanConfig>(configStr);
               
                //验证参数是否有 如果少一个 ，就不给通过



                if (DateTime.Now < config.StartTime || DateTime.Now > config.EndTime)
                {
                    Logger.Info($"{config.StartTime:yyyy-MM-dd}至{config.EndTime:yyyy-MM-dd}发放机油买一送一券");
                    return;
                }
                string id = DateTime.Now.ToString("yyyyMMddHHMMss") + " " + config.Channel;

                Logger.Info($"{id}启动任务机油买一送一券");
                var dt = DalCoupon.GetBaoYangFanQuanOrderList(config.InstallDays, config.GetRuleId, config.RuleId, config.OrderStartTime, config.OrderEndTime, config.Channel);

                Logger.Info($"{id}查出订单数量为" + dt.Rows.Count);

                var dic = dt.Rows.OfType<DataRow>()
                    .Where(x => x["UserTel"] != DBNull.Value && x["InstallDatetime"] != DBNull.Value)
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
                        if (!DalCoupon.GetHasGetPromotionCode(new Guid(temp.UserID), config.Channel))
                        {
                            var orderIds = orderClient
                                .GetRelatedSplitOrderIDsAsync(Convert.ToInt32(temp.OrderID), SplitQueryType.Full)
                                .GetAwaiter().GetResult();
                            if (orderIds != null && orderIds.Result != null)
                            {
                                var orderList = DalCoupon.SelectOrderListByOrderIdForOL(orderIds.Result);
                                if (orderList != null && orderList.Any())
                                {
                                    int money = Convert.ToInt32(orderList.Sum(s => s.TotalPrice));
                                    Logger.Info(id + $"塞券订单{temp.OrderID}-关联订单总金额{money}");
                                    
                                    if (config.MaxMoney > 0 && money > config.MaxMoney) //最高返还多少元元
                                        money = config.MaxMoney;
                                    Logger.Info(id + $"塞券订单{temp.OrderID}-实际塞券金额{money}");
                                    int result = CreateOrYzPromotion(temp.UserID, money, temp.OrderID);
                                    if (result > 0)
                                    {
                                        string userTel = DalCoupon.GetUserPhoneByUserId(new Guid(temp.UserID));
                                        BLL.Business.SendMarketingSms(userTel, config.SMSDescription);
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
                promotionCodeModel.Description = config.PromotionDescription;
                promotionCodeModel.PromtionName = config.PromotionName;
                promotionCodeModel.Discount = money;
                promotionCodeModel.MinMoney = money;
                promotionCodeModel.EndTime = config.PromotionEndTime;
                promotionCodeModel.StartTime = config.PromotionStartTime;
                promotionCodeModel.UserId = new Guid(userId);
                promotionCodeModel.CodeChannel = config.Channel;
                promotionCodeModel.BatchId = Convert.ToInt32(orderID);
                promotionCodeModel.RuleId = config.RuleId;
                promotionCodeModel.Number = 1;
                var result = DalCoupon.CreatePromotionCode(promotionCodeModel, null, "BaoYangFanQuanWithConfigJob");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }
    }

    public class BaoYangFanQuanConfig
    {
        /// <summary>
        /// 这次发券的Channel
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 安装多少天之后发券
        /// </summary>
        public int InstallDays { get; set; }

        /// <summary>
        /// 订单使用了哪张券 才进行返券
        /// </summary>
        public int GetRuleId { get; set; }

        /// <summary>
        /// 要发的券的RuleId
        /// </summary>
        public int RuleId { get; set; }

        /// <summary>
        /// 开始执行日期
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束执行日期
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 下单的范围 开始时间
        /// </summary>
        public DateTime OrderStartTime { get; set; }
        /// <summary>
        /// 下单的范围 结束时间
        /// </summary>
        public DateTime OrderEndTime { get; set; }

        /// <summary>
        /// 最高返多少钱 如果配置为0 ，则认为没有规则，买多少返多少
        /// </summary>
        public int MaxMoney { get; set; }

        /// <summary>
        /// 优惠券开始时间
        /// </summary>
        public DateTime PromotionStartTime { get; set; }

        /// <summary>
        /// 优惠券结束时间
        /// </summary>
        public DateTime PromotionEndTime { get; set; }

        /// <summary>
        /// 优惠券名字
        /// </summary>
        public string PromotionName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string PromotionDescription { get; set; }

        /// <summary>
        /// 短信描述  感谢您参与{SMSDescription}活动，活动返券已发至您的账户，可下载手机APP查看
        /// </summary>
        public string SMSDescription { get; set; }
    }
}
