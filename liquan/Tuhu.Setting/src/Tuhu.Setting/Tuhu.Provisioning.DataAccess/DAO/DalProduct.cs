using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalProduct
    {
        /// <summary>
        /// 根据类别获取品牌
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static string[] GetProductBrand(SqlConnection conn, string category)
        {
            const string sql = @"
                                 SELECT DISTINCT
       p.CP_Brand
FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
WHERE p.PrimaryParentCategory = @PrimaryParentCategory
      AND p.CP_Brand IS NOT NULL
      AND p.CP_Brand != N'' ;
                                ";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@PrimaryParentCategory",category)
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).AsEnumerable().Select(x => x["CP_Brand"].ToString()).ToArray(); ;
        }
        /// <summary>
        /// 根据类别和品牌获取系列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static string[] GetProductSeries(SqlConnection conn, string category, string brand)
        {
            const string sql = @"   SELECT DISTINCT
       p.CP_ShuXing6
FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
WHERE p.PrimaryParentCategory = @PrimaryParentCategory
      AND p.CP_Brand = @Brand
      AND p.CP_ShuXing6 IS NOT NULL
      AND p.CP_ShuXing6 != N'' ;
                                ";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@PrimaryParentCategory",category),
                 new SqlParameter("@Brand",brand)
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).AsEnumerable().Select(x => x["CP_ShuXing6"].ToString()).ToArray();
        }

        /// <summary>
        /// 获取产品推荐排序
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static IEnumerable<BaoYangProductPriorityModel> GetBaoYangProductSeriesByCategory(SqlConnection conn, string category)
        {
            const string sql = @"SELECT  DISTINCT
                                        p.CP_Brand    AS Brand ,
                                        p.CP_ShuXing6 AS Series
                                FROM    Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
                                WHERE   p.CP_Brand IS NOT NULL
                                        AND p.CP_Brand <> N''
                                        AND p.OnSale = 1
                                        AND p.CP_Brand IS NOT NULL
                                        AND p.PrimaryParentCategory = @category
                                        AND p.IsUsedInAdaptation = 1
                                        AND p.i_ClassType IN ( 2, 4 );";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@category",category)
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).ConvertTo<BaoYangProductPriorityModel>();
        }

        public static IEnumerable<BaoYangProductPriorityModel> GetBaoYangWiperProductSeries(SqlConnection conn, string category, string brakePosition, string type)
        {
            const string sql = @"SELECT  DISTINCT
                                        p.CP_Brand    AS Brand ,
                                        p.CP_ShuXing6 AS Series
                                FROM    Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
                                WHERE   p.CP_Brand IS NOT NULL
                                        AND p.CP_Brand <> N''
                                        AND p.OnSale = 1
                                        AND p.PrimaryParentCategory = @category
                                        AND (
                                            p.CP_Brake_Position = @brakePosition
                                            OR  p.CP_Brake_Position = N''
                                            OR  p.CP_Brake_Position IS NULL
                                        )
                                        AND (
                                            p.CP_ShuXing4 = @type
                                            OR  p.CP_ShuXing4 = N''
                                            OR  p.CP_ShuXing4 IS NULL
                                        )
                                        AND p.IsUsedInAdaptation = 1
                                        AND p.i_ClassType IN ( 2, 4 );";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@category",category),
                 new SqlParameter("@brakePosition",brakePosition?? string.Empty),
                 new SqlParameter("@type",type?? string.Empty)
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).ConvertTo<BaoYangProductPriorityModel>();
        }

        /// <summary>
        /// 获取机油推荐啊排序
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static IEnumerable<OilBaoYangProductPriority> GetOilBaoYangProductSeries(SqlConnection conn)
        {
            const string sql = @"SELECT DISTINCT
                                        p.CP_ShuXing2 AS Viscosity ,
                                        p.CP_ShuXing1 AS Grade ,
                                        p.CP_Brand    AS Brand ,
                                        p.CP_Remark   AS Series
                                FROM    Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
                                WHERE   p.PrimaryParentCategory = N'Oil'
                                        AND p.CP_Brand <> N''
                                        AND p.CP_Brand IS NOT NULL
                                        AND p.IsUsedInAdaptation = 1
                                        AND p.OnSale = 1
                                        AND p.CP_ShuXing2 IS NOT NULL
                                        AND p.CP_ShuXing2 <> N''
                                        AND p.i_ClassType IN ( 2, 4 );";

            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql).ConvertTo<OilBaoYangProductPriority>();
        }

        /// <summary>
        /// 根据类别和品牌获取机油系列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static string[] GetOilProductSeries(SqlConnection conn, string category, string brand)
        {
            const string sql = @"   SELECT DISTINCT
       p.CP_Remark
FROM Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS p WITH (NOLOCK)
WHERE p.PrimaryParentCategory = @PrimaryParentCategory
      AND p.CP_Brand = @Brand
      AND p.CP_Remark IS NOT NULL
      AND p.CP_Remark != N'';
                                ";
            SqlParameter[] parameter = new SqlParameter[] {
                 new SqlParameter("@PrimaryParentCategory",category),
                 new SqlParameter("@Brand",brand)
            };
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).AsEnumerable().Select(x => x["Cp_remark"].ToString()).ToArray(); ;
        }
    }
}
