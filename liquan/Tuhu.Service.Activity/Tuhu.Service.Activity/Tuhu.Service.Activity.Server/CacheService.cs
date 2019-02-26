using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Nosql;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    public class CacheService : ICacheService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CacheService));
        /// <summary>
        /// 清理活动站的Redis缓存
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RemoveRedisCacheKeyAsync(string cacheName, string cacheKey, string prefixKey = null)
        {
            var realCacheKey = cacheKey;
            using (var client = CacheHelper.CreateCacheClient(cacheName))
            {
                if (!string.IsNullOrEmpty(prefixKey))
                {
                    var result = GlobalConstant.GetCacheKeyPrefixWithCache(prefixKey, cacheName);
                    if (result != null)
                    {
                        var timeSpan = result?.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList().LastOrDefault();
                        if (!string.IsNullOrEmpty(timeSpan))
                        {
                            realCacheKey = GlobalConstant.ActivityDeafultCachPrefixValue
                                .Replace("{cacheKey}", cacheKey)
                                .Replace("{timespansecond}", timeSpan);
                        }
                    }
                }
                var isExists = client.Exists(realCacheKey);
                if (!string.IsNullOrEmpty(isExists.RealKey))
                {
                    var result = await client.RemoveAsync(realCacheKey);
                    return OperationResult.FromResult<bool>(result.Success);
                }
            }
            return OperationResult.FromResult(true);
        }

        public async Task<OperationResult<bool>> RefreshVipCardCacheByActivityIdAsync(string activityId)
        {
            return OperationResult.FromResult(await CacheManager.RefreshVipCardCacheByActivityIdAsync(activityId));
        }

        public async Task<OperationResult<bool>> RefreshRedisCachePrefixForCommonAsync(RefreshCachePrefixRequest request)
        {
            using (var client = CacheHelper.CreateCacheClient(request.ClientName))
            {
                var result = await client.SetAsync(request.Prefix, GlobalConstant.GenerateKeyPrefix(request.Prefix), request.Expiration);
                if (!result.Success)
                {
                    Logger.Warn($"更新Redis缓存失败,prefix==>{request.Prefix}", result.Exception);
                }
                return OperationResult.FromResult(result.Success);

            }
        }

        /// <summary>
        /// 刷新客户专享活动配置缓存
        /// </summary>
        /// <param name="activityExclusiveId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshRedisCacheCustomerSettingAsync(string activityExclusiveId)
        {
            if (string.IsNullOrWhiteSpace(activityExclusiveId))
            {
                return OperationResult.FromError<bool>(nameof(Resource.ParameterError), Resource.ParameterError);
            }
            return OperationResult.FromResult(await CacheManager.RefreshRedisCacheCustomerSetting(activityExclusiveId));
        }
    }
}
