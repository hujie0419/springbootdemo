using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Shop.Models;

namespace Tuhu.Service.Promotion.Server.ServiceProxy.IServiceProxy
{
    /// <summary>
    /// 门店服务接口
    /// </summary>
    public interface IShopService
    {
        /// <summary>
        ///  获取门店基本信息
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OperationResult<ShopModel>> FetchShopAsync(int shopId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
