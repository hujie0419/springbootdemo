using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using Quartz;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class BaoYangBIDataJob : BaseJob
    {
        public BaoYangBIDataJob()
        {
            logger = LogManager.GetLogger(typeof(BaoYangBIDataJob));
        }

        protected override string JobName
        {
            get { return typeof(BaoYangBIDataJob).ToString(); }
        }

        public override void Exec()
        {
            BaoYangDataProvider provider = new BaoYangDataProvider(logger);

            provider.GetPriceData();
        }
    }
}
