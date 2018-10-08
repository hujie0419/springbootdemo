using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductTireBiScoreRefreshJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductTireBiScoreRefreshJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("开始刷新BI推荐缓存");
                var batch = 200;
                var i = 0;
                var vehicleInfors = DAL.ProductCacheDal.SelectBiTireSortSize();
                if (vehicleInfors != null && vehicleInfors.Any())
                {
                    Logger.Info($"开始刷新BI推荐缓存-->{vehicleInfors.Count()}-->总页数:{vehicleInfors.Count % batch}");
                    foreach (var oneBatch in vehicleInfors.Split(batch))
                    {
                        Thread.Sleep(500);
                        Logger.Info($"开始刷新{++i}");
                        var requestList = new List<SearchProductRequest>();
                        oneBatch.ForEach(p =>
                        {
                            var tireSize = TireSizeModel.Parse(p.TireSize);
                            if (tireSize != null)
                                requestList.Add(new SearchProductRequest()
                                {
                                    VehicleId = p.VehicleId,
                                    Parameters = new Dictionary<string, IEnumerable<string>>
                                    {
                                        ["CP_Tire_Width"] = new[] { tireSize.Width },
                                        ["CP_Tire_AspectRatio"] = new[] { tireSize.AspectRatio },
                                        ["CP_Tire_Rim"] = new[] { tireSize.Rim },
                                    }
                                });
                        });
                        using (var client = new Tuhu.Service.Product.CacheClient())
                        {
                            var result = client.RefreshBiTireSort(requestList);
                            result.ThrowIfException(true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Warn(ex);
            }
            Logger.Info("结束刷新BI推荐缓存");
        }
    }
}
