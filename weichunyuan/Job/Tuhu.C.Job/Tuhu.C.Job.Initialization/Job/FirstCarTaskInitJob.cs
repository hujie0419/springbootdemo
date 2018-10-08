using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Tuhu.C.Job.Initialization.Dal;
using Tuhu.C.Job.Initialization.Model;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class FirstCarTaskInitJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FirstCarTaskInitJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("首次增加车辆任务初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("首次增加车辆任务初始化Job出现异常", ex);
            }
            watcher.Stop();
            Logger.Info($"首次增加车辆任务初始化Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private void DoJob()
        {
            var count = DalTask.GetCarUserCount();
            if (count == 0)
            {
                Logger.Warn("查询添加车辆任务初始化用户数量为0");
                return;
            }
            var step = 3000;
            var start = 0;
            Logger.Info($"查询添加车辆任务初始化数据为{count}条,每批{step}条,共{count / step + 1}批");
            var watcher = new Stopwatch();
            var taskId = new Guid("4c24109b-2d80-449b-b074-52c877dacfa0");        
            //var taskId = new Guid("951BE020-E3A5-40EC-A234-EA67E71E8E90");
            var guidPara = Guid.Empty;
            var Coo = 0;
            var Coo1 = 0;
            var Coo2 = 0;
            while (start < count)
            {
                var result = new Tuple<int, int>(0, 0);
                watcher.Start();
                var data = DalTask.GetCarUserList(guidPara, step);
                var dat = data.Where(g => g.u_user_id != null && g.u_user_id != Guid.Empty && !string.IsNullOrWhiteSpace(g.u_cartype_pid_vid)).Select(g => g.u_user_id.Value).ToList();
                var dat2 = DalTask.CheckTaskUserId(dat, taskId, Logger);
                var count2 = dat2.Count;
                Coo += count2;
                Coo1 += data.Count;
                Coo2 += dat.Count;
                if (count2 > 0)
                    result = DalTask.InitUserTaskInfo(dat2, taskId, "6AddCar", Logger);
                watcher.Stop();               
                start += step;
                Logger.Info($"添加车辆任务初始化，起始GUID==>{guidPara:D},第{start / step + 1}批数据，共{count / step + 1}批,有效数据{count2}条,插入用户任务信息表{result.Item1}条,插入用户任务详情表{result.Item2}条,用时{watcher.ElapsedMilliseconds}毫秒");
                guidPara = data.Last().CarId;
                watcher.Reset();
            }
            Logger.Info($"{Coo1}==>{Coo2}==>{Coo}");
            watcher.Stop();
        }
    }
}
