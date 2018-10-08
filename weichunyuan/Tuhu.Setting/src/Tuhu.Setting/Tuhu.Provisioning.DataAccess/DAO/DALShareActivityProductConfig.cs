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
    public class DALShareActivityProductConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<ShareActivityProductConfig> GetShareActivityProductConfigList(ShareActivityProductConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                         FROM    ( SELECT  
                                            [Id] ,
                                            [Status] ,
                                            [ProductId] ,
                                            [ProductName] ,
                                            [CreateTime]
                                  FROM      [Configuration].[dbo].[ShareActivityProductConfig] WITH ( NOLOCK )
                                  WHERE      
                                            ( @ProductId = ''
                                              OR ( @ProductId <> ''
                                                   AND ProductId = @ProductId
                                                 )
                                            )
                                ) AS PG
                          ORDER BY PG.CreateTime DESC
                          OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS   ONLY ";
            string sqlCount = @"SELECT 
                                COUNT(1)
                                FROM [Configuration].[dbo].[ShareActivityProductConfig] WITH (NOLOCK) 
                                WHERE  ( @ProductId = ''
                                            OR ( @ProductId <> ''
                                                AND ProductId = @ProductId
                                                )
                                            )";
            var sqlParameters = new SqlParameter[]
                  {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                        new SqlParameter("@ProductId",model.ProductId??"")
                  };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ShareActivityProductConfig>().ToList();

        }

        public static ShareActivityProductConfig GetShareActivityProductConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Status]
                                      ,[ProductId]
                                      ,[ProductName]
                                      ,[CreateTime]
                                  FROM [Configuration].[dbo].[ShareActivityProductConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ShareActivityProductConfig>().ToList().FirstOrDefault();

        }

        public static List<ShareActivityProductConfig> GetShareActivityProduct()
        {
            const string sql = @"SELECT 
                                       [ProductId]
                                      ,[ProductName]                                   
                                  FROM [Configuration].[dbo].[ShareActivityProductConfig] WITH (NOLOCK)";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<ShareActivityProductConfig>().ToList();
        }

        public static bool InsertShareActivityProductConfig(ShareActivityProductConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..ShareActivityProductConfig
                                          (    [Status]
                                              ,[ProductId]
                                              ,[ProductName]
                                              ,[CreateTime]
                                          )
                                  VALUES(     @Status
                                              ,@ProductId
                                              ,@ProductName
                                              ,GETDATE()
                                          
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@ProductId",model.ProductId),
                    new SqlParameter("@ProductName",model.ProductName??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdateShareActivityProductConfig(ShareActivityProductConfig model)
        {
            const string sql = @"UPDATE Configuration..ShareActivityProductConfig SET                                      
                                            Status=@Status
                                            ,ProductId=@ProductId
                                            ,ProductName=@ProductName
                                            ,CreateTime=GETDATE()
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@ProductId",model.ProductId),
                    new SqlParameter("@ProductName",model.ProductName??string.Empty),
                    new SqlParameter("@Id",model.Id)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteShareActivityProductConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.ShareActivityProductConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
