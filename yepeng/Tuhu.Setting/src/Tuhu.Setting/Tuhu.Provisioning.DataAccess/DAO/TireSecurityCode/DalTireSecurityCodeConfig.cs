using System;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.TireSecurityCode;
using System.Collections;
using System.Reflection;
using System.Configuration;

namespace Tuhu.Provisioning.DataAccess.DAO.TireSecurityCode
{
    public class DalTireSecurityCodeConfig
    {
        public static List<TireSecurityCodeConfig> QuerySecurityCodeConfigModel(SqlConnection conn, TireSecurityCodeConfigQuery query)
        {
            string countSql = @"SELECT Count(1) 
                            FROM [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK)";
            string sql = @"SELECT * 
                            FROM [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK)";
            string addsql = "";
            if (!string.IsNullOrWhiteSpace(query.SecurityCodeCriterion)) addsql += @" SecurityCode LIKE N'%" + query.SecurityCodeCriterion + "%' ";
            if (!string.IsNullOrWhiteSpace(query.UCodeCriterion))
            {
                if (addsql != "") addsql += " and ";
                addsql += @" UCode LIKE N'%" + query.UCodeCriterion + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(query.FCodeCriterion))
            {
                if (addsql != "") addsql += " and ";
                addsql += @" FCode LIKE N'%" + query.FCodeCriterion + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(query.BarCodeCriterion))
            {
                if (addsql != "") addsql += " and ";
                addsql += @" BarCode LIKE N'%" + query.BarCodeCriterion + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(query.GuidCriterion))
            {
                if (addsql != "") addsql += " and ";
                addsql += @" CodeID LIKE N'%" + query.GuidCriterion + "%'";
            }

            if (addsql != "")
            {
                sql += " where ";
                countSql += " where ";
            }
            sql = sql + addsql +
                @" ORDER BY CreateTime DESC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";
            countSql = countSql + addsql;

            var sqlParam = new[]
                {
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };

            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<TireSecurityCodeConfig>().ToList();
        }

        public static List<UploadSecurityCodeLog> QueryUploadSecurityCodeLogModel(SqlConnection conn, LogSearchQuery query)
        {
            string countSql = @"SELECT Count(1) 
                            FROM [Configuration].dbo.UploadSecurityCodeLog WITH ( NOLOCK)";
            string sql = @"SELECT * 
                            FROM [Configuration].dbo.UploadSecurityCodeLog WITH ( NOLOCK)
                            ORDER BY CreateTime DESC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";

            var sqlParam = new[]
                {
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };

            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<UploadSecurityCodeLog>().ToList();
        }

        public static List<UploadBarCodeLog> QueryUploadBarCodeLogModel(SqlConnection conn, LogSearchQuery query)
        {
            string countSql = @"SELECT Count(1) 
                            FROM [Configuration].dbo.UploadBarCodeLog WITH ( NOLOCK)";
            string sql = @"SELECT * 
                            FROM [Configuration].dbo.UploadBarCodeLog WITH ( NOLOCK)
                            ORDER BY CreateTime DESC OFFSET @pagesdata ROWS FETCH NEXT @pagedataquantity ROWS ONLY";

            var sqlParam = new[]
                {
                    new SqlParameter("@pagesdata",(query.PageIndex-1)*query.PageDataQuantity),
                    new SqlParameter("@pagedataquantity",query.PageDataQuantity),
                };

            query.TotalCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, countSql, sqlParam);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<UploadBarCodeLog>().ToList();
        }

        public static bool InsertTireSecurityCodeConfig(List<TireSecurityCodeConfig> list)
        {
            DataTable dt = ToDataTable(list);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("", conn))
                {
                    try
                    {
                        cmd.CommandText = @"create table #TmpTable (
                                                [PKID] int IDENTITY(1,1) NOT NULL primary key,
                                                [CodeID] [uniqueidentifier]  NOT NULL,
                                                [SecurityCode] [nvarchar] (50) NULL,
                                                [UCode] [nvarchar] (50) NULL,
                                                [FCode] [nvarchar] (50) NULL,
                                                [BarCode] [nvarchar] (50) NULL,
                                                [DataIntegrity] bit not null default(0),
                                                [CreateTime] DATETIME NOT NULL DEFAULT(GETDATE()),
                                                [LastUpdateDataTime] DATETIME NOT NULL DEFAULT(GETDATE()),
                                                [BatchNum] [nvarchar] (50) NULL,
                                                )";
                        cmd.ExecuteNonQuery();
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        string merge = @"MERGE INTO [Configuration].dbo.TireSecurityCodeConfig WITH ( ROWLOCK ) AS s
                                            USING #TmpTable AS c
                                                ON c.SecurityCode = s.SecurityCode
                                         WHEN NOT MATCHED THEN
    INSERT (   CodeID ,
               SecurityCode ,
               UCode ,
               FCode ,
                BarCode,
               DataIntegrity ,
               BatchNum ,
               CreateTime ,
               LastUpdateDataTime
           )
VALUES ( c.CodeID ,
                                                                          c.SecurityCode,
                                                                          c.UCode,
                                                                          c.FCode,
                                                                          c.BarCode,
                                                                          c.DataIntegrity,
                                                                          c.BatchNum,
                                                                          GETDATE(),
                                                                          GETDATE()
                                                                          );";
                        cmd.CommandText = merge;
                        var result = cmd.ExecuteNonQuery();
                        return result >= 0;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public static int InsertBarCodeConfig(List<InputBarCode> list)
        {
            DataTable dt = ToDataTable(list);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("", conn))
                {
                    try
                    {
                        cmd.CommandText = @"create table #TmpTable (
                                                [SecurityCode] [nvarchar](50) NULL,
                                                [BarCode] [nvarchar](50) NULL,
                                                [BarCodeBatchNum] [nvarchar](50) NULL,
                                                )";
                        cmd.ExecuteNonQuery();
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        //判断插入条码数据里是否有不存在的防伪码
                        string checkItemNotExist = @"SELECT COUNT(1)
                                                FROM   #TmpTable AS C WITH ( NOLOCK )
                                                WHERE  C.SecurityCode NOT IN (   SELECT SecurityCode
                                                                                 FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                             );";
                        cmd.CommandText = checkItemNotExist;
                        var esistResult = Convert.ToInt32(cmd.ExecuteScalar());
                        if (esistResult > 0) return -2;

                        //判断传入的条码二维码是否已经上传过
                        string checkAllExist = @"SELECT COUNT(1)
                                                FROM   #TmpTable AS C WITH ( NOLOCK )
                                                WHERE  NOT EXISTS (   SELECT *
                                                                      FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                      WHERE  C.SecurityCode = SecurityCode
                                                                             AND C.BarCode = BarCode
                                                                  );";
                        cmd.CommandText = checkAllExist;
                        var allEsistResult = Convert.ToInt32(cmd.ExecuteScalar());
                        if (allEsistResult == 0) return -3;

                        //判断存在已有条码对应的防伪码对应新上传的条码不一致
                        string checkDifference = @"SELECT COUNT(1)
                                                FROM   #TmpTable AS C WITH ( NOLOCK )
                                                WHERE  EXISTS (   SELECT *
                                                                  FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                  WHERE  C.SecurityCode = SecurityCode
                                                                         AND C.BarCode <> BarCode
                                                              )
                                                       OR EXISTS (   SELECT *
                                                                     FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                     WHERE  BarCode IS NOT NULL
                                                                            AND C.BarCode = BarCode
                                                                            AND C.SecurityCode <> SecurityCode
                                                                 );";
                        cmd.CommandText = checkDifference;
                        var diffResult = Convert.ToInt32(cmd.ExecuteScalar());
                        if (diffResult > 0) return -4;

                        string merge = @"MERGE INTO [Configuration].dbo.TireSecurityCodeConfig WITH ( ROWLOCK ) AS s
                                                USING #TmpTable AS c
                                                ON c.SecurityCode = s.SecurityCode
                                                WHEN MATCHED THEN
                                                    UPDATE SET s.BarCode = ISNULL(s.BarCode, c.BarCode) ,
                                                               s.DataIntegrity = CASE s.DataIntegrity
                                                                                      WHEN 0 THEN 1
                                                                                      ELSE s.DataIntegrity
                                                                                 END ,
                                                               s.BarCodeBatchNum = ISNULL(
                                                                                             s.BarCodeBatchNum ,
                                                                                             c.BarCodeBatchNum
                                                                                         ) ,
                                                               s.LastUpdateDataTime = GETDATE();";
                        cmd.CommandText = merge;
                        var result = cmd.ExecuteNonQuery();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        return -1;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public static List<InputBarCode> QueryInputBarCodeByError(string error, List<InputBarCode> list)
        {
            DataTable dt = ToDataTable(list);
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("", conn))
                {
                    try
                    {
                        cmd.CommandText = @"create table #TmpTable (
                                                [SecurityCode] [nvarchar](50) NULL,
                                                [BarCode] [nvarchar](50) NULL,
                                                [BarCodeBatchNum] [nvarchar](50) NULL,
                                                )";
                        cmd.ExecuteNonQuery();
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }

                        string sql = "";
                        switch (error)
                        {
                            case "ItemNotExist":
                                sql = @"SELECT *
                                                FROM   #TmpTable AS C WITH ( NOLOCK )
                                                WHERE  C.SecurityCode NOT IN (   SELECT SecurityCode
                                                                                 FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                             );";
                                break;
                            case "Difference":
                                sql = @"SELECT *
                                                FROM   #TmpTable AS C WITH ( NOLOCK )
                                                WHERE  EXISTS (   SELECT *
                                                                  FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                  WHERE  C.SecurityCode = SecurityCode
                                                                         AND C.BarCode <> BarCode
                                                              )
                                                       OR EXISTS (   SELECT *
                                                                     FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                                                                     WHERE  BarCode IS NOT NULL
                                                                            AND C.BarCode = BarCode
                                                                            AND C.SecurityCode <> SecurityCode
                                                                 );";
                                break;
                        }
                        if (string.IsNullOrWhiteSpace(sql)) return null;
                        else return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<InputBarCode>().ToList();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public static List<TireSecurityCodeConfig> QuerySecurityCodeConfigModelByBatchNum(SqlConnection conn, string batchNum)
        {
            string sql = @"SELECT *
                            FROM   [Configuration].dbo.TireSecurityCodeConfig WITH ( NOLOCK )
                            WHERE  BatchNum = @batchNum";
            var sqlParam = new[]
                {
                    new SqlParameter("@batchNum",batchNum),
                };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParam).ConvertTo<TireSecurityCodeConfig>().ToList();
        }

        public static bool DeleleSecurityCodeConfigModelByBatchNum(SqlConnection conn, string batchNum)
        {
            string sql = @"DELETE [Configuration].dbo.TireSecurityCodeConfig
                            WHERE  BatchNum = @batchNum";
            var sqlParam = new[]
                {
                    new SqlParameter("@batchNum",batchNum),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool DeleleBarCodeByBatchNum(SqlConnection conn, string batchNum)
        {
            string sql = @"UPDATE [Configuration].dbo.TireSecurityCodeConfig
                           SET    BarCode = NULL ,
                                  DataIntegrity = 0 ,
                                  BarCodeBatchNum = NULL ,
                                  LastUpdateDataTime = GETDATE()
                           WHERE  BarCodeBatchNum = @batchNum";
            var sqlParam = new[]
                {
                    new SqlParameter("@batchNum",batchNum),
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool InsertUploadSecurityCodeLog(SqlConnection conn, UploadSecurityCodeLog log)
        {
            string sql = @"INSERT [Configuration].dbo.UploadSecurityCodeLog (   UploadFileName ,
                                                     UploadFileAddress ,
                                                     SuccessFileName ,
                                                     SuccessFileAddress ,
                                                     FailFileName ,
                                                     FailFileAddress ,
                                                     CreateTime ,
                                                     LastUpdateDataTime
                                                 )
                            VALUES ( @uploadFileName ,
                                     @uploadFileAddress ,
                                     @successFileName ,
                                     @successFileAddress ,
                                     @failFileName ,
                                     @failFileAddress ,
                                     GETDATE(),
                                     GETDATE()
                                     )";
            var sqlParam = new[]
            {
                new SqlParameter("@uploadFileName",log.UploadFileName),
                new SqlParameter("@uploadFileAddress",log.UploadFileAddress),
                new SqlParameter("@successFileName",log.SuccessFileName),
                new SqlParameter("@successFileAddress", log.SuccessFileAddress),
                new SqlParameter("@failFileName", log.FailFileName),
                new SqlParameter("@failFileAddress", log.FailFileAddress),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static bool InsertUploadBarCodeLog(SqlConnection conn, UploadBarCodeLog log)
        {
            string sql = @"INSERT [Configuration].dbo.UploadBarCodeLog (   UploadFileName ,
                                                     UploadFileAddress ,
                                                     FailFileName ,
                                                     FailFileAddress ,
                                                     CreateTime ,
                                                     LastUpdateDataTime
                                                 )
                            VALUES ( @uploadFileName ,
                                     @uploadFileAddress ,
                                     @failFileName ,
                                     @failFileAddress ,
                                     GETDATE(),
                                     GETDATE()
                                     )";
            var sqlParam = new[]
            {
                new SqlParameter("@uploadFileName",log.UploadFileName),
                new SqlParameter("@uploadFileAddress",log.UploadFileAddress),
                new SqlParameter("@failFileName", log.FailFileName),
                new SqlParameter("@failFileAddress", log.FailFileAddress),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0 ? true : false;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }
    }
}
