using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;

namespace Tuhu.C.Job.Activity.ServiceInterface
{
    public class ProductInterface
    {
        protected static ILog Logger;
        public ProductInterface(ILog _logger)
        {
            Logger = _logger;
        }

        public async Task<List<string>> RefreshProductSaleActivityCacheByPidsWithPrefixAsync(
            List<string> pids, string prefix, int cacheSpanHours)
        {
            var loggerPrefix = $"RefreshProductSaleActivityCacheByPidsWithPrefixAsync-->{prefix}";
            Logger.Info($"{loggerPrefix}-->start");
            using (var client = new CacheClient())
            {
                var result = await client.RefreshProductSaleActivityCacheByPidsWithPrefixAsync(pids, prefix.ToString(), cacheSpanHours);
                if (result.Success)
                {
                    if (result.Result != null && result.Result.Any())
                        Logger.Warn($"{loggerPrefix}-->Warn-->" +
                            $"{string.Join(";", result.Result)}");
                }
                else
                {
                    Logger.Error($"{loggerPrefix}-->error-->" +
                        $"ErrorCode:{result.ErrorCode}-->" +
                        $"ErrorMessage:{result.ErrorMessage}", result.Exception);
                }

                Logger.Info($"{loggerPrefix}-->end");
                return result?.Result ?? new List<string>();
            }
        }

        public async Task<string> ResetPrefixForProductSaleActivityCacheByPidsCacheAsync(string prefix, int cacheSpanHours)
        {
            var loggerPrefix = $"ResetPrefixForProductSaleActivityCacheByPidsCacheAsync-->{prefix}";
            Logger.Info($"{loggerPrefix}-->start");
            using (var client = new CacheClient())
            {
                var result =await client.ResetPrefixForProductSaleActivityCacheByPidsCacheAsync(prefix, cacheSpanHours);
                if (!result.Success)
                    Logger.Error($"{loggerPrefix}-->error-->" +
                        $"ErrorCode:{result.ErrorCode}-->{result.ErrorMessage}", result.Exception);
                else
                    Logger.Info($"{loggerPrefix}-->success-->" +
                        $"{result.Result}");

                Logger.Info($"{loggerPrefix}-->end");
                return result?.Result ?? "";
            }
        }
    }
}
