using Common.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.New;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductQueryServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProductQueryServiceProxy));

        /// <summary>
        /// //批量获取商品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<List<SkuProductDetailModel>> SelectSkuProductListByPidsAsync(List<string> pids)
        {
            var result = new List<SkuProductDetailModel>();

            if (!(pids?.Count > 0))
            {
                return result;
            }

            using (var client = new ProductClient())
            {
                var productResult = await client.SelectSkuProductListByPidsAsync(pids);
                if (productResult.Success)
                {
                    result = productResult.Result;
                }
                else
                {
                    Logger.Warn($"SelectSkuProductListByPidsAsync失败,PID:{string.Join(",",pids)},Message:{productResult.ErrorMessage}");
                }
            }

            return result;
        }

    }
}
