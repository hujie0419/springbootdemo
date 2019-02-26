using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;

namespace Tuhu.Service.Activity.DataAccess.DragonBall
{
    /// <summary>
    ///     七龙珠 - 用户奖励表
    /// </summary>
    public class DalDragonBallUserLoot
    {

        /// <summary>
        ///     获取用户奖励   
        /// </summary>
        /// <returns></returns>
        public static async Task<List<DragonBallUserLootModel>> SearchDragonBallUserLootByUserIdAsync(bool readOnly, Guid userId)
        {
            var sql = @"
                    SELECT [PKID]
                      ,[UserId]
                      ,[LootName]
                      ,[LootStartTime]
                      ,[LootEndTime]

                      ,[LootDesc]
                      ,[LootPicUrl]

                      ,[CreateDatetime]
                      ,[LastUpdateDateTime]
                      ,[LootType]
                      ,[LootTitile]
                      ,[LootMemo]


                  FROM [Activity].[dbo].[tbl_DragonBallUserLoot] with (nolock)
                  where [UserId] = @UserId
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", userId);

                return (await DbHelper.ExecuteSelectAsync<DragonBallUserLootModel>(readOnly, cmd)).OrderBy(p => p.PKID).ToList();
            }
        }

        /// <summary>
        ///     获取用户奖励   
        /// </summary>
        /// <returns></returns>
        public static async Task<List<DragonBallUserLootModel>> SearchDragonBallUserLootAsync(int count,int lootType)
        {
            var sql = @"
                    SELECT
                       top " + count + @"
                       [PKID]
                      ,[UserId]
                      ,[LootName]
                      ,[LootStartTime]
                      ,[LootEndTime]

                      ,[LootDesc]
                      ,[LootPicUrl]

                      ,[CreateDatetime]
                      ,[LastUpdateDateTime]
                      ,[LootType]
                      ,[LootTitile]
                      ,[LootMemo]


                  FROM [Activity].[dbo].[tbl_DragonBallUserLoot] with (nolock)
                  where lootType = @lootType
                  order by pkid desc
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@lootType", lootType);
                return (await DbHelper.ExecuteSelectAsync<DragonBallUserLootModel>(true, cmd)).ToList();
            }
        }

        /// <summary>
        ///     新增用户奖励
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertDragonBallUserLootAsync(BaseDbHelper dbHelper,
            DragonBallUserLootModel model)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_DragonBallUserLoot]
                         (  
                           [UserId]
                          ,[LootName]
                          ,[LootStartTime]
                          ,[LootEndTime]

                          ,[LootDesc]
                          ,[LootPicUrl]

                          ,[CreateDatetime]
                          ,[LastUpdateDateTime]
                          ,[LootType]
                          ,[LootTitile]
                          ,[LootMemo]

                         )
                         values (
                            @UserId
                            ,@LootName
                            ,@LootStartTime
                            ,@LootEndTime

                            ,@LootDesc
                            ,@LootPicUrl
                            ,getdate()
                            ,getdate()
                            ,@LootType
                            ,@LootTitile
                            ,@LootMemo


                         );
                        SELECT SCOPE_IDENTITY();
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", model.UserId);
                cmd.AddParameter("@LootName", model.LootName ?? "");
                cmd.AddParameter("@LootStartTime", model.LootStartTime);
                cmd.AddParameter("@LootEndTime", model.LootEndTime);

                cmd.AddParameter("@LootDesc", model.LootDesc ?? "");
                cmd.AddParameter("@LootPicUrl", model.LootPicUrl ?? "");
                cmd.AddParameter("@LootType", model.LootType);
                cmd.AddParameter("@LootTitile", model.LootTitile ?? "");
                cmd.AddParameter("@LootMemo", model.LootMemo ?? "");

                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     删除用户领取记录
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteDragonBallUserLootAsync(BaseDbHelper dbHelper,
            Guid userId)
        {
            var sql = @" delete Activity.[dbo].[tbl_DragonBallUserLoot]
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
