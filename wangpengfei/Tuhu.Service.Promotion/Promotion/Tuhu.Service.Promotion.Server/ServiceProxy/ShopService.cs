using System.Threading.Tasks;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    /// <summary>
    /// 门店服务
    /// </summary>
    public class ShopService : IShopService
    {
        private readonly ILogger _logger;
        private IShopClient _Client;


        public ShopService(ILogger<ShopService> logger, IShopClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        ///  获取门店基本信息
        /// </summary>
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<ShopModel>> FetchShopAsync(int shopId, CancellationToken cancellationToken = default)
        {
            var result = await _Client.FetchShopAsync(shopId, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"ShopService FetchShopAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

    }
}
