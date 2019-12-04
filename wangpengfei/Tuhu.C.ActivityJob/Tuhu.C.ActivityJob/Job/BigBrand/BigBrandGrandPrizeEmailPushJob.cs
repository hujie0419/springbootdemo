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
    /// 大翻盘中大奖信息邮件推送job
    /// </summary>
    public class BigBrandGrandPrizeEmailPushJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(BigBrandGrandPrizeEmailPushJob));
        protected static readonly string jobName = "BigBrandGrandPrizeEmailPushJob";

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
                var request = new Service.LotteryDrawActivity.Request.GrandPrizeEmailPushRequest()
                {
                    StartTime = DateTime.Now.AddDays(-1).Date,
                    EndTime = DateTime.Now.Date
                };
                await BigBrandActivityAsyncService.GrandPrizeEmailPushAsync(request);
            }
            catch (Exception ex)
            {
                Logger.Error($"{jobName}", ex);
            }
        }

    }
}
