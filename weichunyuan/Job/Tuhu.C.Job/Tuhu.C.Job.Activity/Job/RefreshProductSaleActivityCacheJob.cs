using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.Activity.ServiceInterface;
using Tuhu.Service.Product;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class RefreshProductSaleActivityCacheJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RefreshProductSaleActivityCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"开始刷新所有活动缓存");
            var sw = Stopwatch.StartNew();
            sw.Start();
            RefreshAllPids();
            Logger.Info($"按pid刷新所有活动缓存耗时{sw.ElapsedMilliseconds}");

        }

        public void RefreshAllPids()
        {
            try
            {
                Logger.Info($"开始刷新");
                var datas = DalFlashSale.SelectAllSaleActivity() ?? new List<Models.FlashSaleProductModel>();
                Logger.Info($"总数:{datas.Count}");
                if (!datas.Any())
                {
                    return;
                }
                var pidCacheHour = 12;// pid缓存小时数
                var prefixCacheHour = 12;//前缀缓存小时数
                var index = 0;
                var productInterface = new ProductInterface(Logger);
                var prefix = (DateTime.Now - DateTime.MinValue).TotalMilliseconds.ToString();
                var errorPids = new List<string>();
                datas.Select(r => r.Pid).Split(100).Select(p => p.ToList())
                    .ForEach(pids =>
                    {
                        errorPids.AddRange(
                            AsyncHelper.RunSync(
                                () => productInterface.RefreshProductSaleActivityCacheByPidsWithPrefixAsync(
                                    pids, prefix, pidCacheHour)));
                        Logger.Info($"第{++index}批刷新完成");
                        Thread.Sleep(50);
                    });
                Logger.Info($"刷新完成,失败数:{errorPids.Count}");
                var result = AsyncHelper.RunSync(
                                () => productInterface.ResetPrefixForProductSaleActivityCacheByPidsCacheAsync(
                                    prefix, prefixCacheHour));
                Logger.Info($"重置缓存-->result:{result}");
            }
            catch (Exception e)
            {
                Logger.Error($"刷新异常", e);
            }
        }
    }
}
