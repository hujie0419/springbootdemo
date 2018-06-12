using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Sitemap.Models;

namespace Tuhu.C.Job.Sitemap.DAL
{
    class BaseDataDAL
    {
        public static IEnumerable<int> GetArticleID(bool isFull = false)
        {
            string sqltext = string.Empty;
            List<int> articleIDList = new List<int>();
            if (isFull)
            {
                sqltext = BaseDataSqlText.SqlTextAllArticleIDs;
            }
            else
            {
                sqltext = BaseDataSqlText.sqlTextPartialArticleIDs;
            }

            using (
               var dbhelper =
                   DbHelper.CreateDbHelper(
                       ConfigurationManager.ConnectionStrings["Marketing"].ConnectionString))
            {
                using (var cmd = new SqlCommand(sqltext) { CommandTimeout = 10 * 60 })
                {
                    return dbhelper.ExecuteQuery(cmd, dt => dt.ToList<int>("PKID"));
                }
            }
        }

        public static IEnumerable<Product> GetProducts(bool isFull = false)
        {
            string sqltext = string.Empty;
            List<int> articleIDList = new List<int>();
            if (isFull)
            {
                sqltext = BaseDataSqlText.sqlTextProductIDs;
            }
            else
            {
                sqltext = BaseDataSqlText.sqlTextPartialProductIDs;
            }
            using (
              var dbhelper =
                  DbHelper.CreateDbHelper())
            {
                using (var cmd = new SqlCommand(sqltext) { CommandTimeout = 10 * 60 })
                {
                    #region AddParameters

                    //  cmd.Parameters.AddWithValue("@VehicleId", vehicleId)

                    #endregion

                    return dbhelper.ExecuteSelect<Product>(cmd);
                }
            }
        }

        public static IEnumerable<VehicleInfo> GetVehicleInfo(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.SqlTextVehicles;
            }
            else
            {
                sql = BaseDataSqlText.SqlTextPartialVehicles;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<VehicleInfo>(cmd);
                }
            }
        }

        public static IEnumerable<ShopInfo> GetShopListInfo(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.SqlTextShopList;
            }
            else
            {
                sql = BaseDataSqlText.SqlTextPartialShopList;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<ShopInfo>(cmd);
                }
            }
        }

        public static IEnumerable<ShopInfo> GetShopDetailInfo(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.SqlTextShops;
            }
            else
            {
                sql = BaseDataSqlText.SqlTextPartialShops;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<ShopInfo>(cmd);
                }
            }
        }
        public static IEnumerable<VehicleInfo> GetHotVehicles(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.SqlTextHotVehicles;
            }
            else
            {
                sql = BaseDataSqlText.SqlTextPartialHotVehicles;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<VehicleInfo>(cmd);
                }
            }
        }


        public static IEnumerable<UrlNode> GetUrls(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.sqlTextAllUrl;
            }
            else
            {
                sql = BaseDataSqlText.sqlTextPartialUrl;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<UrlNode>(cmd);
                }
            }
        }

        public static int GetTireCount(string vehicleId, string tiresize, bool isFull)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = string.Format(@"SELECT  COUNT(*)
                FROM    Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK )
                WHERE   VP.TireSize COLLATE Chinese_PRC_CI_AS IN (
                        SELECT DISTINCT
                                SS.Item
                        FROM    Gungnir.dbo.tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.Tires, ';',
                                                              1) AS SS
                        WHERE   V.ProductID = '{0}' ) AND VP.TireSize='{1}' AND VP.IsShow=1 AND VP.OnSale=1", vehicleId, tiresize);
            }
            else
            {
                sql = string.Format(@"SELECT  COUNT(*)
                FROM    Tuhu_productcatalog..vw_Products AS VP WITH ( NOLOCK )
                WHERE   VP.TireSize COLLATE Chinese_PRC_CI_AS IN (
                        SELECT DISTINCT
                                SS.Item
                        FROM    Gungnir.dbo.tbl_Vehicle_Type AS V WITH ( NOLOCK )
                                CROSS APPLY Gungnir..SplitString(V.Tires, ';',
                                                              1) AS SS
                        WHERE   V.ProductID = '{0}' ) AND VP.TireSize='{1}' AND VP.IsShow=1 AND VP.OnSale=1 AND DATEDIFF(dd,VP.CreateDateTime,GETDATE())<=6", vehicleId, tiresize);
            }

            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return (int)dbhelper.ExecuteScalar(cmd);
                }
            }
        }

        public static IEnumerable<TireInfo> GetTireInfo(bool isFull = false)
        {
            string sql = string.Empty;
            if (isFull)
            {
                sql = BaseDataSqlText.SqlTextTireBrand;
            }
            else
            {
                sql = BaseDataSqlText.SqlTextPartialTireBrand;
            }
            using (var dbhelper = DbHelper.CreateDbHelper())
            {
                using (
                     var cmd =
                         new SqlCommand(sql))
                {
                    return dbhelper.ExecuteSelect<TireInfo>(cmd);
                }
            }
        }

        public static void InsertURL(string url, string type)
        {
            if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(type))
            {
                using (var dbhelper =DbHelper.CreateDbHelper())
                {
                    using (
                         var cmd =
                             new SqlCommand(BaseDataSqlText.sqlTextInsertUrl))
                    {
                        DateTime time = DateTime.Now;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@URL", url);
                        cmd.Parameters.AddWithValue("@Type", type);
                        cmd.Parameters.AddWithValue("@DataCreate_Time", time);
                        cmd.Parameters.AddWithValue("@DataUpdate_Time", time);
                        dbhelper.ExecuteNonQuery(cmd);
                    }
                }
            }
        }
    }
}
