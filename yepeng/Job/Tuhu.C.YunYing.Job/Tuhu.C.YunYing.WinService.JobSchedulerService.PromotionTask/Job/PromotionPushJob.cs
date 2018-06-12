using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL;
using System.Diagnostics;
using Common.Logging;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Job
{
    /// <summary>
    /// 给需要提前提醒的券 用户 到期前提醒
    /// </summary>
    [DisallowConcurrentExecution]
    public class PromotionPushJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PromotionPushJob>();

        public static List<CouponRuleList> CouponRuleList;

        private static IPromotionPushDalHelper DalHelper;
        private static string JobName = typeof(PromotionPushJob).Name;
        private static readonly int TemplateId = 1446;
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var types = new List<Type>() {typeof(PromotionPushDalHelper), typeof(TaskPromotionPushDalHelper)};

                Logger.Info($"{JobName} 开始执行");
                foreach (var type in types)
                {
                    DalHelper = (IPromotionPushDalHelper)Activator.CreateInstance(type);
                    if (DalHelper != null)
                    {
                        JobName = type.Name;
                        CouponRuleList = new List<Model.CouponRuleList>();
                        GetCouponRuleList();
                        Push();
                        System.Threading.Thread.Sleep(5000);
                    }
                }
                Logger.Info($"{JobName} 执行结束");
            }
            catch (Exception e)
            {
                Logger.Error($"{JobName} 出现异常",e);
            }
            
        }


        private static void Push()
        {
            
            Logger.Info($"{JobName} 开始获取需要推送 的优惠券");

            Parallel.ForEach(CouponRuleList, new ParallelOptions() {MaxDegreeOfParallelism = 3}, item =>
            {
                Stopwatch watcher = new Stopwatch();
                Logger.Info($"{JobName} 开始发提前{item.Days}天的提醒");
                item.Days = item.Days - 1; //因为要包含当天 所以days要减去1

                int minPkid = 0;
                int pageSize = 1000;
                var Date = DateTime.Now.AddDays(item.Days).ToString("yyyy年MM月dd日");
                while (true)
                {
                    watcher.Restart();
                    var dt = DalHelper.SelectPushPromotion(minPkid, pageSize, item.RuleIds, item.GetRuleIds, item.Days);
                    if (!dt.Any())
                    {
                        Logger.Info(
                            $"{JobName} 提前{item.Days}天 没有获取到优惠券 RuleIds={string.Join(",", item.RuleIds)} GetRuleIds ={string.Join(",", item.GetRuleIds)} MinPKID{minPkid} PageSize {pageSize}");
                        break;
                    }
                    Logger.Info($"{JobName} 提前{item.Days}天 获取到优惠券MinPKID{minPkid}");
                    minPkid = dt.Last().PKID;
                    Logger.Info($"{JobName} 提前{item.Days}天 获取到优惠券MaxPKID{minPkid}");
                    var userIds = new List<string>(dt.Count());
                    dt.ForEach(x =>
                    {
                        if (!item.PushedUserIds.ContainsKey(x.UserId) && x.UserId != Guid.Empty)
                        {
                            userIds.Add(x.UserId.ToString());
                        }
                    });
                    if (!userIds.Any()) continue;
                    var pushList = DalHelper.SelectPushPromotionByUserId(userIds, item.RuleIds, item.GetRuleIds, item.Days);
                    Logger.Info($"{JobName} 提前{item.Days}天 获取到待发送列表{pushList.Count()} 开始发送");
                    Parallel.ForEach(pushList.GroupBy(x => x.UserId), new ParallelOptions() { MaxDegreeOfParallelism = 20 },
                        x =>
                        {
                            if (item.PushedUserIds.ContainsKey(x.Key)) return; //如果已经发送过push 就不再发送了
                            var tryAdd = false;
                            int count = x.Count();
                            if (count > 1) //如果这个人有好几张优惠券需要提醒，则需要 添加到已发送列表里
                            {
                                tryAdd = item.PushedUserIds.TryAdd(x.Key, true);
                            }

                            if (tryAdd || count == 1)
                            {
                                var discountSum = x.Sum(_ => _.Discount);
                                //发送push
                                using (var client = new Service.Push.TemplatePushClient())
                                {
                                    var response = client.PushByUserIDAndBatchID(new List<string> { x.Key.ToString() }, TemplateId,
                                        new Service.Push.Models.Push.PushTemplateLog()
                                        {
                                            Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(
                                                new Dictionary<string, string>
                                                {
                                                    ["{{sumcouponamount}}"] = discountSum.ToString(),
                                                    ["{{couponendtime}}"] = Date
                                                })
                                        });
                                    if (!response.Success)
                                    {
                                        Logger.Error(
                                            $"{JobName} 提前{item.Days}天 Push Exception {response.ErrorMessage}", response.Exception);
                                    }
                                }
                            }
                        });
                    watcher.Stop();
                    Logger.Info(
                        $"{JobName} 提前{item.Days}天 待发送列表发送完毕 PushedUserIdsCount {item.PushedUserIds.Count} MaxPkid {minPkid} 执行时间{watcher.Elapsed} ");
                }
            });

            

        }

        private static void GetCouponRuleList()
        {
            var GetCouponRuleList = DalHelper.SelectIsPushList();
            List<CouponRule> list = new List<CouponRule>();
            GetCouponRuleList.Rows.OfType<DataRow>().ForEach((dr) =>
            {
                string pushSetting = dr["PushSetting"].ToString();
                if (!string.IsNullOrEmpty(pushSetting))
                {
                    var ruleId = int.Parse(dr["RuleId"].ToString());
                    int getRuleId = int.Parse(dr["GetRuleId"].ToString());
                    var arr = pushSetting.Split(',').Where(x => int.TryParse(x, out int result))
                        .Select(x => int.Parse(x)).ToList();
                    foreach (var a in arr)
                    {
                        list.Add(new CouponRule()
                        {
                            RuleId = ruleId,
                            GetRuleId = getRuleId,
                            Day=a
                        });
                    }
                }
            });

            list.GroupBy(x => x.Day).ForEach(x =>
            {
                var item = new CouponRuleList() {Days = x.Key};
                var ruleIds = new List<int>();
                var getRuleIds = new List<int>();
                foreach (var xItem in x)
                {
                    ruleIds.Add(xItem.RuleId);
                    getRuleIds.Add(xItem.GetRuleId);
                }
                item.GetRuleIds = getRuleIds;
                item.RuleIds = ruleIds;
                CouponRuleList.Add(item);
            });

            Logger.Info($"{JobName} 获取优惠券结束总共获取到Days :{CouponRuleList.Count}个");
        }
    }
}
