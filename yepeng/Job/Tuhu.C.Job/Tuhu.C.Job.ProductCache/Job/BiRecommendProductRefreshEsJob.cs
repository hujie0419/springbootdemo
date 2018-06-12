using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class BiRecommendProductRefreshEsJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(BiRecommendProductRefreshEsJob));
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var userDatas = ProductCacheDal.GetProductRecommendByUserId();
                var defaultDatas = ProductCacheDal.GetCommonRecommend();
                var needRefreshPids = userDatas.Union(defaultDatas).Distinct().ToList();
                var referData = DateTime.Now.Subtract(TimeSpan.FromHours(1));
                if (!needRefreshPids.Any())
                {
                    Logger.Info("bi推荐产品刷新到esjob执行，并没有找到需要更新的数据");
                }
                else
                {
                    Logger.Info($"bi推荐产品刷新到esjob开始执行，一共{needRefreshPids.Count}个产品");
                    using (var client = new CacheClient())
                    {
                        foreach (var pids in needRefreshPids.Split(1).Select(r => r.ToList()))
                        {
                            var switcher = ProductCacheDal.SelectRuntimeSwitchBySwitchName();
                            if (switcher == "run")
                            {
                                var esresult = client.RefreshProductRecommendEsCacheByPids(pids);
                                if (!esresult.Success || !esresult.Result)
                                {
                                    Logger.Error($"调用bi推荐产品刷新到es接口失败", esresult.Exception.InnerException);
                                }
                            }
                            else
                            {
                                return;
                            }

                        }
                    }
                        using (var client = new ProductSearchClient())
                        {
                            var delResult = client.DeleteOldProductEs(referData, ExpiredEsDataType.Recommend);
                            if (!delResult.Success || !delResult.Result)
                            {
                                Logger.Error($"调用bi推荐产品删除es过期数据接口失败",delResult.Exception.InnerException);
                            }
                        }
                    }
                    sw.Stop();
                    Logger.Info($"bi推荐产品刷新到esjob执行结束，耗时{sw.ElapsedMilliseconds}");               
            }
            catch (Exception ex)
            {
                Logger.Error("bi推荐产品刷新到es失败", ex);
            }
        }
    }
}
