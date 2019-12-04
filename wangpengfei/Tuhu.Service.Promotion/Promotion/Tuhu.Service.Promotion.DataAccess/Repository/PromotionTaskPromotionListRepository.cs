using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Microsoft.Extensions.Logging;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public class PromotionTaskPromotionListRepository : IPromotionTaskPromotionListRepository
    {
        private string DBName = "Gungnir..tbl_PromotionTaskPromotionList";
        private readonly IDbHelperFactory _factory;
        public PromotionTaskPromotionListRepository(IDbHelperFactory factory) => _factory = factory;
        /// <summary>
        /// 根据任务id获取发券配置
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskPromotionListEntity>> GetPromotionTaskPromotionListByPromotionTaskIdAsync(int promotionTaskId, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [TaskPromotionListId]
                                  ,[PromotionTaskId]
                                  ,[CouponRulesId]
                                  ,[PromotionDescription]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[MinMoney]
                                  ,[DiscountMoney]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[FinanceMarkName]
                                  ,[Issuer]
                                  ,[IssueChannle]
                                  ,[IssueChannleId]
                                  ,[DepartmentId]
                                  ,[IntentionId]
                                  ,[Creater]
                                  ,[DepartmentName]
                                  ,[IntentionName]
                                  ,[Number]
                                  ,[BusinessLineId]
                                  ,[BusinessLineName]
                                  ,[IsPush]
                                  ,[PushSetting]
                                  ,[IsRemind]
                                  ,[GetCouponRuleID]
                        FROM  {DBName} with (nolock)
                        where PromotionTaskId = @PromotionTaskId
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionTaskPromotionListEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 根据任务id获取发券配置 [批量]
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask<List<PromotionTaskPromotionListEntity>> GetPromotionTaskPromotionLisByPromotionTaskIdsAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
