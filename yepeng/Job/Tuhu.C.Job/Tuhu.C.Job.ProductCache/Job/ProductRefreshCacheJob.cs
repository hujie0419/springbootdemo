using System;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using log4net;
using Quartz;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;
using System.Threading;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductRefreshCacheJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductRefreshCacheJob));

        private static int JobBatch = 1;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                const int pageSize = 800;
                decimal num = ProductCacheDal.SelectSkuProductCount();
                var pageNum = Convert.ToInt32(Math.Ceiling(num / pageSize));
                bool issuccess = true;
                string key = "";
                Logger.Info($"{JobBatch}=>刷新缓存服务开始.一共{num}个产品.一共{pageNum}批次.");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                if (DateTime.Now.Hour >= 2 && DateTime.Now.Hour <= 4)
                {
                    using (var cacheclient = new CacheClient())
                    {
                        var keyresult = cacheclient.GenerateProdutCacheKeyPrefix();
                        keyresult.ThrowIfException(true);
                        key = keyresult.Result;
                    }
                }

                var client = new CacheClient();
                for (var i = 1; i <= pageNum + 1; i++)
                {
                    Thread.Sleep(5);
                    var result = client.RefreshProductCacheByPageNumAndKeyPrefix(i, pageSize, JobBatch, key);
                    Logger.Info($"{JobBatch}=>{i}批次刷新{(result.Result ? "成功" : "失败")}.用时{result.ElapsedMilliseconds}");

                    if (!result.Success || !result.Result) //失败重新创建client
                    {
                        Thread.Sleep(1000);
                        client = new CacheClient();
                        var result2 = client.RefreshProductCacheByPageNumAndKeyPrefix(i, pageSize, JobBatch, key);
                        Logger.Info($"{JobBatch}=>{i}批次重试刷新{(result2.Result ? "成功" : "失败")}.用时{result2.ElapsedMilliseconds}");

                        if (!result2.Success) //失败重新创建client
                            client = new CacheClient();
                    }
                    issuccess = issuccess && result.Result;
                }

                using (var cacheclient = new CacheClient())
                {
                    var setresult = cacheclient.SetProdutCacheKeyPrefix(key);
                    if (!setresult.Success && setresult.ErrorCode != "keyPrefix")
                    {
                        Logger.Warn($"刷新产品缓存,换key失败。{setresult.ErrorMessage}", setresult.Exception);
                    }
                    Logger.Info($"{JobBatch}=>刷新产品缓存.换key.{setresult.Result};key=>{key}");
                }

                watch.Stop();
                if (issuccess)
                    Logger.Info($"{JobBatch}=>刷新成功用时{watch.ElapsedMilliseconds}");
                else
                    Logger.Error($"{JobBatch}=>刷新失败用时{watch.ElapsedMilliseconds}");

            }
            catch (Exception e)
            {
                Logger.Error($"{JobBatch}=>刷新产品缓存异常", e);
            }
            JobBatch++;
        }
        //public void Execute(IJobExecutionContext context)
        //{
        //    using (var client = new CacheClient())
        //    {
        //        try
        //        {
        //            Stopwatch watcher = new Stopwatch();
        //            watcher.Start();
        //            Logger.Info("刷新缓存服务开始");
        //            var result = client.RefreshAllProductCacheAsync();

        //            if (result.Result.Success && result.Result != null)
        //            {
        //                Logger.Info("刷新成功");
        //            }
        //            else
        //            {
        //                Logger.Info("刷新失败");
        //            }
        //            watcher.Stop();
        //            Logger.Info($"刷新缓存服务结束, 耗时：{watcher.ElapsedMilliseconds} ms");
        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Error(e.Message, e);
        //        }
        //    }
        //}
    }
}
