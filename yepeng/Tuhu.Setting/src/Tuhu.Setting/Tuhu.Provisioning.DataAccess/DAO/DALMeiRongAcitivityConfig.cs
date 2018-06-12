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
using Newtonsoft.Json;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALMeiRongAcitivityConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<MeiRongAcitivityConfig> GetMeiRongAcitivityConfig(MeiRongAcitivityConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT DISTINCT   A.[Id] ,
                                                [Name] ,
                                                [SignUpStartTime] ,
                                                [SignUpEndTime] ,
                                                [ActivityStartTime] ,
                                                [ActivityEndTime] ,
                                                [CategoryId] ,                                          
                                                [MinPrice] ,
                                                [MaxPrice] ,
                                                [EverydayQuantity] ,
                                                [MinShopQuantity] ,
                                                [VehicleGrade] ,
                                                [ApplicationVehicle] ,
                                                [ShopType] ,
                                                [ShopGrade] ,
                                                [MeiRongAppraise] ,
                                                [ActivityRequire] ,
                                                [ActivityNotification] ,
                                                [Status] ,
                                                A.[CreateTime] ,
                                                [UpdateTime] ,
                                                [CreateName] ,
                                                [UpdateName], CategoryName,ShowName,PlanStartTime
                                      FROM      [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] AS A WITH(NOLOCK)
                                                LEFT JOIN  Tuhu_Groupon.dbo.SE_MDBeautyCategoryConfig AS B WITH(NOLOCK)  ON B.Id = A.CategoryId 
                                                LEFT JOIN Tuhu_Groupon..RegionRelation AS C ON C.ActivityId = A.Id
		                              WHERE 
                                        ( @Id = 0
                                            OR ( @Id <> 0
                                                AND A.Id = @Id
                                                )
                                        )AND
                                        ( @Name = ''
                                            OR ( @Name <> ''
                                                AND Name = @Name
                                                )
                                        )AND                                       
                                        ( @CategoryId = -1
                                            OR ( @CategoryId <> -1
                                                AND CategoryId = @CategoryId
                                                )
                                        )AND                                  
                                        ( @Status = -1
                                            OR ( @Status <> -1
                                                AND Status = @Status
                                                )
                                        )
                                        AND                                  
                                        ( @CityId = -1
                                            OR ( @CityId <> -1
                                                AND C.CityId = @CityId
                                                )
                                        )
                                        AND                                  
                                        ( @ProvinceId = -1
                                            OR ( @ProvinceId <> -1
                                                AND  C.ProvinceId = @ProvinceId
                                                )
                                        )
                                    ) AS PG
                            ORDER BY PG.[UpdateTime] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY 
                                ";
            string sqlCount = @" SELECT COUNT(0)
                                 FROM   ( SELECT  DISTINCT
                                                    A.[Id] ,
                                                    COUNT(0) AS CountNum
                                          FROM      [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] AS A WITH ( NOLOCK )
                                                    LEFT JOIN Tuhu_Groupon.dbo.SE_MDBeautyCategoryConfig AS B
                                                    WITH ( NOLOCK ) ON B.Id = A.CategoryId
                                                    LEFT JOIN Tuhu_Groupon..RegionRelation AS C ON C.ActivityId = A.Id
                                          WHERE     ( @Id = 0
                                                      OR ( @Id <> 0
                                                           AND A.Id = @Id
                                                         )
                                                    )
                                                    AND ( @Name = ''
                                                          OR ( @Name <> ''
                                                               AND Name = @Name
                                                             )
                                                        )
                                                    AND ( @CategoryId = -1
                                                          OR ( @CategoryId <> -1
                                                               AND CategoryId = @CategoryId
                                                             )
                                                        )
                                                    AND ( @Status = -1
                                                          OR ( @Status <> -1
                                                               AND Status = @Status
                                                             )
                                                        )
                                                    AND ( @CityId = -1
                                                          OR ( @CityId <> -1
                                                               AND C.CityId = @CityId
                                                             )
                                                        )
                                                    AND ( @ProvinceId = -1
                                                          OR ( @ProvinceId <> -1
                                                               AND C.ProvinceId = @ProvinceId
                                                             )
                                                        )  	GROUP BY A.Id
                                        ) AS CountNUN";

            var sqlParameters = new SqlParameter[]
                   {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                        new SqlParameter("@Id",model.Id),
                        new SqlParameter("@Status",model.Status),
                        new SqlParameter("@ProvinceId",model.StrProvince??"-1"),
                        new SqlParameter("@CityId",model.StrCity??"-1"),
                        new SqlParameter("@CategoryId",model.CategoryId),
                        new SqlParameter("@Name",model.Name??string.Empty)
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<MeiRongAcitivityConfig>().ToList();

        }


        public static List<MeiRongAcitivityConfig> GetMeiRongAcitivityConfig()
        {
            string sql = @"SELECT  A.Id ,
                                    A.ActivityId ,
                                    A.Name
                            FROM    [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] AS A WITH ( NOLOCK )  
                                ";

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<MeiRongAcitivityConfig>().ToList();

        }
        public static MeiRongAcitivityConfig GetMeiRongAcitivityConfigById(int id)
        {
            const string sql = @"SELECT    [Id]
                                          ,[ActivityId]
                                          ,[Name]
                                          ,[SignUpStartTime]
                                          ,[SignUpEndTime]
                                          ,[ActivityStartTime]
                                          ,[ActivityEndTime]
                                          ,[CategoryId]
                                          ,[MinPrice]
                                          ,[MaxPrice]
                                          ,[EverydayQuantity]
                                          ,[MinShopQuantity]
                                          ,[VehicleGrade]
                                          ,[ApplicationVehicle]
                                          ,[ShopType]
                                          ,[ShopGrade]
                                          ,[MeiRongAppraise]
                                          ,[ActivityRequire]
                                          ,[ActivityNotification]
                                          ,[Status]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[CreateName]
                                          ,[UpdateName]
                                          ,[PlanStartTime] 
                                  FROM [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] WITH(NOLOCK)  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<MeiRongAcitivityConfig>().ToList().FirstOrDefault();
        }
        public static bool InsertMeiRongAcitivityConfig(MeiRongAcitivityConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Tuhu_Groupon.dbo.[ShopBeautyAcitivity]
                                              (  
                                               [Name]
                                              ,[SignUpStartTime]
                                              ,[SignUpEndTime]
                                              ,[PlanStartTime]
                                              ,[ActivityEndTime]
                                              ,[CategoryId]
                                           
                                              ,[MinPrice]
                                              ,[MaxPrice]
                                              ,[EverydayQuantity]
                                              ,[MinShopQuantity]
                                              ,[VehicleGrade]
                                              ,[ApplicationVehicle]
                                              ,[ShopType]
                                              ,[ShopGrade]
                                              ,[MeiRongAppraise]
                                              ,[ActivityRequire]
                                              ,[ActivityNotification]
	                                          ,[Status]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[CreateName]
                                              ,[UpdateName]                                            
                                              ,[ActivityId]
                                              )
                                      VALUES  ( 
                                               @Name
                                              ,@SignUpStartTime
                                              ,@SignUpEndTime
                                              ,@PlanStartTime
                                              ,@ActivityEndTime
                                              ,@CategoryId
                                                                                    
                                              ,@MinPrice
                                              ,@MaxPrice
                                              ,@EverydayQuantity
                                              ,@MinShopQuantity
                                              ,@VehicleGrade
                                              ,@ApplicationVehicle
                                              ,@ShopType
                                              ,@ShopGrade
                                              ,@MeiRongAppraise
                                              ,@ActivityRequire
                                              ,@ActivityNotification
	                                          ,@Status
                                              ,GETDATE()
                                              ,GETDATE()
                                              ,@CreateName
                                              ,@UpdateName                                          
                                              ,NEWID()                                        
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityEndTime",model.ActivityEndTime.Value),
                    new SqlParameter("@ActivityNotification",model.ActivityNotification??string.Empty),
                    new SqlParameter("@ActivityRequire",model.ActivityRequire??string.Empty),
                    new SqlParameter("@PlanStartTime",model.PlanStartTime.Value),
                    new SqlParameter("@CategoryId",model.CategoryId),
                    new SqlParameter("@ApplicationVehicle",model.ApplicationVehicle),
                    new SqlParameter("@CreateName",model.CreateName??string.Empty),
                    new SqlParameter("@EverydayQuantity",model.EverydayQuantity),
                    new SqlParameter("@MaxPrice",model.MaxPrice),
                    new SqlParameter("@MeiRongAppraise",model.MeiRongAppraise),
                    new SqlParameter("@MinPrice",model.MinPrice),
                    new SqlParameter("@MinShopQuantity",model.MinShopQuantity),
                    new SqlParameter("@Name",model.Name),

                    new SqlParameter("@ShopGrade",model.ShopGrade),
                    new SqlParameter("@ShopType",model.ShopType),
                    new SqlParameter("@SignUpEndTime",model.SignUpEndTime.Value),
                    new SqlParameter("@SignUpStartTime",model.SignUpStartTime.Value),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@VehicleGrade",model.VehicleGrade),

                };

            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region);
            List<ShopServiceRelation> service = JsonConvert.DeserializeObject<List<ShopServiceRelation>>(model.ShopServices);

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                id = Convert.ToInt32(SqlHelper.ExecuteScalar(tran, CommandType.Text, sql, sqlParameter));

                DeleteRegionRelation(model.Id, tran);
                DeleteShopServiceRelation(model.Id, tran);
                foreach (var item in region)
                {
                    item.Type = 1;
                    item.ActivityId = id;
                    InsertRegionRelation(item, tran);
                }
                foreach (var item in service)
                {
                    item.Type = 1;
                    item.ActivityId = id;
                    InsertShopServiceRelation(item, tran);
                }
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }
        }


        public static bool UpdateMeiRongAcitivityConfig(MeiRongAcitivityConfig model)
        {
            const string sql = @"UPDATE Tuhu_Groupon.dbo.ShopBeautyAcitivity SET                                      
                                         Name=@Name
                                        ,SignUpStartTime=@SignUpStartTime
                                        ,SignUpEndTime=@SignUpEndTime
                                        ,PlanStartTime=@PlanStartTime
                                        ,ActivityEndTime=@ActivityEndTime
                                        ,CategoryId=@CategoryId
                                                                           
                                        ,MinPrice=@MinPrice
                                        ,MaxPrice=@MaxPrice
                                        ,EverydayQuantity=@EverydayQuantity
                                        ,MinShopQuantity=@MinShopQuantity
                                        ,VehicleGrade=@VehicleGrade
                                        ,ApplicationVehicle=@ApplicationVehicle
                                        ,ShopType=@ShopType
                                        ,ShopGrade=@ShopGrade
                                        ,MeiRongAppraise=@MeiRongAppraise
                                        ,ActivityRequire=@ActivityRequire
                                        ,ActivityNotification=@ActivityNotification
	                                    ,Status=@Status
                                        ,UpdateTime=GETDATE()  
                                        ,UpdateName=@UpdateName                                                                 
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@ActivityEndTime",model.ActivityEndTime.Value),
                    new SqlParameter("@ActivityNotification",model.ActivityNotification??string.Empty),
                    new SqlParameter("@ActivityRequire",model.ActivityRequire??string.Empty),
                    new SqlParameter("@PlanStartTime",model.PlanStartTime.Value),
                    new SqlParameter("@CategoryId",model.CategoryId),
                    new SqlParameter("@ApplicationVehicle",model.ApplicationVehicle),
                    new SqlParameter("@CreateName",model.CreateName??string.Empty),
                    new SqlParameter("@EverydayQuantity",model.EverydayQuantity),
                    new SqlParameter("@MaxPrice",model.MaxPrice),
                    new SqlParameter("@MeiRongAppraise",model.MeiRongAppraise),
                    new SqlParameter("@MinPrice",model.MinPrice),
                    new SqlParameter("@MinShopQuantity",model.MinShopQuantity),
                    new SqlParameter("@Name",model.Name),

                    new SqlParameter("@ShopGrade",model.ShopGrade),
                    new SqlParameter("@ShopType",model.ShopType),
                    new SqlParameter("@SignUpEndTime",model.SignUpEndTime.Value),
                    new SqlParameter("@SignUpStartTime",model.SignUpStartTime.Value),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@VehicleGrade",model.VehicleGrade),
                    new SqlParameter("@Id",model.Id),

               };
            List<RegionRelation> region = JsonConvert.DeserializeObject<List<RegionRelation>>(model.Region);
            List<ShopServiceRelation> service = JsonConvert.DeserializeObject<List<ShopServiceRelation>>(model.ShopServices);

            string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
            string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;

            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction tran = conn.BeginTransaction();
            try
            {
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlParameter);
                DeleteRegionRelation(model.Id, tran);
                DeleteShopServiceRelation(model.Id, tran);
                foreach (var item in region)
                {
                    item.Type = 1;
                    item.ActivityId = model.Id;
                    InsertRegionRelation(item, tran);
                }
                foreach (var item in service)
                {
                    item.Type = 1;
                    item.ActivityId = model.Id;
                    InsertShopServiceRelation(item, tran);
                }
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                tran.Dispose();
            }

        }

        public static bool DeleteMeiRongAcitivityConfig(int id)
        {
            const string sql = @"DELETE FROM Tuhu_Groupon.dbo.ShopBeautyAcitivity WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool CompelStart(int id)
        {
            const string sql = @"--强制开启
                                DECLARE @PlanStartTime DATETIME
                                SET @PlanStartTime = ( SELECT TOP 1
                                                    PlanStartTime
                                                FROM   [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity]
                                                WHERE  Id = @Id
                                            )

                                IF @PlanStartTime >= GETDATE()
                                BEGIN
                                    UPDATE  [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity]
                                    SET     Status = 1 ,                                          
                                            ActivityStartTime = @PlanStartTime
                                    WHERE   Id = @Id
                                END
                                ELSE
                                BEGIN
                                    UPDATE  [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity]
                                    SET     Status = 1 ,                                          
                                            ActivityStartTime = GETDATE()
                                    WHERE   Id = @Id
                                END";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool DeleteShopServiceRelation(int id, SqlTransaction tran)
        {
            const string sql = @"IF EXISTS ( SELECT  *
                                        FROM    Tuhu_Groupon.dbo.ShopServiceRelation
                                        WHERE   ActivityId = @ActivityId )
                                DELETE  FROM Tuhu_Groupon.dbo.ShopServiceRelation
                                WHERE   ActivityId = @ActivityId";

            return SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, new SqlParameter("@ActivityId", id)) > 0;
        }
        public static bool DeleteRegionRelation(int id, SqlTransaction tran)
        {
            const string sql = @"IF EXISTS ( SELECT  *
                                            FROM    Tuhu_Groupon.dbo.RegionRelation
                                            WHERE   ActivityId = @ActivityId )
                                    DELETE  FROM Tuhu_Groupon.dbo.RegionRelation
                                    WHERE   ActivityId = @ActivityId  
                                ";

            return SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, new SqlParameter("@ActivityId", id)) > 0;
        }

        public static List<ShopServiceRelation> GetShopServiceRelation(int id, int type, int catogryId)
        {
            const string sql = @"SELECT [Id]
                                      ,[ActivityId]
                                      ,[PKID]
                                      ,[ProductID]
                                      ,[ServersName]
                                      ,[CatogryID]
                                      ,[Type]
                                      ,[CreateTime]
                                  FROM [Tuhu_Groupon].[dbo].[ShopServiceRelation] WITH(NOLOCK) 
                                  WHERE ActivityId=@ActivityId AND Type=@Type AND CatogryID=@CatogryID";
            var sqlParameter = new SqlParameter[]
              {
                   new SqlParameter("@Type",type),
                   new SqlParameter("@ActivityId", id),
                   new SqlParameter("@CatogryID",catogryId)
              };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameter).ConvertTo<ShopServiceRelation>().ToList();
        }


        public static List<RegionRelation> GetRegion(int id, int type)
        {
            const string sql = @"SELECT *                                  
                                  FROM [Tuhu_Groupon].[dbo].[RegionRelation] WITH(NOLOCK) 
                                  WHERE ActivityId=@ActivityId AND Type=@Type";
            var sqlParameter = new SqlParameter[]
                         {
                  new SqlParameter("@Type",type),
                   new SqlParameter("@ActivityId", id)
                         };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameter).ConvertTo<RegionRelation>().ToList();
        }
        public static List<RegionRelation> GetRegionRelation(int id, int type)
        {
            const string sql = @"SELECT [ProvinceId]
                                       ,[ProvinceName]                                     
                                  FROM [Tuhu_Groupon].[dbo].[RegionRelation] WITH(NOLOCK) 
                                  WHERE ActivityId=@ActivityId AND Type=@Type GROUP BY  ProvinceId,ProvinceName";
            var sqlParameter = new SqlParameter[]
                         {
                  new SqlParameter("@Type",type),
                   new SqlParameter("@ActivityId", id)
                         };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameter).ConvertTo<RegionRelation>().ToList();
        }

        public static List<RegionRelation> GetRegionRelation(int pid, int id, int type)
        {
            const string sql = @"SELECT [CityId]
                                      ,[CityName]                                                                
                                  FROM [Tuhu_Groupon].[dbo].[RegionRelation] WITH(NOLOCK) 
                                  WHERE ActivityId=@ActivityId AND Type=@Type AND ProvinceId=@ProvinceId";
            var sqlParameter = new SqlParameter[]
                         {
                  new SqlParameter("@Type",type),
                   new SqlParameter("@ActivityId", id),
                   new SqlParameter("@ProvinceId",pid)
                         };
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameter).ConvertTo<RegionRelation>().ToList();
        }

        public static bool InsertRegionRelation(RegionRelation model, SqlTransaction tran)
        {
            const string sql = @"  INSERT INTO Tuhu_Groupon.dbo.[RegionRelation]
                                              (                                           
                                               [ActivityId]
                                              ,[ProvinceId]
                                              ,[ProvinceName]
                                              ,[CityId]
                                              ,[CityName]
                                              ,[Type]
                                              ,[CreateTime]
                                              )
                                      VALUES  ( 
                                               @ActivityId
                                              ,@ProvinceId
                                              ,@ProvinceName
                                              ,@CityId
                                              ,@CityName
                                              ,@Type                                           
                                              ,GETDATE()                                                                               
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityId",model.ActivityId),
                    new SqlParameter("@CityId",model.CityId),
                    new SqlParameter("@CityName",model.CityName??string.Empty),
                    new SqlParameter("@ProvinceId",model.ProvinceId),
                    new SqlParameter("@ProvinceName",model.ProvinceName),
                    new SqlParameter("@Type",model.Type),
                };
            return SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlParameter) > 0;

        }

        public static bool InsertShopServiceRelation(ShopServiceRelation model, SqlTransaction tran)
        {
            const string sql = @"  INSERT INTO Tuhu_Groupon.dbo.[ShopServiceRelation]
                                              (                                           
                                               [ActivityId]
                                              ,[PKID]
                                              ,[ProductID]
                                              ,[ServersName]
                                              ,[CatogryID]
                                              ,[Type]
                                              ,[CreateTime]
                                              )
                                      VALUES  ( 
                                               @ActivityId
                                              ,@PKID
                                              ,@ProductID
                                              ,@ServersName
                                              ,@CatogryID
                                              ,@Type                                                                
                                              ,GETDATE()                                                                               
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityId",model.ActivityId),
                    new SqlParameter("@PKID",model.PKID),
                    new SqlParameter("@ProductID",model.ProductID??string.Empty),
                    new SqlParameter("@ServersName",model.ServersName),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@CatogryID",model.CatogryID),

                };
            return SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlParameter) > 0;

        }
        public static bool InsertShopNotificationRecord(ShopNotificationRecord model)
        {
            const string sql = @"  INSERT INTO Tuhu_Groupon.dbo.[ShopNotificationRecord]
                                              (  
                                               [ActivityId]
                                              ,[ShopId]
                                              ,[Notification]
                                              )
                                      VALUES  ( 
                                               @ActivityId
                                              ,@ShopId
                                              ,@Notification
                                              )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ActivityId",model.ActivityId),
                    new SqlParameter("@Notification",model.Notification??string.Empty),
                    new SqlParameter("@ShopId",model.ShopId),

                };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
    }
}

