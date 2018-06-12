using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Nosql;

namespace Tuhu.WebSite.Web.Activity
{
    /// <summary>
    /// 缓存管理类
    /// </summary>
    public static partial class CacheManager
    {
        /// <summary>
        /// TODO: 
        /// </summary>
        private const string CACHE_CLIENT = "FaXianCacheClient";

        /// <summary>
        /// 获取缓存异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cacheEnum"></param>
        /// <param name="func"></param>
        /// <param name="time">只有cacheEnum传Customize才有效</param>
        /// <returns></returns>
        public static async Task<T> GetFromCacheAsync<T>(string key, Func<Task<T>> func = null, CacheTimeEnum cacheEnum = CacheTimeEnum.Customize, TimeSpan? time = null)
        {
            return func == null ? await GetAsync<T>(key) : await GetOrSetAsync(key, func, GetCacheTime(cacheEnum, time));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="cacheEnum"></param>
        /// <param name="func"></param>
        /// <param name="time">只有cacheEnum传Customize才有效</param>
        /// <returns></returns>
        public static T GetFromCache<T>(string key, Func<T> func = null, CacheTimeEnum cacheEnum = CacheTimeEnum.Customize, TimeSpan? time = null)
        {
            return func == null ? Get<T>(key) : GetOrSet(key, func, GetCacheTime(cacheEnum, time));
        }

        /// <summary>
        /// 设置缓存异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheEnum"></param>
        /// <param name="time">只有cacheEnum传Customize才有效</param>
        /// <returns></returns>
        public static async Task<bool> SetCacheValueAsync<T>(string key, T value, CacheTimeEnum cacheEnum, TimeSpan? time = null)
        {
            return await SetAsync(key, value, GetCacheTime(cacheEnum, time));
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheEnum"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool SetCacheValue<T>(string key, T value, CacheTimeEnum cacheEnum, TimeSpan? time = null)
        {
            return Set(key, value, GetCacheTime(cacheEnum, time));
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveFromCache(string key)
        {
            return Remove(key);
        }

        /// <summary>
        /// 清除缓存异步方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<bool> RemoveFromCacheAsync(string key)
        {
            return await RemoveAsync(key);
        }

        public static async Task<IResult> RemoveFromCacheResultAsync(string key)
        {
            return await RemoveResultAsync(key);
        }


        /// <summary>
        /// 根据枚举获取缓存时间
        /// </summary>
        /// <param name="cacheEnum"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static TimeSpan GetCacheTime(CacheTimeEnum cacheEnum, TimeSpan? time = null)
        {
            switch (cacheEnum)
            {
                case CacheTimeEnum.Second:
                    return TimeSpan.FromSeconds(1);
                case CacheTimeEnum.Rapid:
                    return TimeSpan.FromSeconds(5);
                case CacheTimeEnum.Short:
                    return TimeSpan.FromMinutes(10);
                case CacheTimeEnum.Quarter:
                    return TimeSpan.FromMinutes(15);
                case CacheTimeEnum.HalfHour:
                    return TimeSpan.FromMinutes(30);
                case CacheTimeEnum.Hour:
                    return TimeSpan.FromHours(1);
                case CacheTimeEnum.Day:
                    return TimeSpan.FromDays(1);
                case CacheTimeEnum.HalfWeek:
                    return TimeSpan.FromDays(3);
                case CacheTimeEnum.Week:
                    return TimeSpan.FromDays(7);
                case CacheTimeEnum.Month:
                    return TimeSpan.FromDays(30);
                case CacheTimeEnum.Customize:
                default:
                    return time ?? TimeSpan.FromMinutes(10);
            }
        }

        #region Base Methods

        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = await client.GetOrSetAsync(key, func, time);
                return result.GetValue();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static T GetOrSet<T>(string key, Func<T> func, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = client.GetOrSet(key, func, time);
                return result.GetValue();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private static T Get<T>(string key)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = client.Get<T>(key);
                return result.GetValue();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private static async Task<T> GetAsync<T>(string key)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = await client.GetAsync<T>(key);
                return result.GetValue();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static bool Set<T>(string key, Func<T> func, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = client.Set(key, func, time);
                return result.GetSucess();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static bool Set<T>(string key, T value, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = client.Set(key, value, time);
                return result.GetSucess();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static async Task<bool> SetAsync<T>(string key, Func<T> func, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = await client.SetAsync(key, func, time);
                return result.GetSucess();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static async Task<bool> SetAsync<T>(string key, T value, TimeSpan time)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = await client.SetAsync(key, value, time);
                return result.GetSucess();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool Remove(string key)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = client.Remove(key);
                return result.GetSucess();
            }
        }
        /// <summary>
        /// TODO: 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static async Task<bool> RemoveAsync(string key)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                var result = await client.RemoveAsync(key);
                return result.GetSucess();
            }
        }

        private static async Task<IResult> RemoveResultAsync(string key)
        {
            using (var client = CacheHelper.CreateCacheClient(CACHE_CLIENT))
            {
                return await client.RemoveAsync(key);
            }
        }

        /// <summary>
        /// redis result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IResult<T> result)
        {
            return result == null ? default(T) : result.Value;
        }

        /// <summary>
        /// redis result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetSucess<T>(this IResult<Func<T>> result)
        {
            return result == null ? false : result.Success;
        }

        /// <summary>
        /// redis result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetSucess(this IResult result)
        {
            return result == null ? false : result.Success;
        }

        #endregion
    }
}
