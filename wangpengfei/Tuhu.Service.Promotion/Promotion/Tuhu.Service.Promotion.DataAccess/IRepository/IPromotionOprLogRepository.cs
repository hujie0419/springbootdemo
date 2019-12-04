using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券操作日志
    /// </summary>
    public interface IPromotionOprLogRepository
    {
        /// <summary>
        /// 增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> CreateAsync(PromotionOprLogEntity entity, CancellationToken cancellationToken);
        /// <summary>
        /// 改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> UpdateAsync(PromotionOprLogEntity entity, CancellationToken cancellationToken);
        /// <summary>
        /// 查
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<PromotionOprLogEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);
    }
}
