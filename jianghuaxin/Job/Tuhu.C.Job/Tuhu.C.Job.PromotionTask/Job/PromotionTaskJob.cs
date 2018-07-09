using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.PromotionTask.Dal;
using Tuhu.C.Job.PromotionTask.Model;

namespace Tuhu.C.Job.PromotionTask.Job
{
    /// <summary>
    /// 执行塞券任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PromotionTaskJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PromotionTaskJob>();
        public void Execute(IJobExecutionContext context)   
        {
            Logger.Info("PromotionTaskJob启动");
            try
            {
                List<PromotionTaskCls> curList = PromotionDalHelper.GetValidPromotionTask();
                if (curList == null || curList.Count == 0)
                {
                    Logger.Info("PromotionTaskJob结束:没有任务需要执行");
                    return;
                }
                curList.ForEach(oneTask =>
                {
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 开始执行", oneTask.PromotionTaskId, oneTask.TaskName);
                    PromotionDalHelper.RunPromotionTask(oneTask);
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 执行完毕",oneTask.PromotionTaskId, oneTask.TaskName);
                });
                Logger.Info($"PromotionTaskJob结束：共有{curList.Count}个任务执行");
            }
            catch (Exception ex)
            {
                Logger.Error($"PromotionTaskJob异常：{ex.ToString()}");
            }

        }
    }
}
