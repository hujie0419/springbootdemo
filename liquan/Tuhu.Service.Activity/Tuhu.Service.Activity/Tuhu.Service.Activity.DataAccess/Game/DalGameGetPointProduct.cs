using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess.Models.Game;

namespace Tuhu.Service.Activity.DataAccess.Game
{
    /// <summary>
    /// [Activity].[dbo].tbl_GameGetPointProduct 购买可获得积分的pidc 
    /// </summary>
    public class DalGameGetPointProduct
    {

        /// <summary>
        /// 获取购买可获得积分的商品
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static async Task<List<GameGetPointProductModel>> SelectGetPointProductList(int activityId, bool readOnly)
        {
            string sql = @"Select Pid
                            FROM [Activity].[dbo].tbl_GameGetPointProduct WITH (NOLOCK)
                            WHERE ActivityId = @ActivityId
                                  AND IsDelete = 0;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.AddParameter("@ActivityId", activityId);
                var result = await DbHelper.ExecuteSelectAsync<GameGetPointProductModel>(readOnly, cmd);
                return result?.ToList();
            }
        }
    }
}
