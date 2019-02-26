using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 用户信息
    /// </summary>
    public class DalGameUserInfo
    {
        /// <summary>
        ///     查询小游戏 - 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<GameUserInfoModel> GetGameUserInfoAsync(bool readOnly, int activityId, Guid userId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[Point]
                                  ,IsVisit
                                  ,VisitDateTime
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Activity.[dbo].[tbl_GameUserInfo] with (nolock)
                              where userid = @userid and ActivityId = @ActivityId
                              order by pkid asc
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                var result = await DbHelper.ExecuteFetchAsync<GameUserInfoModel>(readOnly, cmd);
                return result;
            }
        }

        /// <summary>
        ///     新增 - 小游戏 - 用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertGameUserInfoAsync(int activityId, Guid userId, DateTime date, BaseDbHelper dbHelper = null)
        {
            var sql = @"insert into  Activity.[dbo].[tbl_GameUserInfo]
                         (
                            UserId,
                            ActivityId,
                            Point,
                            IsVisit,
                            VisitDateTime,
                            CreateDatetime,
                            LastUpdateDateTime
                         )
                        select
                            @UserId,
                            @ActivityId,
                            0,
                            1,
                            @date,
                            @date,
                            @date
                        where NOT EXISTS (
                                SELECT 1 FROM Activity.[dbo].[tbl_GameUserInfo] xx
                                WHERE xx.UserId = @UserId  and ActivityId = @ActivityId
                        );
                        SELECT SCOPE_IDENTITY();
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                cmd.AddParameter("@date", date);

                long result = 0;

                if (dbHelper == null)
                {
                    long.TryParse((await DbHelper.ExecuteScalarAsync(cmd)).ToString(),out result);
                }
                else
                {
                    long.TryParse((await dbHelper.ExecuteScalarAsync(cmd)).ToString(), out result);
                }

                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     更新 - 小游戏 - 用户积分信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateGameUserInfoPointAsync(long pkid, int point)
        {
            var sql = @" update Activity.[dbo].[tbl_GameUserInfo]
                         set
                                  [point] = point + @point
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid  and (point + @point) >= 0
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pkid", pkid);
                cmd.AddParameter("@point", point);

                var result = await DbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

        /// <summary>
        ///     更新 - 小游戏 - 用户积分信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateGameUserInfoPointAsync(BaseDbHelper helper, long pkid,
            int point)
        {
            var sql = @" update Activity.[dbo].[tbl_GameUserInfo]
                         set
                                  [point] = point + @point
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid  and (point + @point) >= 0
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pkid", pkid);
                cmd.AddParameter("@point", point);

                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

        /// <summary>
        ///     删除 - 小游戏 - 用户积分信息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameUserInfoAsync(BaseDbHelper helper, int activityId, Guid userId)
        {
            var sql = @" delete Activity.[dbo].[tbl_GameUserInfo]
                        where userid = @userid and ActivityId = @ActivityId
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@userid", userId);
                cmd.AddParameter("@ActivityId", activityId);

                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }

    }
}
