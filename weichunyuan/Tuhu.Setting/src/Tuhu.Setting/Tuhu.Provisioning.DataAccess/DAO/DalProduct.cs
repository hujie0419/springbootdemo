using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return SqlHelper.ExecuteDataTable2(conn, CommandType.Text, sql, parameter).AsEnumerable().Select(x => x["CP_ShuXing6"].ToString()).ToArray(); ;
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
