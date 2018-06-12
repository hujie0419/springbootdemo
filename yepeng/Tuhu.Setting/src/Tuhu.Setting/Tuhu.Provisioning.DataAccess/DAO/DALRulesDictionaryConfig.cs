using Microsoft.ApplicationBlocks.Data;
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
    public class DALRulesDictionaryConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static RulesDictionaryConfig GetRulesDictionaryConfig(int type)
        {
            const string sql = @"SELECT TOP 1  [Id]
                                      ,[Type]
                                      ,[Content]
                                      ,[Status]
                                      ,[CreateTime]
                                  FROM [Configuration].[dbo].[SE_RulesDictionaryConfig] WITH(NOLOCK)  WHERE Type=@Type AND Status=1  ORDER BY CreateTime DESC
                                ";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Type",type)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RulesDictionaryConfig>().ToList().FirstOrDefault();
        }


        public static RulesDictionaryConfig GetRulesDictionaryConfigById(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Type]
                                      ,[Content]
                                      ,[Status]
                                      ,[CreateTime]
                                  FROM [Configuration].[dbo].[SE_RulesDictionaryConfig] WITH(NOLOCK)  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RulesDictionaryConfig>().ToList().FirstOrDefault();
        }
        public static bool InsertRulesDictionaryConfig(RulesDictionaryConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..[SE_RulesDictionaryConfig]
                                              ( Type, Content, Status, CreateTime 
                                              )
                                      VALUES  ( @Type ,
                                                @Content ,
                                                @Status ,                                             
                                                GETDATE()                                              
                                              )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Content",model.Content??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Type",model.Type)

                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }


        public static bool UpdateRulesDictionaryConfig(RulesDictionaryConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_RulesDictionaryConfig SET                                      
                                  Content=@Content, Status=@Status, CreateTime=GETDATE()                                        
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                  new SqlParameter("@Id",model.Id),
                  new SqlParameter("@Content",model.Content??string.Empty),
                  new SqlParameter("@Status",model.Status)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteRulesDictionaryConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_RulesDictionaryConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}

