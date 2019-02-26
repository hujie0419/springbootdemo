

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Config;

namespace Tuhu.Service.Activity.Server.Manager
{
    public class CacheManager
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(CacheManager));
        private static string GenerateKeyPrefix(string prefix)
        {
            return DateTime.Now.Ticks + prefix;
        }
        public static async Task<bool> CommonRefreshKeyPrefixAsync(string clientName, string prefix, TimeSpan expiration)
        {
            using (var client = CacheHelper.CreateCacheClient(clientName))
            {
                var result = await client.SetAsync(prefix, GenerateKeyPrefix(prefix), expiration);
                if (!result.Success)
                {
                    Logger.Warn($"更新{prefix}缓存失败", result.Exception);
                }
                return result.Success;
            }
        }

        public static async Task<string> CommonGetKeyPrefixAsync(string clientName, string prefix)
        {
            using (var client = CacheHelper.CreateCacheClient(clientName))
            {
                var result = await client.GetAsync<string>(prefix);
                return result.Success ? result.Value : prefix;
            }
        }

        public static async Task<bool> RefreshVipCardCacheByActivityIdAsync(string activityId)
        {
            using (var client = CacheHelper.CreateCacheClient(ActivityManager.DefaultClientName))
            {
                var data = await DalActivity.GetVipCardSaleConfigDetailsAsync(activityId);
                var result = await client.SetAsync(GlobalConstant.VipCardPrefix + activityId, data
                    , TimeSpan.FromDays(10));

                if (result.Success)
                {
                    return true;
                }
                else
                {
                    Logger.Warn($"set redis缓存失败，接口==》RefreshVipCardCacheByActivityIdAsync");
                    return false;
                }
            }
        }


        /// <summary>
        /// 刷新客户专享活动配置缓存
        /// </summary>
        /// <param name="activityExclusiveId"></param>
        /// <returns></returns>
        public static async Task<bool> RefreshRedisCacheCustomerSetting(string activityExclusiveId)
        {
            using (var client = CacheHelper.CreateCacheClient(ActivityManager.DefaultClientName))
            {
                var data = await DalActivity.SelectCustomerExclusiveSettingInfo(activityExclusiveId, false);

                var result = await client.SetAsync(string.Concat("CustomerSettingInfo/", activityExclusiveId), data
                    , TimeSpan.FromDays(2));

                if (result.Success)
                {
                    return true;
                }
                else
                {
                    Logger.Warn($"set redis缓存失败，接口==》RefreshRedisCacheCustomerSettingAsync");
                    return false;
                }
            }
        }
    }
}
