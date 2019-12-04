using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tuhu.Service.ProductQuery;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using System.Collections.Generic;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ILogger _logger;
        private IProductInfoQueryClient _Client;

        public ProductService(ILogger<ProductService> logger, IProductInfoQueryClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        /// 根据pid查询产品基础信息
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<ProductBaseInfo>>> SelectProductBaseInfoAsync(List<string> pids)
        {
            var result = await _Client.SelectProductBaseInfoAsync(pids).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"ProductQueryService SelectProductBaseInfoAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

    }
}
