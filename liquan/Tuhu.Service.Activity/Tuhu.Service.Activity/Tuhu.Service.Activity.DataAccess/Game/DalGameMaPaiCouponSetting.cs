using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 马牌奖品优惠券设置
    /// </summary>
    public class DalGameMaPaiCouponSetting
    {
        /// <summary>
        ///     查询 马牌奖品优惠券设置
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameMaPaiCouponSettingModel>> GetGameMaPaiCouponSettingAsync(int prizeId)
        {
            var sql = @" SELECT [PKID]
                                  ,[GameMaPaiPrizeSettingPrizeId]
                                  ,[PromotionGetRuleGuid]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Configuration.[dbo].[tbl_GameMaPaiCouponSetting] with (nolock)
                              where GameMaPaiPrizeSettingPrizeId = @prizeId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@prizeId", prizeId);
                var result = await DbHelper.ExecuteSelectAsync<GameMaPaiCouponSettingModel>(true, cmd);
                return result?.OrderBy(p => p.PKID).ToList();
            }
        }


    }
}
