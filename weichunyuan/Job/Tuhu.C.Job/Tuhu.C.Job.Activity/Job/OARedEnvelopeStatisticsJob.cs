using System;
using System.Diagnostics;
using Common.Logging;
using Quartz;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models.Requests;


namespace Tuhu.C.Job.Activity.Job
{
    /// <summary>
    ///     公众号送红包 - 统计数据同步JOB
    /// </summary>
    [DisallowConcurrentExecution]
    public class OARedEnvelopeStatisticsJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<OARedEnvelopeStatisticsJob>();

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("公众号送红包 - 统计数据同步JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"公众号送红包 - 统计数据同步JOB 结束执行,用时{stopwatch.Elapsed.Seconds}秒");
        }

        private void Run()
        {
            try
            {
                using (var client = new OARedEnvelopeClient())
                {

                    // 更新昨天的 和 今天的 统计信息
                    client.OARedEnvelopeStatisticsUpdate(new OARedEnvelopeStatisticsUpdateRequest()
                    {
                        OfficialAccountType = 1,
                        StatisticsDate = DateTime.Now.Date.AddDays(-1)
                    });

                    client.OARedEnvelopeStatisticsUpdate(new OARedEnvelopeStatisticsUpdateRequest()
                    {
                        OfficialAccountType = 1,
                        StatisticsDate = DateTime.Now.Date
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error($"OARedEnvelopeStatisticsJob -> Run -> error ", e.InnerException ?? e);
            }
        }


    }
}
