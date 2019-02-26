using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.PinTuan;
using Tuhu.Service.PinTuan.Models.Response.PinTuan;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 拼团服务
    /// </summary>
    public class PinTuanService
    {
        private readonly ILog _logger;

        public PinTuanService()
        {
            _logger = LogManager.GetLogger<PinTuanService>();
        }

        /// <summary>
        /// 根据产品组Id获取轮胎商品集合
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, PinTuanTireProductResponse>>
            GetGroupBuyingTireProductSetById(string productGroupId)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .GetPinTuanTireProductSetByIdAsync(productGroupId);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取轮胎商品集合失败 {productGroupId}", ex);
                return null;
            }
        }

        /// <summary>
        /// 移除拼团轮胎商品缓存
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <returns></returns>
        public async Task<bool> RemoveGroupBuyingTireProductsCache(string productGroupId)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .RemovePinTuanTireProductsCacheAsync(productGroupId);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"移除拼团轮胎商品缓存失败 {productGroupId}", ex);
                return false;
            }
        }

        /// <summary>
        /// 获取拼团商品数量
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <returns></returns>
        public async Task<int> GetGroupBuyingProductCount(string productGroupId)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .GetPinTuanProductCountAsync(productGroupId);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取拼团商品数量失败 {productGroupId}", ex);
                throw;
            }
        }

        /// <summary>
        /// 检查拼团是否有默认商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <returns></returns>
        public async Task<bool> CheckGroupBuyingHasDefaultProduct(string productGroupId)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .CheckPinTuanHasDefaultProductAsync(productGroupId);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"检查拼团默认商品失败 {productGroupId}", ex);
                throw;
            }
        }

        /// <summary>
        /// 检查拼团是否有指定商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="pid">产品Id</param>
        /// <returns></returns>
        public async Task<bool> CheckGroupBuyingHasSpecifyProduct(string productGroupId, string pid)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .CheckPinTuanHasSpecifyProductAsync(productGroupId, pid);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"检查拼团指定商品失败 {productGroupId} -> {pid}", ex);
                throw;
            }
        }

        /// <summary>
        /// 设置拼团默认商品
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="pid">产品Id</param>
        /// <returns></returns>
        public async Task<bool> SetGroupBuyingDefaultProduct(string productGroupId, string pid)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .SetPinTuanDefaultProductAsync(productGroupId, pid);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"设置拼团默认商品失败 {productGroupId} -> {pid}", ex);
                return false;
            }
        }

        /// <summary>
        /// 删除拼团轮胎商品配置
        /// </summary>
        /// <param name="productConfigId">产品配置Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteGroupBuyingTireProductConfig(int productConfigId)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .DeletePinTuanTireProductConfigAsync(productConfigId);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"删除拼团轮胎商品配置失败 {productConfigId}", ex);
                return false;
            }
        }

        /// <summary>
        /// 批量更新拼团商品配置
        /// </summary>
        /// <param name="productGroupId">产品组Id</param>
        /// <param name="columnName">列名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> BatchUpdateGroupBuyingProductConfig(string productGroupId,
            string columnName, object value)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .BatchUpdatePinTuanProductConfigAsync(productGroupId, columnName, value);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"批量更新拼团商品配置失败 {productGroupId} {columnName} {value}", ex);
                return false;
            }
        }

        /// <summary>
        /// 通用移除拼团Redis缓存方法
        /// </summary>
        /// <param name="clientName"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public async Task<bool> RemovePinTuanRedisCache(string clientName, string cacheKey)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = await pinTuanClient
                        .RemovePinTuanRedisCacheAsync(clientName, cacheKey);

                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"移除拼团Redis缓存失败 {clientName} -> {cacheKey}", ex);
                return false;
            }
        }
    }
}
