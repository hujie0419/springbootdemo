using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.Provisioning.Business.Redis
{
    public class FlashCounter
    {
        /// <summary>
        /// 获取某段时间某个key的计数，加了分布式锁
        /// </summary>
        /// <param name="clentName"></param>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        /// <param name="operate">加或減</param>
        /// <returns></returns>
        public static async Task<long> GetFlashCount(string clentName, string key, TimeSpan ts,
            OperateType operate)
        {
            long result = 0;

            using (var client = CacheHelper.CreateCounterClient(clentName, ts))
            {
                var cacheResult = await client.CountAsync(key);
                result = cacheResult?.Value ?? 0;
                switch (operate)
                {
                    case OperateType.Decrement:
                        await client.DecrementAsync(key);
                        break;
                    case OperateType.Increment:
                        await client.IncrementAsync(key);
                        break;
                    default:
                        await client.IncrementAsync(key);
                        break;
                }

            }

            return result;
        }

        public enum OperateType
        {
            Increment,
            Decrement
        }
    }
}
