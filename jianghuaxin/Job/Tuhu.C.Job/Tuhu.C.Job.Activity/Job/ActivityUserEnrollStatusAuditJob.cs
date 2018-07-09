using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.Service.Push.Models.Push;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.Models;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Models;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class ActivityUserEnrollStatusAuditJob: IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ActivityUserEnrollStatusAuditJob>();

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("系统自动审核上海市闵行区用户的报名状态为已通过 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"系统自动审核上海市闵行区用户的报名状态为已通过 结束执行,用时{stopwatch.Elapsed.Seconds}秒");
        }
        #region

        private void Run()
        {
            using (var acitivtyClient = new ActivityClient())
            {
                #region 系统自动审核上海市闵行区用户的报名状态为已通过
                var result=acitivtyClient.UpdateUserEnrollStatus("上海市闵行区");
                var data = result.Result;
                #endregion
            }
        }

        #endregion
    }
}
