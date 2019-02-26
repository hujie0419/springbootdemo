using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Request;
using Tuhu.Service.Product.Models;
using Common.Logging;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 产品搜索服务
    /// </summary>
    public class ProductSearchService
    {
        private readonly ILog _logger;

        public ProductSearchService()
        {
            _logger = LogManager.GetLogger<ProductSearchService>();
        }

        /// <summary>
        /// 查询轮胎列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterPropertys"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<ProductFilterProperty>>>
            GetTiresByFilter(SearchProductRequest filter, string[] filterPropertys)
        {
            try
            {
                using (var searchClient = new ProductSearchClient())
                {
                    var searchResult = await searchClient.QueryTireListFilterValuesAsync(filter, filterPropertys.ToList());
                    searchResult.ThrowIfException(true);
                    return searchResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"查询轮胎列表失败 {JsonConvert.SerializeObject(filter)}", ex);
                return null;
            }
        }

        /// <summary>
        /// 查询商品信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PagedModel<string>> SearchProduct(SearchProductRequest query)
        {
            try
            {
                using (var searchClient = new ProductSearchClient())
                {
                    var searchResult = await searchClient.SearchProductAsync(query);
                    searchResult.ThrowIfException(true);
                    return searchResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"查询商品信息失败 {JsonConvert.SerializeObject(query)}", ex);
                return null;
            }
        }
    }
}
