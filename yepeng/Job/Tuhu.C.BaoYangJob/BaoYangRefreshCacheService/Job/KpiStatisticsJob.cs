using BaoYangRefreshCacheService.BLL;
using Common.Logging;
using Quartz;
using System;
using BaoYangRefreshCacheService.Common;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class KpiStatisticsJob : BaseJob
    {
        public KpiStatisticsJob()
        {
            logger = LogManager.GetLogger(typeof(KpiStatisticsJob));
        }
        protected override string JobName
        {
            get { return typeof(KpiStatisticsJob).ToString(); }
        }

        public override void Exec()
        {
            var bll = new KpiBll(logger);
            bll.CreateKpiReport();
        }
    }
}
