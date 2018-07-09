using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.New;


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

    }
}
