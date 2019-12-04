using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 发送记录
    /// </summary>
    public class PromotionSingleTaskUsersHistoryRepository : IPromotionSingleTaskUsersHistoryRepository
    {
        private string DBName = "Gungnir..tbl_PromotionSingleTaskUsersHistory";
        private readonly IDbHelperFactory _factory;
        public PromotionSingleTaskUsersHistoryRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(PromotionSingleTaskUsersHistoryEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (PromotionTaskId
                                   ,UserCellPhone
                                   ,CreateTime
                                   ,SendState
                                   ,OrderNo
                                   ,PromotionSingleTaskUsersPKID
                                   ,IsSuccess
                                   ,Message
                                   ,PromotionCodeIDs
                                    ,UserID
                                    )
                             VALUES
                                   (@PromotionTaskId
                                   ,@UserCellPhone
                                   ,getdate()
                                   ,@SendState
                                   ,@OrderNo
                                   ,@PromotionSingleTaskUsersPKID
                                   ,@IsSuccess
                                   ,@Message
                                   ,@PromotionCodeIDs
                                    ,@UserID
                                    );
                                select SCOPE_IDENTITY();";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", entity.PromotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@UserCellPhone", entity.UserCellPhone));
                    cmd.Parameters.Add(new SqlParameter("@SendState", entity.SendState));
                    cmd.Parameters.Add(new SqlParameter("@OrderNo", entity.OrderNo));
                    cmd.Parameters.Add(new SqlParameter("@PromotionSingleTaskUsersPKID", entity.PromotionSingleTaskUsersPKID));
                    cmd.Parameters.Add(new SqlParameter("@IsSuccess", entity.IsSuccess));
                    cmd.Parameters.Add(new SqlParameter("@Message", entity.Message));
                    cmd.Parameters.Add(new SqlParameter("@PromotionCodeIDs", entity.PromotionCodeIDs));
                    cmd.Parameters.Add(new SqlParameter("@UserID", entity.UserID));

                    var temp = await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false);
                    return Convert.ToInt32(temp);

                }
            }
        }

        /// <summary>
        /// 通过任务id和手机号获取
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="phone"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndPhoneAsync(int promotionTaskId, string phone, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PromotionSingleTaskUsersHistoryId]
                                  ,[PromotionTaskId]
                                  ,[UserCellPhone]
                                  ,[CreateTime]
                                  ,[SendState]
                                  ,[OrderNo]
                                  ,[PromotionSingleTaskUsersPKID]
                                  ,[IsSuccess]
                                  ,[Message]
                                  ,[PromotionCodeIDs]
                                  ,[UserID]
                        FROM  {DBName} with (nolock)
                        where PromotionTaskId = @PromotionTaskId
                        and UserCellPhone = @UserCellPhone
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir"))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@UserCellPhone", phone));
                    var result = (await dbHelper.ExecuteFetchAsync<PromotionSingleTaskUsersHistoryEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }


        public async ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndOrderIDAsync(int promotionTaskId, string OrderNo, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PromotionSingleTaskUsersHistoryId]
                                  ,[PromotionTaskId]
                                  ,[UserCellPhone]
                                  ,[CreateTime]
                                  ,[SendState]
                                  ,[OrderNo]
                                  ,[PromotionSingleTaskUsersPKID]
                                  ,[IsSuccess]
                                  ,[Message]
                                  ,[PromotionCodeIDs]
                                  ,[UserID]
                        FROM  {DBName} with (nolock)
                        where PromotionTaskId = @PromotionTaskId
                        and OrderNo = @OrderNo
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir"))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@OrderNo", OrderNo));
                    var result = (await dbHelper.ExecuteFetchAsync<PromotionSingleTaskUsersHistoryEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        /// <summary>
        ///  通过任务id和userid获取
        /// </summary>
        /// <param name="promotionTaskId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PromotionSingleTaskUsersHistoryEntity> GetByPromotionTaskIdAndUserIdAsync(int promotionTaskId, Guid userId, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PromotionSingleTaskUsersHistoryId]
                                  ,[PromotionTaskId]
                                  ,[UserCellPhone]
                                  ,[CreateTime]
                                  ,[SendState]
                                  ,[OrderNo]
                                  ,[PromotionSingleTaskUsersPKID]
                                  ,[IsSuccess]
                                  ,[Message]
                                  ,[PromotionCodeIDs]
                                  ,[UserID]
                        FROM  {DBName} with (nolock)
                        where PromotionTaskId = @PromotionTaskId
                        and Userid = @Userid
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", promotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@userId", userId));
                    var result = (await dbHelper.ExecuteFetchAsync<PromotionSingleTaskUsersHistoryEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }


    }
}
