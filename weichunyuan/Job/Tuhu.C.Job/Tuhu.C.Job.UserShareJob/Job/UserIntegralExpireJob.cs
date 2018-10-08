
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.C.Job.UserShareJob.Dal;
using Tuhu.C.Job.UserShareJob.Model;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;

namespace Tuhu.C.Job.UserShareJob.Job
{
    [DisallowConcurrentExecution]
    public class UserIntegralExpireJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UserIntegralExpireJob>();
        private static readonly Guid IntegralRule = new Guid("48430A22-CCA8-4101-BE30-13F4F47A5BA7");


        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("用户积分过期Job开始执行");
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var isTest = dataMap.GetBoolean("IsTest");
                DoJob(isTest);
            }
            catch (Exception ex)
            {
                Logger.Warn("用户积分过期Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"用户积分过期Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private static void DoJob(bool isTest)
        {
            var count = DalExpiringIntegral.GetExpiringIntegralUserCount();
            if (count < 1)
            {
                Logger.Warn("待过期用户数量为0;");
                return;
            }

            var start = 1500*95;
            const int step = 1500;
            Logger.Info($"需处理{count}位用户数据,共{count / step + 1}批");
            var watcher = new Stopwatch();
            while (start <= count)
            {
                watcher.Start();
                var data = DalExpiringIntegral.GetUserExpiringPointInfo(start, step);
                start += step;

                if (!data.Any()) continue;
                var data2 = DalExpiringIntegral.GetUserConsumeIntegral(data.Select(g => g.UserId).ToList());
                var totalData = data.Union(data2).GroupBy(g => g.UserId).Select(g =>
                    new UserExpiringPointModel
                    {
                        UserId = g.Key,
                        Integral = g.Sum(t => t.Integral),
                        IntegralId =
                            g.Where(t => t.IntegralId != Guid.Empty).Select(t => t.IntegralId).FirstOrDefault()
                    }).Where(g => g.Integral > 0 && g.IntegralId != Guid.Empty).ToList();

                Logger.Info($"积分过期Job, 第{start / step}批数据，需过期{totalData.Count}条数据");

                if (!totalData.Any()) continue;
                var client = new UserIntegralClient();
                try
                {
                    //DalExpiringIntegral.ExpireUserPoint(totalData, IntegralRule);
                    foreach (var item in totalData)
                    {
                        var result = client.UserIntegralChangeByUserID(item.UserId, new UserIntegralDetailModel
                        {
                            IntegralID = item.IntegralId,
                            IntegralRuleID = IntegralRule,
                            TransactionIntegral = item.Integral,
                            TransactionRemark = $"{DateTime.Now.Year - 2}年积分过期",
                            TransactionDescribe = $"{DateTime.Now.Year - 2}年积分过期",
                            TransactionChannel = "CJob",
                            Versions = $"{DateTime.Now.Year - 2}",
                            IntegralType = 1
                        }, null, 0);
                        if (!result.Success || result.Result == null)
                        {
                            Logger.Warn(
                                $"UserIntegralExpireJob==>fail==>{JsonConvert.SerializeObject(item)}==>{result.ErrorMessage}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Warn("插入数据出现异常", ex);
                }
                finally
                {
                    client.Dispose();
                }

                watcher.Stop();
                Logger.Info($"积分过期Job,第{start / step}批完成,共{count / step + 1}批,用时{watcher.ElapsedMilliseconds}毫秒");
                watcher.Reset();
                if (isTest)
                {
                    Logger.Info($"isTest==>{JsonConvert.SerializeObject(totalData.Take(10))}");
                    break;
                }
            }
        }
    }
}
