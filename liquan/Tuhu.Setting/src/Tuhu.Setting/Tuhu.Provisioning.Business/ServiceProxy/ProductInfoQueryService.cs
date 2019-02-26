using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.ProductQuery;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 产品信息查询服务
    /// </summary>
    public class ProductInfoQueryService
    {
        private readonly ILog _logger;

        public ProductInfoQueryService()
        {
            _logger = LogManager.GetLogger<ProductInfoQueryService>();
        }

        /// <summary>
        /// 获取商品基本信息
        /// </summary>
        /// <param name="pids">商品Id</param>
        /// <returns></returns>
        public List<ProductBaseInfo> GetProductsInfoByPids(string[] pids)
        {
            try
            {
                using (var queryClient = new ProductInfoQueryClient())
                {
                    var queryResult = queryClient.SelectProductBaseInfo(pids.ToList());
                    queryResult.ThrowIfException(true);
                    return queryResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取商品信息失败 {string.Join(",", pids)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取轮胎商品信息
        /// </summary>
        /// <param name="pids">商品Id</param>
        /// <returns></returns>
        public List<ProductTireInfo> GetTireProductsInfoByPids(string[] pids)
        {
            try
            {
                using (var queryClient = new ProductInfoQueryClient())
                {
                    var queryResult = queryClient.SelectProductTireInfo(pids.ToList());
                    queryResult.ThrowIfException(true);
                    return queryResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取轮胎商品信息失败 {string.Join(",", pids)}", ex);
                return null;
            }
        }
    }
}
