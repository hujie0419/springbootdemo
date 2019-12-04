using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券任务预算
    /// </summary>
    public interface IPromotionTaskBudgetRepository
    {
        /// <summary>
        /// 增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> CreateAsync(PromotionTaskBudgetEntity entity, CancellationToken cancellationToken);
    }
}
