using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 奖品配置
    /// </summary>
    public class DalGamePrizeSetting
    {
        /// <summary>
        ///     获取游戏奖品信息 条件:活动id,活动的奖品类型
        /// </summary>
        /// <returns></returns>
        public static async Task<GamePrizeSettingModel> GetGamePrizeSettingAsync(int activityId, string prizeType,int thisDay)
        {
            //非保养的券分时间
            string sqlCondition = "";
            if (prizeType == GameCommons.PrizeTypeCarModel|| prizeType == GameCommons.PrizeTypeEngineOil)
            {
                sqlCondition += "AND PrizeGiveDay = @PrizeGiveDay";
            }

            var sql = string.Format(@"SELECT [PKID],
                               [ActivityId],
                               [PrizeId],
                               [PrizeType],
                               [PrizeName],
                               [PrizePicUrl],
                               [PrizeCount],
                               [GiveCount],
                               [DayCount],
                               [CreateDatetime],
                               [LastUpdateDateTime],
                               [IsBroadCastShow],
                               BroadCastTitle,
                               PrizeDesc
                        FROM [Activity].[dbo].[tbl_GamePrizeSetting] WITH (NOLOCK)
                        WHERE ActivityId = @ActivityId
                              AND IsDeleted = 0 {0}
                              AND PrizeType = @PrizeType; ", sqlCondition);

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                cmd.AddParameter("@PrizeType", prizeType);
                cmd.AddParameter("@PrizeGiveDay", thisDay);
                var result = await DbHelper.ExecuteFetchAsync<GamePrizeSettingModel>(true, cmd);
                return result;
            }
        }
    }
}
