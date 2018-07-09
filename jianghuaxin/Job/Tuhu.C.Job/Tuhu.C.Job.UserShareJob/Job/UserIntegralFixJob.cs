using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.UserShareJob.Dal;

namespace Tuhu.C.Job.UserShareJob.Job
{
    [DisallowConcurrentExecution]
    public class UserIntegralFixJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UserIntegralFixJob>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("用户积分统计数据修补Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("用户积分统计数据修补Job出现异常", ex);
            }

            watcher.Stop();
            Logger.Info($"用户积分统计数据修补Job完成,用时{watcher.ElapsedMilliseconds}毫秒");

        }

        private void DoJob()
        {
            var dataList = DalExpiringIntegral.GetTerribleIntegralIds();
            if (dataList.Any())
            {
                var dt = DalExpiringIntegral.ConvertToDataTable(dataList);
                var deleteResult = DalExpiringIntegral.DeleteTerribleItem(dt);
                if (deleteResult)
                {
                    var insertResult = DalExpiringIntegral.InsertIntegralStatistics(dt);
                    if (insertResult > 0)
                    {
                        Logger.Info("数据修补完成");
                    }
                    else
                    {
                        Logger.Warn("数据插入失败");
                    }
                }
                else
                {
                    Logger.Warn("数据删除失败");
                }
            }
        }
    }
}
