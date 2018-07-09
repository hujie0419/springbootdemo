using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Job.Initialization.Dal;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class FIxBindWXTaskJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FIxBindWXTaskJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("绑定微信任务异常数据补偿Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("绑定微信任务异常数据补偿Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"绑定微信任务异常数据补偿Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private static void DoJob()
        {
            const int step = 10000;
            var start = 0;
            var count = DalTask.GetWXBindUserCount();
            while (start < count)
            {
                var sw = new Stopwatch();
                sw.Start();
                var info = DalTask.GetWXBindUserList(start, step);
                start += step;
                if (info.Any())
                {
                    foreach (var item in info)
                    {
                        TuhuNotification.SendNotification("notification.TaskActionQueue", new Dictionary<string, string>
                        {
                            ["UserId"] = item.ToString("D"),
                            ["ActionName"] = "4BindWX"
                        });
                        Thread.Sleep(100);
                    }
                }
                sw.Stop();
                Logger.Info($"会员任务-->绑定微信异常数据补偿Job-->第{start / step + 1}批，用时{sw.ElapsedMilliseconds}毫秒，共{count / step + 1}批");
            }
        }
    }
}
