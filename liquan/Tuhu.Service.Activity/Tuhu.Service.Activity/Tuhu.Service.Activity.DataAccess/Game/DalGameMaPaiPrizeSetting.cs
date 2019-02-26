using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 马牌奖品设置
    /// </summary>
    public class DalGameMaPaiPrizeSetting
    {
        /// <summary>
        ///     查询马牌奖品设置
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameMaPaiPrizeSettingModel>> GetGameMaPaiPrizeSettingAsync(bool readOnly,
            int activityId)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[MaPaiMilepostSettingId]
                                  ,[Point]
                                  ,[PrizeName]
                                  ,[PrizePicUrl]
                                  ,[Count]
                                  ,[LCount]
                                  ,[DayCount]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[IsBroadCastShow]
                              FROM Configuration.[dbo].[tbl_GameMaPaiPrizeSetting] with (nolock)
                              where ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<GameMaPaiPrizeSettingModel>(readOnly, cmd);
                return result?.OrderBy(p => p.PKID).ToList();
            }
        }


        /// <summary>
        ///     查询马牌奖品设置 - 通过主键
        /// </summary>
        /// <returns></returns>
        public static async Task<GameMaPaiPrizeSettingModel> GetGameMaPaiPrizeSettingByPkidAsync(int pkid)
        {
            var sql = @" SELECT [PKID]
                                  ,[ActivityId]
                                  ,[MaPaiMilepostSettingId]
                                  ,[Point]
                                  ,[PrizeName]
                                  ,[PrizePicUrl]
                                  ,[Count]
                                  ,[LCount]
                                  ,[DayCount]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,[IsBroadCastShow]
                              FROM Configuration.[dbo].[tbl_GameMaPaiPrizeSetting] with (nolock)
                              where PKID = @PKID
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", pkid);
                var result = await DbHelper.ExecuteFetchAsync<GameMaPaiPrizeSettingModel>(true, cmd);
                return result;
            }
        }


        /// <summary>
        ///     更新 - 小游戏 - 马牌奖品库存
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateGameMaPaiPrizeSettingCountAsync(BaseDbHelper helper, long pkid,
            int count)
        {
            var sql = @" update Configuration.[dbo].[tbl_GameMaPaiPrizeSetting]
                         set
                                  [LCount] =  LCount + @LCount
                                  ,LastUpdateDateTime = getdate()
                        where pkid = @pkid  and ((LCount + @LCount) >= 0 or Count = -1)
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@pkid", pkid);
                cmd.AddParameter("@LCount", count);

                var result = await helper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
