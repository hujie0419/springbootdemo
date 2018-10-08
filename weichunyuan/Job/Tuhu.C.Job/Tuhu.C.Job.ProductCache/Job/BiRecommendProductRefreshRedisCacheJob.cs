using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.ProductCache.DAL;
using Tuhu.Service.Product;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class BiRecommendProductRefreshRedisCacheJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(BiRecommendProductRefreshRedisCacheJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var needRefreshPids = ProductCacheDal.GetProductRecommendByPid()?.ToList();
                var referData = DateTime.Now.Subtract(TimeSpan.FromHours(1));
                if (needRefreshPids == null || !needRefreshPids.Any())
                {
                    Logger.Info("bi按照产品推荐产品刷新到Redisjob执行，并没有找到需要更新的数据");
                }
                else
                {
                    Logger.Info($"bi按照产品推荐产品刷新到Redisjob开始执行，一共{needRefreshPids.Count}个产品");
                    using (var client = new CacheClient())
                    {
                        foreach (var pids in needRefreshPids.Split(100).Select(r => r.ToList()))
                        {
                            var esresult = client.RefreshProductRecommendByPids(pids);
                            if (!esresult.Success || !esresult.Result)
                            {
                                Logger.Error($"调用bi按照产品推荐产品刷新到Redis失败", esresult.Exception.InnerException);
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                }
                sw.Stop();
                Logger.Info($"bi推荐产品刷新到redisjob执行结束，耗时{sw.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                Logger.Error("bi推荐产品刷新到redis失败", ex);
            }
        }
    }
}
