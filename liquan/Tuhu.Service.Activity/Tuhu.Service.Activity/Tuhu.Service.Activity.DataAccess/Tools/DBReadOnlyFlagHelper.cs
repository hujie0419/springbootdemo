using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.Service.Activity.DataAccess.Tools
{
   public class DBReadOnlyFlagHelper
    {
        private static readonly string DefaultClientName = "Activity";
        private static readonly string DBReadOnlyFlagCachePrefix = "DBReadOnlyFlag";


        /// <summary>
        /// 验证是否走只读库
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<bool> GetDBReadOnlyFlagFromCache(string tableName, List<string> keys)
        {
            //1.拼接key集合
            bool result = true;//默认读库
            if (keys?.Count > 0)
            {
                var keyList = GetPidDBReadOnlyFlagCacheKey(tableName, keys);
                using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var cacheResultList = await cacheClient.GetAsync<string>(keyList);
                    return !cacheResultList.Any(ca => ca.Value.Success);
                }
            }
            return result;
        }

        /// <summary>
        /// 设置缓存标识
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async void SetDBReadOnlyFlagCache(string tableName, List<string> keys)
        {
            //1.拼接key集合 
            if (keys?.Count > 0)
            {
                var keyList = GetPidDBReadOnlyFlagCacheKey(tableName, keys);
                using (var cacheClient = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    foreach (var item in keyList)
                    {
                        await cacheClient.SetAsync(item, 1, TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        /// <summary>
        /// 获取是否只读库缓存验证标识key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static List<string> GetPidDBReadOnlyFlagCacheKey(string tableName, List<string> pidList)
        {
            var keyList = new List<string>();
            if (pidList?.Count > 0)
            {
                foreach (var item in pidList)
                {
                    string key = $"{DBReadOnlyFlagCachePrefix}/{tableName}/{item}";
                    keyList.Add(key);
                }
            }
            return keyList;
        }

    }
}
