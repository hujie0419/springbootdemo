using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quartz;

namespace Tuhu.C.YunYing.WinService.PushJobSchedulerService.PushMessage.Job
{
    /// <summary>
    /// 友盟推送任务状态查询
    /// </summary>
    [DisallowConcurrentExecution]
    public class QueryPushTaskStatusJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<QueryPushTaskStatusJob>();

        public void Execute(IJobExecutionContext context)
        {

            Logger.Info("QueryPushTaskStatusJob启动");

            try
            {
                
            }
            catch (Exception ex)
            {
                Logger.Error("QueryPushTaskStatusJob", ex);
            }

            Logger.Info("QueryPushTaskStatusJob结束");
        }
    }
}
