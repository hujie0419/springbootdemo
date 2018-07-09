using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.PromotionTask.Dal;

namespace Tuhu.C.Job.PromotionTask.Job
{
    /// <summary>
    /// 关闭触发任务Job
    /// </summary>
    [DisallowConcurrentExecution]
    class PromotionTaskCloseJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PromotionTaskJob>();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int closeCNT = PromotionDalHelper.ClosePromotionTaskJob();
                Logger.Info($"PromotionTaskJob:关闭了 {closeCNT} 个超期任务");
            }
            catch (Exception ex)
            {
                Logger.Info($"PromotionTaskJob:异常 {ex.ToString()} ");
            }
        }
    }
}
