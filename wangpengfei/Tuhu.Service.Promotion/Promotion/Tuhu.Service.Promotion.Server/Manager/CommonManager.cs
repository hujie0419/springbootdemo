using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.C.Monitor;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;

namespace Tuhu.Service.Promotion.Server.Manager
{
    /// <summary>
    /// 通用层
    /// </summary>
    public class CommonManager : ICommonManager
    {
        private readonly ILogger logger;
        private readonly ICacheHelper cacheHelper;
        private readonly IMonitorService monitorService;


        public CommonManager(
               ILogger<CommonManager> Logger,
               IMonitorService IMonitorService,
               ICacheHelper ICacheHelper
            )
        {
            logger = Logger;
            monitorService = IMonitorService;
            cacheHelper = ICacheHelper;
        }

        /// <summary>
        /// 防止并发 【true：通过】
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="delta">步长</param>
        /// <param name="expiration">过期时间</param>
        /// <returns> true=通过，false=不通过</returns>
        public async ValueTask<bool> PreventConcurrencyAsync(string key, long delta, TimeSpan expiration)
        {
            using (var client = cacheHelper.CreateCounterClient(GlobalConstant.RedisClient, expiration))
            {
                var count = await client.IncrementAsync(key, delta).ConfigureAwait(false);
                if (count.Success && count.Value > delta)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Metrics 埋点
        /// </summary>
        /// <param name="statName"></param>
        /// <param name="sKeyValuePairs"></param>
        /// <returns></returns>
        public async Task MetricsCounterAsync(string statName, List<KeyValuePair<string, string>> sKeyValuePairs)
        {
            MetricsCounterRequest metricsRequest = new MetricsCounterRequest();
            try
            {
                metricsRequest.StatName = statName;
                metricsRequest.Value = 1;
                metricsRequest.SampleRate = 1.0;
                metricsRequest.Tags = sKeyValuePairs;
                var result = await monitorService.MetricsCounterAsync(metricsRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.Error($"优惠券 查询接口调用打点接口异常;异常信息：{e.Message};堆栈：{e.StackTrace}");
            }
        }



    }
}
