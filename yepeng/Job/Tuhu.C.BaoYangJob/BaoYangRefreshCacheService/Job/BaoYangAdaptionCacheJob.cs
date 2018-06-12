using BaoYangRefreshCacheService.BLL;
using Common.Logging;
using Quartz;
using System;
using BaoYangRefreshCacheService.Common;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 定时刷新保养适配缓存
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaoYangAdaptionCacheJob : BaseJob
    {
        public BaoYangAdaptionCacheJob()
        {
            logger = LogManager.GetLogger(typeof(BaoYangAdaptionCacheJob));
        }
        
        protected override string JobName
        {
            get { return typeof (BaoYangAdaptionCacheJob).ToString(); }
        }

        public override void Exec()
        {
            BaoYangBLL bll = new BaoYangBLL(logger);
            bll.RefreshAdaptionByTid();
            bll.RefreshAdaptionByVehicle();
        }
    }
}
