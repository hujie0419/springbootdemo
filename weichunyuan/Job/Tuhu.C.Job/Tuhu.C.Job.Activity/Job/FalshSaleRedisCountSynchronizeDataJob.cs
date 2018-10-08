using System;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.Nosql;
using Tuhu.C.Job.Activity.Dal;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class FalshSaleRedisCountSynchronizeDataJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<FalshSaleRedisCountSynchronizeDataJob>();
        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = new Stopwatch();
            Logger.Info("限时抢购销售数量同步开始");
            stopwatch.Start();
            var saleProducts = DalFlashSale.SelectFlashSaleProductModel().ToList();
            var count = 0;
            foreach (var item in saleProducts)
            {
                using (var client = CacheHelper.CreateCounterClient($"test_{DateTime.Now.Date:yyyy_MM_dd}", TimeSpan.FromDays(30)))
                {
                    var record = client.Count(item.ActivityId + item.Pid);

                    if (record.Success)
                    {
                        item.Num = (int)record.Value;
                        var result = DalFlashSale.UpdateFlashSaleProducts(item);
                        if (result <= 0)
                        {
                            Logger.Error($"限时抢购销售数量同步失败:活动id=>{item.ActivityId}产品id=>{item.Pid}数量=>{item.Num}");
                        }
                        else
                        {
                            count++;
                        }
                    }
                }
            }
            Logger.Error($"限时抢购销售数量同步完成消耗时间=>{stopwatch.ElapsedMilliseconds}共同步{count}条数据");
            stopwatch.Stop();
        }
    }
}
