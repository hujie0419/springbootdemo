using System;
using System.Linq;
using System.Threading;
using BaoYangRefreshCacheService.Common;
using BaoYangRefreshCacheService.DAL;
using Common.Logging;
using ThBiz.Common;
using Tuhu.Service;
using Tuhu.Service.BaoYang;
using System.Collections.Generic;
using Tuhu.Service.Shop;
using Tuhu.Service.BaoYang.Enums;
using BaoYangRefreshCacheService.Model;
using Tuhu.Service.BaoYang.Models;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.BLL
{
    public class BaoYangBLL
    {
        private ILog logger;

        public BaoYangBLL(ILog logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 刷新地区支持的服务id缓存
        /// </summary>
        public void RefreshRegionSupportServiceCache()
        {
            logger.Info("刷新地区支持服务的缓存开始");
            IDictionary<string, IEnumerable<string>> serviceMap = new Dictionary<string, IEnumerable<string>>();
            using (var client = new BaoYangClient())
            {
                var configResult = client.GetBaoYangPackageDescription();
                var serviceResult = client.GetInstallServiceConfig();
                if (configResult.Success && configResult.Result != null && serviceResult.Success
                    && serviceResult.Result != null)
                {
                    var alltypes = configResult.Result.SelectMany(o => o.Items)
                        .Where(o => string.Equals(o.Group, BaoYangTypeGroup.Maintainence.ToString()))
                        .Select(o => o.BaoYangType);
                    serviceMap = serviceResult.Result.ServiceMap.Where(o => alltypes.Contains(o.Key))
                        .ToDictionary(o => o.Key, o => o.Value);
                }
            }

            List<int> allRegionIds = new List<int>();
            using (var client = new RegionClient())
            {
                var regionsResult = client.GetAllMiniRegion();
                if (regionsResult.Success && regionsResult.Result != null)
                {
                    foreach (var province in regionsResult.Result)
                    {
                        if (province != null && province.ChildRegions != null)
                        {
                            allRegionIds.Add(province.RegionId);
                            allRegionIds.AddRange(province.ChildRegions.Select(o => o.RegionId));
                        }
                    }
                }
            }

            foreach (var item in serviceMap)
            {
                if (item.Value != null && item.Value.Any())
                {
                    foreach (var regionId in allRegionIds)
                    {
                        using (var client = new Tuhu.Service.BaoYang.CacheClient())
                        {
                            var result = client.RefreshIsRegionSupportServiceCache(regionId, item.Value.ToList());
                            if (!result.Success || !result.Result)
                            {
                                logger.Info($"刷新地区支持服务的缓存失败！regionId:{regionId}");
                            }
                        }

                        Thread.Sleep(20);
                    }
                }
            }

            logger.Info("刷新地区支持服务的缓存结束");
        }

        public async Task<bool> CacheBaoYangProducts2Async()
        {
            bool result = false;
            var allPids = ProductDal.GetAllBaoYangPids();

            string keyPrefix = string.Empty;
            using (var client = new Tuhu.Service.BaoYang.CacheClient())
            {
                var beginResult = await client.BeginRefreshProductCacheAsync();
                if (beginResult.Success)
                {
                    keyPrefix = beginResult.Result;
                }
            }

            var tasks = allPids.ParallelSelect(async pid =>
            {
                bool flag = await RefreshSingleProduct(keyPrefix, pid);
                if (!flag)
                {
                    flag = await RefreshSingleProduct(keyPrefix, pid);
                }

                if (!flag)
                {
                    flag = await RefreshSingleProduct(keyPrefix, pid);
                }
                
                return flag;
            }, 60);

            var refreshAllResult = await Task.WhenAll(tasks);

            if (refreshAllResult.All(o => o))
            {
                using (var client = new Tuhu.Service.BaoYang.CacheClient())
                {
                    var finialResult = await client.FinishRefreshProductCacheAsync(keyPrefix, allPids.Count);

                    if (finialResult.Success)
                    {
                        result = finialResult.Result;
                        logger.Info("success");
                    }
                    else
                    {
                        logger.Error(finialResult.ErrorMessage, finialResult.Exception);
                    }
                }
            }
            else
            {
                logger.Info($"failed：{string.Join(",", refreshAllResult.Where(o => !o))}");
            }

            return result;
        }

        private async Task<bool> RefreshSingleProduct(string keyPrefix, string pid)
        {
            bool result = false;
            using (var client = new Tuhu.Service.BaoYang.CacheClient())
            {
                var refreshResult = await client.RefreshSingleProductAsync(keyPrefix, pid);
                if (refreshResult.Success)
                {
                    result = refreshResult.Result;
                }
                else
                {
                    logger.Error($"{refreshResult.ErrorMessage}, {keyPrefix}, {pid}", refreshResult.Exception);
                }
            }

            return result;
        }

        public void CacheBaoYangProducts()
        {
            try
            {
                using (var client = new Tuhu.Service.BaoYang.CacheClient())
                {
                    var result = client.RefreshProductCache();
                    result.ThrowIfException(true);
                    logger.Info($"刷新产品缓存耗时：{result.ElapsedMilliseconds/1000}s");
                    if (!result.Result)
                    {
                        logger.Error("CacheBaoYangProducts failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CacheBaoYangProducts failed!", ex);
            }
        }

        /// <summary>
        /// 刷新车型的适配
        /// </summary>
        public void RefreshAdaptionByVehicle()
        {
            int newCount = 0;
            int closeCount = 0;
            Tuhu.Service.BaoYang.CacheClient client = null;
            var allVehicles = VehicleDal.GetAllVehicles();
            Retry retry = new Retry(logger);
            try
            {
                if (allVehicles.Any())
                {
                    int count = 0;
                    foreach (var item in allVehicles)
                    {
                        if (item != null)
                        {
                            int startYear = CommonUtil.ConvertObjectToInt32(item.StartYear);
                            int endYear = CommonUtil.ConvertObjectToInt32(item.EndYear);
                            if (startYear > 0 && endYear > startYear)
                            {
                                for (int i = startYear; i <= endYear; i++)
                                {
                                    count++;
                                    if (count % 1000 == 0)
                                    {
                                        logger.Info("RefreshAdaptionByVehicle Count: " + count);
                                    }
                                    Func<OperationResult<bool>> func = delegate()
                                    {
                                        OperationResult<bool> result = null;
                                        try
                                        {
                                            if (client == null)
                                            {
                                                client = new Tuhu.Service.BaoYang.CacheClient();
                                                newCount++;
                                            }
                                            result = client.UpdateBaoYangAdaptation(item.VehicleId, item.PaiLiang,
                                                i.ToString());
                                            if (!result.Success)
                                            {
                                                result.ThrowIfException(true);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(
                                                "RefreshAdaptionByVehicle failed:" + item.VehicleId + item.PaiLiang +
                                                i.ToString(), ex);
                                            if (client != null)
                                            {
                                                client.Dispose();
                                                closeCount++;
                                            }

                                            client = null;
                                        }

                                        return result;
                                    };

                                    retry.RetryFunctionWithWait(func, CommonUtil.VerifyServiceResult);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("RefreshAdaptionByVehicle failed", ex);
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                    closeCount++;
                }
            }

            logger.Info("New Count:" + newCount + " Close Count:" + closeCount);
        }

        /// <summary>
        /// 刷新Tid的适配
        /// </summary>
        public void RefreshAdaptionByTid()
        {
            int newCount = 0;
            int closeCount = 0;
            Retry retry = new Retry(logger);
            Tuhu.Service.BaoYang.CacheClient client = null;
            var allTids = VehicleDal.GetAllTids();
            try
            {
                if (allTids.Any())
                {
                    int count = 0;
                    foreach (var tid in allTids.OrderBy(o => o).ToList())
                    {
                        count ++;
                        if (count % 1000 == 0)
                        {
                            logger.Info("RefreshAdaptionByTid Count: " + count);
                        }
                        if (!string.IsNullOrWhiteSpace(tid))
                        {
                            Func<OperationResult<bool>> func = delegate()
                            {
                                OperationResult<bool> result = null;
                                try
                                {
                                    if (client == null)
                                    {
                                        client = new Tuhu.Service.BaoYang.CacheClient();
                                        newCount++;
                                    }
                                    result = client.UpdateBaoYangAdaptationByTid(tid);
                                    if (!result.Success)
                                    {
                                        result.ThrowIfException(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("RefreshAdaptionByTid failed:" + tid, ex);
                                    if (client != null)
                                    {
                                        client.Dispose();
                                        closeCount++;
                                    }

                                    client = null;
                                }

                                return result;
                            };

                            retry.RetryFunctionWithWait(func, CommonUtil.VerifyServiceResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("RefreshAdaptionByTid failed", ex);
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                    closeCount++;
                }
            }

            logger.Info("New Count:" + newCount + " Close Count:" + closeCount);
        }

        #region 刷新用户购买过的机油缓存
        /// <summary>
        /// 刷新用户购买过的机油缓存
        /// </summary>
        public void RefreshOilUserOrderCache()
        {
            logger.Info("开始刷新用户购买过的机油推荐缓存");
            try
            {
                var batchSize = 1000;
                var cachePrefix = $"UserOrderOil/{DateTime.Now.Ticks}";
                var totalCount = BaoYangDal.GetOilOrderUserDailyCount();
                var totalPage = Math.Ceiling((double)totalCount / batchSize);
                for (var pageIndex = 1; pageIndex <= totalPage; pageIndex++)//分批获取购买记录,新订单覆盖旧数据
                {
                    var models = BaoYangDal.SelectOilOrderUserDaily(pageIndex, batchSize);
                    if (models != null && models.Any())
                    {
                        var resultModels = ConvertUserOrderCaches(cachePrefix, models);
                        if (resultModels != null && resultModels.Any())
                        {
                            using (var client = new Tuhu.Service.BaoYang.CacheClient())
                            {
                                var refreshResult = client.RefreshUserOrderOilCache(resultModels);
                                if (!refreshResult.Success || !refreshResult.Result)
                                {
                                    logger.Info($"刷新用户购买过的机油推荐缓存{pageIndex} 失败", refreshResult.Exception);
                                }
                                else
                                {
                                    logger.Info($"刷新用户购买过的机油推荐缓存{pageIndex}/{totalPage}");
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
                using (var client = new Tuhu.Service.BaoYang.CacheClient())
                {
                    var result = client.RefreshUserOrderOilCachePrefix(cachePrefix);//缓存刷新完毕换Key
                    if (!result.Success || !result.Result)
                    {
                        logger.Error($"刷新用户购买过的机油推荐缓存 换Key失败,key前缀{cachePrefix}", result.Exception);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("RefreshOilUserOrderCacheJob", ex);
            }
            logger.Info("刷新用户购买过的机油推荐缓存 结束");
        }

        /// <summary>
        /// 用户购买过的机油 数据库记录转换成缓存Key
        /// </summary>
        /// <param name="cachePrefix"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        private static List<BaoYangOilUserOrderModel> ConvertUserOrderCaches
            (string cachePrefix, IEnumerable<OilOrderUserDailyModel> models)
        {
            var result = null as List<BaoYangOilUserOrderModel>;
            if (models != null && models.Any())
            {
                models = models.Where(w => w.UserId != Guid.Empty && !string.IsNullOrEmpty(w.VehicleId)
                && !string.IsNullOrEmpty(w.PaiLiang) && !string.IsNullOrEmpty(w.Nian) && !string.IsNullOrEmpty(w.Pid)
                );//过滤掉错误数据
                result = models.GroupBy(g => new
                {
                    g.UserId,
                    g.VehicleId,
                    g.PaiLiang,
                    g.Nian
                }).Select(v =>
                {
                    var model = v.FirstOrDefault();
                    var order = v.Where(w => w.OrderId.Equals(v.Max(m => m.OrderId)));//取最近的一次订单记录
                    var pids = order.Select(s => s.Pid).Distinct().ToList();//相同订单可能多个机油产品
                    return new BaoYangOilUserOrderModel
                    {
                        CachePrefix = cachePrefix,
                        UserId = model.UserId,
                        VehicleId = model.VehicleId,
                        PaiLiang = model.PaiLiang,
                        Nian = model.Nian,
                        Pids = pids
                    };
                }).ToList();
            }
            return result;
        }
        #endregion
    }
}
