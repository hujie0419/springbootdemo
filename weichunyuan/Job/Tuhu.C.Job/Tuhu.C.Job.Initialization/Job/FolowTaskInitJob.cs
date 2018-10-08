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
    public class FolowTaskInitJob:IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FolowTaskInitJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("关注公众号任务初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("关注公众号任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"关注公众号任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var watcher = new Stopwatch();
            var taskId = new Guid("6128230d-8a6d-495a-91e2-08fd476d9355");//关注公众号TaskId
            //var taskId = new Guid("5897E3CC-5B7C-49BB-AF5F-22780DDCE234");//dev
            const int userCount = 1484709;
            Logger.Info($"共{userCount}条用户信息进行需要首单初始化");
            const int step = 3000;
            var start = 0;
            while (start < userCount)
            {
                watcher.Start();
                var userList = DalTask.GetBindWxInfoList(start, step, false);
                var count = 0;
                var result = new Tuple<int, int>(0, 0);
                if (userList.Any())
                {
                    var data = userList.Where(g =>
                            g.AuthSource.ToLower().Equals("weixin") && g.BindingStatus.ToLower().Equals("bound") &&
                            g.UserId != null &&
                            !string.IsNullOrWhiteSpace(g.UnionId))
                        .Select(g => g.UserId.Value).Distinct().ToList();
                    var dat = DalTask.CheckTaskUserId(data, taskId, Logger);
                    count = dat.Count;
                    if (count > 0)
                        result = DalTask.InitUserTaskInfo(dat, taskId, "5Follow", Logger);
                }
                watcher.Stop();
                Logger.Info(
                    $"关注公众号任务初始化，最大的PKID==>{start + step},第{start / step + 1}批数据,共{userCount / step + 1}批,有效数据{count}条,插入用户任务表共==>{result.Item1}条,插入用户任务详情==>{result.Item2}条数据,用时{watcher.ElapsedMilliseconds}毫秒");
                watcher.Reset();
                start += step;
            }
            watcher.Stop();
        }
    }
}
