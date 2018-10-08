using System;
using System.Diagnostics;
using Common.Logging;
using Quartz;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;


namespace Tuhu.C.Job.Activity.Job
{
    /// <summary>
    ///     公众号送红包 - 次日数据预生成JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class OARedEnvelopeDailyDataInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<OARedEnvelopeDailyDataInitJob>();

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("公众号送红包 - 次日数据预生成JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"公众号送红包 - 次日数据预生成JOB 结束执行,用时{stopwatch.Elapsed.Seconds}秒");
        }

        private void Run()
        {
            try
            {
                using (var client = new OARedEnvelopeClient())
                {

                    client.OARedEnvelopeDailyDataInit(new OARedEnvelopeDailyDataInitRequest()
                    {
                        // 生成下一天数据
                        Date = DateTime.Now.AddDays(1)
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error($"OARedEnvelopeDailyDataInitJob -> Run -> error ", e.InnerException ?? e);
            }
        }


    }
}
