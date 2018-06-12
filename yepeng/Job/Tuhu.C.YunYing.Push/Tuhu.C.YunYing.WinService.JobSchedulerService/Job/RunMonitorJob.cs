using Common.Logging;
using Quartz;
using System;
using Tuhu.Service.Utility;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.Job
{

    [DisallowConcurrentExecution]
    public class RunMonitorJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RunMonitorJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始运行监控脚本文件");
            try
            {
                using (var client = new MonitorClient())
                {
                    var result = client.KeepJobAlive("WinService_YunYingPush");
                    result.ThrowIfException(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"WinService_YunYingPush/{ex.Message}");
            }

            Logger.Info("脚本运行完成");
        }
    }
    
}
