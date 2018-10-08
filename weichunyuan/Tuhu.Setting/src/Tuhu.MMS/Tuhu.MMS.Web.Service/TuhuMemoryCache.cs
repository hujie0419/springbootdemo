using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tuhu.MMS.Web.Service
{
    public class TuhuMemoryCache
    {
        public static readonly TuhuMemoryCache Instance = new TuhuMemoryCache();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
        private readonly ObjectCache _cache;
         

        private TuhuMemoryCache()
        {
            _locks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _cache = MemoryCache.Default;
           
        }

        private SemaphoreSlim GetLock(string key)
        {
            return _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan expireTime)
        {
            T result = default(T);

            //var semaphore = GetLock(key);
            try
            {
                //semaphore.Wait();
                string json = (string)_cache.Get(key);

                if (json == null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.Add(expireTime)
                    };

                    var data = await func();
                    var dataStr = JsonConvert.SerializeObject(data);
                    json = string.Equals(dataStr, "null") ? string.Empty : dataStr;
                    _cache.Set(key, json, policy);
                }

                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //_logger.Error($"内存缓存出错, key:{key}", ex);
            }
            finally
            {
                //semaphore.Release();
            }

            return result;
        }

        public void Remove(string key)
        {
            try
            {
                _cache.Remove(key);
            }
            catch (System.Exception ex)
            {
                //_logger.Error($"内存缓存出错, key:{key}", ex);
            }
        }
        public T Set<T>(string key, T value, TimeSpan expireTime)
        {
            T result = default(T);

            //var semaphore = GetLock(key);
            try
            {
                //semaphore.Wait();
                CacheItemPolicy policy = new CacheItemPolicy()
                {
                    SlidingExpiration = expireTime
                };
                var dataStr = JsonConvert.SerializeObject(value);
                var json = dataStr == "null" ? string.Empty : dataStr;
                _cache.Set(key, json, policy);
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                //_logger.Error("内存缓存出错", ex);
            }
            finally
            {
                //semaphore.Release();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns>只返回缓存的结果，未缓存的key不返回</returns>
        public Dictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();

            if (keys != null)
            {
                var cacheDic = _cache.GetValues(keys);

                if (cacheDic != null)
                {
                    foreach (var pair in cacheDic)
                    {
                        T value = default(T);
                        string key = pair.Key;
                        string json = (string)pair.Value;

                        if (json != null)
                        {
                            value = JsonConvert.DeserializeObject<T>(json);
                            result.Add(key, value);
                        }
                    }
                }
            }

            return result;
        }


    }
}
