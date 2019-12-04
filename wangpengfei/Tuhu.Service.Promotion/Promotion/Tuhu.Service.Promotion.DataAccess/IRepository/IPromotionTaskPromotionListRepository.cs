using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public interface IPromotionTaskPromotionListRepository
    {
        //ValueTask<int> CreateAsync(PromotionTaskEntity entity, CancellationToken cancellationTok);
        //ValueTask<bool> UpdateAsync(PromotionTaskEntity entity, CancellationToken cancellationToken);
        //ValueTask<PromotionTaskEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);

        /// <summary>
        /// 根据任务id获取配置优惠券规则 【多个】
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskPromotionListEntity>> GetPromotionTaskPromotionListByPromotionTaskIdAsync(int promotionTaskId, CancellationToken cancellationToken);

        /// <summary>
        /// 批量根据任务id获取配置优惠券规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskPromotionListEntity>> GetPromotionTaskPromotionLisByPromotionTaskIdsAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken);

    }
}
