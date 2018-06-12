using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.PromotionTask.Job
{
    /// <summary> 刘阳阳 </summary>
    /// <summary>
    /// 执行塞券任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class PromotionTaskJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PromotionTaskJob>();
        private static readonly Common common = new Common(Logger);
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
                Logger.Info($"PromotionTaskJob开始:{curList.Count}个任务");
                for (var i = 0; i < curList.Count; i++)
                {
                    var oneTask = curList[i];
                    Thread.Sleep(1000);
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 开始执行", oneTask.PromotionTaskId, oneTask.TaskName);
                    try
                    {
                        Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} ExecutBefore 结束", oneTask.PromotionTaskId, oneTask.TaskName);

                        TuhuNotification.SendNotification("ExecutePromotionTask.promotiontaskjob", new
                        {
                            oneTask.PromotionTaskId
                        }, 10000);

                        Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} SendNotification 结束", oneTask.PromotionTaskId, oneTask.TaskName);
                        //common.Executed(oneTask);
                        //Logger.WarnFormat(@"执行塞券任务=》id:{0} name={1} Executed 结束", oneTask.PromotionTaskId, oneTask.TaskName);
                    }
                    catch (Exception outEx1)
                    {
                        Logger.ErrorFormat(@"执行塞券任务=》id:{0} name={1} 出现异常 ", outEx1, oneTask.PromotionTaskId, oneTask.TaskName);
                        continue;
                    }
                    Logger.InfoFormat(@"执行塞券任务=》id:{0} name={1} 执行完毕", oneTask.PromotionTaskId, oneTask.TaskName);
                }
                Logger.Info($"PromotionTaskJob结束");
            }
            catch (Exception ex)
            {
                Logger.Error($"PromotionTaskJob异常：{ex.ToString()}");
            }

        }

        
    }
}
