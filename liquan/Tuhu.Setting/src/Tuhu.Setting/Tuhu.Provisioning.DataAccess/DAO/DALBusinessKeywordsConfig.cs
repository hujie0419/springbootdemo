using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
    public class DALBusinessKeywordsConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<BusinessKeywordsConfig> GetBusinessKeywordsConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                            [Id] ,
                                            [Keywords] ,
                                            [CreateTime] ,
                                            Sort
                                  FROM      [Configuration].[dbo].[BusinessKeywordsConfig] WITH ( NOLOCK )
                                  WHERE     1 = 1   " + sqlStr + @"
                                ) AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize)
                        ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[BusinessKeywordsConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BusinessKeywordsConfig>().ToList();

        }


        public static BusinessKeywordsConfig GetBusinessKeywordsConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Keywords]
                                      ,[CreateTime]
                                      ,Sort
                                  FROM [Configuration].[dbo].[BusinessKeywordsConfig] WITH (NOLOCK) WHERE Id=@id";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BusinessKeywordsConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertBusinessKeywordsConfig(BusinessKeywordsConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..BusinessKeywordsConfig
                                          ([Keywords]
                                          ,[CreateTime]
                                          ,Sort
                                          )
                                  VALUES( @Keywords
                                          ,GETDATE() 
                                          ,@Sort                                         
                                        )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Keywords",model.Keywords),
                    new SqlParameter("@Sort",model.Sort)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdateBusinessKeywordsConfig(BusinessKeywordsConfig model)
        {
            const string sql = @"UPDATE Configuration..BusinessKeywordsConfig SET                                      
                                              Keywords=@Keywords                                        
                                              ,CreateTime=GETDATE(),
                                               Sort=@Sort
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                  new SqlParameter("@Keywords",model.Keywords??string.Empty),
                  new SqlParameter("@Id",model.Id),
                  new SqlParameter("@Sort",model.Sort)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteBusinessKeywordsConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.BusinessKeywordsConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
