using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    public class ProductQueryService
    {
        /// <summary>
        /// 根据类目获取产品Pid
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPidsByCategoryAsync(string categoryName, bool includeOffSale = true)
        {
            using (var client = new Service.ProductQuery.ProductQueryClient())
            {
                var clientResult = await client.GetPidsByCategoryAsync(categoryName, includeOffSale);
                clientResult.ThrowIfException(true);
                return clientResult.Result ?? new List<string>();
            }
        }

        /// <summary>
        /// 根据类目获取产品品牌
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetBrandsByCategoryNameAsync(string categoryName)
        {
            using (var client = new Service.ProductQuery.ProductQueryClient())
            {
                var clientResult = await client.GetBrandsByCategoryNameAsync(categoryName);
                clientResult.ThrowIfException(true);
                return clientResult.Result ?? new List<string>();
            }
        }
    }
}
