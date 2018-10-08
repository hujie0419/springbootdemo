using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Tuhu.C.Job.Initialization.Dal;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class AuthenticationTaskInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<AuthenticationTaskInitJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("车型认证任务初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("车型认证任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"车型认证任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private void DoJob()
        {
            var count = DalTask.GetAuthenticationUserCount();
            if (count == 0)
            {
                Logger.Warn("查询车型认证任务初始化用户数量为0");
                return;
            }
            var step = 10000;
            var start = 0;
            Logger.Info($"查询车型认证任务初始化数据为{count}条,每批{step}条,共{count / step + 1}批");
            var watcher = new Stopwatch();
            while (start < count)
            {
                watcher.Start();
                var data = DalTask.GetAuthenticationUserList(start, step);
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        TuhuNotification.SendNotification("notification.TaskActionQueue", new Dictionary<string, string>
                        {
                            ["UserId"] = item.ToString("D"),
                            ["ActionName"] = "7Authentication"
                        });
                        Thread.Sleep(100);
                    }
                }
                watcher.Stop();
                Logger.Info($"车型认证任务初始化，第{start / step + 1}批数据，用时{watcher.ElapsedMilliseconds}毫秒");
                watcher.Reset();
                start += step;
            }
            watcher.Stop();
        }
    }
}
