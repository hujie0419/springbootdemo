using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.Service.Config;

namespace Tuhu.C.Job.UserTask
{
    [DisallowConcurrentExecution]
    class TaskDataBackupStep3Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<TaskDataBackupStep3Job>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("会员任务数据备份Job--step3开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("会员任务数据备份Job--step3出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"会员任务数据备份Job--step3完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {

            #region [备份并删除会员任务详情信息]
            var sw = new Stopwatch();
            sw.Start();
            var maxPkid = DalTaskBackup.GetMaxPkid();
            const int newStep = 1000;
            var newIndex = 0;
            var start = 0;
            while (start < maxPkid)
            {
                if (!CheckSwitch())
                {
                    Logger.Warn($"数据备份开关关闭-->ste3-->备份并删除会员任务详情信息-->min:{start};max:{start + newStep}");
                    return;
                }

                sw.Restart();
                var backupCount = DalTaskBackup.BackupDetailInfo(start, start + newStep);
                var deletedCount = DalTaskBackup.DeleteDetailInfo(start, start + newStep);
                sw.Stop();
                Logger.Info(
                    $"备份并删除任务详情数据，第{newIndex + 1}批,共{maxPkid / newStep + 1}批,最小PKID为{start},最大PKID为{start + newStep},备份{backupCount}条数据,删除{deletedCount}条数据,用时{sw.ElapsedMilliseconds}毫秒");
                start += newStep;
                newIndex++;
            }

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
