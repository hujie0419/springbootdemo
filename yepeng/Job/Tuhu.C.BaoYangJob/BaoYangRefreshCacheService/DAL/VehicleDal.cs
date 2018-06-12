using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu;
using Tuhu.Service.BaoYang.Models;
using Tuhu.Service.Vehicle.Model;

namespace BaoYangRefreshCacheService.DAL
{
    public static class VehicleDal
    {
        public static IEnumerable<BaseVehicleModel> GetAllVehicles()
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

        public static IEnumerable<VehicleRequestModel> GetAllTidVehicles()
        {
            using (var cmd = new SqlCommand(@"SELECT  distinct VehicleID AS VehicleId ,
                                                    PaiLiang ,
                                                    ListedYear AS Nian,
                                                    TID AS Tid
                                            FROM    Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                                            WHERE   TID IS NOT NULL
                                            Order by TID asc;"))
            {
                return DbHelper.ExecuteSelect<VehicleRequestModel>(true, cmd);
            }
        } 

        public static IEnumerable<string> GetAllTids()
        {
            using (var cmd = new SqlCommand(@"
                SELECT DISTINCT TID FROM Gungnir..tbl_Vehicle_Type_Timing(NOLOCK)
                WHERE TID IS NOT NULL"))
            {
                return DbHelper.ExecuteQuery(true, cmd, (dt) =>
                {
                    List<string> result = new List<string>();

                    if (dt != null && dt.Rows.Count>0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string tid = row.IsNull(0) ? string.Empty : row[0].ToString();
                            result.Add(tid);
                        }
                    }

                    return result;
                });
            }
        } 
    }
}
