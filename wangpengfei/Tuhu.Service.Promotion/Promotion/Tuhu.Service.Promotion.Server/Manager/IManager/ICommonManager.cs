using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;

namespace Tuhu.Service.Promotion.Server.Manager.IManager
{
    /// <summary>
    /// 通用接口
    /// </summary>
    public interface ICommonManager
    {
        /// <summary>
        /// 防止并发 true：通过
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="delta">步长</param>
        /// <param name="expiration">过期时间</param>
        /// <returns> true=通过，false=不通过</returns>
        ValueTask<bool> PreventConcurrencyAsync(string key, long delta, TimeSpan expiration);


        /// <summary>
        /// Metrics埋点 
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="sKeyValuePairs"></param>
        /// <returns></returns>
        Task MetricsCounterAsync(string statName, List<KeyValuePair<string, string>> sKeyValuePairs);
    }
}
