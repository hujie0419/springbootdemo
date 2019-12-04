using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Order.Models;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 订单服务接口
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// 根据订单PKID获取订单和订单产品列表信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<OrderModel>> FetchOrderAndListByOrderIdAsync(int orderId, bool isReadOnly = true, CancellationToken cancellationToken = default);
    }
}
