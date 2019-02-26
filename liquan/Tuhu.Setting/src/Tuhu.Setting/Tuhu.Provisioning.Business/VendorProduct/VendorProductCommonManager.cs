using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business.ServiceProxy;
using Tuhu.Service.Product.Request;

namespace Tuhu.Provisioning.Business
{
    public class VendorProductCommonManager
    {
        private readonly Common.Logging.ILog _logger;
        private const string CacheClientName = "setting";

        public VendorProductCommonManager()
        {
            _logger = LogManager.GetLogger(typeof(VendorProductCommonManager));
        }

        /// <summary>
        /// 根据产品类型获取产品类目
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public string GetCategoryName(string productType)
        {
            var categoryName = string.Empty;
            switch (productType)
            {
                case "Glass": categoryName = "BLGH"; break;
                case "Battery": categoryName = productType; break;
                default: break;
            }
            return categoryName;
        }

        /// <summary>
        /// 根据产品类型或者展示类型对应的中文名称
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public string GetZhNameByProductType(string productType)
        {
            var zhName = string.Empty;
            switch (productType)
            {
                case "Glass": zhName = "玻璃"; break;
                case "Battery": zhName = "蓄电池"; break;
                default: break;
            }
            return zhName;
        }

        /// <summary>
        /// 根据产品类型获取对应产品类目下的产品Pids
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetAllPidsFromCache(string productType)
        {
            List<string> result;
            var message = string.Empty;
            try
            {
                var key = $"GetAllPids/{productType}";
                using (var cacheHelper = CacheHelper.CreateCacheClient(CacheClientName))
                {
                    var cacheResult = await cacheHelper.GetOrSetAsync(key, () =>
                         GetAllPids(productType), TimeSpan.FromMinutes(10));
                    if (cacheResult.Success)
                    {
                        result = cacheResult.Value?.Item1;
                        message = cacheResult.Value?.Item2;
                    }
                    else
                    {
                        var data = await GetAllPids(productType);
                        result = data.Item1;
                        message = data.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllPidsFromCache", ex);
                result = null;
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 根据产品类型获取对应产品类目下的产品Pids
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetAllPids(string productType)
        {
            var result = null as List<string>;
            var message = string.Empty;
            var categoryName = GetCategoryName(productType);
            if (string.IsNullOrEmpty(categoryName))
            {
                message = "不支持的产品类型";
            }
            else
            {
                var manager = new ProductQueryService();
                result = await manager.GetPidsByCategoryAsync(categoryName);
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 根据产品类型获取对应产品类目下的产品品牌
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetAllBrandsFromCache(string productType)
        {
            List<string> result;
            var message = string.Empty;
            try
            {
                var key = $"GetAllBrands/{productType}";
                using (var cacheHelper = CacheHelper.CreateCacheClient(CacheClientName))
                {
                    var cacheResult = await cacheHelper.GetOrSetAsync(key, () =>
                         GetAllBrands(productType), TimeSpan.FromMinutes(10));
                    if (cacheResult.Success)
                    {
                        result = cacheResult.Value?.Item1;
                        message = cacheResult.Value?.Item2;
                    }
                    else
                    {
                        var data = await GetAllBrands(productType);
                        result = data.Item1;
                        message = data.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllBrandsFromCache", ex);
                result = null;
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 根据产品类型获取对应产品类目下的产品品牌
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetAllBrands(string productType)
        {
            var result = null as List<string>;
            var message = string.Empty;
            var categoryName = GetCategoryName(productType);
            if (string.IsNullOrEmpty(categoryName))
            {
                message = "不支持的产品类型";
            }
            else
            {
                var manager = new ProductQueryService();
                result = await manager.GetBrandsByCategoryNameAsync(categoryName);
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 获取指定类目指定品牌的产品Pids
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetPidsByBrandFromCache(string productType, string brand)
        {
            List<string> result;
            var message = string.Empty;
            try
            {
                var key = $"GetPidsByBrand/{productType}/{brand}";
                using (var cacheHelper = CacheHelper.CreateCacheClient(CacheClientName))
                {
                    var cacheResult = await cacheHelper.GetOrSetAsync(key, () =>
                         GetPidsByBrand(productType, brand), TimeSpan.FromMinutes(10));
                    if (cacheResult.Success)
                    {
                        result = cacheResult.Value?.Item1;
                        message = cacheResult.Value?.Item2;
                    }
                    else
                    {
                        var data = await GetPidsByBrand(productType, brand);
                        result = data.Item1;
                        message = data.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("GetPidsByBrandFromCache", ex);
                result = null;
                message = ex.Message;
            }
            return Tuple.Create(result, message);
        }

        /// <summary>
        /// 获取指定类目指定品牌的产品Pids
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetPidsByBrand(string productType, string brand)
        {
            var result = null as List<string>;
            var message = string.Empty;
            var categoryName = GetCategoryName(productType);
            if (string.IsNullOrEmpty(categoryName))
            {
                message = "不支持的产品类型";
            }
            else
            {
                var manager = new ProductService();
                result = await manager.ProductCommonQueryForAll(new ProductCommonQueryRequest()
                {
                    PageIndex = 1,
                    PageSize = 100,
                    AndQuerys = new Dictionary<string, ProductCommonQuery>()
                        {
                            { nameof(ProductCommonQueryModel.TopParentCategory) ,new ProductCommonQuery()
                                {
                                    CompareType=CompareType.Equal,
                                    Value=categoryName
                                }
                            },
                            { nameof(ProductCommonQueryModel.Brand) ,new ProductCommonQuery()
                                {
                                    CompareType=CompareType.Equal,
                                    Value=brand
                                }
                            },
                        }
                });
            }
            return Tuple.Create(result, message);
        }
    }
}
