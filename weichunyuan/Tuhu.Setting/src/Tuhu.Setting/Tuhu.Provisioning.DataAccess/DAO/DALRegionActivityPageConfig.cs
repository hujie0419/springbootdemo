using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
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
    public class DALRegionActivityPageConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<ActivityPageConfig> GetActivityPageConfig(ActivityPageConfig model, int pageSize, int pageIndex, out int totalIteme)
        {
            string sql = @"SELECT [Id]
                                  ,[DefaultUrlId]
                                  ,[Name]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[CreateName]
                                  ,[UpdateName]
                                  ,[ShareParameters]
                              FROM [Configuration].[dbo].[SE_ActivityPageConfig] AS A WITH (NOLOCK)                                
                                    ORDER BY  A.[Id] DESC 
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_ActivityPageConfig] AS A WITH (NOLOCK)";

            var sqlParameters = new SqlParameter[]
                   {
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex)
                   };
            totalIteme = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ActivityPageConfig>().ToList();

        }

        public static ActivityPageConfig GetActivityPageConfigById(int id)
        {
            const string sql = @"SELECT [Id]
                                  ,[DefaultUrlId]
                                  ,[Name]
                                  ,[StartTime]
                                  ,[EndTime]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[CreateName]
                                  ,[UpdateName]
                                  ,[ShareParameters]
                              FROM [Configuration].[dbo].[SE_ActivityPageConfig]  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ActivityPageConfig>().ToList().FirstOrDefault();
        }

        public static List<ActivityPageUrlConfig> GetActivityPageUrlConfigById(int id)
        {
            const string sql = @"
                                SELECT [Id]
                                      ,[ActivityPageId]
                                      ,[Name]
                                      ,[Url]
                                      ,[CreateTime]
                                      ,[CreateName]
                                      ,[UpdateTime]
                                      ,[UpdateName]
                                  FROM [Configuration].[dbo].[SE_ActivityPageUrlConfig]  WHERE ActivityPageId=@ActivityPageId";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityPageId",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ActivityPageUrlConfig>().ToList();
        }
        public static bool InsertActivityPageConfig(ActivityPageConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Configuration..[SE_ActivityPageConfig]
                                              (                                        
                                               [DefaultUrlId]
                                              ,[Name]
                                              ,[StartTime]
                                              ,[EndTime]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[CreateName]
                                              ,[UpdateName]
                                              ,[ShareParameters]
                                              )
                                      VALUES  ( 
                                               @DefaultUrlId
                                              ,@Name                                            
                                              ,@StartTime
                                              ,@EndTime
                                              ,GETDATE()
                                              ,GETDATE()
                                              ,@CreateName
                                              ,@UpdateName 
                                              ,@ShareParameters                                                                               
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@CreateName",model.CreateName??"途虎测试"),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@DefaultUrlId",model.DefaultUrlId),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@UpdateName",model.UpdateName??"途虎测试"),
                    new SqlParameter("@ShareParameters",model.ShareParameters),

                };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return id > 0;
        }


        public static bool InsertActivityPageUrlConfig(ActivityPageUrlConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Configuration..[SE_ActivityPageUrlConfig]
                                              (                                        
                                               [ActivityPageId]
                                              ,[Url]
                                              ,[CreateTime]
                                              ,[CreateName]
                                              ,[UpdateTime]
                                              ,[UpdateName]
                                              ,[Name]
                                              )
                                      VALUES  ( 
                                               @ActivityPageId
                                              ,@Url
                                              ,GETDATE()
                                              ,@CreateName
                                              ,GETDATE()
                                              ,@UpdateName 
                                              ,@Name                                                                              
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@CreateName",model.CreateName??"途虎测试"),
                    new SqlParameter("@Url",model.Url),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@ActivityPageId",model.ActivityPageId),
                    new SqlParameter("@UpdateName",model.UpdateName??"途虎测试"),
                };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return id > 0;
        }

        public static bool UpdateActivityPageConfig(ActivityPageConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_ActivityPageConfig SET                                      
                                        [DefaultUrlId]=@DefaultUrlId
                                        ,[Name]=@Name
                                        ,[StartTime]=@StartTime
                                        ,[EndTime]=@EndTime                                        
                                        ,[UpdateTime]=GETDATE()                                            
                                        ,[UpdateName]=@UpdateName
                                        ,[ShareParameters]=@ShareParameters                                      
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@CreateName",model.CreateName),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@DefaultUrlId",model.DefaultUrlId),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@UpdateName",model.UpdateName),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@ShareParameters",model.ShareParameters??string.Empty)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
        public static bool UpdateActivityPageUrlConfig(ActivityPageUrlConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_ActivityPageUrlConfig SET 
                                         [Url]=@Url                                       
                                        ,[ActivityPageId]=@ActivityPageId
                                        ,[UpdateTime]=GETDATE()
                                        ,[UpdateName]=@UpdateName
                                        ,[Name]=@Name                                     
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@CreateName",model.CreateName??"途虎测试"),
                    new SqlParameter("@Url",model.Url),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@ActivityPageId",model.ActivityPageId),
                    new SqlParameter("@UpdateName",model.UpdateName??"途虎测试"),
                    new SqlParameter("@Id",model.Id),
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
        public static bool DeleteActivityPageConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_ActivityPageConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool DeleteActivityPageUrlConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_ActivityPageUrlConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool DeleteActivityPageRegionConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_ActivityPageRegionConfig WHERE UrlId=@Id";

            if (id == 0)
            {
                return true;
            }
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool InsertActivityPageRegionConfig(ActivityPageRegionConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Configuration..[SE_ActivityPageRegionConfig]
                                              (                                        
                                               [UrlId]
                                              ,[Province]
                                              ,[City]
                                              ,[CreateTime]
                                              ,[CreateName]
                                              )
                                      VALUES  ( 
                                               @UrlId
                                              ,@Province
                                              ,@City
                                              ,GETDATE()
                                              ,@CreateName                                       
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@UrlId",model.UrlId),
                    new SqlParameter("@Province",model.Province),
                    new SqlParameter("@City",model.City),
                    new SqlParameter("@CreateName",model.CreateName??"途虎测试")
                };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return id > 0;
        }

        public static List<ActivityPageRegionConfig> GetRegionRelation(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[UrlId]
                                      ,[Province]
                                      ,[City]
                                      ,[CreateTime]
                                  FROM[Configuration].[dbo].[SE_ActivityPageRegionConfig] WHERE UrlId =@UrlId ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@UrlId", id)).ConvertTo<ActivityPageRegionConfig>().ToList();
        }

        public static List<ActivityPageRegionConfig> GetRegionRelationGroup(int id)
        {
            const string sql = @"SELECT 
                                      Province                                                                     
                                  FROM[Configuration].[dbo].[SE_ActivityPageRegionConfig] WHERE UrlId =@UrlId GROUP BY Province";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@UrlId", id)).ConvertTo<ActivityPageRegionConfig>().ToList();
        }

        public static List<ActivityPageRegionConfig> GetRegionRelation(int id, string name)
        {
            const string sql = @"SELECT [Id]
                                      ,[UrlId]
                                      ,[Province]
                                      ,[City]
                                      ,[CreateTime]
                                  FROM[Configuration].[dbo].[SE_ActivityPageRegionConfig] WHERE UrlId =@UrlId AND Province=@Province ";

            var sqlPram = new SqlParameter[] {
               new SqlParameter("@UrlId", id),
               new SqlParameter("@Province", name)
            };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlPram).ConvertTo<ActivityPageRegionConfig>().ToList();
        }
    }
}

