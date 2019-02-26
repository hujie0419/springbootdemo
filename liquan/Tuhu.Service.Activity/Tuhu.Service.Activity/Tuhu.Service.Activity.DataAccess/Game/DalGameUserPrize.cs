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
    ///     小游戏 - 用户奖品表
    /// </summary>
    public static class DalGameUserPrize
    {
        /// <summary>
        ///     查询用户奖品
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameUserPrizeModel>> GetGameUserPrizeAsync(bool readOnly, int activityId,
            Guid userId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[Point]
                                  ,[PrizeId]
                                  ,[PrizeName]
                                  ,[PrizePicUrl]
                                  ,[PrizeTitle]
                                  ,[PrizeDesc]
                                  ,[PrizeStartTime]
                                  ,[PrizeEndTime]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[PromotionGetRuleGuid]
                                  ,[PromotionCode]
                                  ,[PromotionId]
                                  ,[IsBroadCastShow]
                                  ,TodayRank
                                  ,PrizeType
                                  ,GetPrizeDate 
                              FROM Activity.[dbo].[tbl_GameUserPrize] with (nolock)
                              where userId = @userId and  ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@userId", userId);
                var result = await DbHelper.ExecuteSelectAsync<GameUserPrizeModel>(readOnly, cmd);
                return result?.ToList();
            }
        }

        /// <summary>
        ///     查询用户奖品    分页
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="showAll">1 显示全部 2 只显示显示的商品</param>
        /// <returns></returns>
        public static async Task<IList<GameUserPrizeModel>> GetGameUserPrizeAsync(int activityId, int pageIndex,
            int pageSize, int showAll = 1)
        {
            var showAllSql = string.Empty;
            if (showAll == 2)
            {
                showAllSql = " and IsBroadCastShow = 1 ";
            }

            var sql = $@" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[UserId]
                                  ,[Point]
                                  ,[PrizeId]
                                  ,[PrizeName]
                                  ,[PrizePicUrl]
                                  ,[PrizeTitle]
                                  ,[PrizeDesc]
                                  ,[PrizeStartTime]
                                  ,[PrizeEndTime]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[PromotionGetRuleGuid]
                                  ,[PromotionCode]
                                  ,[PromotionId]
                                  ,[IsBroadCastShow]
                                  ,GetPrizeDate 
                              FROM Activity.[dbo].[tbl_GameUserPrize] with (nolock)
                              where ActivityId = @ActivityId  {showAllSql}
                              ORDER BY PKID DESC
                              OFFSET (@PageNumber - 1) * @PageSize ROW
				              FETCH NEXT @PageSize ROW ONLY;
                ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@PageNumber", pageIndex);
                cmd.AddParameter("@PageSize", pageSize);


                var result = await DbHelper.ExecuteSelectAsync<GameUserPrizeModel>(true, cmd);
                return result?.ToList();
            }
        }

        /// <summary>
        /// 分页查询某段时间的所有用户活动奖品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static async Task<IList<GameUserPrizeModel>> GetGameUserPrizeByTimeAsync(
            int activityId, int pageIndex, int pageSize, DateTime? startTime)
        {
            var startTimeSQL = string.Empty;
            if (startTime != null)
            {
                startTimeSQL = " and CreateDatetime>@startTime";
            }

            var sql = $@" SELECT [PKID]
                                ,[ActivityId]
                                ,[UserId]
                                ,[Point]
                                ,[PrizeId]
                                ,[PrizeName]
                                ,[PrizePicUrl]
                                ,[PrizeTitle]
                                ,[PrizeDesc]
                                ,[PrizeStartTime]
                                ,[PrizeEndTime]
                                ,[CreateDatetime]
                                ,[LastUpdateDateTime]
                                ,[PromotionGetRuleGuid]
                                ,[PromotionCode]
                                ,[PromotionId]
                                ,[IsBroadCastShow]
                                ,GetPrizeDate 
                          FROM Activity.[dbo].[tbl_GameUserPrize] with (nolock)
                          where ActivityId = @ActivityId 
                          AND IsBroadCastShow = 1  
                          {startTimeSQL}
                          ORDER BY PKID DESC
                          OFFSET (@PageNumber - 1) * @PageSize ROW
				          FETCH NEXT @PageSize ROW ONLY; ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@PageNumber", pageIndex);
                cmd.AddParameter("@PageSize", pageSize);
                if (startTime != null)
                {
                    cmd.AddParameter("@startTime", startTime);
                }

                var result = await DbHelper.ExecuteSelectAsync<GameUserPrizeModel>(true, cmd);
                return result?.ToList();
            }
        }

        /// <summary>
        ///     查询奖品 - (查某一个产品、某一天兑换数量
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetGameUserPrizeByPrizeIdDateAsync(bool readOnly, long prizeId, DateTime dateTime)
        {
            var sql = @" SELECT count(*)
                              FROM Activity.[dbo].[tbl_GameUserPrize] with (nolock)
                              where prizeId = @prizeId and CONVERT(NVARCHAR(10),CreateDatetime,120) = @date
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@prizeId", prizeId);
                cmd.AddParameter("@date", dateTime.ToString("yyyy-MM-dd"));
                var result = await DbHelper.ExecuteScalarAsync(readOnly, cmd);
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        ///     新增 - 小游戏 - 新增用户奖品明细
        /// </summary>
        /// <returns></returns>
        public static async Task<long> InsertGameUserPrizeAsync(BaseDbHelper dbHelper = null, GameUserPrizeModel model = null)
        {
            long result = 0;
            var sql = @" insert into  Activity.[dbo].[tbl_GameUserPrize]
                         ([ActivityId]
                           ,[UserId]
                           ,[Point]
                           ,[PrizeId]
                           ,[PrizeName]
                           ,[PrizePicUrl]
                           ,[PrizeTitle]
                           ,[PrizeDesc]
                           ,[PrizeStartTime]
                           ,[PrizeEndTime]
                           ,[CreateDatetime]
                           ,[LastUpdateDateTime]
                           ,[PromotionGetRuleGuid]
                           ,[PromotionCode]
                           ,[PromotionId]
                           ,[IsBroadCastShow]
                           ,TodayRank
                           ,GetPrizeDate
                           ,PrizeType
                        )
                         values (
                                @ActivityId
                               ,@UserId
                               ,@Point
                               ,@PrizeId
                               ,@PrizeName
                               ,@PrizePicUrl
                               ,@PrizeTitle
                               ,@PrizeDesc
                               ,@PrizeStartTime
                               ,@PrizeEndTime
                               ,getdate()
                               ,getdate()
                               ,@PromotionGetRuleGuid
                               ,@PromotionCode
                               ,@PromotionId
                               ,@IsBroadCastShow
                               ,@TodayRank
                               ,@GetPrizeDate
                               ,@PrizeType
                             );
                        SELECT SCOPE_IDENTITY();
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", model.ActivityId);
                cmd.AddParameter("@UserId", model.UserId);
                cmd.AddParameter("@Point", model.Point);
                cmd.AddParameter("@PrizeId", model.PrizeId);
                cmd.AddParameter("@PrizeName", model.PrizeName ?? "");
                cmd.AddParameter("@PrizePicUrl", model.PrizePicUrl ?? "");
                cmd.AddParameter("@PrizeTitle", model.PrizeTitle ?? "");
                cmd.AddParameter("@PrizeDesc", model.PrizeDesc ?? "");
                cmd.AddParameter("@PrizeStartTime", (object)model.PrizeStartTime ?? DBNull.Value);
                cmd.AddParameter("@PrizeEndTime", (object)model.PrizeEndTime ?? DBNull.Value);

                cmd.AddParameter("@PromotionGetRuleGuid", (object)model.PromotionGetRuleGuid ?? DBNull.Value);
                cmd.AddParameter("@PromotionCode", model.PromotionCode ?? "");
                cmd.AddParameter("@PromotionId", (object)model.PromotionId ?? DBNull.Value);
                cmd.AddParameter("@IsBroadCastShow", model.IsBroadCastShow);
                cmd.AddParameter("@TodayRank", model.TodayRank);
                cmd.AddParameter("@GetPrizeDate", model.GetPrizeDate);
                cmd.AddParameter("@PrizeType", model.PrizeType);

                if (dbHelper == null)
                {
                    result = Convert.ToInt64(await DbHelper.ExecuteScalarAsync(cmd));
                }
                else
                {
                    result = Convert.ToInt64(await dbHelper.ExecuteScalarAsync(cmd));
                }

                return result;
            }
        }

        /// <summary>
        ///     删除 - 小游戏 - 用户奖品
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> DeleteGameUserPrizeAsync(BaseDbHelper helper, int activityId, Guid userId)
        {
            var sql = @" delete  Activity.[dbo].[tbl_GameUserPrize]
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
