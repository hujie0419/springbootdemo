using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductAdapterReresh : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductAdapterReresh));
        public void Execute(IJobExecutionContext context)
        {
            using (var cacheClient = new CacheClient())
            {
                //刷新轮胎适配redis缓存
                var refreshTireAsync = cacheClient.RefreshTireAdpterCache(null, 1);
                if (!refreshTireAsync.Success)
                {
                    Logger?.Warn($"RefreshTireAdpterCache刷新失败：{refreshTireAsync.ErrorMessage}", refreshTireAsync.Exception);
                }

            }
            //获取所有tid
            var allTids = ProductCacheDal.SelectAllTids();

            using (var cacheClient = new CacheClient())
            {
                //清除缓存
                var baoyangAdapterTire = cacheClient.RefreshBaoYangVehicleAdpterCache(new List<int>() { }, true);

                //分批次刷新保养适配redis缓存
                allTids.Split(100).ForEach(p => {
                    var resultOne = cacheClient.RefreshBaoYangVehicleAdpterCache(p.ToList(), false);
                    if (!resultOne.Success)
                    {
                        using (var cacheClient2 = new CacheClient())
                        {
                            resultOne = cacheClient2.RefreshBaoYangVehicleAdpterCache(p.ToList(), false);
                            if (!resultOne.Success)
                            {
                                Logger?.Warn($"RefreshBaoYangVehicleAdpterCache刷新失败", resultOne.Exception);
                            }
                        }
                    }
                });
            }
            //发送刷新通知
            Logger.Info($"notification.productmatch.modify.RebuildAdpter=>{DateTime.Now}");
            TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildAdpter" });

            //TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "UpdateAdpter", pids=new string[] { } });

        }
    }
}
