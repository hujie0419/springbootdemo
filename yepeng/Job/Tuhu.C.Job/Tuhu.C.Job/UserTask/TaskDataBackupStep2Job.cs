using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.DAL;
using Tuhu.Service.Config;

namespace Tuhu.C.Job.UserTask
{
    [DisallowConcurrentExecution]
    public class TaskDataBackupStep2Job : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<TaskDataBackupStep2Job>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("会员任务数据备份Job--Step2开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("会员任务数据备份Job--Step2出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"会员任务数据备份Job--Step2完成,用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {

            var st = new Stopwatch();
            st.Start();

            #region [备份并删除会员任务数据]

            const int step = 1000;
            var startGuid = Guid.Empty;
            var index = 0;
            var sw = new Stopwatch();
            sw.Start();
            if (!CheckSwitch())
            {
                Logger.Warn($"数据备份开关关闭-->step4-->备份并删除会员任务数据-->startGuid:{startGuid};step:{step}");
                return;
            }

            var userList = DalTaskBackup.GetUserList(step, startGuid);
            //var userList = new List<Guid> { new Guid("{51cf3203-d28a-4226-b771-6734d8144980}"),new Guid("{0165d634-bd37-47bd-9cc3-3bae23db3823}"),
            //new Guid("{ceee291d-7acb-410a-b4aa-5cfa634bd749}"),new Guid("{430cd5a7-4336-460d-8037-95e2ff3af178}"),new Guid("{1c188e5b-5c9d-42ab-8f2a-4cae8bfb86e6}"),new Guid("{452f1ca9-6f83-4ed1-9b9b-18251310a409}"),new Guid("{97cd1f54-12e6-8f43-2bd1-97d80c1f14fb}") };

            //var userList = new List<Guid> { new Guid("93F1B820-9F61-4C1E-B1A9-550349FB8A23") };
            while (userList.Any())
            {
                var backupCount = DalTaskBackup.BackupUserTaskInfo(userList);
                var deletedCount = DalTaskBackup.DeleteData(userList);
                sw.Stop();
                Logger.Info(
                    $"带备份数据第{index}批,用时{sw.ElapsedMilliseconds}毫秒,备份{backupCount}条数据,删除{deletedCount}条数据,最小用户ID是{startGuid},分批步长为{step}条");
                startGuid = userList.Last();
                index++;
                sw.Reset();
                sw.Start();
                if (!CheckSwitch())
                {
                    Logger.Warn($"数据备份开关关闭-->step2-->备份并删除会员任务数据-->startGuid:{startGuid};step:{step}");
                    return;
                }

                userList = DalTaskBackup.GetUserList(step, startGuid);
                //userList = new List<Guid>();
            }

            st.Stop();
            Logger.Info($"TaskDataBackupStep2Job-->Step2完成-->用时{st.ElapsedMilliseconds}毫秒");

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
