using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.New;
using Tuhu.Service.ProductQuery;

namespace Tuhu.Provisioning.Business.CommonServices
{
    public class ProductService
    {
        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ProductService));

        public static List<SkuProductDetailModel> SelectSkuProductListByPids(List<string> pids)
        {
            List<SkuProductDetailModel> result = new List<SkuProductDetailModel>();
            try
            {
                if (pids != null && pids.Any())
                {
                    using (var client = new ProductClient())
                    {
                        var getResult = client.SelectSkuProductListByPids(pids);
                        getResult.ThrowIfException(true);
                        result = getResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public static ProductModel FetchProduct(string pid)
        {
            ProductModel result = null;
            try
            {
                using (var client = new ProductClient())
                {
                    var getResult = client.FetchProduct(pid);
                    getResult.ThrowIfException(true);
                    result = getResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 查询产品详情
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static ProductBaseInfo GetProductBaseInfo(string pid)
        {
            ProductBaseInfo result = null;

            try
            {
                using (var client = new ProductInfoQueryClient())
                {
                    var listResult = client.SelectProductBaseInfo(new List<string>() { pid });
                    if (listResult.Success && listResult.Result != null)
                    {
                        result = listResult.Result?.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
        /// <summary>
        /// 根据产品属性更新产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="propinfos"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        public static bool UpdateProductByPropertyNames(string pid, Dictionary<string, object> propinfos, string operatorName)
        {
            bool result = false;

            try
            {
                using (var client = new ProductClient())
                {
                    var serviceResult = client.UpdateProductByPropertyNames(pid, propinfos, operatorName);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

    }
}
