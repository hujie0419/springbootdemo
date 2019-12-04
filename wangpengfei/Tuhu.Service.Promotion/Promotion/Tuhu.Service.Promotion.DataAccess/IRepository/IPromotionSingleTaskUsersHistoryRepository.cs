using System;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券任务发送记录 仓储
    /// </summary>
    public interface IPromotionSingleTaskUsersHistoryRepository
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> CreateAsync(PromotionSingleTaskUsersHistoryEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// 通过任务id和手机号获取
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="phone"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndPhoneAsync(int promotionTaskId, String phone, CancellationToken cancellationToken);

        ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndOrderIDAsync(int promotionTaskId, string OrderNo, CancellationToken cancellationToken);

        /// <summary>
        /// 通过任务id和userid获取
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndUserIdAsync(int promotionTaskId,Guid userId, CancellationToken cancellationToken);
    }
}
