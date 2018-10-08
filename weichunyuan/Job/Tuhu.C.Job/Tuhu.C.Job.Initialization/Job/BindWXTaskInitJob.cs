using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.Initialization.Dal;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class BindWXTaskInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<BindWXTaskInitJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("微信任务初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("微信任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"微信任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private static void DoJob()
        {
            var taskId1 = new Guid("6cf07554-9b2e-4e18-9c73-5e15c16a801c");//绑定微信
            var taskId2 = new Guid("6128230d-8a6d-495a-91e2-08fd476d9355");//关注公众号

            //dev
            //var taskId1=new Guid("6A19450D-A1C1-455F-B49D-06EDEF6A1827");
            //var taskId2=new Guid("5897E3CC-5B7C-49BB-AF5F-22780DDCE234");

            var start = 0;
            const int step = 1000;
            var maxPkId = 731398;
            var watcher = new Stopwatch();
            while (start <= maxPkId)
            {
                watcher.Start();
                var data = DalTask.GetWXTaskInfo(start, step);
                var result = new Tuple<int, int>(0, 0);
                if (data.Any())
                {
                    var data1 = data.Where(g => g.BindType.ToLower().Equals("weixinappopen")).Select(g => g.UserId).Distinct()
                        .ToList();
                    Logger.Info($"绑定微信{data1.Count}条有效数据");
                    if (data1.Any())
                    {
                        var dat2 = DalTask.CheckWxTaskUserId(data1, taskId1, Logger);
                        if (dat2.Any())
                            result = DalTask.InitUserTaskInfo(dat2, taskId1, "4BindWX", Logger, 2);
                        Logger.Info($"4BindWX/{result.Item1}/{result.Item2}");
                    }
                    var data2 = data.Where(g => g.BindType.ToLower().Equals("winxin_officialaccount")).Select(g => g.UserId).Distinct()
                        .ToList();
                    Logger.Info($"关注公众号{data2.Count}条有效数据");
                    if (data2.Any())
                    {
                        var dat2 = DalTask.CheckWxTaskUserId(data2, taskId2, Logger);
                        if (dat2.Any())
                            result = DalTask.InitUserTaskInfo(dat2, taskId2, "5Follow", Logger, 2);
                        Logger.Info($"5Follow/{result.Item1}/{result.Item2}");
                    }                  
                }
                start += step;
                watcher.Stop();
                Logger.Info($"微信任务初始化，最大的PKID==>{start + step},第{start / step + 1}批数据,共{maxPkId / step + 1}批,,用时{watcher.ElapsedMilliseconds}毫秒");
                watcher.Reset();
            }
            watcher.Stop();
        }
    }
}
