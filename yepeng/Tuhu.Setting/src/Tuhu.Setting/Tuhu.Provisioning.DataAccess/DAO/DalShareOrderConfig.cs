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
    public class DalShareOrderConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(strConn);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        public static List<ShareOrderConfig> GetShareOrderConfig(ShareOrderConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  [Id]
                                  ,[AppLink]
                                  ,[IOSStartVersions]
                                  ,[IOSEndVersions]
                                  ,[AndroidStartVersions]
                                  ,[AndroidEndVersions]
                                  ,[Prompt]
                                  ,[ButtonValue]
                                  ,[PreviewIntroduce]
                                  ,[Image]
                                  ,[QRCodeIntroduce]
                                  ,[QRCodeLink]
                                  ,[CreateTime]
                                  ,[Status]
                            FROM    [Configuration].[dbo].[SE_ShareOrderConfig] AS A WITH (NOLOCK)                                 
                                    ORDER BY  A.[CreateTime] DESC 
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_ShareOrderConfig] AS A WITH (NOLOCK)";

            var sqlParameters = new SqlParameter[]
                   {
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex)
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ShareOrderConfig>().ToList();

        }


        public static ShareOrderConfig GetShareOrderConfigById(int id)
        {
            const string sql = @"SELECT   [Id]
                                          ,[AppLink]
                                          ,[IOSStartVersions]
                                          ,[IOSEndVersions]
                                          ,[AndroidStartVersions]
                                          ,[AndroidEndVersions]
                                          ,[Prompt]
                                          ,[ButtonValue]
                                          ,[PreviewIntroduce]
                                          ,[Image]
                                          ,[QRCodeIntroduce]
                                          ,[QRCodeLink]
                                          ,[CreateTime]
                                          ,[Status]
                              FROM [Configuration].[dbo].[SE_ShareOrderConfig] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<ShareOrderConfig>().ToList().FirstOrDefault();
        }


        public static bool InsertShareOrderConfig(ShareOrderConfig model, ref int newId)
        {
            const string sql = @"INSERT INTO [Configuration].[dbo].[SE_ShareOrderConfig]
                                        (                                        
                                           [AppLink]
                                          ,[IOSStartVersions]
                                          ,[IOSEndVersions]
                                          ,[AndroidStartVersions]
                                          ,[AndroidEndVersions]
                                          ,[Prompt]
                                          ,[ButtonValue]
                                          ,[PreviewIntroduce]
                                          ,[Image]
                                          ,[QRCodeIntroduce]
                                          ,[QRCodeLink]
                                          ,[CreateTime]
                                          ,[Status]
                                        )
                                VALUES  (  @AppLink
                                          ,@IOSStartVersions
                                          ,@IOSEndVersions
                                          ,@AndroidStartVersions                                   
                                          ,@AndroidEndVersions
                                          ,@Prompt
                                          ,@ButtonValue                                    
                                          ,@PreviewIntroduce
                                          ,@Image
                                          ,@QRCodeIntroduce
                                          ,@QRCodeLink
                                          ,GETDATE()
                                          ,@Status
                                        )SELECT @@IDENTITY
                                ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@AppLink",model.AppLink??string.Empty),
                 new SqlParameter("@AndroidEndVersions",model.AndroidEndVersions??string.Empty),
                  new SqlParameter("@AndroidStartVersions",model.AndroidStartVersions??string.Empty),
                   new SqlParameter("@ButtonValue",model.ButtonValue??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                     new SqlParameter("@IOSEndVersions",model.IOSEndVersions??string.Empty),
                      new SqlParameter("@IOSStartVersions",model.IOSStartVersions??string.Empty),
                       new SqlParameter("@PreviewIntroduce",model.PreviewIntroduce??string.Empty),
                        new SqlParameter("@QRCodeIntroduce",model.QRCodeIntroduce??string.Empty),
                         new SqlParameter("@QRCodeLink",model.QRCodeLink??string.Empty),
                          new SqlParameter("@Status",model.Status),
                           new SqlParameter("@Prompt",model.Prompt??string.Empty)

            };

            newId = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameters));
            return newId > 0;
        }


        public static bool UpdateShareOrderConfig(ShareOrderConfig model)
        {
            const string sql = @"UPDATE  [Configuration].[dbo].[SE_ShareOrderConfig]
                                SET        AppLink=@AppLink
                                          ,IOSStartVersions=@IOSStartVersions
                                          ,IOSEndVersions=@IOSEndVersions
                                          ,AndroidStartVersions=@AndroidStartVersions                                   
                                          ,AndroidEndVersions=@AndroidEndVersions
                                          ,Prompt=@Prompt
                                          ,ButtonValue=@ButtonValue                                    
                                          ,PreviewIntroduce=@PreviewIntroduce
                                          ,Image=@Image
                                          ,QRCodeIntroduce=@QRCodeIntroduce
                                          ,QRCodeLink=@QRCodeLink
                                          ,Status=@Status
                                          ,CreateTime=GETDATE()
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
              {
                new SqlParameter("@AppLink",model.AppLink??string.Empty),
                 new SqlParameter("@AndroidEndVersions",model.AndroidEndVersions??string.Empty),
                  new SqlParameter("@AndroidStartVersions",model.AndroidStartVersions??string.Empty),
                   new SqlParameter("@ButtonValue",model.ButtonValue??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                     new SqlParameter("@IOSEndVersions",model.IOSEndVersions??string.Empty),
                      new SqlParameter("@IOSStartVersions",model.IOSStartVersions??string.Empty),
                       new SqlParameter("@PreviewIntroduce",model.PreviewIntroduce??string.Empty),
                        new SqlParameter("@QRCodeIntroduce",model.QRCodeIntroduce??string.Empty),
                         new SqlParameter("@QRCodeLink",model.QRCodeLink??string.Empty),
                          new SqlParameter("@Status",model.Status),
                           new SqlParameter("@Prompt",model.Prompt??string.Empty),
                             new SqlParameter("@Id",model.Id),
              };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }

        public static bool DeleteShareOrderConfig(int id)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[SE_ShareOrderConfig] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }


        //------推送消息配置
        public static List<OrderSharedPushMessageConfig> GetOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  [Id]
                                  ,[Type]
                                  ,[MessageTilte]
                                  ,[MessageContent]
                                  ,[PushTitile]
                                  ,[PushContent]
                                  ,[CreateTime]                             
                                  ,[IOSCommunicationValue]
                                  ,[AndroidCommunicationValue]
                          FROM [Configuration].[dbo].[SE_OrderSharedPushMessageConfig] AS A WITH (NOLOCK)                                 
                                    ORDER BY  A.[CreateTime] DESC 
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_OrderSharedPushMessageConfig] AS A WITH (NOLOCK)";

            var sqlParameters = new SqlParameter[]
                   {
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex)
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<OrderSharedPushMessageConfig>().ToList();

        }

        public static OrderSharedPushMessageConfig GetOrderSharedPushMessageConfig()
        {
            const string sql = @"SELECT TOP 1 [Id]
                                  ,[Type]
                                  ,[MessageTilte]
                                  ,[MessageContent]
                                  ,[PushTitile]
                                  ,[PushContent]                             
                                  ,[CreateTime]
                                  ,[IOSCommunicationValue]
                                  ,[AndroidCommunicationValue]
                              FROM [Configuration].[dbo].[SE_OrderSharedPushMessageConfig] ORDER BY CreateTime DESC";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<OrderSharedPushMessageConfig>().ToList().FirstOrDefault();
        }

        public static OrderSharedPushMessageConfig GetOrderSharedPushMessageConfig(int id)
        {
            const string sql = @"SELECT   [Id]
                                          ,[Type]
                                          ,[MessageTilte]
                                          ,[MessageContent]
                                          ,[PushTitile]
                                          ,[PushContent]                                     
                                          ,[CreateTime]                                   
                                          ,[IOSCommunicationValue]
                                          ,[AndroidCommunicationValue]
                              FROM [Configuration].[dbo].[SE_OrderSharedPushMessageConfig] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<OrderSharedPushMessageConfig>().ToList().FirstOrDefault();
        }


        public static bool InsertOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model, ref int newId)
        {
            const string sql = @"INSERT INTO [Configuration].[dbo].[SE_OrderSharedPushMessageConfig]
                                        (                                        
                                           [Type]
                                          ,[MessageTilte]
                                          ,[MessageContent]
                                          ,[PushTitile]
                                          ,[PushContent]
                                          ,[CreateTime]
                                          ,IOSCommunicationValue
                                          ,AndroidCommunicationValue
                                        )
                                VALUES  (  @Type
                                          ,@MessageTilte
                                          ,@MessageContent
                                          ,@PushTitile
                                          ,@PushContent                                      
                                          ,GETDATE()
                                          ,@IOSCommunicationValue
                                          ,@AndroidCommunicationValue
                                        )SELECT @@IDENTITY
                                ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Type",model.Type??string.Empty),
                 new SqlParameter("@MessageTilte",model.MessageTilte??string.Empty),
                  new SqlParameter("@MessageContent",model.MessageContent??string.Empty),
                   new SqlParameter("@PushTitile",model.PushTitile??string.Empty),
                    new SqlParameter("@PushContent",model.PushContent??string.Empty),
                      new SqlParameter("@Id",model.Id),
                           new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue??string.Empty),
                            new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
            };

            newId = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameters));
            return newId > 0;
        }


        public static bool UpdateOrderSharedPushMessageConfig(OrderSharedPushMessageConfig model)
        {
            const string sql = @"UPDATE  [Configuration].[dbo].[SE_OrderSharedPushMessageConfig]
                                SET        Type=@Type
                                          ,MessageTilte=@MessageTilte
                                          ,MessageContent=@MessageContent
                                          ,PushTitile=@PushTitile
                                          ,PushContent=@PushContent
                                          ,CreateTime=GETDATE()  
                                          ,IOSCommunicationValue=@IOSCommunicationValue
                                          ,AndroidCommunicationValue=@AndroidCommunicationValue
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
              {
                new SqlParameter("@Type",model.Type??string.Empty),
                 new SqlParameter("@MessageTilte",model.MessageTilte??string.Empty),
                  new SqlParameter("@MessageContent",model.MessageContent??string.Empty),
                   new SqlParameter("@PushTitile",model.PushTitile??string.Empty),
                    new SqlParameter("@PushContent",model.PushContent??string.Empty),
                      new SqlParameter("@Id",model.Id),
                            new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue??string.Empty),
                               new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
              };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }

        public static bool DeleteOrderSharedPushMessageConfig(int id)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[SE_OrderSharedPushMessageConfig] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}
