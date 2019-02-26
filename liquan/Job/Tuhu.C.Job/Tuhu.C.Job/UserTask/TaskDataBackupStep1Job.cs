using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.C.Job.Job;
using Tuhu.Service.Config;

namespace Tuhu.C.Job.UserTask
{
    [DisallowConcurrentExecution]
    public class TaskDataBackupStep1Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<TaskDataBackupStep1Job>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("会员任务数据备份Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("会员任务数据备份Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"会员任务数据备份Job完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }
        private void DoJob()
        {
            #region [备份已删除任务数据]

            if (!CheckSwitch())
            {
                Logger.Warn("数据备份开关关闭-->step1->备份已删除任务数据");
                return;
            }

            var st = new Stopwatch();
            st.Start();
            var backupCount = DalTaskBackup.BackupDeletedTask();
            Logger.Info($"备份已删除任务数据{backupCount}条!");


            if (!CheckSwitch())
            {
                Logger.Warn("数据备份开关关闭-->清理已删除任务数据");
                return;
            }

            var deletedCount = DalTaskBackup.DeleteData();
            Logger.Warn($"删除已备份数据{deletedCount}条!");

            if (!CheckSwitch())
            {
                Logger.Warn("数据备份开关关闭-->清理已删除任务详情数据");
                return;
            }
            var deleteCount2 = DalTaskBackup.DeleteDetailDate();
            Logger.Warn($"删除已删除任务详情记录{deleteCount2}条");

            st.Stop();
            Logger.Info($"TaskDataBackupStep1Job-->Step1完成-->用时{st.ElapsedMilliseconds}毫秒");
            #endregion


        }
        private bool CheckSwitch()
        {
            using (var client = new ConfigClient())
            {
                var result = client.GetOrSetRuntimeSwitch("TaskBackup");
                return result.Success && result.Result.Value;
            }
        }
    }
}
