using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 用户分享
    /// </summary>
    public class DalGameUserShare
    {
        /// <summary>
        ///     添加用户分享数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<long> InsertGameUserShareAsync(GameUserShareModel model)
        {
            var sql = @"
                     IF NOT EXISTS (SELECT * FROM Activity..tbl_GameUserShare WHERE ActivityId = @ActivityId and UserId = @UserId and ShareDate = CONVERT(varchar(10),GETDATE(),120))
                        insert into Activity.[dbo].[tbl_GameUserShare]
                        (
                            [ActivityId]
                           ,[UserId]
                           ,[ShareDate]
                           ,[CreateDatetime]
                           ,[LastUpdateDateTime]
                        )
                        values(
                             @ActivityId
                            ,@UserId
                            ,CONVERT(varchar(10),GETDATE(),120)
                            ,getdate()
                            ,getdate()
                        );
                   SELECT SCOPE_IDENTITY();
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", model.ActivityId);
                cmd.AddParameter("@UserId", model.UserId);
                var result = await DbHelper.ExecuteScalarAsync(cmd);
                if (result is DBNull)
                {
                    return 0;
                }

                return Convert.ToInt64(result);
            }
        }


        /// <summary>
        ///     删除 - 小游戏 - 分享数据
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameUserShareAsync(BaseDbHelper helper,int activityId, Guid userId)
        {
            var sql = @" delete  Activity.[dbo].[tbl_GameUserShare]
                         where userid = @userid and ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);

                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
