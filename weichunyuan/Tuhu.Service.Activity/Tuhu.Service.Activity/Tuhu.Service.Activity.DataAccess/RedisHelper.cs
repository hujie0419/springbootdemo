using System;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Nosql;

namespace Tuhu.Service.Activity.DataAccess
{
    //解决读写库同步的问题
    public static class RedisHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RedisHelper));
        public static async Task CreateZeroActivityApplyCache()
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApply"))
            {
                var result = await client.SetAsync("ZeroActivityApplication", TimeSpan.FromSeconds(10));
                if(!result.Success)
                    Logger.Warn($"设置ZeroActivity/ZeroActivityApply缓存redis失败:Key:ZeroActivityApplication;Error:{result.Message}", result.Exception);
            }
        }

        public static async Task CreateZeroActivityApplyCacheOnPeriod(int period)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApplyOnPeriod"))
            {
                var result = await client.SetAsync(period.ToString(), 1, TimeSpan.FromSeconds(10));
                if (!result.Success)
                    Logger.Warn($"设置ZeroActivity/ZeroActivityApplyOnPeriod缓存redis失败:Key:{period.ToString()};Error:{result.Message}", result.Exception);
            }
        }

        public static async Task CreateZeroActivityApplyCacheOnUserId(Guid userId)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApplyOnUserId"))
            {
                var result = await client.SetAsync(userId.ToString("B"), 1, TimeSpan.FromSeconds(10));
                if (!result.Success)
                    Logger.Warn($"设置ZeroActivity/ZeroActivityApplyOnUserId缓存redis失败:Key:{userId.ToString("B")};Error:{result.Message}", result.Exception);
            }
        }

        public static async Task<bool> GetZeroActivityApplyCache()
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApply"))
            {
                var result = await client.GetAsync("ZeroActivityApplication");
                return !string.IsNullOrEmpty(result.Value);
            }
        }
        public static async Task<bool> GetZeroActivityApplyCacheOnPeriod(int period)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApplyOnPeriod"))
            {
                var result = await client.GetAsync(period.ToString());
                return !string.IsNullOrEmpty(result.Value);
            }
        }

        public static async Task<bool> GetZeroActivityApplyCacheOnUserId(Guid userId)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityApplyOnUserId"))
            {
                var result = await client.GetAsync(userId.ToString("B"));
                return !string.IsNullOrEmpty(result.Value);
            }
        }

        public static async Task CreateZeroActivityReminderCache(Guid userId, int period)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityReminder"))
            {
                var result = await client.SetAsync(string.Join("/", userId.ToString("B"), period.ToString()), 1, TimeSpan.FromSeconds(10));
                if (!result.Success)
                    Logger.Warn($"设置ZeroActivity/ZeroActivityReminder缓存redis失败:Key:{string.Join("/", userId.ToString("B"), period.ToString())};Error:{result.Message}", result.Exception);
            }
        }

        public static async Task<bool> GetZeroActivityReminderCache(Guid userId, int period)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivity/ZeroActivityReminder"))
            {
                var result = await client.GetAsync(string.Join("/", userId.ToString("B"), period.ToString()));
                return !string.IsNullOrEmpty(result.Value);
            }
        }
        public static async Task<string> GetZeroActivityCacheKeyPrefix(string prefix)
        {
            using (var client = CacheHelper.CreateCacheClient("ZeroActivityPrefix"))
            {
                var result = await client.GetAsync<string>(prefix);
                if (result.Success)
                    return result.Value;
                else
                {
                    Logger.Warn($"获取{prefix}对应的零元购缓存key前缀失败;Error:{result.Message}", result.Exception);
                    return prefix;
                }
            }
        }
        public static string GenerateZeroActivityCacheKeyPrefix(string prefix)
        {
            return DateTime.Now.Ticks.ToString() + prefix;
        }


        /// <summary>
        /// 增加计数器次数加一次
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="keyItem"></param>
        /// <param name="func"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static async Task Increment(string keyName, string keyItem ,TimeSpan timeSpan)
        {
            using (var client = CacheHelper.CreateCounterClient(keyName,timeSpan))
            {
                var result = await client.IncrementAsync(keyItem);
                if (!result.Success)
                {
                    Logger.Warn($"增加次数失败:Key:{keyName} ItemKey:{keyItem};Error:{result.Message}", result.Exception);
                }
            }
        }


       


        

    }
}
