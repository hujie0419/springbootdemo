using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.DragonBall;

namespace Tuhu.Service.Activity.DataAccess.DragonBall
{
    /// <summary>
    ///     七龙珠 - 设置
    /// </summary>
    public class DalDragonBallSetting
    {

        /// <summary>
        ///     查询七龙珠设置
        /// </summary>
        /// <returns></returns>
        public static async Task<DragonBallSettingModel> GetDragonBallSettingAsync()
        {
            var sql = @" select    [PKID]
                                  ,[MissionBigBrand]
                                  ,[SummonBigBrand]
                                  ,[Pids]
                        from [Configuration].[dbo].tbl_DragonBallSetting with (nolock) ";

            using (var cmd = new SqlCommand(sql))
            {
                var result = await DbHelper.ExecuteFetchAsync<DragonBallSettingModel>(true, cmd);
                return result;
            }
        }


        /// <summary>
        ///     更新七龙珠设置
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> UpdateDragonBallSettingAsync(DragonBallSettingModel model)
        {
            var sql = @" update [Configuration].[dbo].tbl_DragonBallSetting
                         set 
                                  [MissionBigBrand] = @MissionBigBrand
                                  ,[SummonBigBrand] = @SummonBigBrand
                                  ,[Pids] = @Pids
                          ";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@MissionBigBrand", model.MissionBigBrand ?? "");
                cmd.AddParameter("@SummonBigBrand", model.SummonBigBrand ?? "");
                cmd.AddParameter("@Pids", model.Pids ?? "");


                var result = await DbHelper.ExecuteNonQueryAsync(cmd);
                return result > 0;
            }
        }
    }
}
