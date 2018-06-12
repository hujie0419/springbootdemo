using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using log4net.Repository.Hierarchy;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Job
{
    /// <summary>
    /// 从BI数据库读取数据 进行塞券逻辑调整到新的逻辑
    /// </summary>
    [DisallowConcurrentExecution]
    public class PromotionTaskActivityJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PromotionTaskActivityJob>();
        private static readonly Common common = new Common(Logger);
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("PromotionTaskActivityJob启动");
            try
            {
                List<PromotionTaskCls> curList = PromotionDalHelper.GetValidActivityPromotionTask();
                if (curList == null || curList.Count == 0)
                {
                    Logger.Info("PromotionTaskActivityJob结束:没有任务需要执行");
                    return;
                }
                Logger.Info($"PromotionTaskActivityJob开始:{curList.Count}个任务");
                for (var i = 0; i < curList.Count; i++)
                {
                    var oneTask = curList[i];
                    Thread.Sleep(1000);
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 开始执行", oneTask.PromotionTaskId, oneTask.TaskName);
                    try
                    {
                        Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} ExecutBefore 结束", oneTask.PromotionTaskId, oneTask.TaskName);

                        TuhuNotification.SendNotification("ExecutePromotionTask.promotiontaskactivity", new
                        {
                            oneTask.PromotionTaskId
                        }, 10000);

                        //common.RunPromotionTask(oneTask);
                        Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} SendNotification 结束", oneTask.PromotionTaskId, oneTask.TaskName);
                        //common.Executed(oneTask);
                        //Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} Executed 结束", oneTask.PromotionTaskId, oneTask.TaskName);
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(@"执行塞券任务=》id:{0} name={1}塞券失败", ex, oneTask.PromotionTaskId, oneTask.TaskName);
                    }
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 执行完毕", oneTask.PromotionTaskId, oneTask.TaskName);
                }
                Logger.Info($"PromotionTaskActivityJob结束");
            }
            catch (Exception ex)
            {
                Logger.Error($"PromotionTaskActivityJob异常：{ex.ToString()}");
            }
        }

        

       
    }


}
