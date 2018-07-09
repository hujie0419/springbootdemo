using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductCityRefreshCacheJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductCityRefreshCacheJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("开始刷新城市缓存");
                var cityids = DAL.ProductCacheDal.SelectCityIds();
                if (cityids != null && cityids.Any())
                {
                    Logger.Info($"开始刷新城市缓存 共{cityids.Count()}个.");
                    foreach (var cityid in cityids)
                    {
                        using (var client = new Tuhu.Service.Product.ProductClient())
                        {
                            Logger.Info($"开始刷新{cityid}");
                            var result = client.SelectProductsRegionStockByForceUpdate(cityid, new List<string>(), true);
                            Logger.Info($"结束刷新{cityid}.Success:{result.Success}");
                            result.ThrowIfException(true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Warn(ex);
            }
            Logger.Info("结束刷新城市缓存");
        }
    }
}
