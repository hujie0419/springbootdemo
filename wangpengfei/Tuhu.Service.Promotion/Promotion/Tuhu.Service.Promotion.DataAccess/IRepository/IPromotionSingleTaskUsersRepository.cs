using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券待发表 - 仓储
    /// </summary>
    public interface IPromotionSingleTaskUsersRepository
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> CreateAsync(PromotionSingleTaskUsersEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteAsync(int PKID, CancellationToken cancellationToken);
    }
}
