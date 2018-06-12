using BaoYangRefreshCacheService.BLL;
using Common.Logging;
using Quartz;
using System;
using BaoYangRefreshCacheService.Common;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 刷新地区支持的服务id缓存
    /// </summary>
    [DisallowConcurrentExecution]
    public class RefreshRegionSupportServiceCacheJob : BaseJob
    {
        public RefreshRegionSupportServiceCacheJob()
        {
            logger = LogManager.GetLogger(typeof(RefreshRegionSupportServiceCacheJob));
        }
        
        protected override string JobName
        {
            get { return typeof (RefreshRegionSupportServiceCacheJob).ToString(); }
        }

        public override void Exec()
        {
            BaoYangBLL bll = new BaoYangBLL(logger);
            bll.RefreshRegionSupportServiceCache();
        }
    }
}
