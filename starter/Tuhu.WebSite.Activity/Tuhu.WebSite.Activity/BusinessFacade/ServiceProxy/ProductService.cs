using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models.New;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity
{
    public class ProductService
    {
        /// <summary>
        /// 查询多个产品(有缓存)
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SkuProductDetailModel>> SelectSkuProductListByPidsAsync(IEnumerable<string> pids)
        {
            IEnumerable<SkuProductDetailModel> products = null;
            try
            {
                using (var client = new ProductClient())
                {
                    var result = await client.SelectSkuProductListByPidsAsync(pids.Distinct().ToList());
                    result.ThrowIfException(true);
                    products = result.Result;
                }
            }
            catch (Exception ex)
            {
                WebLog.LogException(ex);
            }
            return products ?? new SkuProductDetailModel[0];
        }
    }
}
