using System;
using System.Threading.Tasks;
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
    public class BaoYangPoductsCacheJob2 : BaseJob
    {
        public BaoYangPoductsCacheJob2()
        {
            logger = LogManager.GetLogger(typeof(BaoYangPoductsCacheJob2));
        }

        protected override string JobName
        {
            get { return typeof(BaoYangPoductsCacheJob2).ToString(); }
        }

        public override void Exec()
        {
            BaoYangBLL bll = new BaoYangBLL(logger);
            var task = bll.CacheBaoYangProducts2Async();
            task.Wait();
            logger.Info($"新版刷新保养缓存结果：{task.Result}");
        }
    }
}
