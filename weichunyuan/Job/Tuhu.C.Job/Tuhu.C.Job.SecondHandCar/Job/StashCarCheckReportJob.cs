using Common.Logging;
using Quartz;
using System;
using System.Linq;
using Tuhu.C.Job.SecondHandCar.ServiceManager;

namespace Tuhu.C.Job.SecondHandCar.Job
{
    [DisallowConcurrentExecution]
    public class StashCarCheckReportJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<StashCarCheckReportJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("StashCarCheckReportJob:二手车暂存报告过期检查开始");
            DoJob();
            Logger.Info("StashCarCheckReportJob:二手车暂存报告过期检查结束");
        }

        private void DoJob()
        {
            try
            {
                var stashReports = SecondHandCarManager.GetAllStashReport();
                if (stashReports == null || !stashReports.Any())
                {
                    Logger.Info("StashCarCheckReportJob:没有过期的暂存报告");
                    return;
                }
                Logger.Info($"StashCarCheckReportJob:过期的暂存报告一共{stashReports.Count}个");
                stashReports.ForEach(f =>
                {
                    if (!SecondHandCarManager.DeleteStashReport(f.ShopId, f.DetectOrderId))
                        Logger.Info($"StashCarCheckReportJob:删除过期暂存报告失败：shoId={f.ShopId},detectOrderId={f.DetectOrderId}");
                    Logger.Info($"StashCarCheckReportJob:删除过期暂存报告成功：shoId={f.ShopId},detectOrderId={f.DetectOrderId}");
                });
            }
            catch (Exception ex)
            {
                Logger.Warn($"StashCarCheckReportJob:异常：Error={ex}");
                return;
            }
        }
    }
}
