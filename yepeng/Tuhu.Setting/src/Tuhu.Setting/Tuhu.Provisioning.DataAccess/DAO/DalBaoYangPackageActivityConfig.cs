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
    public class DalBaoYangPackageActivityConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(strConn);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        public static List<BaoYangPackageActivityConfig> GetBaoYangPackageActivityConfig(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT A.[Id] ,
                                    [ActivityNum] ,
                                    [ActivityName] ,
                                    [ActivityStatus] ,
                                    [CheckStatus] ,                              
                                    [LayerImage] ,
                                    [CouponId] ,
                                    [ButtonChar] ,                           
                                    ActivityImage,
                                    A.GetRuleGUID,
                                    [CreateTime],
                                    [UpdateTime]
                            FROM    [Configuration].[dbo].[SE_BaoYangPackageActivityConfig] AS A WITH (NOLOCK)                                 
                                    ORDER BY  A.[UpdateTime] DESC 
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                                " + sqlStr;
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_BaoYangPackageActivityConfig] AS A WITH (NOLOCK)" + sqlStr;

            var sqlParameters = new SqlParameter[]
                   {                 
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex)                
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangPackageActivityConfig>().ToList();

        }


        public static BaoYangPackageActivityConfig GetBaoYangPackageActivityConfigById(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[ActivityNum]
                                      ,[ActivityName]
                                      ,[ActivityStatus]
                                      ,[CheckStatus]                                  
                                      ,[LayerImage]
                                      ,[CouponId]
                                      ,[ButtonChar]                                                        
                                      ,[CreateTime] 
                                      ,[UpdateTime]
                                      ,ActivityImage 
                                      ,GetRuleGUID    
                              FROM [Configuration].[dbo].[SE_BaoYangPackageActivityConfig] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<BaoYangPackageActivityConfig>().ToList().FirstOrDefault();
        }


        public static bool InsertBaoYangPackageActivityConfig(BaoYangPackageActivityConfig model)
        {

            const string sql = @"INSERT INTO [Configuration].[dbo].[SE_BaoYangPackageActivityConfig]
                                        ( 
                                           [ActivityNum]
                                          ,[ActivityName]
                                          ,[ActivityStatus]
                                          ,[CheckStatus]
                                     
                                          ,[LayerImage]
                                          ,[CouponId]
                                          ,[ButtonChar]                                 
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,ActivityImage
                                          ,GetRuleGUID
                                        )
                                VALUES  ( @ActivityNum
                                          ,@ActivityName
                                          ,@ActivityStatus
                                          ,@CheckStatus
                                   
                                          ,@LayerImage
                                          ,@CouponId
                                          ,@ButtonChar                                    
                                          ,GETDATE()
                                          ,GETDATE()
                                          ,@ActivityImage
                                          ,@GetRuleGUID
                                        )SELECT @@IDENTITY
                                ";
            string activityNum = Guid.NewGuid().ToString();
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityName",model.ActivityName),
                 new SqlParameter("@ActivityNum",activityNum),
                  new SqlParameter("@ActivityStatus",model.ActivityStatus),
                   new SqlParameter("@ButtonChar",model.ButtonChar),
                    new SqlParameter("@CheckStatus",model.CheckStatus),
                     new SqlParameter("@CouponId",model.CouponId),
                      new SqlParameter("@LayerImage",model.LayerImage),
                             new SqlParameter("@ActivityImage",model.ActivityImage),
                                new SqlParameter("@GetRuleGUID",model.GetRuleGUID)

            };
            return Convert.ToInt32(SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters)) > 0;


        }


        public static bool UpdateBaoYangPackageActivityConfig(BaoYangPackageActivityConfig model)
        {
            const string sql = @"UPDATE  [Configuration].[dbo].[SE_BaoYangPackageActivityConfig]
                                SET    
                                        ActivityName = @ActivityName ,
                                        ActivityStatus = @ActivityStatus ,
                                        CheckStatus = @CheckStatus ,
                                  
                                        LayerImage = @LayerImage ,
                                        CouponId = @CouponId ,
                                        ButtonChar = @ButtonChar ,    
                                        ActivityImage = @ActivityImage,
                                        UpdateTime = GETDATE(),
                                        GetRuleGUID=@GetRuleGUID
                                WHERE   Id = @Id";

            var sqlParameters = new SqlParameter[]
           {
                new SqlParameter("@ActivityName",model.ActivityName),
                 new SqlParameter("@ActivityNum",model.ActivityNum),
                  new SqlParameter("@ActivityStatus",model.ActivityStatus),
                   new SqlParameter("@ButtonChar",model.ButtonChar),
                    new SqlParameter("@CheckStatus",model.CheckStatus),
                     new SqlParameter("@CouponId",model.CouponId),
                      new SqlParameter("@LayerImage",model.LayerImage),
                       new SqlParameter("@Id",model.Id),
                         new SqlParameter("@ActivityImage",model.ActivityImage),
                           new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty)
           };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }

        public static bool DeleteBaoYangPackageActivityConfig(int id)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[SE_BaoYangPackageActivityConfig] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

    }
}
