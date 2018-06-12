using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class PerMinuteJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(PerMinuteJob));

        public void Execute(IJobExecutionContext context)
        {
            var curDateTime = DateTime.Now;

            if (curDateTime.Hour == 8 && curDateTime.Minute == 20) //8点20分
            {
                Logger.Info($"notification.productmatch.modify.RebuildActivity=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildActivity" });
            }
             if (curDateTime.Hour == 16 && curDateTime.Minute == 30) //16点30分
            {
                Logger.Info($"notification.productmatch.modify.RebuildActivity=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildActivity" });
            }
            if (curDateTime.Hour >= 4 && curDateTime.Hour <= 23 && curDateTime.Minute == 13) //4点到23点 每隔1小时触发一次
            {
                Logger.Info($"notification.productmatch.modify.RebuildPerHourEsCache=>{curDateTime}");
                TuhuNotification.SendNotification("notification.productmatch.modify", new { type = "RebuildPerHourEsCache" });
            }
            if (curDateTime.Hour == 2 && curDateTime.Minute == 16) //2点16分
            {
                Logger.Info($"ProductNodeCntRefreshJob刷新类目下的产品数量=>{curDateTime}");
                new ProductNodeCntRefreshJob().Execute(null);
            }
            if (curDateTime.Hour == 7 && curDateTime.Minute == 16) //7点16分
            {
                Logger.Info($"notification.ProductModify.ProductCacheModify.ProductLimit=>{curDateTime}");
                TuhuNotification.SendNotification("notification.ProductModify.ProductCacheModify", new { type = "ProductLimit" });
            }
            if (curDateTime.Minute % 10 == 0) //每十分钟刷新一次
            {
                #region 刷新所有缓存
                Logger.Info($"开始刷新所有活动缓存");
                var sw = new Stopwatch();
                sw.Start();
                try
                {
                    using (var client = new CacheClient())
                    {
                        var result = client.RefreshProductSaleActivityCache();
                        if (!result.Success)
                            Logger.Error("刷新所有活动失败RefreshProductSaleActivityCache");
                    }

                    sw.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error("调用刷新所有活动失败RefreshProductSaleActivityCache" + e.InnerException + e.Message);
                }

                Logger.Info($"刷新所有活动缓存耗时{sw.ElapsedMilliseconds}");
                #endregion
                #region 按pid刷新缓存
                sw.Restart();
                Logger.Info($"开始按pid刷新所有活动缓存");
                try
                {
                    var datas = ProductCacheDal.SelectAllSaleActivity();
                    var pids = datas.Select(r => r.PID).ToList();
                    using (var client = new CacheClient())
                    {
                        var result = client.RefreshProductSaleActivityCacheByPids(pids);
                        if (!result.Success)
                            Logger.Error("按pid刷新所有活动失败RefreshProductSaleActivityCache");
                    }

                    sw.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error("调用按pid刷新所有活动失败RefreshProductSaleActivityCache" + e.InnerException + e.Message);
                }

                Logger.Info($"按pid刷新所有活动缓存耗时{sw.ElapsedMilliseconds}");
                #endregion

            }

        }
    }

}
