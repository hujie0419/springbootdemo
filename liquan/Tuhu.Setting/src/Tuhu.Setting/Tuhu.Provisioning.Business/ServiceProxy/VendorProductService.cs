using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.VendorProduct;
using Tuhu.Service.VendorProduct.Request;

namespace Tuhu.Provisioning.Business.ServiceProxy
{
    public class VendorProductService
    {
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> RemoveCacheByType(RemoveCacheByTypeRequest request)
        {
            using (var client = new VendorProductCacheClient())
            {
                var clientResult = await client.RemoveCacheByTypeAsync(request);
                clientResult.ThrowIfException(true);
                return clientResult.Result;
            }
        }
    }
}
