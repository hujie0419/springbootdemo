using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class RunMonitorJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RunMonitorJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始运行监控脚本文件");
            try
            {
                using(var client=new MonitorClient())
                {
                    var result = client.KeepJobAlive("WinService_CJob");
                    result.ThrowIfException(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"WinService_JobSchedulerService{ex.Message}");
            }

            Logger.Info("脚本运行完成");
        }
    }
}
