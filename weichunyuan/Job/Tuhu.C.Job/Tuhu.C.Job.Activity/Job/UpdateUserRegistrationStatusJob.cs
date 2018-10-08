using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity;
using Tuhu.Service.Activity.Enum;
using Common.Logging;
using Quartz;

namespace Tuhu.C.Job.Activity.Job
{
    /// <summary>
    /// 更新用户报名状态
    /// </summary>
    [DisallowConcurrentExecution]
    public class UpdateUserRegistrationStatusJob : IJob
    {
        private readonly ILog _logger;

        public UpdateUserRegistrationStatusJob()
        {
            _logger = LogManager.GetLogger<UpdateUserRegistrationStatusJob>();
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("开始自动更新用户报名状态...");

            var watcher = new Stopwatch();
            watcher.Start();

            try
            {
                using (var client = new ActivityClient())
                {
                    client.UpdateUserRegistrationStatus(
                        "上海市闵行区",
                        RegistrationStatus.Passed);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("更新用户报名状态出现异常", ex);
            }

            watcher.Stop();

            _logger.Info($"报名状态更新完成，用时{watcher.ElapsedMilliseconds}毫秒。");
        }
    }
}
