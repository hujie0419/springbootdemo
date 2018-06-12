using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu;

namespace BaoYangRefreshCacheService.DAL
{
    public static class ProductDal
    {
        public static int GetProductEsCount()
        {
            var cntSql = @"SELECT  COUNT(1)
        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS cp WITH(NOLOCK)
                JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH(NOLOCK) ON cp.oid = ch.child_oid
        WHERE   ( ch.NodeNo = N'55450'
                  OR ch.NodeNo LIKE N'55450.%'
                )
        AND cp.i_ClassType IN ( 2, 4 )";

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                return (int)(DbHelper.ExecuteScalar(true, cmd) ?? "0");
            }
        }

        public static List<string> GetProductEsPids(int pageSize, int pageNum)
        {
            var cntSql = @"SELECT  DISTINCT cp.PID
        FROM    Tuhu_productcatalog..[CarPAR_zh-CN] AS cp WITH(NOLOCK)
                JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH(NOLOCK) ON cp.oid = ch.child_oid
        WHERE   ( ch.NodeNo = N'55450'
                  OR ch.NodeNo LIKE N'55450.%'
                )
                AND cp.i_ClassType IN ( 2, 4 )
        ORDER BY cp.PID
        OFFSET ( @PageNumber - 1 ) * @PageSize ROW
				FETCH NEXT @PageSize ROW ONLY;";
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cntSql;
                cmd.Parameters.AddWithValue("@PageNumber", pageNum);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return (DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<string>();
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow one in dt.Rows)
                            result.Add(one[0]?.ToString());
                    return result;
                }));
            }
        }

        public static List<string> GetAllBaoYangPids()
        {
            const string sql = @"SELECT DISTINCT cp.PID
                                FROM Tuhu_productcatalog..[CarPAR_zh-CN] ( NOLOCK ) cp
                                JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy ( NOLOCK ) ch ON cp.oid = ch.child_oid
                                WHERE (ch.NodeNo = N'28656'
                                       OR ch.NodeNo LIKE N'28656.%'
                                      )
                                      AND cp.PrimaryParentCategory IS NOT NULL";
            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                return (DbHelper.ExecuteQuery(true, cmd, dt =>
                {
                    var result = new List<string>();
                    if (dt != null && dt.Rows.Count > 0)
                        foreach (DataRow one in dt.Rows)
                            result.Add(one[0]?.ToString());
                    return result;
                }));
            }
        }
    }
}
