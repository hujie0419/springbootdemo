using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;

namespace Tuhu.Provisioning.DataAccess
{
    public static class DalVehicleType
    {
        private const string ProcSelectAllBrands = "VehicleType_SelectAllBrands";

        private const string ProcSelectVehicleSeriesByBrand = "VehicleType_SelectVehicleSeriesByBrand";

        public static List<string> SelectAllVehicleBrands(SqlConnection connection)
        {
            var result = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, ProcSelectAllBrands))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        public static List<string> SelectAllBrandCategories(SqlConnection connection)
        {
            var result = new List<string>();

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text,
                @"SELECT DISTINCT BrandCategory
                FROM Gungnir..tbl_Vehicle_Type WITH ( NOLOCK )
                WHERE BrandCategory IS NOT NULL"))
            {
                while (reader.Read())
                {
                    result.Add(reader.IsDBNull(0) ? string.Empty : reader.GetString(0));
                }
            }

            return result;
        }

        public static IDictionary<string, string> SelectVehicleSeries(SqlConnection connection, string brand)
        {
            var result = new Dictionary<string, string>();

            var parameters = new[]
            {
                new SqlParameter("@Brand", brand)
            };

            using (var reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, ProcSelectVehicleSeriesByBrand, parameters))
            {
                while (reader.Read())
                {

                    var key = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                    var value = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, value);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取车型排量信息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="vid"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetVehiclePaiLiang(SqlConnection conn, string vid)
        {
            var result = new List<string>();
            var sql = @"SELECT  PaiLiang
                        FROM    Gungnir.dbo.tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                        WHERE   VehicleID = @VehicleID
                        GROUP BY PaiLiang
                        ORDER BY PaiLiang;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleID", vid)
            };

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = dt.Rows.OfType<DataRow>().Select(row => row["PaiLiang"].ToString()).ToList();
            return result;
        }

        /// <summary>
        /// 获取年产信息
        /// 开始生产-停止生产年份
        /// 如果停产年份取当前年份
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetVehicleNian(SqlConnection connection, string vid, string paiLiang)
        {
            var sql = @"SELECT  CAST(MIN(ListedYear) AS INT) AS ListedYear ,
                                        MAX(CAST(ISNULL(StopProductionYear,
                                                        YEAR(GETDATE())) AS INT)) AS StopProductionYear
                                FROM    Gungnir.dbo.tbl_Vehicle_Type_Timing
                                        WITH ( NOLOCK )
                                WHERE   VehicleID = @VehicleID
                                        AND PaiLiang = @PaiLiang;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleID", vid),
                new SqlParameter("@PaiLiang", paiLiang)
            };

            var dt = SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters);
            var result = dt.Rows.OfType<DataRow>().Select(row =>
                Tuple.Create(Convert.ToInt32(row["ListedYear"]), Convert.ToInt32(row["StopProductionYear"]))).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取款型名称
        /// TID,SalesName
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <param name="nian"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetVehicleSalesName(SqlConnection conn, string vid, string paiLiang, int nian)
        {
            var result = new Dictionary<string, string>();
            var sql = @"SELECT  Nian + N'款 ' + SalesName AS SalesName ,
                                TID
                        FROM    Gungnir.dbo.tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                        WHERE   VehicleID = @VehicleID
                                AND PaiLiang = @PaiLiang
                                AND ( @Nian BETWEEN ListedYear
                                            AND     ISNULL(StopProductionYear, YEAR(GETDATE())) )
                                AND TID IS NOT NULL
                        ORDER BY SalesName;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleID", vid),
                new SqlParameter("@PaiLiang", paiLiang),
                new SqlParameter("@Nian", nian)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = dt.Rows.OfType<DataRow>().ToDictionary(row => row["TID"].ToString(), row => row["SalesName"].ToString());
            return result;
        }

        /// <summary>
        /// 获取Tid
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vid"></param>
        /// <param name="paiLiang"></param>
        /// <param name="nian"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetTids(SqlConnection conn, string vid, string paiLiang, string nian)
        {
            var result = new List<string>();
            var sql = @"SELECT  DISTINCT
                                TID
                        FROM    Gungnir.dbo.tbl_Vehicle_Type_Timing WITH ( NOLOCK )
                        WHERE   VehicleID = @VehicleID
                                AND PaiLiang = @PaiLiang
                                AND ( @Nian = ''
                                      OR @Nian IS NULL
                                      OR @Nian BETWEEN ListedYear
                                               AND     ISNULL(StopProductionYear, YEAR(GETDATE()))
                                    )
                                AND TID IS NOT NULL
                        ORDER BY TID;";
            var parameters = new[]
            {
                new SqlParameter("@VehicleID", vid),
                new SqlParameter("@PaiLiang", paiLiang),
                new SqlParameter("@Nian", nian)
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            result = dt.Rows.OfType<DataRow>().Select(row => row["TID"].ToString()).ToList();
            return result;
        }
    }
}
