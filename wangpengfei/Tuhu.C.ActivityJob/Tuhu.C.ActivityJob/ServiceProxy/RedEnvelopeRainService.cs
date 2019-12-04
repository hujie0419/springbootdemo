using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.LotteryDrawActivity;
using Tuhu.Service.LotteryDrawActivity.Response;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class RedEnvelopeRainService
    {
        private static readonly ILog _logger = LogManager.GetLogger<RedEnvelopeRainService>();

        /// <summary>
        /// 红包雨缓存预热
        /// </summary>
        /// <param name="hashKey"></param>
        /// <returns></returns>
        public async static Task<bool> PreHeatRedisCacheAsync(string hashKey)
        {
            try
            {
                using (var client = new RedEnvelopeRainClient())
                {
                    var preheatResult = await client.PreHeatRedisCacheAsync(hashKey);

                    return preheatResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"PreHeatRedisCacheAsync 异常：{ex.Message + ex.StackTrace}", ex);
                return false;
            }
        }

        /// <summary>
        /// 查询红包雨活动配置
        /// </summary>
        /// <returns></returns>
        public async static Task<List<GetRedEnvelopeRainActivityListResponse>> GetRedEnvelopeRainActivityListAsync()
        {
            try
            {
                using (var client = new RedEnvelopeRainClient())
                {
                    var preheatResult = await client.GetRedEnvelopeRainActivityListAsync(new Service.LotteryDrawActivity.Request.GetRedEnvelopeRainActivityListRequest());

                    return preheatResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"PreHeatRedisCacheAsync 异常：{ex.Message + ex.StackTrace}", ex);
                return null;
            }
        }

    }
}
