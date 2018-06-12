using Common.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    [DisallowConcurrentExecution]
    public abstract class BaseQuartzJob : IJob
    {
        protected readonly ILog Logger;

        protected BaseQuartzJob(ILog logger) => Logger = logger;

        public virtual void Execute(IJobExecutionContext context)
        {
            Logger.Info("Job 开始执行");
            try
            {
                ExecuteAsync(context).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error("Job 执行出错", ex);
            }
            finally
            {
                Logger.Info("Job 执行结束");
            }
        }

        protected abstract Task ExecuteAsync(IJobExecutionContext context);
    }
}
