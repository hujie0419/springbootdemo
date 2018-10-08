using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.C.Job.Activity.Job
{
    public class RefreshActivityPageSortedPidsHashCacheJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<RefreshActivityPageSortedPidsHashCacheJob>();
        public void Execute(IJobExecutionContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Info($"开始定时刷新活动页适配车型的产品缓存");
            try
            {
                var activityIds = DalActivity.GetSystemActivityIds();
                var result = true;
                if (activityIds != null && activityIds.Any())
                {
                    using (var client = new Service.Activity.ActivityClient())
                    {
                        foreach (var activityId in activityIds)
                        {
                            var result1 = client.GetOrSetActivityPageSortedPids(new SortedPidsRequest()
                            {
                                IsRefresh = true,
                                DicActivityId = new KeyValuePair<string, ActivityIdType>(activityId, ActivityIdType.AutoActivity)
                            });
                            result1.ThrowIfException(true);
                            if (result1.ErrorCode == "-1")
                            {
                                result = false;
                            }
                        }

                    }

                }
                if (result == false)
                {
                    Logger.Error($"刷新活动页适配车型的产品缓存执行失败");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"开始定时刷新活动页适配车型的产品缓存执行失败", e.InnerException);
            }
            sw.Stop();
            Logger.Info($"定时刷新活动页适配车型的产品缓存耗时{sw.ElapsedMilliseconds}");
        }
    }
}