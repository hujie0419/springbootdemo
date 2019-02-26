using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALVIPAuthorizationExchangeCodeConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<VIPAuthorizationExchangeCodeConfig> GetVIPAuthorizationExchangeCodeConfigList(string CodeBatch, int pageSize, int pageIndex, out int recordCount)
        {
            string strSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(CodeBatch))
            {
                strSql += " AND CodeBatch = @CodeBatch";
            }
            string sql = @"SELECT  *
                        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY SE_VIPAuthorizationExchangeCodeConfig.[CreateTime] DESC ) AS ROWNUMBER ,
                                            [ExchangeCode] ,
                                            [CodeBatch] ,
                                            [VIPAuthorizationRuleId] ,
                                            [Status] ,
                                            SE_VIPAuthorizationExchangeCodeConfig.[CreateTime] ,
                                            [EndTime] ,
                                            SE_VIPAuthorizationRuleConfig.RuleName
                                  FROM      [Configuration].[dbo].[SE_VIPAuthorizationExchangeCodeConfig]
                                            WITH ( NOLOCK )
                                            LEFT JOIN Configuration..SE_VIPAuthorizationRuleConfig
                                            WITH ( NOLOCK ) ON SE_VIPAuthorizationRuleConfig.Id = SE_VIPAuthorizationExchangeCodeConfig.VIPAuthorizationRuleId
                                  WHERE     1 = 1 " + strSql + @") AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize) ";

            string sqlCount = @"
                    SELECT COUNT(0)
                    FROM    [Configuration].[dbo].[SE_VIPAuthorizationExchangeCodeConfig]
                            WITH ( NOLOCK )
                            LEFT JOIN Configuration..SE_VIPAuthorizationRuleConfig WITH (NOLOCK)
                    ON SE_VIPAuthorizationRuleConfig.Id = SE_VIPAuthorizationExchangeCodeConfig.VIPAuthorizationRuleId   WHERE 1=1  " + strSql;
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@CodeBatch",CodeBatch),
            };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);            

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VIPAuthorizationExchangeCodeConfig>().ToList();

        }


        public static VIPAuthorizationExchangeCodeConfig GetVIPAuthorizationExchangeCodeConfig(int id)
        {
            const string sql = @"SELECT [ExchangeCode]
                                      ,[CodeBatch]
                                      ,[VIPAuthorizationRuleId]
                                      ,[Status]
                                      ,[CreateTime]
                                      ,[EndTime]
                                  FROM [Configuration].[dbo].[SE_VIPAuthorizationExchangeCodeConfig] WITH (NOLOCK) WHERE Id=@id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VIPAuthorizationExchangeCodeConfig>().ToList().FirstOrDefault();

        }

        public static List<VIPAuthorizationExchangeCodeConfig> GetCodeBatch()
        {
            const string sql = @"SELECT  DISTINCT [CodeBatch]
                                  FROM [Configuration].[dbo].[SE_VIPAuthorizationExchangeCodeConfig] WITH (NOLOCK)";

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<VIPAuthorizationExchangeCodeConfig>().ToList();

        }

        public static bool InsertVIPAuthorizationExchangeCodeConfig(VIPAuthorizationExchangeCodeConfig model)
        {
            int RandKey = new Random().Next(100000, 999999);

            const string sql = @" DECLARE @i INT        
                                 DECLARE @code VARCHAR(1000)
                                 SET @i = 1

                                 WHILE ( @i <= @SumNum )
                                    BEGIN 
                                        SET @code = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)
                                                              * CHECKSUM(NEWID())), 12)
                                        IF NOT EXISTS ( SELECT  1
                                                        FROM    Configuration.dbo.SE_VIPAuthorizationExchangeCodeConfig
                                                        WHERE   ExchangeCode = @code )
                                            BEGIN
                                                INSERT  INTO Configuration.dbo.SE_VIPAuthorizationExchangeCodeConfig
                                                        ( ExchangeCode ,
                                                          CodeBatch ,
                                                          VIPAuthorizationRuleId ,
                                                          Status ,
                                                          CreateTime ,
                                                          EndTime
                                                        )
                                                VALUES  ( @code ,
                                                          @CodeBatch , -- CodeBatch - nvarchar(100)
                                                          @VIPAuthorizationRuleId , -- VIPAuthorizationRuleId - int
                                                          @Status , -- Status - smallint
                                                          GETDATE() , -- CreateTime - datetime
                                                          @EndTime  -- EndTime - datetime
                                                        )  
                                                SET @i = @i + 1    
                                            END 
                                    END   ";


            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@CodeBatch",RandKey),
                    new SqlParameter("@VIPAuthorizationRuleId",model.VIPAuthorizationRuleId),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@SumNum",model.SumNum)
                };

            const string sql1 = @"SELECT COUNT(1)
                                 FROM   Configuration.dbo.SE_VIPAuthorizationExchangeCodeConfig
                                 WHERE  CodeBatch = @CodeBatch";
            if ((int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, new SqlParameter("@CodeBatch", RandKey)) > 0)
            {
                return false;
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
            }

        }


        public static bool UpdateVIPAuthorizationExchangeCodeConfig(VIPAuthorizationExchangeCodeConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_VIPAuthorizationExchangeCodeConfig SET 
                                       [VIPAuthorizationRuleId]=@VIPAuthorizationRuleId
                                      ,[Status]=@Status                               
                                      ,[EndTime]=@EndTime                
                                WHERE ExchangeCode=@ExchangeCode";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@ExchangeCode",model.ExchangeCode??string.Empty),
                    new SqlParameter("@CodeBatch",model.CodeBatch??string.Empty),
                    new SqlParameter("@VIPAuthorizationRuleId",model.VIPAuthorizationRuleId),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@EndTime",model.EndTime)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteVIPAuthorizationExchangeCodeConfig(string id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_VIPAuthorizationExchangeCodeConfig WHERE ExchangeCode=@ExchangeCode";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@ExchangeCode", id)) > 0;
        }
    }
}

