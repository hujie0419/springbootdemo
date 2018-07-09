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
    public class DAlPointsCenterConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<PointsCenterConfig> GetPointsCenterConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [Sort] ,
                                                [Grade] ,
                                                [Status] ,
                                                [Image] ,
                                                [Link] ,
                                                [IOSProcessValue] ,
                                                [AndroidProcessValue] ,
                                                [IOSCommunicationValue] ,
                                                [AndroidCommunicationValue] ,
                                                [CreateTime] ,
                                                Description
                                      FROM      [Configuration].[dbo].[PointsCenterConfig] WITH ( NOLOCK )
                                      WHERE     1 = 1  " + sqlStr + @"
                                    ) AS PG 
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                            ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[PointsCenterConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsCenterConfig>().ToList();

        }


        public static PointsCenterConfig GetPointsCenterConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Sort]
                                      ,[Grade]
                                      ,[Status]
                                      ,[Image]
                                      ,[Link]
                                      ,[IOSProcessValue]
                                      ,[AndroidProcessValue]
                                      ,[IOSCommunicationValue]
                                      ,[AndroidCommunicationValue]
                                      ,[CreateTime]
                                      ,Description
                                  FROM [Configuration].[dbo].[PointsCenterConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsCenterConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertPointsCenterConfig(PointsCenterConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..PointsCenterConfig
                                          (Sort ,
                                            Grade ,
                                            Status ,
                                            Image ,
                                            Link ,
                                            IOSProcessValue ,
                                            AndroidProcessValue ,
                                            IOSCommunicationValue ,
                                            AndroidCommunicationValue ,
                                            CreateTime,
                                            Description
                                          )
                                  VALUES(  
                                            @Sort, --Sort - int
                                            @Grade, --Grade - int
                                            @Status, --Status - bit
                                            @Image, --Image - nvarchar(1000)
                                            @Link, --Link - nvarchar(500)
                                            @IOSProcessValue, --IOSProcessValue - nvarchar(500)
                                            @AndroidProcessValue, --AndroidProcessValue - nvarchar(500)
                                            @IOSCommunicationValue, --IOSCommunicationValue - nvarchar(500)
                                            @AndroidCommunicationValue, --AndroidCommunicationValue - nvarchar(500)
                                            GETDATE(),-- CreateTime - datetime
                                            @Description
                                          )";

            var sqlParameter = new SqlParameter[]
                {                  
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@Image",model.Image),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Description",model.Description??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdatePointsCenterConfig(PointsCenterConfig model)
        {
            const string sql = @"UPDATE Configuration..PointsCenterConfig SET                                      
                                        Sort=@Sort, 
                                        Grade=@Grade,
                                        Status=@Status, 
                                        Image=@Image, 
                                        Link=@Link, 
                                        IOSProcessValue=@IOSProcessValue,
                                        AndroidProcessValue=@AndroidProcessValue, 
                                        IOSCommunicationValue=@IOSCommunicationValue, 
                                        AndroidCommunicationValue=@AndroidCommunicationValue,
                                        CreateTime= GETDATE(),
                                        Description=@Description
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {                  
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@Image",model.Image),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Description",model.Description??string.Empty)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeletePointsCenterConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.PointsCenterConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

    }
}
