using System;
using System.Linq;
using System.Threading;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Common.Logging;

namespace Tuhu.Service.Activity.Server.Utils
{
    public class TuhuMemoryCacheNoJson
    {
        public static readonly TuhuMemoryCacheNoJson Instance = new TuhuMemoryCacheNoJson();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
        private readonly ObjectCache _cache;
        private readonly ILog _logger;

        private TuhuMemoryCacheNoJson()
        {
            _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _cache = MemoryCache.Default;
            _logger = LogManager.GetLogger("内存缓存");
        }

        private SemaphoreSlim GetLock(string key)
        {
            return _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        }

        /// <summary>
        /// 获取或设置缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="func">缓存数据源func</param>
        /// <param name="expireTime">过期时间</param>
        /// <returns></returns>
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan expireTime)
        {
            T result = default(T);
            try
            {
                var cacheData = Gets<T>(key);

                if (cacheData == null || !cacheData.Any())
                {
                    result = await SetAsync(key, func, expireTime);
                }
                else
                {
                    result = cacheData[key];
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"内存缓存出错, key:{key}", ex);
            }
            finally
            {
            }

            return result;
        }

        public async Task<T> SetAsync<T>(string key, Func<Task<T>> func, TimeSpan expireTime)
        {
            T result = default(T);
            try
            {
                CacheItemPolicy policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.Add(expireTime)
                };

                result = await func();
                _cache.Set(key, result == null ? (object)"null" : result, policy);
            }
            catch (Exception ex)
            {
                _logger.Error($"内存缓存出错, key:{key}", ex);
            }
            finally
            {
            }

            return result;
        }

        public T Get<T>(string key)
        {
            var result = Gets<T>(new string[] { key });
            result.TryGetValue(key, out T value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns>
        /// 仅返回有缓存的数据
        /// </returns>
        public Dictionary<string, T> Gets<T>(params string[] keys)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
            var checkKeys = keys?.Where(p => p != null)?.Distinct() ?? new List<string>();

            if (checkKeys.Any())
            {
                var cacheDic = _cache.GetValues(keys);
                if (cacheDic == null) cacheDic = new Dictionary<string, object>();
                result = cacheDic.ToDictionary(p => p.Key, p => {
                    if (p.Value is string)
                    {
                        if ((string)p.Value == "null")
                        {
                            return default(T);
                        }
                    }
                    return (T)p.Value;
                });
            }
            return result;
        }
    }
}
