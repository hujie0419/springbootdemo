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
    /// 活动 - 仓储
    /// </summary>
    public interface IActivityRepository
    {
        /// <summary>
        /// 获取所有活动列表数量
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> GetActivityListCountAsync(GetActivityListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<ActivityEntity>> GetActivityListAsync(GetActivityListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="ActivityID">活动ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<ActivityEntity> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken);
    }
}
