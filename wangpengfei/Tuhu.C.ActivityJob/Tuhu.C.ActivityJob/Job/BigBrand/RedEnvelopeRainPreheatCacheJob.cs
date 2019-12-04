using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Job.BigBrand
{
    /// <summary>
    /// 红包雨缓存预热  1分钟执行一次 
    /// </summary>
    public class RedEnvelopeRainPreheatCacheJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(BigBrandGrandPrizeEmailPushJob));
        protected static readonly string jobName = "RedEnvelopeRainPreheatCacheJob";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"{jobName}开始执行");

            AsyncHelper.RunSync(() => DoJob());

            Logger.Info($"{jobName} 执行结束");
        }

        public async Task DoJob()
        {
            try
            {
                var activityList = await RedEnvelopeRainService.GetRedEnvelopeRainActivityListAsync();
                if (activityList == null || activityList.Count == 0)
                {
                    Logger.Info($"{jobName} 当前无红包雨活动");
                }
                else
                {
                    foreach (var item in activityList)
                    {
                        Logger.Info($"{jobName} 开始预热缓存,HashKey:{item.HashKey}");
                        var result = await RedEnvelopeRainService.PreHeatRedisCacheAsync(item.HashKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{jobName}", ex);
            }
        }

    }
}
