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
using Tuhu.Service.Promotion.DataAccess.QueryModel;

namespace Tuhu.Service.Promotion.DataAccess.IRepository
{
    /// <summary>
    /// 活动申请 - 仓储
    /// </summary>
    public interface IUserActivityApplyRepository
    {
        /// <summary>
        /// 活动申请信息总数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> GetUserActivityApplyListCountAsync(GetUserActivityApplyListQueryModel request,
            CancellationToken cancellationToken);

        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<UserActivityApplyEntity>> GetUserActivityApplyListAsync(GetUserActivityApplyListQueryModel request,
             CancellationToken cancellationToken);

        /// <summary>
        /// 根据PKID自动通过活动申请
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs,
            CancellationToken cancellationToken);

        /// <summary>
        /// 新增活动申请
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> CreateUserActivityApplyAsync(CreateUserActivityApplyRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteUserActivityApplyByPKIDAsync(int PKID,
            CancellationToken cancellationToken);

        /// <summary>
        /// 获取可自动审核的活动申请数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<int>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request,
            CancellationToken cancellationToken);
    }
}
