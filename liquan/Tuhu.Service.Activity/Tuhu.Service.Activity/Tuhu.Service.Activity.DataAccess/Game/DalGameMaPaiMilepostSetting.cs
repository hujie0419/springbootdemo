using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    ///     小游戏 - 马牌里程设置
    /// </summary>
    public static class DalGameMaPaiMilepostSetting
    {
        /// <summary>
        ///     查询马牌里程设置
        /// </summary>
        /// <returns></returns>
        public static async Task<IList<GameMaPaiMilepostSettingModel>> GetGameMaPaiMilepostSettingAsync(int activityId)
        {
            var sql = @" select    [PKID]
                                  ,[ActivityId]
                                  ,[MilepostName]
                                  ,[MilepostPicUrl]
                                  ,[Distance]
                                  ,[Sort]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Configuration.[dbo].[tbl_GameMaPaiMilepostSetting] with (nolock)
                              where ActivityId = @ActivityId
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<GameMaPaiMilepostSettingModel>(true, cmd);
                return result?.OrderBy(p => p.Sort).ToList();
            }
        }

        /// <summary>
        ///     查询马牌里程设置 By Id
        /// </summary>
        /// <returns></returns>
        public static async Task<GameMaPaiMilepostSettingModel> GetGameMaPaiMilepostSettingByIdAsync(long pkid)
        {
            var sql = @" select    [PKID]
                                  ,[ActivityId]
                                  ,[MilepostName]
                                  ,[MilepostPicUrl]
                                  ,[Distance]
                                  ,[Sort]
                                  ,[CreateDatetime]
                                  ,[LastUpdateDateTime]
                              FROM Configuration.[dbo].[tbl_GameMaPaiMilepostSetting] with (nolock)
                              where PKID = @PKID
            ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@PKID", pkid);
                var result = await DbHelper.ExecuteFetchAsync<GameMaPaiMilepostSettingModel>(true, cmd);
                return result;
            }
        }
    }
}
