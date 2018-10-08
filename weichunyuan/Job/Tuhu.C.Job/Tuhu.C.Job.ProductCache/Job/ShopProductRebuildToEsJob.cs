using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ShopProductRebuildToEsJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ShopProductRebuildToEsJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"开始刷新");
            int count = 0;
            var result = ProductCacheDal.SelectCommonProductEsPids(500);
            if (result != null)
            {
                Parallel.ForEach(result, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 4,
                    TaskScheduler = TaskScheduler.Default
                }, parseitem =>
                {
                    count++;
                    Logger.Info($"开始刷新{count}批次");
                    if (parseitem != null && parseitem.Any())
                    {
                        var setitems = (from item in parseitem.AsParallel()
                                        group item by item.Value
                            into g
                                        select new
                                        {
                                            items = g.Select(x => x.Key)?.ToList(),
                                            type = g.Key
                                        })?.ToList();
                        foreach (var setitem in setitems)
                        {
                            try
                            {
                                using (var client = new Tuhu.Service.Product.CacheClient())
                                {
                                    var setresult = client.SetEsCacheByPids(setitem.items, setitem.type);
                                    setresult.ThrowIfException(true);

                                }
                            }
                            catch (System.Exception ex)
                            {
                                Logger.Warn(ex);
                            }
                        }  
                    }
                    Logger.Info($"结束刷新{count}批次");
                });
            }

            Logger.Info($"结束刷新");
        }
    }
}
