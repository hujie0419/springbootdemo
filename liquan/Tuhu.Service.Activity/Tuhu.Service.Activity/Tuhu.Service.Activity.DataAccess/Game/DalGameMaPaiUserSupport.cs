using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 马牌用户助力
    /// </summary>
    public class DalGameMaPaiUserSupport
    {
        /// <summary>
        ///     小游戏 - 马牌用户助力
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameMaPaiUserSupportModel>> GetGameMaPaiUserSupportAsync(bool readOnly,
            int activityId,
            Guid userId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[SupportOpenId]
                                  ,[SupportNickName]
                                  ,[SupportHeadImgURL]
                                  ,[Distance]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Activity.[dbo].[tbl_GameMaPaiUserSupport] with (nolock)
                              where userId = @userId and ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                var result = await DbHelper.ExecuteSelectAsync<GameMaPaiUserSupportModel>(readOnly, cmd);
                return result?.ToList() ?? new List<GameMaPaiUserSupportModel>();
            }
        }

        /// <summary>
        ///     小游戏 - 马牌用户助力
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameMaPaiUserSupportModel>> GetGameMaPaiUserSupportAsync(bool readOnly,
            int activityId,
            string openId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[SupportOpenId]
                                  ,[SupportNickName]
                                  ,[SupportHeadImgURL]
                                  ,[Distance]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Activity.[dbo].[tbl_GameMaPaiUserSupport] with (nolock)
                              where SupportOpenId = @openid  and ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@openid", openId);
                var result = await DbHelper.ExecuteSelectAsync<GameMaPaiUserSupportModel>(readOnly, cmd);
                return result?.ToList() ?? new List<GameMaPaiUserSupportModel>();
            }
        }

        /// <summary>
        ///     新增 - 小游戏 - 马牌用户助力
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertGameMaPaiUserSupportAsync(BaseDbHelper dbHelper,
            GameMaPaiUserSupportModel data)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_GameMaPaiUserSupport]
                         (
                             [ActivityId]
                             ,[UserId]
                             ,[SupportOpenId]
                             ,[SupportNickName]
                             ,[SupportHeadImgURL]
                             ,[Distance]
                             ,[CreateDatetime]
                             ,[LastUpdateDateTime]
                         )
                         values (
                            @ActivityId,
                            @userId,
                            @SupportOpenId,
                            @SupportNickName,
                            @SupportHeadImgURL,
                            @Distance,
                            getdate(),
                            getdate()
                         );
                        SELECT SCOPE_IDENTITY();
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", data.ActivityId);
                cmd.AddParameter("@userId", data.UserId);
                cmd.AddParameter("@SupportOpenId", data.SupportOpenId ?? "");
                cmd.AddParameter("@SupportNickName", data.SupportNickName ?? "");
                cmd.AddParameter("@SupportHeadImgURL", data.SupportHeadImgURL ?? "");
                cmd.AddParameter("@Distance", data.Distance);


                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     删除 - 小游戏 - 马牌用户助力
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameMaPaiUserSupportAsync(BaseDbHelper helper, int activityId, Guid userId)
        {
            var sql = @" delete Activity.[dbo].[tbl_GameMaPaiUserSupport]
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

        /// <summary>
        ///     删除 - 小游戏 - 马牌用户助力
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameMaPaiUserSupportAsync(BaseDbHelper helper, int activityId, string openId)
        {
            var sql = @" delete Activity.[dbo].[tbl_GameMaPaiUserSupport]
                        where SupportOpenId = @SupportOpenId and ActivityId = @ActivityId
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@SupportOpenId", openId);
                cmd.AddParameter("@ActivityId", activityId);

                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
