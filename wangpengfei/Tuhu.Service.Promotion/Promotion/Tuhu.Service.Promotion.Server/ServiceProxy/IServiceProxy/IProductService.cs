using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Product.ModelDefinition.ProductModels;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 产品服务接口
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// 根据pid查询产品基础信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        Task<OperationResult<List<ProductBaseInfo>>> SelectProductBaseInfoAsync(List<string> pids);
    }
}
