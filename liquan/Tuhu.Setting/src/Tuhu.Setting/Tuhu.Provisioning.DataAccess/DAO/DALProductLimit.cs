using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    /// <summary>
    /// 
    /// </summary>
    public class DALProductLimit
    {
        private static readonly string ConnGungnir = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly string StrConn = SecurityHelp.IsBase64Formatted(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString) ? SecurityHelp.DecryptAES(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString) : ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static ProductLimitCountEntity GetModelByName(string category, int level)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"SELECT  *
                            FROM    [Tuhu_productcatalog]..ProductLimitCount(NOLOCK)
                            WHERE   CategoryLevel = @CategoryLevel
                                    AND CategoryName = @CategoryName;";
                return conn.Query<ProductLimitCountEntity>(sql, new ProductLimitCountEntity { CategoryName = category?.Trim(), CategoryLevel = level }).FirstOrDefault() ?? new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 查询单个类目是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProductLimitCountEntity FetchCategoryLimitCount(ProductLimitCountEntity model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"SELECT  *
                            FROM    [Tuhu_productcatalog]..ProductLimitCount(NOLOCK)
                            WHERE   PID IS NULL AND CategoryLevel = @CategoryLevel AND CategoryCode = @CategoryCode AND CategoryName = @CategoryName;";
                return conn.Query<ProductLimitCountEntity>(sql, new ProductLimitCountEntity { CategoryCode = model.CategoryCode?.Trim(), CategoryName = model.CategoryName?.Trim(), CategoryLevel = model.CategoryLevel }).FirstOrDefault() ?? new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 查询单个类目是否存在
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProductLimitCountEntity GetCategoryLimitCount(ProductLimitCountEntity model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"SELECT  *
                            FROM    [Tuhu_productcatalog]..ProductLimitCount(NOLOCK)
                            WHERE   PID IS NULL AND CategoryLevel = @CategoryLevel AND CategoryCode = @CategoryCode;";
                return conn.Query<ProductLimitCountEntity>(sql, new ProductLimitCountEntity { CategoryCode = model.CategoryCode?.Trim(), CategoryLevel = model.CategoryLevel }).FirstOrDefault() ?? new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProductLimitCountEntity FetchProductLimitCount(ProductLimitCountEntity model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"SELECT  *
                            FROM    [Tuhu_productcatalog]..ProductLimitCount(NOLOCK)
                            WHERE   Pid = @Pid AND CategoryCode = @CategoryCode;";
                return conn.Query<ProductLimitCountEntity>(sql, new ProductLimitCountEntity { CategoryCode = model.CategoryCode?.Trim(), Pid = model.Pid }).FirstOrDefault() ?? new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 添加产品限购
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertProductLimitCount(ProductLimitCountEntity model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"INSERT INTO Tuhu_productcatalog.dbo.ProductLimitCount
                        ( 
                          PID ,
                          ProductName ,
                          CategoryCode ,
                          CategoryName ,
                          CategoryLevel ,
                          LimitCount ,
                          CreateTime ,
                          LastUpdateTime
                        )
                        VALUES  ( 
                                  @PID ,
                                  @ProductName ,
                                  @CategoryCode ,
                                  @CategoryName ,
                                  @CategoryLevel ,
                                  @LimitCount ,
                                  @CreateTime ,
                                  @LastUpdateTime
                        );
                        SELECT ISNULL(@@IDENTITY,0)";
                var sqlPrams = new[] {
                    new SqlParameter("@PID", model.Pid),
                    new SqlParameter("@ProductName", model.ProductName?.Trim()),
                    new SqlParameter("@CategoryCode", model.CategoryCode?.Trim()),
                    new SqlParameter("@CategoryName", model.CategoryName?.Trim()),
                    new SqlParameter("@CategoryLevel", model.CategoryLevel),
                    new SqlParameter("@LimitCount", model.LimitCount),
                    new SqlParameter("@CreateTime", DateTime.Now),
                    new SqlParameter("@LastUpdateTime", DateTime.Now)
                };
                return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlPrams));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateProductLimitCount(ProductLimitCountEntity model)
        {
            using (var conn = new SqlConnection(StrConn))
            {
                const string sql = @"UPDATE   [Tuhu_productcatalog]..ProductLimitCount
                         SET PID = @PID,
                         ProductName = @ProductName,
                         CategoryCode = @CategoryCode,
                         CategoryName = @CategoryName,
                         CategoryLevel = @CategoryLevel,
                         LimitCount = @LimitCount,
                         LastUpdateTime = GETDATE()
                         WHERE PKID = @PKID;";
                var sqlPrams = new[]
                {
                    new SqlParameter("@PID", model.Pid),
                    new SqlParameter("@ProductName", model.ProductName?.Trim()),
                    new SqlParameter("@CategoryCode", model.CategoryCode?.Trim()),
                    new SqlParameter("@CategoryName", model.CategoryName?.Trim()),
                    new SqlParameter("@CategoryLevel", model.CategoryLevel),
                    new SqlParameter("@LimitCount", model.LimitCount),
                    new SqlParameter("@PKID", model.PKID)
                };
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public static List<ProductLimitCountEntity> GetProductListByCategory(string category, int pageIndex, int pageSize, string sqlWhere, out int total)
        {
            using (var conn = new SqlConnection(ConnGungnir))
            {
                var sql = @"SELECT  CP.PID ,
                                    CP.DisplayName AS ProductName ,
                                    ISNULL(PLC.PKID, 0) PKID ,
                                    ISNULL(PLC.LimitCount, 0) LimitCount ,
                                    PLC.CategoryCode ,
                                    PLC.CategoryName ,
                                    PLC.CategoryLevel
                            FROM    Tuhu_productcatalog..vw_Products AS CP WITH ( NOLOCK )
                                    JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH ( NOLOCK ) ON ch.child_oid = CP.oid
                                    LEFT JOIN Tuhu_productcatalog..ProductLimitCount PLC WITH ( NOLOCK ) ON CP.PID = PLC.PID
                            WHERE   CP.i_ClassType IN ( 2, 4 )
                                    AND ( CP.oid IN (
                                          SELECT    CH1.child_oid
                                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS CH1
                                                    WITH ( NOLOCK )
                                                    JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS CH2
                                                    WITH ( NOLOCK ) ON CH1.NodeNo LIKE CH2.NodeNo + '.%'
                                          WHERE     CH2.CategoryName = @CategoryName ) ) {0}
                            ORDER BY CP.PID
                            OFFSET (@pageIndex - 1) * @pageSize ROW
				            FETCH NEXT @pageSize ROW ONLY;
                            SELECT  COUNT(0) Total
                            FROM    Tuhu_productcatalog..vw_Products AS CP WITH ( NOLOCK )
                                    JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS ch WITH ( NOLOCK ) ON ch.child_oid = CP.oid
                            WHERE   CP.i_ClassType IN ( 2, 4 )
                                    AND ( CP.oid IN (
                                          SELECT    CH1.child_oid
                                          FROM      Tuhu_productcatalog..CarPAR_CatalogHierarchy AS CH1
                                                    WITH ( NOLOCK )
                                                    JOIN Tuhu_productcatalog..CarPAR_CatalogHierarchy AS CH2
                                                    WITH ( NOLOCK ) ON CH1.NodeNo LIKE CH2.NodeNo + '.%'
                                          WHERE     CH2.CategoryName = @CategoryName ) ) {0};";
                sql = !string.IsNullOrWhiteSpace(sqlWhere) ? string.Format(sql, sqlWhere) : string.Format(sql, "", "");

                var parames = new List<SqlParameter>
                {
                    new SqlParameter("@CategoryName", category?.Trim()),
                    new SqlParameter("@pageIndex", pageIndex),
                    new SqlParameter("@pageSize", pageSize)
                };
                total = 0;
                var ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql, parames.ToArray());
                if (ds != null && ds.Tables.Count > 0)
                {
                    total = int.Parse(ds.Tables[1].Rows[0][0].ToString());
                    return ds.Tables[0].ConvertTo<ProductLimitCountEntity>().ToList();
                }
                return new List<ProductLimitCountEntity>();
            }
        }
    }
}
