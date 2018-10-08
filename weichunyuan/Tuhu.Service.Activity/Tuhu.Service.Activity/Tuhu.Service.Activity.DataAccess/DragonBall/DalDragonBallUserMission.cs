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
    ///     七龙珠 - 用户任务列表
    /// </summary>
    public class DalDragonBallUserMission
    {

        /// <summary>
        ///     获取用户正在做的任务列表    
        /// </summary>
        /// <returns></returns>
        public static async Task<List<DragonBallUserMissionModel>> SearchDragonBallUserMissionByUserIdAsync(bool readOnly, Guid userId)
        {
            var sql = @"
                    SELECT [PKID]
                      ,[UserId]
                      ,[MissionId]
                      ,[MissionStatus]
                      ,[DragonBallCount]
                      ,[CreateDatetime]
                      ,[LastUpdateDateTime]
                  FROM [Activity].[dbo].[tbl_DragonBallUserMission] with (nolock)
                  where [UserId] = @UserId
                ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", userId);

                return (await DbHelper.ExecuteSelectAsync<DragonBallUserMissionModel>(readOnly, cmd)).OrderBy(p => p.PKID).ToList();
            }
        }

        /// <summary>
        ///     更新用户正在做的任务
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateDragonBallUserMissionAsync(BaseDbHelper dbHelper,
            DragonBallUserMissionModel model)
        {
            var sql = @" update Activity.[dbo].[tbl_DragonBallUserMission]
                         set 
                                  [MissionId] = @MissionId
                                  ,[MissionStatus] = @MissionStatus
                                  ,[DragonBallCount] = @DragonBallCount
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid and LastUpdateDateTime = @LastUpdateDateTime
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@MissionId", model.MissionId);
                cmd.AddParameter("@MissionStatus", model.MissionStatus);
                cmd.AddParameter("@DragonBallCount", model.DragonBallCount);


                cmd.AddParameter("@pkid", model.PKID);
                cmd.AddParameter("@LastUpdateDateTime", model.LastUpdateDateTime);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }


        /// <summary>
        ///     新增用户正在做的任务
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertDragonBallUserMissionAsync(BaseDbHelper dbHelper,
            DragonBallUserMissionModel model)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_DragonBallUserMission]
                         (  
                            UserId,
                            MissionId,
                            MissionStatus,
                            DragonBallCount,
                            CreateDatetime,
                            LastUpdateDateTime
                         )
                         values (
                            @UserId,
                            @MissionId,
                            @MissionStatus,
                            @DragonBallCount,
                            getdate(),
                            getdate()
                         );
                        SELECT SCOPE_IDENTITY();
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@UserId", model.UserId);
                cmd.AddParameter("@MissionId", model.MissionId);
                cmd.AddParameter("@MissionStatus", model.MissionStatus);
                cmd.AddParameter("@DragonBallCount", model.DragonBallCount);



                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }


        /// <summary>
        ///     删除用户任务记录
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteDragonBallUserMissionAsync(BaseDbHelper dbHelper,
            Guid userId)
        {
            var sql = @" delete Activity.[dbo].[tbl_DragonBallUserMission]
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
