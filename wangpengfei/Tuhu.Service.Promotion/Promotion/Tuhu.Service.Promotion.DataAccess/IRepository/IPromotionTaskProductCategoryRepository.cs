using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券任务对于的产品类目
    /// </summary>
    public interface IPromotionTaskProductCategoryRepository
    {
        /// <summary>
        /// 根据任务id 获取配置的 类目信息
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskProductCategoryEntity>> GetByPromotionTaskIdsync(int promotionTaskId, CancellationToken cancellationToken);
    }
}
