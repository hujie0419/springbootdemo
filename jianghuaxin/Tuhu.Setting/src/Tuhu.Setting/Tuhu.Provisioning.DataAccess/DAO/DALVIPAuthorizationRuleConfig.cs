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
    public class DALVIPAuthorizationRuleConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<VIPAuthorizationRuleConfig> GetVIPAuthorizationRuleConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [RuleName] ,
                                                [Description] ,
                                                [ValidityDay] ,
                                                [PrivilegeType] ,
                                                [CreateTime] ,
                                                [UpdateTime]
                                      FROM      [Configuration].[dbo].[SE_VIPAuthorizationRuleConfig] WITH ( NOLOCK )
                                      WHERE     1 = 1   " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                             ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[SE_VIPAuthorizationRuleConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VIPAuthorizationRuleConfig>().ToList();

        }

        public static List<VIPAuthorizationRuleConfig> GetVIPAuthorizationRuleAndId()
        {
            string sql = @"SELECT [Id]
                          ,[RuleName]                       
                      FROM [Configuration].[dbo].[SE_VIPAuthorizationRuleConfig] WITH (NOLOCK) ";

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<VIPAuthorizationRuleConfig>().ToList();

        }
        public static VIPAuthorizationRuleConfig GetVIPAuthorizationRuleConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[RuleName]
                                      ,[Description]
                                      ,[ValidityDay]
                                      ,[PrivilegeType] 
                                      ,[CreateTime]
                                      ,[UpdateTime]
                                  FROM [Configuration].[dbo].[SE_VIPAuthorizationRuleConfig] WITH (NOLOCK) WHERE Id=@id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VIPAuthorizationRuleConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertVIPAuthorizationRuleConfig(VIPAuthorizationRuleConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..SE_VIPAuthorizationRuleConfig
                                              ( RuleName ,
                                                Description ,
                                                ValidityDay ,
                                                PrivilegeType ,
                                                CreateTime ,
                                                UpdateTime
                                              )
                                      VALUES  ( @RuleName ,
                                                @Description ,
                                                @ValidityDay ,
                                                @PrivilegeType ,
                                                GETDATE() , 
                                                GETDATE()  
                                              )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@RuleName",model.RuleName??string.Empty),
                    new SqlParameter("@Description",model.Description??string.Empty),
                    new SqlParameter("@ValidityDay",model.ValidityDay),
                    new SqlParameter("@PrivilegeType",model.PrivilegeType)

                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }


        public static bool UpdateVIPAuthorizationRuleConfig(VIPAuthorizationRuleConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_VIPAuthorizationRuleConfig SET                                      
                                        RuleName=@RuleName ,
                                        Description=@Description ,
                                        ValidityDay=@ValidityDay ,
                                        PrivilegeType=@PrivilegeType ,                                      
                                        UpdateTime=GETDATE()                                        
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                  new SqlParameter("@Description",model.Description??string.Empty),
                  new SqlParameter("@Id",model.Id),
                  new SqlParameter("@PrivilegeType",model.PrivilegeType),
                  new SqlParameter("@RuleName",model.RuleName??string.Empty),
                  new SqlParameter("@ValidityDay",model.ValidityDay)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteVIPAuthorizationRuleConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_VIPAuthorizationRuleConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}

