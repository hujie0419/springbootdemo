using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Dal;

namespace Tuhu.C.Job.Activity.Job
{
    public class ActivityJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ActivityJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("自动审核活动Job开始执行");
            try
            {
                DalActivity.ReviewActivityTask();
            }
            catch (Exception ex)
            {
                Logger.Warn("自动审核活动Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"会员任务数据备份Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
    }
}
