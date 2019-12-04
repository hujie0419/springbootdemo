using System.Threading.Tasks;
using Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy;
using System.Threading;
using Microsoft.Extensions.Logging;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{
    /// <summary>
    /// 订单服务
    /// </summary>
    public class OrderService : IOrderService
    {

        private readonly ILogger _logger;
        private IOrderApiForCClient _Client;


        public OrderService(ILogger<OrderService> logger, IOrderApiForCClient client)
        {
            _logger = logger;
            _Client = client;
        }

        /// <summary>
        /// 根据订单PKID获取订单和订单产品列表信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<OrderModel>> FetchOrderAndListByOrderIdAsync(int orderId, bool isReadOnly = true, CancellationToken cancellationToken = default)
        {
            var result = await _Client.FetchOrderAndListByOrderIdAsync(orderId, isReadOnly, cancellationToken).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"OrderService FetchOrderAndListByOrderIdAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

    }
}
