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
    /// 优惠券任务 - 仓储
    /// </summary>
    public interface IPromotionTaskRepository
    {
        //ValueTask<int> CreateAsync(PromotionTaskEntity entity, CancellationToken cancellationTok);
        //ValueTask<bool> UpdateAsync(PromotionTaskEntity entity, CancellationToken cancellationToken);
        //ValueTask<PromotionTaskEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);

        /// <summary>
        /// 获取有效的优惠券任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskEntity>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 更新任务的领取成功和失败数目
        /// </summary>
        /// <param name="PromotionTaskId">任务id</param>
        /// <param name="type">0=成功次数，1=失败次数</param>
        /// <param name="IncrementCount">增加的次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> UpdatePromotionTaskSendCountAsync(int PromotionTaskId, int type, int IncrementCount, CancellationToken cancellationToken);

        /// <summary>
        ///更新优惠券任务 发送成功和失败数目
        /// </summary>
        /// <param name="PromotionTaskId">任务id</param>
        /// <param name="successCount">成功数</param>
        /// <param name="failCount">失败数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> UpdatePromotionTaskCountAsync(int PromotionTaskId, int successCount, int failCount, CancellationToken cancellationToken);
    }
}
