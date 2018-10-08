using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.Tire;

namespace Tuhu.Provisioning.DataAccess.DAO.Tire
{
    public class DalTirePatternConfig
    {
        public static List<TirePatternChangeLog> SelectTirePatternChangeLog(SqlConnection conn, string pattern)
        {
            string sql = @"SELECT * FROM Tuhu_productcatalog.dbo.TirePatternChangeLog WITH ( NOLOCK )
                            WHERE Tire_Pattern=@pattern
                            ORDER BY CreateTime DESC";
            var sqlParam = new[]
                {
                    new SqlParameter("@pattern",pattern),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<TirePatternChangeLog>()
                .ToList();
        }

        public static List<TirePatternConfig> QueryTirePatternConfig(SqlConnection conn, TirePatternConfigQuery query)
        {
            string countSql = @"SELECT COUNT(1)
                                FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK ) 
                                WHERE  IsDelete = 0 ";
            string sql = @"SELECT *
                                FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK )  
                                WHERE  IsDelete = 0 ";
            string addsql = string.Empty;
            if (string.IsNullOrWhiteSpace(query.PatternCriterion))
            {
                if (!string.IsNullOrWhiteSpace(query.BrandCriterion))
                    addsql = "and CP_Brand like N'%" + query.BrandCriterion + "%'";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(query.BrandCriterion))
                    addsql = "and CP_Brand like N'%" + query.BrandCriterion + "%' and Tire_Pattern like N'%" + query.PatternCriterion + "%'";
                else
                    addsql = "and Tire_Pattern like N'%" + query.PatternCriterion + "%'";
            }
            sql = sql + addsql +
                @" ORDER BY LastUpdateDateTime desc OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";
            countSql = countSql + addsql;

            var sqlParam = new[]
                {
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };

            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<TirePatternConfig>().ToList();
        }

        public static List<TirePatternConfig> GetConfigByPattern(SqlConnection conn, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return null;
            string sql = @"SELECT *
                                FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK )
                                WHERE  IsDelete = 0 AND Tire_Pattern=@pattern";
            var sqlParam = new[]
            {
                new SqlParameter("@pattern",pattern),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<TirePatternConfig>()
                .ToList();
        }

        public static List<TirePatternConfig> GetConfigByPKID(SqlConnection conn, int pkid)
        {
            if (pkid == 0) return null;
            string sql = @"SELECT *
                                FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK )
                                WHERE  PKID=@pkid";
            var sqlParam = new[]
            {
                new SqlParameter("@pkid",pkid),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam)
                .ConvertTo<TirePatternConfig>()
                .ToList();
        }

        public static List<string> GetALlPattern(SqlConnection conn)
        {
            string sql = @"SELECT DISTINCT Tire_Pattern
                                FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK )
                                WHERE  IsDelete = 0";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql)
                .ConvertTo<TirePatternConfig>()
                .ToList();
            return result.Select(item => item.Tire_Pattern).Distinct().ToList();
        }

        public static bool InsertTirePatternConfig(SqlConnection conn, TirePatternConfig config)
        {
            string sql = @"INSERT Tuhu_productcatalog.[dbo].[TirePatternConfig] (   CP_Brand ,
                                                         Tire_Pattern ,
                                                         Pattern_EN ,
                                                         Pattern_CN ,
                                                         CreateDateTime ,
                                                         LastUpdateDateTime
                                                     )
                           VALUES ( @brand ,
                                    @pattern ,
                                    @patternEN ,
                                    @patternCN ,
                                    @createTime ,
                                    @updateTime
                                  );";
            var sqlParam = new[]
            {
                new SqlParameter("@brand", config.CP_Brand),
                new SqlParameter("@pattern", config.Tire_Pattern),
                new SqlParameter("@patternEN", config.Pattern_EN),
                new SqlParameter("@patternCN", config.Pattern_CN),
                new SqlParameter("@createTime", config.CreateDateTime),
                new SqlParameter("@updateTime", config.LastUpdateDateTime),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool InsertTirePatternChangeLog(SqlConnection conn, TirePatternChangeLog log)
        {
            string sql = @"INSERT Tuhu_productcatalog.dbo.TirePatternChangeLog (   Tire_Pattern ,
                                                        [ChangeBefore] ,
                                                        [ChangeAfter] ,
                                                        [Operator] ,
                                                        [CreateTime] ,
                                                        [LastUpdateDataTime]
                                                    )
                           VALUES ( @Pattern ,
                                    @ChangeBefore ,
                                    @ChangeAfter ,
                                    @Operator ,
                                    @createTime ,
                                    @updateTime
                                  );";
            var sqlParam = new[]
            {
                new SqlParameter("@Pattern", log.Tire_Pattern),
                new SqlParameter("@ChangeBefore", log.ChangeBefore),
                new SqlParameter("@ChangeAfter", log.ChangeAfter),
                new SqlParameter("@Operator", log.Operator),
                new SqlParameter("@createTime", log.CreateTime),
                new SqlParameter("@updateTime", log.LastUpdateDataTime),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool UpdateTirePatternConfigExpectPattern(SqlConnection conn, TirePatternConfig config)
        {
            string sql = @"UPDATE Tuhu_productcatalog.[dbo].[TirePatternConfig]  WITH (ROWLOCK)
                           SET    CP_Brand = @brand ,
                                  Pattern_EN = @patternEN ,
                                  Pattern_CN = @patternCN ,
                                  LastUpdateDateTime = @updateTime
                           WHERE  Tire_Pattern = @pattern;";
            var sqlParam = new[]
            {
                new SqlParameter("@brand", config.CP_Brand),
                new SqlParameter("@pattern", config.Tire_Pattern),
                new SqlParameter("@patternEN", config.Pattern_EN),
                new SqlParameter("@patternCN", config.Pattern_CN),
                new SqlParameter("@updateTime", config.LastUpdateDateTime),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool UpdateTirePatternConfig(SqlConnection conn, TirePatternConfig oldConfig, TirePatternConfig newConfig)
        {
            string Sql_DisableOldPattern = @"UPDATE Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH (ROWLOCK)
                                             SET    IsDelete = 1 ,
                                                    LastUpdateDateTime = GETDATE()
                                             WHERE  Tire_Pattern = @pattern;";
            string Sql_NewPatternExist = @"SELECT *
                                           FROM   Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH ( NOLOCK )
                                           WHERE  Tire_Pattern = @pattern;";
            string Sql_InsertNewPattern = @"INSERT Tuhu_productcatalog.[dbo].[TirePatternConfig] (    CP_Brand ,
                                                         Tire_Pattern ,
                                                         Pattern_EN ,
                                                         Pattern_CN ,
                                                         CreateDateTime ,
                                                         LastUpdateDateTime
                                                    )
                                            VALUES ( @brand ,
                                                     @pattern ,
                                                     @patternEN ,
                                                     @patternCN ,
                                                     GETDATE() ,
                                                     GETDATE() );";
            string Sql_EnableNewPattern = @"UPDATE Tuhu_productcatalog.[dbo].[TirePatternConfig] WITH (ROWLOCK)
                                            SET    IsDelete = 0 ,
                                                   CP_Brand = @brand ,
                                                   Pattern_EN = @patternEN ,
                                                   Pattern_CN = @patternCN ,
                                                   LastUpdateDateTime = GETDATE()
                                            WHERE  Tire_Pattern = @pattern;";
            string Sql_UpdateCatalog = @"UPDATE Tuhu_productcatalog.dbo.[CarPAR_zh-CN_Catalog] WITH (ROWLOCK)
                                         SET    CP_Tire_Pattern = @newPattern
                                         WHERE  CP_Tire_Pattern = @oldPattern;";

            using (var disableCmd = new SqlCommand(Sql_DisableOldPattern))
            using (var existCmd = new SqlCommand(Sql_NewPatternExist))
            using (var insertCmd = new SqlCommand(Sql_InsertNewPattern))
            using (var enableCmd = new SqlCommand(Sql_EnableNewPattern))
            using (var updateCmd = new SqlCommand(Sql_UpdateCatalog))
            using (var dbHelper = DbHelper.CreateDefaultDbHelper())
            {
                dbHelper.BeginTransaction();
                try
                {
                    disableCmd.Parameters.AddWithValue("@pattern", oldConfig.Tire_Pattern);
                    var result_disable = dbHelper.ExecuteNonQuery(disableCmd);
                    if (result_disable <= 0)
                    {
                        dbHelper.Rollback();
                        return false;
                    }

                    existCmd.Parameters.AddWithValue("@pattern", newConfig.Tire_Pattern);
                    var result_exist = dbHelper.ExecuteDataTable(existCmd).ConvertTo<TirePatternConfig>().ToList();
                    if (result_exist == null || !result_exist.Any())
                    {
                        insertCmd.Parameters.AddWithValue("@brand", newConfig.CP_Brand);
                        insertCmd.Parameters.AddWithValue("@pattern", newConfig.Tire_Pattern);
                        insertCmd.Parameters.AddWithValue("@patternEN", newConfig.Pattern_EN);
                        insertCmd.Parameters.AddWithValue("@patternCN", newConfig.Pattern_CN);
                        var result_insert = dbHelper.ExecuteNonQuery(insertCmd);
                        if (result_insert <= 0)
                        {
                            dbHelper.Rollback();
                            return false;
                        }
                    }
                    else
                    {
                        enableCmd.Parameters.AddWithValue("@brand", newConfig.CP_Brand);
                        enableCmd.Parameters.AddWithValue("@patternEN", newConfig.Pattern_EN);
                        enableCmd.Parameters.AddWithValue("@patternCN", newConfig.Pattern_CN);
                        enableCmd.Parameters.AddWithValue("@pattern", newConfig.Tire_Pattern);
                        var result_enable = dbHelper.ExecuteNonQuery(enableCmd);
                        if (result_enable <= 0)
                        {
                            dbHelper.Rollback();
                            return false;
                        }
                    }

                    updateCmd.Parameters.AddWithValue("oldPattern", oldConfig.Tire_Pattern);
                    updateCmd.Parameters.AddWithValue("newPattern", newConfig.Tire_Pattern);

                    var result_update = dbHelper.ExecuteNonQuery(updateCmd);
                    if (result_update < 0)
                    {
                        dbHelper.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    return false;
                }
                dbHelper.Commit();
                return true;
            }
        }

        public static List<string> GetAffectedPids(SqlConnection conn, string pattern)
        {
            string sql = @"SELECT DISTINCT PID
                           FROM   Tuhu_productcatalog..vw_Products WITH ( NOLOCK )
                           WHERE  CP_Tire_Pattern = @pattern;";
            var salParam = new[]
            {
                new SqlParameter("@pattern", pattern),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, salParam)
                .ConvertTo<ChangePatternAffectedProduct>()
                .ToList();
            return result.Select(item => item.PID).Distinct().ToList();
        }
    }
}
