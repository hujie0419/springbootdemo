using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Initialization.Dal;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class BindTaskInitJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<BindTaskInitJob>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("绑定微信任务初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("绑定微信任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"绑定微信任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private static void DoJob()
        {
            var watcher = new Stopwatch();
            var taskId = new Guid("6cf07554-9b2e-4e18-9c73-5e15c16a801c");//绑定微信TaskId
            //var taskId = new Guid("6A19450D-A1C1-455F-B49D-06EDEF6A1827");//dev
            const int userCount = 2552500;
            Logger.Info($"共{userCount}条用户信息进行绑定微信初始化");
            const int step = 3000;
            var start = 0;
            while (start < userCount)
            {
                watcher.Start();
                var userList = DalTask.GetBindWxInfoList(start, step);
                var count = 0;
                var result = new Tuple<int, int>(0, 0);
                if (userList.Any())
                {
                    var data = userList.Where(g =>
                            g.AuthSource.ToLower().Equals("weixin") && g.BindingStatus.ToLower().Equals("bound") && g.UserId != null)
                        .Select(g => g.UserId.Value).Distinct().ToList();
                    var dat = DalTask.CheckTaskUserId(data, taskId, Logger);
                    count = dat.Count;
                    if (count > 0)
                        result = DalTask.InitUserTaskInfo(dat, taskId, "4BindWX", Logger);
                }
                watcher.Stop();
                Logger.Info($"绑定微信任务初始化，最大的PKID==>{start + step},第{start / step + 1}批数据,共{userCount / step + 1}批,有效数据{count}条,插入用户任务表共==>{result.Item1}条,插入用户任务详情==>{result.Item2}条数据,用时{watcher.ElapsedMilliseconds}毫秒");
                watcher.Reset();
                start += step;
            }
            watcher.Stop();
        }

    }
}
