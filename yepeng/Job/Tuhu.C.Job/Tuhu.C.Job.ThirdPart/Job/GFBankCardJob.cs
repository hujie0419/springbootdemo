using Common.Logging;
using Quartz;
using System;
using System.Diagnostics;
using Tuhu.C.Job.ThirdPart.Manager;

namespace Tuhu.C.Job.ThirdPart.Job
{
    /// <summary>
    /// 广发联名卡用户发券job
    /// </summary>
    [DisallowConcurrentExecution]
    public class GFBankCardJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                JobLogger.GFLogger.Info($"广发联名卡Job开始执行啦！");
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var jobExecutor = new GFJobExecutor();
                jobExecutor.Execute();
                stopWatch.Stop();
                JobLogger.GFLogger.Info($"广发联名卡Job执行成功结束啦,耗时：{stopWatch.ElapsedMilliseconds}Milliseconds");
            }
            catch (Exception ex)
            {
                JobLogger.GFLogger.Error("广发联名卡Job异常", ex);
            }            
        }
    }
}
