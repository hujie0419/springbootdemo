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
    public class DALMDBeautyApplyConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<MDBeautyApplyConfig> GetMDBeautyApplyConfig(MDBeautyApplyConfig model, int pageSize, int pageIndex, string type, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT DISTINCT   M.[Id] ,
                                                M.[ActivityId] ,
                                                A.Id AS ActivityPKID,
                                                [ShopId] ,
                                                [ShopName] ,
                                                [ProductId] ,
                                                [ProductName] ,
                                                [Quantity] ,
                                                [Price] ,
                                                [ApplyAuditStatus] ,
                                                [ExitAuditStatus] ,
                                                [AuditReason] ,
                                                [ExitReason] ,
                                                M.[CreateTime] ,
                                                M.[UpdateTime] ,
                                                A.Id AS BeautyAcitivityId,
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
                                                [CategoryName] ,
                                                [ShowName],PlanStartTime,ExitActivityReason                                            
                                        FROM    [Tuhu_Groupon].[dbo].[ShopBeautyAcitivityApply] AS M WITH ( NOLOCK )
                                                LEFT JOIN [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] AS A WITH ( NOLOCK ) ON A.ActivityId = M.ActivityId
                                                LEFT JOIN Tuhu_Groupon.dbo.SE_MDBeautyCategoryConfig AS B WITH ( NOLOCK ) ON B.Id = A.CategoryId  
                                                LEFT JOIN Tuhu_Groupon.dbo.RegionRelation AS C WITH ( NOLOCK ) ON C.ActivityId = A.Id
                                              
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
                                        ( @StrProvince =-1
                                            OR ( @StrProvince <> -1
                                                AND C.ProvinceId=@StrProvince
                                                )
                                        )AND
                                        ( @StrCity =-1
                                            OR ( @StrCity <> -1
                                                AND C.CityId=@StrCity
                                                )
                                        )AND
                                        ( @Status = -1
                                            OR ( @Status <> -1
                                                AND Status = @Status
                                                )
                                        )
                                        AND
                                        ( @ApplyAuditStatus = 0
                                            OR ( @ApplyAuditStatus <> 0
                                                AND ApplyAuditStatus = @ApplyAuditStatus
                                                )
                                        )
                                        AND
                                        ( @ExitAuditStatus = 0
                                            OR ( @ExitAuditStatus <> 0
                                                AND ExitAuditStatus = @ExitAuditStatus
                                                )
                                        )
                                        AND
                                        ( @Type = ''
                                            OR ( @Type = 'Apply'
                                                AND ExitAuditStatus  =0
                                                )
                                            OR ( @Type = 'Exit'
                                                AND ApplyAuditStatus =3 AND ExitAuditStatus <>0
                                                )
                                        )
                                    ) AS PG
                            ORDER BY PG.[UpdateTime] DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Tuhu_Groupon].[dbo].[ShopBeautyAcitivityApply] AS M WITH ( NOLOCK )
                                        LEFT JOIN [Tuhu_Groupon].[dbo].[ShopBeautyAcitivity] AS A WITH ( NOLOCK ) ON A.ActivityId = M.ActivityId
                                        LEFT JOIN Tuhu_Groupon.dbo.SE_MDBeautyCategoryConfig AS B WITH ( NOLOCK ) ON B.Id = A.CategoryId  
                                        LEFT JOIN Tuhu_Groupon.dbo.RegionRelation  AS C WITH ( NOLOCK ) ON C.ActivityId = A.Id                                
                                WHERE  ( @Id = 0
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
                                        ( @StrProvince = -1
                                            OR ( @StrProvince <> -1
                                                AND C.ProvinceId=@StrProvince
                                                )
                                        )AND
                                        ( @StrCity = -1
                                            OR ( @StrCity <> -1
                                                AND C.CityId=@StrCity
                                                )
                                        )
                                        AND
                                        ( @Status = -1
                                            OR ( @Status <> -1
                                                AND Status = @Status
                                                )
                                        )AND
                                        ( @ApplyAuditStatus = 0
                                            OR ( @ApplyAuditStatus <> 0
                                                AND ApplyAuditStatus = @ApplyAuditStatus
                                                )
                                        )
                                        AND
                                        ( @ExitAuditStatus = 0
                                            OR ( @ExitAuditStatus <> 0
                                                AND ExitAuditStatus = @ExitAuditStatus
                                                )
                                        )AND
                                        ( @Type = ''
                                            OR ( @Type = 'Apply'
                                                AND ExitAuditStatus  =0
                                                )
                                            OR ( @Type = 'Exit'
                                                AND ApplyAuditStatus =3 AND ExitAuditStatus <>0
                                                )
                                        )";

            var sqlParameters = new SqlParameter[]
                   {
                        new SqlParameter("@PageSize",pageSize),
                        new SqlParameter("@PageIndex",pageIndex),
                        new SqlParameter("@Id",model.Id),
                        new SqlParameter("@Status",model.Status),
                        new SqlParameter("@StrProvince",model.StrProvince??"-1"),
                        new SqlParameter("@StrCity",model.StrCity??"-1"),
                        new SqlParameter("@CategoryId",model.CategoryId),
                        new SqlParameter("@Name",model.Name??string.Empty),
                        new SqlParameter("@type",type??string.Empty),
                        new SqlParameter("@ApplyAuditStatus",model.ApplyAuditStatus),
                        new SqlParameter("@ExitAuditStatus",model.ExitAuditStatus),
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<MDBeautyApplyConfig>().ToList();

        }

        public static MDBeautyApplyConfig GetMDBeautyApplyConfigById(int id)
        {
            const string sql = @"SELECT    [Id]
                                          ,[ActivityId]
                                          ,[ShopId]
                                          ,[ShopName]
                                          ,[ProductId]
                                          ,[ProductName]
                                          ,[Quantity]
                                          ,[Price]
                                          ,[ApplyAuditStatus]
                                          ,[ExitAuditStatus]
                                          ,[AuditReason]
                                          ,[ExitReason]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                  FROM [Tuhu_Groupon].[dbo].[ShopBeautyAcitivityApply] WITH(NOLOCK)  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<MDBeautyApplyConfig>().ToList().FirstOrDefault();
        }

        public static bool UpdateMDBeautyApplyConfig(MDBeautyApplyConfig model, string type)
        {
            string sql1 = @"UPDATE Tuhu_Groupon.dbo.[ShopBeautyAcitivityApply] SET                                      
                                           [ApplyAuditStatus]=@ApplyAuditStatus                                     
                                          ,[AuditReason]=@AuditReason                                                                      
                                          ,[UpdateTime]=GETDATE()                                                                           
                                 WHERE Id=@Id";

            string sql2 = @"UPDATE Tuhu_Groupon.dbo.[ShopBeautyAcitivityApply] SET 
                                           ExitAuditStatus=@ExitAuditStatus
                                          ,ExitReason=@ExitReason                                         
                                          ,UpdateTime=GETDATE()                                    
                                 WHERE Id=@Id";


            if (!string.IsNullOrWhiteSpace(type))
            {
                if (type == "Apply")
                {
                    var sqlParameter = new SqlParameter[]
                        {
                          new SqlParameter("@ApplyAuditStatus",model.ApplyAuditStatus),
                          new SqlParameter("@AuditReason",model.AuditReason),
                          new SqlParameter("@Id",model.Id),
                        };
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql1, sqlParameter) > 0;
                }
                else if (type == "Exit")
                {
                    var sqlParameter = new SqlParameter[]
                        {
                          new SqlParameter("@ExitAuditStatus",model.ExitAuditStatus),
                          new SqlParameter("@ExitReason",model.ExitReason),
                          new SqlParameter("@Id",model.Id),
                         };
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql2, sqlParameter) > 0;
                }
            }

            return false;
        }

        public static bool AuditMDBeautyApplyConfig(MDBeautyApplyConfig model, string type)
        {
            string sql1 = @"UPDATE Tuhu_Groupon.dbo.[ShopBeautyAcitivityApply] SET                                      
                                           [ApplyAuditStatus]=3                       
                                          ,[UpdateTime]=GETDATE()                                                                           
                                 WHERE Id=@Id";

            string sql2 = @"UPDATE Tuhu_Groupon.dbo.[ShopBeautyAcitivityApply] SET 
                                            ExitAuditStatus=3                                                                    
                                            ,UpdateTime=GETDATE()                                    
                                 WHERE Id=@Id";

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (type == "Apply")
                {
                    var sqlParameter = new SqlParameter[]
                        {
                          new SqlParameter("@ApplyAuditStatus",model.ApplyAuditStatus),
                          new SqlParameter("@AuditReason",model.AuditReason),
                          new SqlParameter("@Id",model.Id),
                        };
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql1, sqlParameter) > 0;
                }
                else if (type == "Exit")
                {
                    var sqlParameter = new SqlParameter[]
                        {
                          new SqlParameter("@ExitAuditStatus",model.ExitAuditStatus),
                          new SqlParameter("@ExitReason",model.ExitReason),
                          new SqlParameter("@Id",model.Id),
                         };
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql2, sqlParameter) > 0;
                }
            }

            return false;
        }
        public static bool DeleteMDBeautyApplyConfig(int id)
        {
            const string sql = @"DELETE FROM Tuhu_Groupon.dbo.ShopBeautyAcitivityApply WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}

