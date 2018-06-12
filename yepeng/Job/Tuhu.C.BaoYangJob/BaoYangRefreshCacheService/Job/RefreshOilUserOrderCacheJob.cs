using BaoYangRefreshCacheService.BLL;
using Common.Logging;
using Quartz;
using System;
using BaoYangRefreshCacheService.Common;

namespace BaoYangRefreshCacheService.Job
{
    [DisallowConcurrentExecution]
    public class RefreshOilUserOrderCacheJob : BaseJob
    {
        public RefreshOilUserOrderCacheJob()
        {
            logger = LogManager.GetLogger(typeof(RefreshOilUserOrderCacheJob));
        }

        protected override string JobName
        {
            get { return typeof(RefreshOilUserOrderCacheJob).ToString(); }
        }

        public override void Exec()
        {
            BaoYangBLL bll = new BaoYangBLL(logger);
            bll.RefreshOilUserOrderCache();
        }
    }
}
