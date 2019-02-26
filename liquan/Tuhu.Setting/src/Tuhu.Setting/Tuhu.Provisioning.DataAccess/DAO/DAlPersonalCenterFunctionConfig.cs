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
    public class DALPersonalCenterFunctionConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<PersonalCenterFunctionConfig> GetPersonalCenterFunctionConfigList(PersonalCenterFunctionConfig sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" 
                        SELECT  *
                        FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                            [Id] ,
                                            [Icon] ,
                                            [Title] ,
                                            [Sort] ,
                                            [Status] ,
                                            [Highlight] ,
                                            [CreateTime] ,
                                            DisplayName ,
                                            [AppLink],
                                            [IOSStartVersions],
                                            [IOSEndVersions],
                                            [AndroidStartVersions],
                                            [AndroidEndVersions]
                                  FROM      [Configuration].[dbo].[PersonalCenterFunctionConfig] WITH ( NOLOCK )
                                  WHERE     1 = 1   
                                ) AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize)
                          ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[PersonalCenterFunctionConfig] WITH (NOLOCK)  WHERE 1=1  ";
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PersonalCenterFunctionConfig>().ToList();

        }


        public static PersonalCenterFunctionConfig GetPersonalCenterFunctionConfig(int id)
        {
            const string sql = @"SELECT
                                       [Id]
                                      ,[Icon]
                                      ,[Title]
                                      ,[Sort]
                                      ,[Status]
                                      ,[Highlight]
                                      ,[CreateTime]
                                      ,DisplayName
                                      ,[AppLink]
                                      ,[IOSStartVersions]
                                      ,[IOSEndVersions]
                                      ,[AndroidStartVersions]
                                      ,[AndroidEndVersions]
                                  FROM [Configuration].[dbo].[PersonalCenterFunctionConfig] WITH (NOLOCK) WHERE Id=@id";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PersonalCenterFunctionConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertPersonalCenterFunctionConfig(PersonalCenterFunctionConfig model)
        {
            const string sql = @"INSERT INTO Configuration..PersonalCenterFunctionConfig
                                          (    
                                             [Icon]
                                            ,[Title]
                                            ,[Sort]
                                            ,[Status]
                                            ,[Highlight]
                                            ,[CreateTime]
                                            ,DisplayName
                                            ,[AppLink]
                                            ,[IOSStartVersions]
                                            ,[IOSEndVersions]
                                            ,[AndroidStartVersions]
                                            ,[AndroidEndVersions]
                                          )
                                  VALUES(  @Icon
                                          ,@Title
                                          ,@Sort
                                          ,@Status
                                          ,@Highlight
                                          ,GETDATE() 
                                          ,@DisplayName 
                                          ,@AppLink
                                          ,@IOSStartVersions
                                          ,@IOSEndVersions
                                          ,@AndroidStartVersions
                                          ,@AndroidEndVersions                                      
                                        )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Highlight",model.Highlight),
                    new SqlParameter("@Icon",model.Icon??string.Empty),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Title",model.Title??string.Empty),
                    new SqlParameter("@DisplayName",model.DisplayName??string.Empty),
                    new SqlParameter("@AppLink",model.AppLink??string.Empty),
                    new SqlParameter("@IOSEndVersions",model.IOSEndVersions??string.Empty),
                    new SqlParameter("@IOSStartVersions",model.IOSStartVersions??string.Empty),
                    new SqlParameter("@AndroidEndVersions",model.AndroidEndVersions??string.Empty),
                    new SqlParameter("@AndroidStartVersions",model.AndroidStartVersions??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdatePersonalCenterFunctionConfig(PersonalCenterFunctionConfig model)
        {
            const string sql = @"UPDATE Configuration..PersonalCenterFunctionConfig SET                                      
                                           Icon=@Icon
                                          ,Title=@Title
                                          ,Sort=@Sort
                                          ,Status=@Status
                                          ,Highlight=@Highlight
                                          ,CreateTime=GETDATE() 
                                          ,DisplayName=@DisplayName
                                          ,AppLink=@AppLink
                                          ,IOSStartVersions=@IOSStartVersions
                                          ,IOSEndVersions=@IOSEndVersions
                                          ,AndroidStartVersions=@AndroidStartVersions
                                          ,AndroidEndVersions=@AndroidEndVersions  
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                  new SqlParameter("@Highlight",model.Highlight),
                  new SqlParameter("@Id",model.Id),
                  new SqlParameter("@Icon",model.Icon??string.Empty),
                  new SqlParameter("@Sort",model.Sort),
                  new SqlParameter("@Status",model.Status),
                  new SqlParameter("@Title",model.Title??string.Empty),
                  new SqlParameter("@DisplayName",model.DisplayName??string.Empty),
                  new SqlParameter("@AppLink",model.AppLink??string.Empty),
                  new SqlParameter("@IOSEndVersions",model.IOSEndVersions??string.Empty),
                  new SqlParameter("@IOSStartVersions",model.IOSStartVersions??string.Empty),
                  new SqlParameter("@AndroidEndVersions",model.AndroidEndVersions??string.Empty),
                  new SqlParameter("@AndroidStartVersions",model.AndroidStartVersions??string.Empty)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeletePersonalCenterFunctionConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.PersonalCenterFunctionConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
