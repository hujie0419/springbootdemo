using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    class ProductBusiness
    {
        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static ProductModel GetProduct(string pid)
        {
            ProductModel product = null;

            using (var productClient = new ProductClient())
            {
                var productResult = productClient.FetchProduct(pid);
                productResult.ThrowIfException(true);
                product = productResult.Result;
            }

            return product;
        }
    }
}
