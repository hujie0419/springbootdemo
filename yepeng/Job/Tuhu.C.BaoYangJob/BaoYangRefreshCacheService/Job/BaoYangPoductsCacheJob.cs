using System;
using BaoYangRefreshCacheService.BLL;
using BaoYangRefreshCacheService.Common;
using Common.Logging;
using Quartz;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 定时更新保养产品缓存
    /// </summary>
    [DisallowConcurrentExecution]
    public class BaoYangPoductsCacheJob : BaseJob
    {
        public BaoYangPoductsCacheJob()
        {
            logger = LogManager.GetLogger(typeof(BaoYangPoductsCacheJob));
        }

        protected override string JobName
        {
            get { return typeof(BaoYangPoductsCacheJob).ToString(); }
        }

        public override void Exec()
        {
            BaoYangBLL bll = new BaoYangBLL(logger);
            bll.CacheBaoYangProducts();
        }
    }
}
