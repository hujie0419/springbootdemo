using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.DailyShopSyncJob.Models;

namespace Tuhu.C.Job.DailyShopSyncJob.DAL
{
   public class ShopChangeDAL
    {
        public static List<ShopChange> GetYesterdayShopChangeList()
        {

            List<ShopChange> shopChangeList = new List<ShopChange>(); 
            using (
            var dbhelper =
                DbHelper.CreateDbHelper(ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString))
            {
                using (var cmd = new SqlCommand("select ShopId,ShopService,ShopHoliday,ShopIsInActive,ShopDetail,CreateDateTime from  [Tuhu_thirdparty].[dbo].[ShopChangeLog] where datediff(dd,CreateDateTime,getdate())=1"))
                {
                    cmd.CommandType = CommandType.Text;
                    // cmd.Parameters.AddWithValue("@ModelID", modelID);
                    shopChangeList = dbhelper.ExecuteQuery(cmd, ConvertSpecificType);

                }

            }
            return shopChangeList;
        }


        public static List<ShopChange> ConvertSpecificType(DataTable dt)
        {
            
            List<ShopChange> shopChangelist = new List<ShopChange>();
            try
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var datarow = dt.Rows[i];
                        var shopid = Convert.ToInt32(datarow["ShopId"]);
                        var shopservice = datarow["ShopService"] != DBNull.Value ? true : false;
                        var shopholiday = datarow["ShopHoliday"] != DBNull.Value ? true : false;
                        var shopIsInActive = datarow["ShopIsInActive"] != DBNull.Value ? true : false;
                        var shopDetail = datarow["ShopDetail"] != DBNull.Value ? true : false;
                        var createdatetime = datarow["CreateDateTime"] != DBNull.Value ? Convert.ToDateTime(datarow["CreateDateTime"]) : DateTime.Now.AddDays(-1);
                        var shopchange = new ShopChange()
                        {
                            ShopId = shopid,
                            ShopService = shopservice,
                            ShopHoliday = shopholiday,
                            ShopIsInActive = shopIsInActive,
                            ShopDetail = shopDetail,
                            CreateDateTime= createdatetime
                        };
                        shopChangelist.Add(shopchange);
                    }

                }
            }
            catch (Exception ex)
            {
                TuhuDailyShopSyncJob.Logger.Error("ConvertToShopChange Error", ex);
            }
            return shopChangelist;
        }


        public static bool CheckSwitch(string runtimeSwitch)
        {
            var open = false;

            try
            {
                using (var command = new SqlCommand(@"SELECT [Value] FROM Gungnir.dbo.RuntimeSwitch (NOLOCK) WHERE SwitchName = @SwitchName"))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@SwitchName", runtimeSwitch));
                    open = (bool)DbHelper.ExecuteScalar(command);
                }
            }
            catch (Exception ex)
            {
                TuhuDailyShopSyncJob.Logger.Error(ex.Message, ex);
            }

            return open;
        }

    }
}
