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

    public class DALPersonalCenterConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);


        public static List<PersonalCenterConfig> GetPersonalCenterConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY [PersonalCenterConfig].CreateTime DESC ) AS ROWNUMBER ,
                                                [PersonalCenterConfig].[Id] ,
                                                [Location] ,
                                                [Sort] ,
                                                [Grade] ,
                                                [Status] ,
                                                [Image] ,
                                                [Link] ,
                                                [IOSProcessValue] ,
                                                [AndroidProcessValue] ,
                                                [IOSCommunicationValue] ,
                                                [AndroidCommunicationValue] ,
                                                [PersonalCenterConfig].[CreateTime] ,
                                                SE_VIPAuthorizationRuleConfig.RuleName
                                      FROM      [Configuration].[dbo].[PersonalCenterConfig] WITH ( NOLOCK )
                                                LEFT JOIN Configuration..SE_VIPAuthorizationRuleConfig
                                                WITH ( NOLOCK ) ON SE_VIPAuthorizationRuleConfig.Id = [PersonalCenterConfig].VIPAuthorizationRuleId
                                      WHERE     1 = 1    " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)

                             ";
            string sqlCount = @"SELECT COUNT(1)  FROM [Configuration].[dbo].[PersonalCenterConfig] WITH (NOLOCK)
					   LEFT JOIN Configuration..SE_VIPAuthorizationRuleConfig WITH (NOLOCK)
                    ON SE_VIPAuthorizationRuleConfig.Id = [PersonalCenterConfig].VIPAuthorizationRuleId WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PersonalCenterConfig>().ToList();

        }


        public static PersonalCenterConfig GetPersonalCenterConfig(int id)
        {
            const string sql = @"SELECT *
                      FROM [Configuration].[dbo].[PersonalCenterConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PersonalCenterConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertPersonalCenterConfig(PersonalCenterConfig model)
        {
            const string sql = @"  INSERT INTO Configuration..PersonalCenterConfig
                                          (Location,
                                            Sort,
                                            Grade,
                                            UrlType,
                                            Route,
                                            Status,
                                            Image,
                                            Link,
                                            IOSProcessValue,
                                            AndroidProcessValue,
                                            IOSCommunicationValue,
                                            AndroidCommunicationValue,
                                            CreateTime,
                                            VIPAuthorizationRuleId
                                          )
                                  VALUES(   @Location, --Location - int
                                            @Sort, --Sort - int
                                            @Grade, --Grade - int
                                            @UrlType,
                                            @Route,
                                            @Status, --Status - bit
                                            @Image, --Image - nvarchar(1000)
                                            @Link, --Link - nvarchar(500)
                                            @IOSProcessValue, --IOSProcessValue - nvarchar(500)
                                            @AndroidProcessValue, --AndroidProcessValue - nvarchar(500)
                                            @IOSCommunicationValue, --IOSCommunicationValue - nvarchar(500)
                                            @AndroidCommunicationValue, --AndroidCommunicationValue - nvarchar(500)
                                            GETDATE(),
                                            @VIPAuthorizationRuleId
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Location",model.Location),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@UrlType",model.UrlType),
                    new SqlParameter("@Route",model.Route),
                    new SqlParameter("@Image",model.Image),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@VIPAuthorizationRuleId",model.VIPAuthorizationRuleId)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdatePersonalCenterConfig(PersonalCenterConfig model)
        {
            const string sql = @"UPDATE Configuration..PersonalCenterConfig SET
                                        Location=@Location, 
                                        Sort=@Sort, 
                                        Grade=@Grade,
                                        UrlType=@UrlType,
                                        Route=@Route,
                                        Status=@Status, 
                                        Image=@Image, 
                                        Link=@Link, 
                                        IOSProcessValue=@IOSProcessValue,
                                        AndroidProcessValue=@AndroidProcessValue, 
                                        IOSCommunicationValue=@IOSCommunicationValue, 
                                        AndroidCommunicationValue=@AndroidCommunicationValue,
                                        CreateTime= GETDATE(),
                                        VIPAuthorizationRuleId=@VIPAuthorizationRuleId
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Location",model.Location),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Grade",model.Grade),
                    new SqlParameter("@UrlType",model.UrlType),
                    new SqlParameter("@Route",model.Route),
                    new SqlParameter("@Image",model.Image),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@VIPAuthorizationRuleId",model.VIPAuthorizationRuleId)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeletePersonalCenterConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.PersonalCenterConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
