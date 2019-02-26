using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 产品缓存服务
    /// </summary>
    public class ProductCacheService
    {
        private readonly ILog _logger;

        public ProductCacheService()
        {
            _logger = LogManager.GetLogger<ProductCacheService>();
        }

        /// <summary>
        /// 刷新赠品缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshGiftCache()
        {
            try
            {
                using (var client = new CacheClient())
                {
                    var cacheResult = client.RefreshGiftCache();
                    cacheResult.ThrowIfException(true);
                    return cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("刷新赠品缓存失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 刷新产品标签缓存
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public bool SetProductCommonTagDetailsCache(ProductCommonTag tag, List<string> pids)
        {
            try
            {
                using (var client = new CacheClient())
                {
                    var cacheResult = client.SetProductCommonTagDetailsCache(tag, pids);
                    cacheResult.ThrowIfException(true);
                    return cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{tag} 刷新标签失败 -> {string.Join(",", pids)}", ex);
                return false;
            }
        }
    }
}
