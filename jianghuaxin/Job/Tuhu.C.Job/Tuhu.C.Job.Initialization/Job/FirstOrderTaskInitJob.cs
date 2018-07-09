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
    public class FirstOrderTaskInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FirstOrderTaskInitJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("首单任务初始化Job开始执行");
            try

            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("首单任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"首单任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        public void DoJob()
        {
            var watcher = new Stopwatch();
            var taskId = new Guid("af1c225b-4601-481e-94a7-43682975511b");
            //var taskId = new Guid("C185431C-6FCB-4F46-8824-98B6DEC5934B");
            var userCount = 18778722;
            //var userCount = 10000000;
            Logger.Info($"共{userCount}条订单信息进行需要首单初始化");
            if (userCount == 0) return;
            var step = 3000;
            var start = 0;
            while (start < userCount)
            {
                watcher.Start();
                var userList = DalTask.GetFirstOrderUserList(start, step);
                var count = 0;
                var result = new Tuple<int, int>(0, 0);
                if (userList.Any())
                {
                    var data = userList.Where(g => g.UserId != null && g.UserId != Guid.Empty && (((g.InstallShopId == null || g.InstallShopId == 0) && g.DeliveryStatus == "3.5Signed") || (g.InstallShopId > 0 && g.InstallStatus == "2Installed"))).Select(g => g.UserId.Value).Distinct().ToList();
                    var dat = DalTask.CheckTaskUserId(data, taskId,Logger);
                    count = dat.Count;
                    if (count > 0)
                        result = DalTask.InitUserTaskInfo(dat, taskId, "1FirstOrder", Logger);
                }
                watcher.Stop();
                Logger.Info($"首单任务初始化，最大的PKID==>{start + step},第{start / step + 1}批数据,共{userCount / step + 1}批,有效数据{count}条,插入用户任务表共==>{result.Item1}条,插入用户任务详情==>{result.Item2}条数据,用时{watcher.ElapsedMilliseconds}毫秒");
                //Thread.Sleep(3000);
                watcher.Reset();
                start += step;
            }
            watcher.Stop();
        }
    }
}
