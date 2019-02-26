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
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;
using Tuhu.Provisioning.DataAccess.Request;

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

        /// <summary>
        /// 获取车型机油保养推荐排序
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<VehicleOilProductPriorityView> GetVehicleByProductPriorityRequest(SqlConnection conn, OilVehicleProductPriorityRequst request)
        {
            #region sql
            var sql = @"SELECT  DISTINCT
                                v.ProductID AS VehicleId ,
                                v.Brand ,
                                v.Vehicle ,
                                p.Viscosity ,
                                p.Grade
                        FROM    Gungnir..tbl_Vehicle_Type                  AS v WITH (NOLOCK)
                                LEFT JOIN Gungnir..tbl_Vehicle_Type_Timing AS t WITH (NOLOCK)
                                    ON v.ProductID = t.VehicleID
                                LEFT JOIN BaoYang..tbl_PartAccessory       AS p WITH (NOLOCK)
                                    ON p.TID = t.TID COLLATE Chinese_PRC_CI_AS
                        WHERE   p.AccessoryName = N'发动机油'
                                AND p.IsDeleted = 0
                                AND (
                                    @Brand = N''
                                    OR  @Brand IS NULL
                                    OR  v.Brand = @Brand
                                )
                                AND (
                                    @VehicleId IS NULL
                                    OR  v.ProductID = @VehicleId
                                    OR  @VehicleId = N''
                                )
                                AND (
                                    @MaxPrice = 0
                                    OR  v.AvgPrice <= @MaxPrice
                                )
                                AND (
                                    @MinPrice = 0
                                    OR  v.AvgPrice > @MinPrice
                                )
                                AND (
                                    @VehicleBodyType = N''
                                    OR  @VehicleBodyType IS NULL
                                    OR  v.VehicleBodyType = @VehicleBodyType
                                )
                                AND (
                                    @Viscosity = N''
                                    OR  @Viscosity IS NULL
                                    OR  p.Viscosity = @Viscosity
                                )
                                AND (
                                    @Grade = N''
                                    OR  @Grade IS NULL
                                    OR  p.Grade = @Grade
                                );";
            #endregion
            var parameters = new[]  {
                new SqlParameter("@Brand", request.Brand),
                new SqlParameter("@MaxPrice", request.MaxPrice),
                new SqlParameter("@MinPrice", request.MinPrice),
                new SqlParameter("@VehicleBodyType", request.VehicleBodyType),
                new SqlParameter("@Viscosity", request.Viscosity),
                 new SqlParameter("@Grade", request.Grade),
                new SqlParameter("@VehicleId", request.VehicleId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleOilProductPriorityView>().ToList();
        }

        /// <summary>
        /// 获取车型
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetVehicleTypeBody(SqlConnection conn)
        {
            var sql = @"SELECT      DISTINCT
                                    VehicleBodyType COLLATE Chinese_PRC_CI_AS AS VehicleBodyType
                        FROM        Gungnir..tbl_Vehicle_Type WITH (NOLOCK)
                        WHERE       VehicleBodyType IS NOT NULL
                                    AND VehicleBodyType <> N''
                        ORDER BY    VehicleBodyType COLLATE Chinese_PRC_CI_AS;";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).AsEnumerable().Select(x => x["VehicleBodyType"].ToString()).ToList();
        }

        /// <summary>
        /// 获取车型保养推荐排序
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static List<VehicleProductPriorityView> GetVehicleProductPriorityView(SqlConnection conn, VehicleProductPriorityRequest request)
        {
            #region sql

            string sql = @"SELECT  v.ProductID AS VehicleId ,
                                    v.Brand ,
                                    v.Vehicle
                            FROM    Gungnir..tbl_Vehicle_Type AS v WITH (NOLOCK)
                            WHERE   (
                                @Brand = N''
                                OR  @Brand IS NULL
                                OR  v.Brand = @Brand
                            )
                                    AND (
                                        @VehicleId = N''
                                        OR  @VehicleId IS NULL
                                        OR  v.ProductID = @VehicleId
                                    )
                                    AND (
                                        @MaxPrice = 0
                                        OR  v.AvgPrice <= @MaxPrice
                                    )
                                    AND (
                                        @MinPrice = 0
                                        OR  v.AvgPrice > @MinPrice
                                    )
                                    AND (
                                        @VehicleBodyType = N''
                                        OR  @VehicleBodyType IS NULL
                                        OR  v.VehicleBodyType = @VehicleBodyType
                                    );";

            #endregion
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@Brand",request.Brand) ,
                 new SqlParameter("@MaxPrice",request.MaxPrice) ,
                 new SqlParameter("@MinPrice",request.MinPrice) ,
                 new SqlParameter("@VehicleBodyType",request.VehicleBodyType) ,
                 new SqlParameter("@VehicleId",request.VehicleId)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter).ConvertTo<VehicleProductPriorityView>().ToList();
        }

        public static List<VehicleTypeModel> GetVehicleTypeByVehicleIds(SqlConnection conn, IEnumerable<string> vehicleIds)
        {
            #region sql
            var sql = @"SELECT      v.ProductID ,
                                    v.AvgPrice ,
                                    v.VehicleBodyType
                        FROM        Gungnir..tbl_Vehicle_Type                       AS v WITH (NOLOCK)
                                    INNER JOIN Gungnir..SplitString(@VehicleIds, ',', 1) AS vids
                                        ON v.ProductID = vids.Item COLLATE Chinese_PRC_CI_AS
                        ORDER BY    v.Brand ASC;";
            #endregion
            var parameters = new[]  {
                new SqlParameter("@VehicleIds", string.Join(",",vehicleIds))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<VehicleTypeModel>().ToList();
        }
    }
}
