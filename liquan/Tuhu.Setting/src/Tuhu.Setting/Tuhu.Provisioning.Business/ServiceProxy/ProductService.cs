using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Request;
using Common.Logging;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductService
    {
        private readonly ILog _logger;

        public ProductService()
        {
            _logger = LogManager.GetLogger<ProductService>();
        }

        /// <summary>
        /// 根据分类名称获取品牌列表
        /// </summary>
        /// <param name="categoryName">分类名称</param>
        /// <returns></returns>
        public async Task<List<string>> GetBrandsByCategoryName(string categoryName)
        {
            try
            {
                using (var productClient = new ProductClient())
                {
                    var productResult = await productClient.GetBrandsByCategoryNameAsync(categoryName);
                    productResult.ThrowIfException(true);
                    return productResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取品牌列表失败 {categoryName}", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取产品基本信息
        /// </summary>
        /// <param name="pid">商品Id</param>
        /// <returns></returns>
        public async Task<ProductModel> FetchProduct(string pid)
        {
            try
            {
                using (var productClient = new ProductClient())
                {
                    var productResult = await productClient.FetchProductAsync(pid);
                    productResult.ThrowIfException(true);
                    return productResult?.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"获取产品信息失败 {pid}", ex);
                return null;
            }
        }

        /// <summary>
        /// 通用搜索--查询所有结果
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<string>> ProductCommonQueryForAll(ProductCommonQueryRequest request)
        {
            var searchSize = 0;
            var result = new List<string>();
            var resultPager = new PagerModel();
            do
            {
                var searchResult = await ProductCommonQuery(request);
                if (searchResult?.Source != null && searchResult.Source.Any())
                {
                    resultPager = searchResult.Pager;
                    searchSize = (resultPager.CurrentPage - 1) * resultPager.PageSize + resultPager.PageSize;
                    result.AddRange(searchResult.Source);
                    request.PageIndex++;
                }
                else
                {
                    searchSize = 0;
                }
            }
            while (searchSize > 0 && searchSize < resultPager.Total);
            return result;
        }

        /// <summary>
        /// 通用搜索
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PagedModel<string>> ProductCommonQuery(ProductCommonQueryRequest request)
        {
            using (var client = new ShopProductClient())
            {
                var clientResult = await client.ProductCommonQueryAsync(request);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }
    }
}