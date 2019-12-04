using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess
{
    public interface IPromotionCodeRepository
    {
        ValueTask<List<PromotionCodeEntity>> GetCouponByUserIDAsync(Guid UserId, CancellationToken cancellationToken);
        ValueTask<List<PromotionCodeEntity>> GetHistoryCouponByUserIDAsync(Guid UserId, CancellationToken cancellationToken);

        ValueTask<PromotionCodeEntity> GetCouponByIDAsync(int pkid, CancellationToken cancellationToken);
        ValueTask<bool> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken);

        ValueTask<bool> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken);
        ValueTask<bool> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken);
        ValueTask<List<PromotionCodeEntity>> GetPKIDByCodeChannelANDPKIDAsync(ObsoleteCouponsByChannelRequest request, CancellationToken cancellationToken);
    }

    public class PromotionCodeRepository : IPromotionCodeRepository
    {
        private string DBName = "Gungnir..tbl_PromotionCode";
        private readonly IDbHelperFactory _factory;

        public PromotionCodeRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 获取用户领取的优惠券
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionCodeEntity>> GetCouponByUserIDAsync(Guid UserId, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT    [PKID]
                                          ,[Code]
                                          ,[UserId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UsedTime]
                                          ,[OrderId]
                                          ,[Status]
                                          ,[Type]
                                          ,[Description]
                                          ,[Discount]
                                          ,[MinMoney]
                                          ,[CodeChannel]
                                          ,[BatchID]
                                          ,[DeviceID]
                                          ,[RuleID]
                                          ,[PromtionName]
                                          ,[ExtCol]
                                          ,[GetRuleID]
                                          ,[FinanceMarkName]
                                          ,[Issuer]
                                          ,[IssueChannle]
                                          ,[IssueChannleId]
                                          ,[DepartmentId]
                                          ,[IntentionId]
                                          ,[Creater]
                                          ,[DepartmentName]
                                          ,[IntentionName]
                                          ,[CouponType]
                                          ,[BusinessLineName]
                                          ,[TaskPromotionListId]
                                          ,[DiscountAmount]
                                          ,[LeastCost]
                                          ,[ReduceCost]
                                      FROM {DBName} with (nolock)
                                      where UserId = @UserId
                            ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionCodeEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }



        /// <summary>
        /// 获取用户领取的优惠券 [归档]
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionCodeEntity>> GetHistoryCouponByUserIDAsync(Guid UserId, CancellationToken cancellationToken)
        {
            #region sql
            const string sql = @"SELECT    [PKID]
                                          ,[Code]
                                          ,[UserId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UsedTime]
                                          ,[OrderId]
                                          ,[Status]
                                          ,[Type]
                                          ,[Description]
                                          ,[Discount]
                                          ,[MinMoney]
                                          ,[CodeChannel]
                                          ,[BatchID]
                                          ,[DeviceID]
                                          ,[RuleID]
                                          ,[PromtionName]
                                          ,[ExtCol]
                                          --,[GetRuleID]
                                          --,[FinanceMarkName]
                                          --,[Issuer]
                                          --,[IssueChannle]
                                          --,[IssueChannleId]
                                          --,[DepartmentId]
                                          --,[IntentionId]
                                          --,[Creater]
                                          --,[DepartmentName]
                                          --,[IntentionName]
                                          --,[CouponType]
                                          --,[BusinessLineName]
                                          --,[TaskPromotionListId]
                                          --,[DiscountAmount]
                                          --,[LeastCost]
                                          --,[ReduceCost]
                                      FROM Gungnir_History..tbl_PromotionCode with (nolock)
                                      where UserId = @UserId
                            ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir_History", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionCodeEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 根据pkid获取用户优惠券接口
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PromotionCodeEntity> GetCouponByIDAsync(int pkid, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT    [PKID]
                                          ,[Code]
                                          ,[UserId]
                                          ,[StartTime]
                                          ,[EndTime]
                                          ,[CreateTime]
                                          ,[UsedTime]
                                          ,[OrderId]
                                          ,[Status]
                                          ,[Type]
                                          ,[Description]
                                          ,[Discount]
                                          ,[MinMoney]
                                          ,[CodeChannel]
                                          ,[BatchID]
                                          ,[DeviceID]
                                          ,[RuleID]
                                          ,[PromtionName]
                                          ,[ExtCol]
                                          ,[GetRuleID]
                                          ,[FinanceMarkName]
                                          ,[Issuer]
                                          ,[IssueChannle]
                                          ,[IssueChannleId]
                                          ,[DepartmentId]
                                          ,[IntentionId]
                                          ,[Creater]
                                          ,[DepartmentName]
                                          ,[IntentionName]
                                          ,[CouponType]
                                          ,[BusinessLineName]
                                          ,[TaskPromotionListId]
                                          ,[DiscountAmount]
                                          ,[LeastCost]
                                          ,[ReduceCost]
                                      FROM {DBName} with (nolock)
                                      where PKID = @PKID
                            ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                    var result = (await dbHelper.ExecuteFetchAsync<PromotionCodeEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        public async ValueTask<bool> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"Update
                               {DBName} with(rowlock)
                            set
                                EndTime = @EndTime
                            where
                                PKID = @PKID
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", request.PromotionPKID));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", request.EndTime));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }

        public async ValueTask<bool> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"Update
                                {DBName} with(rowlock)
                            set
                                ReduceCost = @ReduceCost
                                ,Discount =@Discount
                            where
                                PKID = @PKID
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", request.PromotionPKID));
                    cmd.Parameters.Add(new SqlParameter("@ReduceCost", request.ReduceCost));
                    cmd.Parameters.Add(new SqlParameter("@Discount",Convert.ToInt32(request.ReduceCost)));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }

        public async ValueTask<bool> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken)
        {

            #region sql
            string sql = $@"Update
                                {DBName} with(rowlock)
                            set
                               Status = 3
                            where
                                PKID = @PKID
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", request.PromotionPKID));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }


        public async ValueTask<List<PromotionCodeEntity>> GetPKIDByCodeChannelANDPKIDAsync(ObsoleteCouponsByChannelRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT    PKID
                                      ,UserID
                                      FROM {DBName} with (nolock)
                                      where  pkid >=@MinPromotionCodePKID
                                      and pkid <=@MaxPromotionCodePKID
                                      and CodeChannel =@CodeChannel
                            ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@MinPromotionCodePKID", request.MinPromotionCodePKID));
                    cmd.Parameters.Add(new SqlParameter("@MaxPromotionCodePKID", request.MaxPromotionCodePKID));
                    cmd.Parameters.Add(new SqlParameter("@CodeChannel", request.CodeChannel));
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionCodeEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }
    }
}
