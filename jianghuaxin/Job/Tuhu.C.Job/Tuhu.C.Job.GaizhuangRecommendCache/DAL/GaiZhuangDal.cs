using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu;
using System.Threading.Tasks;
using Tuhu.Service.GaiZhuang.Model;
using Tuhu.C.Job.GaizhuangRecommendCache.Model;

namespace Tuhu.C.Job.GaizhuangRecommendCache.DAL
{
    public class GaiZhuangDal
    {
        public static IEnumerable<BaseVehicleModel> GetAll5VehicleTypeTiming()
        {
            using (var cmd = new SqlCommand(@"SELECT  VehicleID ,
                            PaiLiang ,
                            MIN(ListedYear) AS StartYear ,
                            ISNULL(MAX(StopProductionYear), YEAR(GETDATE())) AS EndYear,
							TID
                    FROM    Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                    WHERE   ListedYear IS NOT NULL
                            AND VehicleID IS NOT NULL
                            AND PaiLiang IS NOT NULL
							AND TID IS NOT NULL
                    GROUP BY VehicleID ,
                            PaiLiang,
							TID"))
            {
                return DbHelper.ExecuteSelect<BaseVehicleModel>(true, cmd);
            }
        }

        public static IEnumerable<BaseVehicleModel> GetAll4VehiclesTiming()
        {
            using (var cmd = new SqlCommand(@"SELECT  VehicleID ,
                            PaiLiang ,
                            MIN(ListedYear) AS StartYear ,
                            ISNULL(MAX(StopProductionYear), YEAR(GETDATE())) AS EndYear
                    FROM    Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                    WHERE   ListedYear IS NOT NULL
                            AND VehicleID IS NOT NULL
                            AND PaiLiang IS NOT NULL
                    GROUP BY VehicleID ,
                            PaiLiang"))
            {
                return DbHelper.ExecuteSelect<BaseVehicleModel>(true, cmd);
            }
        }

        public static List<VehicleTypeTimingModel> GetAll2VehicleTypeTiming()
        {
            string sql = @"SELECT   DISTINCT VehicleID 
                FROM    Gungnir..tbl_Vehicle_Type_Timing WITH ( NOLOCK )
				WHERE VehicleID IS NOT NULL  ; ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return DbHelper.ExecuteQuery(true, cmd, dt => dt.ToList<string>()).Select(r => new VehicleTypeTimingModel
                {
                    VehicleID = r ?? string.Empty
                }).ToList();
            }
        }

        public static List<string> GetAllGaiZhuangProducts()
        {
            using (var cmd = new SqlCommand(@"
SELECT DISTINCT
        PID
FROM    ( SELECT    PID
          FROM      [Configuration].[dbo].[tbl_GaiZhuangRelateProduct] WITH ( NOLOCK )
          UNION
          SELECT    ProductPID PID
          FROM      Tuhu_productcatalog..tbl_ProductConfig AS A WITH ( NOLOCK )
          WHERE     VehicleLevel = N'无需车型'
        ) AS a;
"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 3 * 60;
                return DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var list = new List<string>();
                    foreach (DataRow one in dt.Rows)
                    {
                        list.Add(one[0]?.ToString());
                    }
                    return list;
                }
                );
            }
        }
    }
}
