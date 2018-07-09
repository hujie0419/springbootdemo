using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Tire.Model;

namespace Tuhu.C.Job.Tire.DAL
{
    public class DalTireStockoutStatusWhile
    {
        public static IEnumerable<string> SelectTiresMatchAndSaleQuantityMoreThanEight()
        {
            var matchPids = SelectTiresMatch();
            var salePids = SelectSaleQuantityMoreThanEight();
            var pids = matchPids.Union(salePids).Distinct();
            return pids;
        }

        public static IEnumerable<string> SelectSaleQuantityMoreThanEight()
        {
            using (var dbhelper = DbHelper.CreateLogDbHelper())
            {
                List<string> list = new List<string>();
                var cmd = new SqlCommand(@"SELECT  SD.pid
                                           FROM    Tuhu_bi.dbo.dm_Product_SalespredictData AS SD WITH ( NOLOCK )
                                           WHERE   SD.pid LIKE 'TR-%' AND SD.num_month>=8");
                return dbhelper.ExecuteQuery(cmd, dt =>
                {
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow row in dt.Rows)
                            list.Add(row[0].ToString());
                    return list;
                });
            }
        }

        public static int JoinWhiteList(BaseDbHelper dbhelper, IEnumerable<string> pids)
        {
            return dbhelper.ExecuteNonQuery(@"INSERT  INTO Tuhu_bi.dbo.TireStockWhiteList
                                              ( PID ,
                                                Type
                                              )
                                              SELECT  * ,
                                                      1
                                              FROM    Tuhu_bi.dbo.Split(@pids, ';')", CommandType.Text, new SqlParameter("@pids", string.Join(";", pids)));
        }

        public static int DeleteExsit(BaseDbHelper dbhelper, IEnumerable<string> exsitPids)
        {
            return dbhelper.ExecuteNonQuery(@"DELETE  Tuhu_bi.dbo.TireStockWhiteList
WHERE   PID IN ( SELECT *
                 FROM   Tuhu_bi.dbo.Split(@pids, ';') )", CommandType.Text, new SqlParameter("@pids", string.Join(";", exsitPids)));
        }

        public static IEnumerable<string> SelectExsitWhiteList(BaseDbHelper dbhelper, IEnumerable<string> pids)
        {
            List<string> list = new List<string>();
            using (var cmd = new SqlCommand(@"SELECT  PID
                                              FROM    Tuhu_bi.dbo.TireStockWhiteList AS TWL WITH ( NOLOCK )
                                              WHERE   TWL.PID IN ( SELECT *
                                                                   FROM   Tuhu_bi.dbo.Split(@pids, ';') )"))
            {
                cmd.Parameters.AddWithValue("@pids", string.Join(";", pids));
                return dbhelper.ExecuteQuery(cmd, dt =>
                {
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow row in dt.Rows)
                            list.Add(row[0].ToString());
                    return list;
                });
            }

        }

        public static IEnumerable<string> SelectTiresMatch()
        {
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                List<string> list = new List<string>();
                var cmd = new SqlCommand(@"SELECT  SS.Item
                                           FROM    Gungnir.dbo.tbl_Vehicle_Type AS TV WITH ( NOLOCK )
                                                   CROSS APPLY Gungnir..SplitString(TV.TiresMatch, ';', 1) AS SS;");
                return dbhelper.ExecuteQuery(cmd, dt =>
                {
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow row in dt.Rows)
                            list.Add(row[0].ToString());
                    return list;
                });
            }
        }

        public static List<TireStockProductModel> GetBaseProductInfo(List<string> pids)
        {
            const string sqlStr = @"
WITH    PIDS
          AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
               FROM     Tuhu_productcatalog..SplitString(@pids, ',', 1)
             )
    SELECT  VP.PID ,
            VP.DisplayName ,
            VP.CP_Brand AS Brand ,
            VP.OnSale ,
            VP.stockout AS StockoutStatus
    FROM    PIDS WITH ( NOLOCK )
            LEFT JOIN Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK ) ON PIDS.PID = VP.PID;";
            using (var cmd = new SqlCommand(sqlStr))
            {
                cmd.Parameters.AddWithValue("@pids", string.Join(",", pids));
                return DbHelper.ExecuteSelect<TireStockProductModel>(true, cmd)?.ToList() ?? new List<TireStockProductModel>();
            }
        }

        public static List<TireStockProductModel> GetStockoutStatus(List<string> pids)
        {
            const string sqlStr = @"WITH    PIDS
          AS ( SELECT   Item COLLATE Chinese_PRC_CI_AS AS PID
               FROM     Tuhu_bi..SplitString(@pids, ',', 1)
             )
    SELECT  PIDS.PID ,
            DPSD.totalstock AS CurrentStockCount ,
            DPSD.num_month AS MonthSales ,
            ( CASE WHEN TS.Stock > 0 THEN 0
                   WHEN TS.PID IS NULL THEN 0
                   ELSE 1
              END ) AS SystemStockout
    FROM    PIDS WITH ( NOLOCK )
            LEFT JOIN Tuhu_bi.dbo.dm_Product_SalespredictData AS DPSD WITH ( NOLOCK ) ON PIDS.PID = DPSD.PID
            LEFT JOIN Tuhu_bi..tbl_TireStock AS TS WITH ( NOLOCK ) ON PIDS.PID = TS.PID
                                                              AND TS.CityId = 1; ";
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    cmd.Parameters.AddWithValue("@pids", string.Join(",", pids));
                    return dbHelper.ExecuteSelect<TireStockProductModel>(cmd)?.ToList() ??
                           new List<TireStockProductModel>();
                }
            }
        }

        public static int TruncateTableInfo()
        {
            const string sqlStr = @"delete from Tuhu_bi..TireStockWhiteList where pkid > 0;";
            using (var dbHelper = DbHelper.CreateLogDbHelper())
            {
                using (var cmd = new SqlCommand(sqlStr))
                {
                    return dbHelper.ExecuteNonQuery(cmd);
                }
            }
        }
    }
}
