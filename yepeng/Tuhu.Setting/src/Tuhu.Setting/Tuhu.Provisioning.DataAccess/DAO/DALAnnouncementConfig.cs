using Microsoft.ApplicationBlocks.Data;
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
    public class DALAnnouncementConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<AnnouncementConfig> GetAnnouncementConfigList(AnnouncementConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"
                            SELECT  *
                            FROM    ( SELECT   ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                              [PKID]
                                              ,[Content]     
                                              ,[ImageUrl]
                                              ,[NoticeType]                                           
                                              ,[IsDeleted]
                                              ,[CreateTime]
                                              ,[CreatedUser]
                                              ,[H5Url]
                                              ,[IOSProcessValue]
                                              ,[AndroidProcessValue]
                                              ,[IOSCommunicationValue]
                                              ,[AndroidCommunicationValue]
	                                          ,Status
                                      FROM   [BaoYang].[dbo].[BaoYangNotice] WITH ( NOLOCK )   
                                      WHERE   IsDeleted=0                                
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                            ";
            string sqlCount = @"SELECT COUNT(1) FROM [BaoYang].[dbo].[BaoYangNotice] WITH (NOLOCK) WHERE IsDeleted=0 ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<AnnouncementConfig>().ToList();

        }


        public static AnnouncementConfig GetAnnouncementConfig(int id)
        {
            const string sql = @"SELECT [PKID]
                                      ,[Content]     
                                      ,[ImageUrl]
                                      ,[NoticeType]                                    
                                      ,[IsDeleted]
                                      ,[CreateTime]
                                      ,[CreatedUser]
                                      ,[H5Url]
                                      ,[IOSProcessValue]
                                      ,[AndroidProcessValue]
                                      ,[IOSCommunicationValue]
                                      ,[AndroidCommunicationValue]
	                                  ,Status
                                  FROM [BaoYang].[dbo].[BaoYangNotice] WITH (NOLOCK) WHERE PKID=@id";

            var sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@id",id)
                };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<AnnouncementConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertAnnouncementConfig(AnnouncementConfig model)
        {
            const string sql = @"  INSERT INTO [BaoYang].[dbo].[BaoYangNotice]
                                          ( 
                                           [Content]     
                                          ,[ImageUrl]
                                          ,[NoticeType]                                       
                                          ,[IsDeleted]
                                          ,[CreateTime]
                                          ,[CreatedUser]
                                          ,[H5Url]
                                          ,[IOSProcessValue]
                                          ,[AndroidProcessValue]
                                          ,[IOSCommunicationValue]
                                          ,[AndroidCommunicationValue]
	                                      ,Status
                                          )
                                  VALUES(  
                                           @Content
                                          ,@ImageUrl                                        
                                          ,@NoticeType                                       
                                          ,@IsDeleted
                                          ,GETDATE()
                                          ,@CreatedUser
                                          ,@H5Url
                                          ,@IOSProcessValue
                                          ,@AndroidProcessValue
                                          ,@IOSCommunicationValue
                                          ,@AndroidCommunicationValue
	                                      ,@Status
                                          )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Content",model.Content??string.Empty),
                    new SqlParameter("@CreatedUser",model.CreatedUser??string.Empty),
                    new SqlParameter("@H5Url",model.H5Url??string.Empty),
                    new SqlParameter("@ImageUrl",model.ImageUrl??string.Empty),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@IsDeleted",model.IsDeleted),
                    new SqlParameter("@NoticeType",model.NoticeType),
                    new SqlParameter("@Status",model.Status)
                };

            try
            {
                int newid = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
                if (newid > 0)
                {
                    if (model.Status)
                    {
                        UpdateStatus(newid);
                    }

                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch
            {

                return false;
            }

        }

        public static bool UpdateAnnouncementConfig(AnnouncementConfig model)
        {
            const string sql = @"UPDATE [BaoYang].[dbo].[BaoYangNotice] SET                                      
                                           [Content]=@Content  
                                          ,[ImageUrl]=@ImageUrl                                                                     
                                          ,[IsDeleted]=@IsDeleted
                                          ,[CreateTime]=GETDATE()
                                          ,[CreatedUser]=@CreatedUser
                                          ,[H5Url]=@H5Url
                                          ,[IOSProcessValue]=@IOSProcessValue
                                          ,[AndroidProcessValue]=@AndroidProcessValue
                                          ,[IOSCommunicationValue]=@IOSCommunicationValue
                                          ,[AndroidCommunicationValue]=@AndroidCommunicationValue
	                                      ,Status=@Status
                                WHERE PKID=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@Content",model.Content??string.Empty),
                    new SqlParameter("@CreatedUser",model.CreatedUser??string.Empty),
                    new SqlParameter("@H5Url",model.H5Url??string.Empty),
                    new SqlParameter("@ImageUrl",model.ImageUrl??string.Empty),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue),
                    new SqlParameter("@IsDeleted",model.IsDeleted),
                    new SqlParameter("@NoticeType",model.NoticeType),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Id",model.PKID)
               };
                   
            try
            {
                if (model.Status)
                {
                    UpdateStatus(model.PKID);
                }

                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter);

                return true;
            }
            catch
            {
                return true;
            }


        }

        public static bool UpdateStatus(int id)
        {

            const string sql = @"UPDATE [BaoYang].[dbo].[BaoYangNotice] SET 
	                                      Status=0
                                WHERE PKID<>@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool DeleteAnnouncementConfig(int id)
        {
            const string sql = @"UPDATE [BaoYang].[dbo].[BaoYangNotice] SET                                      
                                          IsDeleted=@IsDeleted
                                WHERE PKID=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Id",id),
                    new SqlParameter("@IsDeleted",true)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
    }
}

