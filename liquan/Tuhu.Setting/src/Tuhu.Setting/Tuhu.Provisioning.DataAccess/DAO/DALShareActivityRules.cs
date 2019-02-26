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
    public class DALShareActivityRules
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        public static List<ShareActivityRulesConfig> GetShareActivityRulesList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                            [Id] ,
                                            [Image] ,
                                            [Status] ,
                                            [Rules] ,
                                            [CreateTime]
                                  FROM      [Configuration].[dbo].[ShareActivityRulesConfig] WITH ( NOLOCK )
                                  WHERE     1 = 1 " + sqlStr + @"
                                ) AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize)

                        ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[ShareActivityRulesConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text,sql, sqlParameters).ConvertTo<ShareActivityRulesConfig>().ToList();

        }


        public static ShareActivityRulesConfig GetShareActivityRules(int id)
        {
            const string sql = @"SELECT [Id]
                                  ,[Image]
                                  ,[Status]
                                  ,[Rules]
                                  ,[CreateTime]
                              FROM [Configuration].[dbo].[ShareActivityRulesConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ShareActivityRulesConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertShareActivityRules(ShareActivityRulesConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..ShareActivityRulesConfig
                                          (    [Image]
                                              ,[Status]
                                              ,[Rules]
                                              ,[CreateTime]
                                          )
                                  VALUES(     @Image  
                                              ,@Status
                                              ,@Rules
                                              ,GETDATE()
                                          
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@Rules",model.Rules??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdateShareActivityRules(ShareActivityRulesConfig model)
        {
            const string sql = @"UPDATE Configuration..ShareActivityRulesConfig SET                                      
                                               Image= @Image  
                                              ,Status=@Status
                                              ,Rules=@Rules
                                              ,CreateTime=GETDATE()
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@Rules",model.Rules??string.Empty),
                    new SqlParameter("@Id",model.Id)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteShareActivityRules(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.ShareActivityRulesConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
