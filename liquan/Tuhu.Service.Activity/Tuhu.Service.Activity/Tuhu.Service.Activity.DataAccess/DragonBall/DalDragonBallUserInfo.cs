using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;

namespace Tuhu.Service.Activity.DataAccess.DragonBall
{
    /// <summary>
    ///     七龙珠 - 用户信息
    /// </summary>
    public class DalDragonBallUserInfo
    {
        /// <summary>
        ///     查询七龙珠 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<DragonBallUserInfoModel> SearchDragonBallUserInfoModelAsync(bool readOnly, Guid userId)
        {
            var sql = @" SELECT [PKID]
                                  ,[UserId]
                                  ,[FinishMissionCount]
                                  ,[DragonBallCount]
                                  ,DragonBallSummonCount
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Activity.[dbo].[tbl_DragonBallUserInfo] with (nolock)
                       where userid = @userid
                ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@userid", userId);
                var result = await DbHelper.ExecuteFetchAsync<DragonBallUserInfoModel>(readOnly, cmd);
                return result;
            }
        }


        /// <summary>
        ///     更新七龙珠 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateDragonBallUserInfoModelUserMissionAsync(BaseDbHelper dbHelper,
            DragonBallUserInfoModel model)
        {
            var sql = @" update Activity.[dbo].[tbl_DragonBallUserInfo]
                         set 
                                  [FinishMissionCount] = @FinishMissionCount
                                  ,[DragonBallCount] = @DragonBallCount
                                  ,DragonBallSummonCount = @DragonBallSummonCount
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid and LastUpdateDateTime = @LastUpdateDateTime
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@FinishMissionCount", model.FinishMissionCount);
                cmd.AddParameter("@DragonBallCount", model.DragonBallCount);
                cmd.AddParameter("@DragonBallSummonCount", model.DragonBallSummonCount);


                cmd.AddParameter("@pkid", model.PKID);
                cmd.AddParameter("@LastUpdateDateTime", model.LastUpdateDateTime);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

        /// <summary>
        ///     更新七龙珠 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateDragonBallUserInfoModelAsync(BaseDbHelper dbHelper,
            DragonBallUserInfoModel model)
        {
            var sql = @" update Activity.[dbo].[tbl_DragonBallUserInfo]
                         set 
                                  [FinishMissionCount] = @FinishMissionCount
                                  ,[DragonBallCount] = @DragonBallCount
                                  ,DragonBallSummonCount = @DragonBallSummonCount
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid and LastUpdateDateTime = @LastUpdateDateTime and (DragonBallCount - @DragonBallCount) >= 0
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@FinishMissionCount", model.FinishMissionCount);
                cmd.AddParameter("@DragonBallCount", model.DragonBallCount);
                cmd.AddParameter("@DragonBallSummonCount", model.DragonBallSummonCount);


                cmd.AddParameter("@pkid", model.PKID);
                cmd.AddParameter("@LastUpdateDateTime", model.LastUpdateDateTime);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
         


        /// <summary>
        ///     新增七龙珠 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertDragonBallUserInfoModelAsync(BaseDbHelper dbHelper,
            DragonBallUserInfoModel model)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_DragonBallUserInfo]
                         (  
                            UserId,
                            FinishMissionCount,
                            DragonBallCount,
                            DragonBallSummonCount,
                            CreateDatetime,
                            LastUpdateDateTime
                         )
                         values (
                            @UserId,
                            @FinishMissionCount,
                            @DragonBallCount,
                            @DragonBallSummonCount,
                            getdate(),
                            getdate()
                         );
                        SELECT SCOPE_IDENTITY();
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@userid", model.UserId);
                cmd.AddParameter("@FinishMissionCount", model.FinishMissionCount);
                cmd.AddParameter("@DragonBallCount", model.DragonBallCount);
                cmd.AddParameter("@DragonBallSummonCount", model.DragonBallSummonCount);


                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     删除七龙珠 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteDragonBallUserInfoModelAsync(BaseDbHelper dbHelper,
            Guid userId)
        {
            var sql = @" delete Activity.[dbo].[tbl_DragonBallUserInfo]
                         where userid = @UserId
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", userId);
              

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

    }
}
