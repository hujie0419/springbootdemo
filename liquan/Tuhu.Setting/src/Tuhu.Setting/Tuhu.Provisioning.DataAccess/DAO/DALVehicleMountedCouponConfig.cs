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
    public class DALVehicleMountedCouponConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);


        public static List<tbl_OrderModel> JiayouCard(string startTime, string endTime, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT    OrderNo ,
                                                O.OrderDatetime ,
                                                O.UserName ,
                                                O.UserTel ,
                                                O.Status ,
                                                O.PayStatus ,
                                                O.PayMothed ,
                                                O.DeliveryType ,
                                                O.DeliveryStatus ,
                                                O.InstallType ,
                                                O.OrderType ,
                                                O.OrderChannel,O.PKID
                                        FROM    Gungnir..tbl_Order AS O ( NOLOCK )
                                        WHERE 1=1 AND  O.OrderType = N'12加油卡'
                                                AND O.PayStatus = '2paid'
                                                AND O.Status <> '7Canceled'
                                                AND ( O.PurchaseStatus = 0
                                                      OR O.PurchaseStatus = 1
                                                    )
		                                        AND O.OrderDatetime>=@StartTime  AND O.OrderDatetime<=@EndTime                            
                                    ) AS PG
                            ORDER BY PG.[PKID] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                               FROM    Gungnir..tbl_Order AS O ( NOLOCK )
                                        WHERE 1=1 AND O.OrderType = N'12加油卡'
                                                AND O.PayStatus = '2paid'
                                                AND O.Status <> '7Canceled'
                                                AND ( O.PurchaseStatus = 0
                                                      OR O.PurchaseStatus = 1
                                                   )
		                                        AND O.OrderDatetime>=@StartTime  AND O.OrderDatetime<=@EndTime  
                             ";

            var sqlParameters = new SqlParameter[]
                   {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                        new SqlParameter("@StartTime",startTime),
                        new SqlParameter("@EndTime",endTime)
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<tbl_OrderModel>().ToList();

        }


        public static List<VehicleMountedCouponConfig> GetVehicleMountedCouponConfig(VehicleMountedCouponConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT   [Id]
                                              ,[Channel]
                                              ,[Versions]
                                              ,[Region]
                                              ,[Status]
                                              ,[Images]
                                              ,[Coupons]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[ActivityId]
                                              ,[Location]
                                              ,[Name]
                                              ,[CouponQuantity]
                                              ,[Type]
                                      FROM    [Configuration].[dbo].[SE_VehicleMountedCouponConfig]                                      
		                              WHERE 1=1                                       
                                    ) AS PG
                            ORDER BY PG.[UpdateTime] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_VehicleMountedCouponConfig] AS A WITH (NOLOCK) 
                             ";

            var sqlParameters = new SqlParameter[]
                   {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VehicleMountedCouponConfig>().ToList();

        }

        public static VehicleMountedCouponConfig GetVehicleMountedCouponConfigById(int id)
        {
            const string sql = @"SELECT    [Id]
                                          ,[Channel]
                                          ,[Versions]
                                          ,[Region]
                                          ,[Status]
                                          ,[Images]
                                          ,[Coupons]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[ActivityId]
                                          ,[Location]
                                          ,[Name]
                                          ,[CouponQuantity]
                                          ,[Type]
                                  FROM [Configuration].[dbo].[SE_VehicleMountedCouponConfig] WITH(NOLOCK)  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<VehicleMountedCouponConfig>().ToList().FirstOrDefault();
        }
        public static bool InsertVehicleMountedCouponConfig(VehicleMountedCouponConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Configuration.dbo.[SE_VehicleMountedCouponConfig]
                                              (                                               
                                               [Channel]
                                              ,[Versions]
                                              ,[Region]
                                              ,[Status]
                                              ,[Images]
                                              ,[Coupons]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[CreateName]
                                              ,[UpdateName]
                                              ,[ActivityId]
                                              ,[Location]
                                              ,[Name]
                                              ,[CouponQuantity]
                                              ,[Type]
                                              )
                                      VALUES  (                                          
                                               @Channel
                                              ,@Versions
                                              ,@Region
                                              ,@Status
                                              ,@Images
                                              ,@Coupons
                                              ,GETDATE()
                                              ,GETDATE() 
                                              ,@CreateName
                                              ,@UpdateName  
                                              ,NEWID()
                                              ,@Location
                                              ,@Name
                                              ,@CouponQuantity 
                                              ,@Type                              
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@Coupons",model.Coupons??string.Empty),
                    new SqlParameter("@Images",model.Images??string.Empty),
                    new SqlParameter("@Region",model.Region??string.Empty),
                    new SqlParameter("@Versions",model.Versions??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CreateName",model.CreateName??string.Empty),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@Location",model.Location),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@CouponQuantity",model.CouponQuantity),
                    new SqlParameter("@Type",model.Type)

                };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return id > 0;
        }


        public static bool UpdateVehicleMountedCouponConfig(VehicleMountedCouponConfig model)
        {
            const string sql = @"UPDATE Configuration.dbo.SE_VehicleMountedCouponConfig SET                                      
                                        Channel=@Channel
                                        ,Versions=@Versions
                                        ,Region=@Region
                                        ,Status=@Status
                                        ,Images=@Images
                                        ,Coupons=@Coupons
                                        ,UpdateTime=GETDATE()
                                        ,UpdateName=@UpdateName 
                                        ,Location=@Location
                                        ,Name=@Name
                                        ,CouponQuantity=@CouponQuantity
                                        ,Type=@Type                                                                 
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Channel",model.Channel??string.Empty),
                    new SqlParameter("@Coupons",model.Coupons??string.Empty),
                    new SqlParameter("@Images",model.Images??string.Empty),
                    new SqlParameter("@Region",model.Region??string.Empty),
                    new SqlParameter("@Versions",model.Versions??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@Location",model.Location),
                    new SqlParameter("@Name",model.Name),
                    new SqlParameter("@CouponQuantity",model.CouponQuantity),
                    new SqlParameter("@Type",model.Type)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteVehicleMountedCouponConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_VehicleMountedCouponConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static List<Region> GetRegion(int id)
        {
            const string sql = @"SELECT [PKID]
                                      ,[RegionName]	    
	                                 , ParentID
                                  FROM [Gungnir].[dbo].[tbl_region] WITH(NOLOCK) WHERE ParentID = @id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@id", id)).ConvertTo<Region>().ToList();
        }
        public static List<Region> GetRegion(string name)
        {
            const string sql = @" SELECT [PKID]
                                        ,[RegionName]	    
	                                    , ParentID
                                  FROM [Gungnir].[dbo].[tbl_region] WITH(NOLOCK) WHERE ParentID=( SELECT [PKID]
                                    
                                  FROM [Gungnir].[dbo].[tbl_region] WITH(NOLOCK) WHERE RegionName=@name) ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@name", name)).ConvertTo<Region>().ToList();
        }
        public static List<ShopCosmetologyServers> GetShopCosmetologyServers(int id = 0)
        {
            const string sql = @" SELECT PKID, ProductID,ServersName,CatogryID
                                 FROM   Gungnir.dbo.ShopCosmetologyServers
                                 WHERE  IsActive = 1
                                        AND CatogryID = @Id";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<ShopCosmetologyServers>().ToList();
        }
    }
}

