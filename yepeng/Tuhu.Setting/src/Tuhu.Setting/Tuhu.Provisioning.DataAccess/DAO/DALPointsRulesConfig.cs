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
    public class DALPointsRulesConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<PointsRulesConfig> GetPointsRulesConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"     SELECT  *
                                FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY [CreateTime] DESC ) AS ROWNUMBER ,
                                                    [Id] ,
                                                    [Content] ,
                                                    [Status] ,
                                                    [CreateTime]
                                          FROM      [Configuration].[dbo].[PointsRulesConfig] WITH ( NOLOCK )
                                          WHERE     1 = 1  " + sqlStr + @"
                                        ) AS PG
                                WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                     AND     STR(@PageIndex * @PageSize)
                                " ;
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[PointsRulesConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsRulesConfig>().ToList();

        }


        public static PointsRulesConfig GetPointsRulesConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Content]
                                      ,[Status]
                                      ,[CreateTime]
                                  FROM [Configuration].[dbo].[PointsRulesConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsRulesConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertPointsRulesConfig(PointsRulesConfig model)
        {
            const string sql = @"INSERT INTO Configuration..PointsRulesConfig
                                          (    [Content]
                                              ,[Status]
                                              ,[CreateTime]
                                          )
                                  VALUES(     @Content  
                                              ,@Status                                           
                                              ,GETDATE()                                          
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Content",model.Content??string.Empty)
               
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdatePointsRulesConfig(PointsRulesConfig model)
        {
            const string sql = @"UPDATE Configuration..PointsRulesConfig SET                                      
                                        Content=@Content  
                                        ,Status=@Status                                           
                                        ,CreateTime=GETDATE()  
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Content",model.Content??string.Empty),
                    new SqlParameter("@Id",model.Id)                
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeletePointsRulesConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.PointsRulesConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
