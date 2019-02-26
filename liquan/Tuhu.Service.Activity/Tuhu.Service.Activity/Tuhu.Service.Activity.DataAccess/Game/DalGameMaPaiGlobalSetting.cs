using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 马牌小游戏全局设置
    /// </summary>
    public class DalGameMaPaiGlobalSetting
    {
        /// <summary>
        ///     查询马牌全局设置
        /// </summary>
        /// <returns></returns>
        public static async Task<GameMaPaiGlobalSettingModel> GetGameMaPaiGlobalSettingAsync(int activityId)
        {
            var sql = @" select    [PKID]
                                  ,[ActivityId]
                                  ,[ShareDistance]
                                  ,[SupportDistance]
                                  ,[DailySupportedUserMax]
                                  ,[DailySupportOtherUserMax]
                                  ,[UserSupportMax]
                                  ,PayAmountPointRate
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                                  ,RankMinPoint
                              FROM Configuration.[dbo].[tbl_GameMaPaiGlobalSetting] with (nolock)
                              where ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteFetchAsync<GameMaPaiGlobalSettingModel>(true, cmd);
                return result;
            }
        }
    }
}
