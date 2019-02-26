using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class BeautyServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(BeautyServiceProxy));
        /// <summary>
        /// 刷新美容模块缓存
        /// </summary>
        /// <param name="beautyModuleId"></param>
        /// <returns></returns>
        public async Task<bool> RefreshModuleCacheAsync(int beautyModuleId)
        {
            var result = false;

            try
            {
                using (var cacheClient = new Tuhu.Service.Beauty.CacheClient())
                {
                    var serviceResult = await cacheClient.RefreshCacheAsync(new Service.Beauty.Request.RefreshCacheRequest()
                    {
                        keys = new List<string>() { beautyModuleId.ToString() },
                        Prefix = "ModuleProductIdsPrefix"
                    });
                    result = serviceResult.Result != null && serviceResult.Result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }
        /// <summary>
        /// 刷新Banner图缓存
        /// </summary>
        /// <param name="beautyModuleId"></param>
        /// <returns></returns>
        public async Task<bool> RefreshBeautyBannerCacheAsync(string channel)
        {
            var result = false;

            try
            {
                using (var cacheClient = new Tuhu.Service.Beauty.CacheClient())
                {
                    var serviceResult = await cacheClient.RefreshCacheAsync(new Service.Beauty.Request.RefreshCacheRequest()
                    {
                        keys = new List<string>() { channel },
                        Prefix = "BannerConfigByChannelPrefix"
                    });
                    result = serviceResult.Result != null && serviceResult.Result.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }
        /// <summary>
        /// 移除新版美容缓存key
        /// </summary>
        /// <param name="beautyModuleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCacheKeyAsync(string key)
        {
            var result = false;

            try
            {
                using (var cacheClient = new Tuhu.Service.Beauty.CacheClient())
                {
                    var removeKeyResult = await cacheClient.RemoveCacheKeyAsync(key);
                    result = removeKeyResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }
    }
}
