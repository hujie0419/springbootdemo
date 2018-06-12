using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class BIAdaptationDataJob : BaseJob
    {
        public BIAdaptationDataJob()
        {
            logger = LogManager.GetLogger(typeof(BIAdaptationDataJob));
        }

        protected override string JobName => typeof(BIAdaptationDataJob).ToString();

        public override void Exec()
        {
            new BaoYangDataProvider(logger).GetAdaptationData();
        }
    }
}
