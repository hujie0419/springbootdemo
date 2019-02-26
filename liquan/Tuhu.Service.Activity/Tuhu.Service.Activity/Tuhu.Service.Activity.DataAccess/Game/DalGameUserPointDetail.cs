using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;
using Tuhu.Service.Activity.Models.Response;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 用户积分变动明细
    /// </summary>
    public class DalGameUserPointDetail
    {
        /// <summary>
        ///     查询小游戏 - 用户积分变动明细
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameUserPointDetailModel>> GetGameUserPointDetailAsync(bool readOnly,
            int activityId,
            Guid userId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[Point]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,Status
                                  ,[Memo]
                                  ,IsUsed
                              FROM Activity.[dbo].[tbl_GameUserPointDetail] with (nolock)
                              where userid = @userid and ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                var result = await DbHelper.ExecuteSelectAsync<GameUserPointDetailModel>(readOnly, cmd);
                return result?.OrderBy(p => p.PKID)?.ToList();
            }
        }


        /// <summary>
        ///     新增 - 小游戏 - 用户积分变动明细
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertGameUserPointDetailAsync(GameUserPointDetailModel data)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_GameUserPointDetail]
                         (
                            UserId,
                            ActivityId,
                            Point,
                            CreateDatetime,
                            LastUpdateDateTime,
                            [Status],
                            [Memo],
                            IsUsed
                         )
                         values (
                            @UserId,
                            @ActivityId,
                            @Point,
                            getdate(),
                            getdate(),
                            @Status,
                            @Memo,
                            0
                         );
                        SELECT SCOPE_IDENTITY();
            ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", data.ActivityId);
                cmd.AddParameter("@userId", data.UserId);
                cmd.AddParameter("@Point", data.Point);
                cmd.AddParameter("@Status", data.Status ?? "");
                cmd.AddParameter("@Memo", data.Memo ?? "");


                var result = await DbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     新增 - 小游戏 - 用户积分变动明细
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertGameUserPointDetailAsync(BaseDbHelper dbHelper,
            GameUserPointDetailModel data)
        {
            var sql = @" insert into  Activity.[dbo].[tbl_GameUserPointDetail]
                         (
                            UserId,
                            ActivityId,
                            Point,
                            CreateDatetime,
                            LastUpdateDateTime,
                            [Status],
                            [Memo],
                            IsUsed
                         )
                         values (
                            @UserId,
                            @ActivityId,
                            @Point,
                            getdate(),
                            getdate(),
                            @Status,
                            @Memo,
                            0
                         );
                        SELECT SCOPE_IDENTITY();
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", data.ActivityId);
                cmd.AddParameter("@userId", data.UserId);
                cmd.AddParameter("@Point", data.Point);
                cmd.AddParameter("@Status", data.Status ?? "");
                cmd.AddParameter("@Memo", data.Memo ?? "");


                var result = await dbHelper.ExecuteScalarAsync(cmd);
                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        ///     删除 - 小游戏 - 用户积分变动明细
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameUserPointDetailAsync(BaseDbHelper helper, int activityId, Guid userId)
        {
            var sql = @" delete  Activity.[dbo].[tbl_GameUserPointDetail]
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

        /// <summary>
        /// 分页获取实时排行榜
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static async Task<List<GameRankInfoModel>> GetRankListAsync(bool readOnly, int activityId, int rankMinPoint,
            int pageIndex = 1, int pageSize = 10)
        {
            var sql = @"SELECT *
                        FROM
                        (
                            SELECT *,
                                ROW_NUMBER() OVER (ORDER BY point DESC, CreateDatetime ASC) AS rank
                            FROM
                            (
                                SELECT UserId,
                                    SUM(Point) AS point,
                                    MAX(CreateDatetime) AS CreateDatetime
                                FROM [Activity].[dbo].tbl_GameUserPointDetail WITH (NOLOCK)
                                WHERE ActivityId = @ActivityId
                                      AND IsUsed = 0
                                GROUP BY UserId
                                HAVING SUM(point) >= @rankMinPoint
                            ) AS a
                        ) AS b
                        WHERE rank
                        BETWEEN @startNum AND @endNum;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@rankMinPoint", rankMinPoint);
                cmd.AddParameter("@startNum", (pageIndex - 1) * pageSize + 1);
                cmd.AddParameter("@endNum", pageIndex * pageSize);
                var result = await DbHelper.ExecuteSelectAsync<GameRankInfoModel>(readOnly, cmd);

                return result?.ToList();
            }
        }

        /// <summary>
        /// 获取当日之前的积分排行榜
        /// </summary>
        /// <param name="readOnly"></param>
        /// <param name="activityId"></param>
        /// <param name="rankMinPoint"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<GameRankInfoModel>> GetRankListBeforeDayAsync(bool readOnly, int activityId, int rankMinPoint,
          DateTime? dayTime, int pageIndex = 1, int pageSize = 10)
        {
            var sql = @"SELECT *
                        FROM
                        (
                            SELECT *,
                                ROW_NUMBER() OVER (ORDER BY point DESC, CreateDatetime ASC) AS rank
                            FROM
                            (
                                SELECT UserId,
                                    SUM(Point) AS point,
                                    MAX(CreateDatetime) AS CreateDatetime
                                FROM [Activity].[dbo].tbl_GameUserPointDetail WITH (NOLOCK)
                                WHERE ActivityId = @ActivityId
                                      AND IsUsed = 0
									  AND DATEDIFF(DAY,CreateDatetime,@dayTime)>0
                                GROUP BY UserId
                                HAVING SUM(point) >= @rankMinPoint
                            ) AS a
                        ) AS b
                        WHERE rank
                        BETWEEN @startNum AND @endNum;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@rankMinPoint", rankMinPoint);
                cmd.AddParameter("@dayTime", dayTime);
                cmd.AddParameter("@startNum", (pageIndex - 1) * pageSize + 1);
                cmd.AddParameter("@endNum", pageIndex * pageSize);
                var result = await DbHelper.ExecuteSelectAsync<GameRankInfoModel>(readOnly, cmd);

                return result?.ToList();
            }
        }

        /// <summary>
        /// 获取用户的活动排名信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userID"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static async Task<GameRankInfoModel> GetUserRankInfoAsync(int activityId, Guid userID, bool readOnly)
        {
            var sql = @"SELECT *
                        FROM
                        (
                            SELECT *,
                                ROW_NUMBER() OVER (ORDER BY point DESC, CreateDatetime ASC) AS rank
                            FROM
                            (
                                SELECT UserId,
                                    SUM(Point) AS point,
                                    MAX(CreateDatetime) AS CreateDatetime
                                FROM [Activity].[dbo].tbl_GameUserPointDetail WITH (NOLOCK)
                                WHERE ActivityId = @ActivityId
                                      AND IsUsed = 0
                                GROUP BY UserId
                                HAVING SUM(point) > 0
                            ) AS a
                        ) AS b
                        WHERE UserId=@UserId;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userID);
                var result = await DbHelper.ExecuteFetchAsync<GameRankInfoModel>(readOnly, cmd);

                return result;
            }
        }

        /// <summary>
        /// 当天通过某种方式获得积分  一天仅一次
        /// </summary>
        /// <param name="dbHelper"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<long> InsertTodayGameUserPointDetailAsync(GameUserPointDetailModel data)
        {
            var sql = @"    IF NOT EXISTS
                            (
                                SELECT *
                                FROM Activity..tbl_GameUserPointDetail
                                WHERE ActivityId = @ActivityId
                                      AND UserId = @UserId
                                      AND Status = @Status
                                      AND DATEDIFF(dd, CreateDatetime, GETDATE()) = 0
                            )
                           INSERT INTO Activity.[dbo].[tbl_GameUserPointDetail]
                           (
                               UserId,
                               ActivityId,
                               Point,
                               CreateDatetime,
                               LastUpdateDateTime,
                               [Status],
                               IsUsed,
                               [Memo]
                           )
                           VALUES
                           (
                               @UserId,
                               @ActivityId,
                               @Point,
                               GETDATE(),
                               GETDATE(),
                               @Status,
                               0,
                               @Memo
                           );
                       SELECT SCOPE_IDENTITY();
                    ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", data.ActivityId);
                cmd.AddParameter("@UserId", data.UserId);
                cmd.AddParameter("@Point", data.Point);
                cmd.AddParameter("@Status", data.Status ?? "");
                cmd.AddParameter("@Memo", data.Memo ?? "");
                int.TryParse((await DbHelper.ExecuteScalarAsync(cmd)).ToString(), out int result);
                return result;
            }
        }

        /// <summary>
        /// 更新用户积分的使用状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<int> UpdateUserPointIsUsedAsync(BaseDbHelper dbHelper,int activityId, Guid userID)
        {
            var sql = @"UPDATE  Activity.[dbo].[tbl_GameUserPointDetail] WITH (ROWLOCK)
                            SET IsUsed = 1,
                                LastUpdateDateTime = GETDATE()
                          WHERE ActivityId = @ActivityId
                                  AND UserId = @UserId
								  AND DATEDIFF(DAY,CreateDatetime,GETDATE())>0
                                  AND IsUsed <> 1;";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@UserId", userID);

                var result = await dbHelper.ExecuteNonQueryAsync(cmd);
                return result;
            }
        }


    }
}
