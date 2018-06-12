using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    public class UShopActivityReturnGift180412Job : IJob
    {
        #region 门店id
        public static List<int> Shops = new List<int>() {
            23454,
            24275,
            11727,
            12498,
            13775,
            14817,
            17498,
            21825,
            22096,
            22308,
            22309,
            22574,
            22956,
            22958,
            22968,
            22969,
            22971,
            23194,
            23540,
            23844,
            24128,
            24265,
            24911,
            25587,
            25643,
            15020,
            15021,
            17980,
            19043,
            19136,
            19224,
            19377,
            20039,
            20510,
            20957,
            22506,
            22955,
            23111,
            23179,
            23188,
            23492,
            23553,
            23559,
            23565,
            23673,
            23677,
            23748,
            24008,
            24227,
            24243,
            24280,
            24292,
            24311,
            24325,
            24345,
            24410,
            24569,
            24875,
            25076,
            25130,
            25131,
            25274,
            25651,
            25652,
            25663,
            25810,
            25833,
            26034,
            26070,
            26176,
            26337,
            26676,
            27376,
            27544,
            26321,
            27041,
            27219,
            27354,
            27389,
            27443,
            27456,
            27718,
            28051,
            28601,
            28623
                    };
        #endregion

        #region 铺货产品id
        public static List<string> Pids = new List<string>() {
            "TR-CT-C2-MC5|17",
            "TR-CT-C2-MC5|2",
            "TR-DP-LM703|7",
            "TR-JJ-GS5|14",
            "TR-JJ-HT5|1",
            "TR-KH-HS61|4",
            "TR-PR-P-CINTURATO-P1|9"
        };
        #endregion
        string Channel = "UShopActivityReturnGift180412Job";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UShopActivityReturnGift180412Job));
        public void Execute(IJobExecutionContext context)
        {
            if (DateTime.Now.Date < new DateTime(2018, 4, 12)) { //12号之前不执行
                return;
            }
            try
            {
                Logger.Info($"开始执行");
                var dt = DAL.DalCoupon.Get180412ReturnGiftOrderList(Pids, Shops);

                var dic = dt.Rows.OfType<DataRow>().Select(x =>
                new {
                    UserID = x.GetValue<Guid>("UserID"),
                    OrderID = x.GetValue<int>("OrderId"),
                    Num=x.GetValue<int>("Num")
                }).ToList();
                Logger.Info("执行订单数量为" + dic.Count);
               
                dic.GroupBy(g => g.UserID).ForEach(f =>
                {
                    var temp = f.FirstOrDefault();
                    if (!DalCoupon.GetHasGetPromotionCode(temp.UserID, Channel))
                    {
                        var groupOrder = f.GroupBy(x => x.OrderID).Where(x => x.Sum(_ => _.Num) >= 2).FirstOrDefault();
                        if (groupOrder != null && groupOrder.Any())
                        {
                            Logger.Info($"用户{temp.UserID} {groupOrder.Key}的满足赠送条件");
                            using (var client = new PromotionClient()) {
                                client.CreatePromotionForYeWu(new Service.Member.Models.CreatePromotionModel()
                                {
                                    Author = "huangtongpeng@tuhu.cn",
                                    PromotionName = "途虎专供轮胎线下活动——购胎送礼品券",
                                    Description = "途虎专供轮胎线下活动——购胎送礼品券",
                                    BathID = groupOrder.Key,
                                    Channel = Channel,
                                    Creater = "huangtongpeng@tuhu.cn",
                                    RuleId = 262,
                                    StartTime = DateTime.Now.Date,
                                    EndTime = DateTime.Now.Date.AddDays(15),
                                    DepartmentName = "线上运营部",
                                    IntentionName = "常规促销",
                                    Discount = 129,
                                    MinMoney = 129,
                                    UserID = temp.UserID,
                                    Issuer = "huangtongpeng@tuhu.cn"
                                });
                            }
                        }
                        else {
                            Logger.Info($"用户{temp.UserID}的订单不满足赠送条件");
                        }
                    }
                    else
                    {
                        Logger.Info($"OrderID:{temp.OrderID}不返券；用户{temp.UserID}已经获取过");
                    }
                });

                Logger.Info($"执行结束");
            }
            catch (Exception e)
            {

                Logger.Warn($"执行异常",e);
            }
            
        }
    }
}
