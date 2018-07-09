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
    public class DALExchangeCenterConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<ExchangeCenterConfig> GetExchangeCenterConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"
                            SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [CouponId] ,
                                                [CouponName] ,
                                                [CouponSum] ,
                                                [CouponSurplus] ,
                                                [Period] ,
                                                [PointsValue] ,
                                                [EndTime] ,
                                                [Status] ,
                                                [Image] ,
                                                [PointsRules] ,
                                                [Description] ,
                                                [CreateTime] ,
                                                [SmallImage] ,
                                                [Sort] ,
                                                [CouponEndTime] ,
                                                [CouponDuration] ,[StartVersion],[EndVersion],
                                                GetRuleGUID,Postion,PID,UserRank,EndCouponPrice,ExchangeCenterType,Chance
                                      FROM      [Configuration].[dbo].[ExchangeCenterConfig] WITH ( NOLOCK )
                                      WHERE     1 = 1   " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                               ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[ExchangeCenterConfig] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ExchangeCenterConfig>().ToList();

        }

        public static List<ExchangeCenterConfig> GetExchangeCenter()
        {
            const string sql = @"SELECT         [Id] ,
                                                [CouponId] ,
                                                [CouponName] ,
                                                [CouponSum] ,
                                                [CouponSurplus] ,
                                                [Period] ,
                                                [PointsValue] ,
                                                [EndTime] ,
                                                [Status] ,
                                                [Image] ,
                                                [PointsRules] ,
                                                [Description] ,
                                                [CreateTime] ,
                                                [SmallImage] ,
                                                [Sort] ,
                                                [CouponEndTime] ,
                                                [CouponDuration] ,[StartVersion],[EndVersion],
                                                GetRuleGUID,Postion,PID,UserRank,EndCouponPrice,ExchangeCenterType,Chance
                                      FROM      [Configuration].[dbo].[ExchangeCenterConfig] WITH ( NOLOCK )";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<ExchangeCenterConfig>().ToList();
        }

        public static List<ExchangeCenterConfig> GetExchangeCenterConfigList(int id)
        {
            string sql = @"
                            SELECT  B.[Id] ,
                                    [CouponId] ,
                                    [CouponName] ,
                                    [CouponSum] ,
                                    [CouponSurplus] ,
                                    [Period] ,
                                    [PointsValue] ,
                                    [EndTime] ,
                                    [Status] ,
                                    [Image] ,
                                    [PointsRules] ,
                                    [Description] ,
                                    A.[CreateTime] ,
                                    [SmallImage] ,
                                    A.[Sort] ,
                                    [CouponEndTime] ,
                                    [CouponDuration] ,
                                    [StartVersion] ,
                                    [EndVersion] ,
                                    A.Id AS ExchangeCenterId
                             FROM   [Configuration].[dbo].[SE_PersonalCenterCouponConfig] AS A WITH ( NOLOCK )
                                    LEFT JOIN [Configuration].[dbo].[ExchangeCenterConfig] AS B WITH ( NOLOCK ) ON A.ExchangeCenterId = B.Id
                            WHERE A.HomePageModuleId=@HomePageModuleId ORDER BY A.Sort ASC
                               ";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@HomePageModuleId",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ExchangeCenterConfig>().ToList();

        }


        public static List<ExchangeCenterConfig> GetExchangeCenterConfigList()
        {
            string sql = @"
                           SELECT  A.*
                            FROM    [Configuration].[dbo].ExchangeCenterConfig AS A WITH ( NOLOCK )
                            WHERE   A.Id NOT IN (
                                    SELECT  ExchangeCenterId
                                    FROM    [Configuration].[dbo].[SE_PersonalCenterCouponConfig] )
                                    AND A.Postion=N'精品通用券'
                               ";

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<ExchangeCenterConfig>().ToList();

        }

        public static ExchangeCenterConfig GetExchangeCenterConfig(int id)
        {
            const string sql = @"SELECT  [Id]
                                      ,[CouponId]
                                      ,[CouponName]
                                      ,[CouponSum]
                                      ,[CouponSurplus]
                                      ,[Period]
                                      ,[PointsValue]
                                      ,[EndTime]
                                      ,[Status]
                                      ,[Image]
                                      ,[PointsRules]
                                      ,[Description]
                                      ,[CreateTime]
                                      ,[SmallImage]
                                      ,[Sort]
                                      ,[CouponEndTime]
                                      ,[CouponDuration]
                                      ,GetRuleGUID,Postion,PID,UserRank,EndCouponPrice,ExchangeCenterType,Chance,StartVersion,EndVersion
                                  FROM [Configuration].[dbo].[ExchangeCenterConfig] WITH (NOLOCK) WHERE Id=@id";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id),

            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<ExchangeCenterConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertExchangeCenterConfig(ExchangeCenterConfig model)
        {
            const string sql = @"  INSERT  INTO Configuration..ExchangeCenterConfig
                                              ( CouponId ,
                                                CouponName ,
                                                CouponSum ,
                                                CouponSurplus ,
                                                Period ,
                                                PointsValue ,
                                                EndTime ,
                                                Status ,
                                                Image ,
                                                PointsRules ,
                                                Description ,
                                                CreateTime ,
                                                SmallImage,
                                                Sort,
                                                CouponEndTime,
                                                CouponDuration,
                                                GetRuleGUID,
                                                Postion,PID,UserRank,EndCouponPrice,ExchangeCenterType,Chance,StartVersion,EndVersion
                                              )
                                      VALUES  ( @CouponId , -- CouponId - int
                                                @CouponName , -- CouponName - nvarchar(50)
                                                @CouponSum , -- CouponSum - int
                                                @CouponSurplus , -- CouponSurplus - int
                                                @Period , -- Period - int
                                                @PointsValue , -- PointsValue - int
                                                @EndTime , -- EndTime - datetime
                                                @Status , -- Status - bit
                                                @Image , -- Image - nvarchar(1000)
                                                @PointsRules , -- PointsRules - text
                                                @Description , -- Description - nvarchar(1000)
                                                GETDATE() , -- CreateTime - datetime
                                                @SmallImage , -- SmallImage - nvarchar(1000)
                                                @Sort,
                                                @CouponEndTime,
                                                @CouponDuration,
                                                @GetRuleGUID,
                                                @Postion,@PID,@UserRank,@EndCouponPrice,@ExchangeCenterType,@Chance,@StartVersion,@EndVersion
                                              )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@CouponId",model.CouponId),
                    new SqlParameter("@CouponName",model.CouponName??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CouponSum",model.CouponSum),
                    new SqlParameter("@CouponSurplus",model.CouponSum),
                    new SqlParameter("@Description",model.Description??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@PointsValue",model.PointsValue),
                    new SqlParameter("@Period",model.Period),
                    new SqlParameter("@PointsRules",model.PointsRules),
                    new SqlParameter("@SmallImage",model.SmallImage),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@CouponEndTime",model.CouponEndTime??null),
                    new SqlParameter("@CouponDuration",model.CouponDuration??null),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID),
                     new SqlParameter("@Postion",model.Postion),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@UserRank",model.UserRank),
                    new SqlParameter("@EndCouponPrice",model.EndCouponPrice),
                    new SqlParameter("@ExchangeCenterType",model.ExchangeCenterType),
                    new SqlParameter("@Chance",model.Chance),
                    new SqlParameter("@StartVersion",model.StartVersion),
                    new SqlParameter("@EndVersion",model.EndVersion)

                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdateExchangeCenterConfig(ExchangeCenterConfig model)
        {
            const string sql = @"UPDATE Configuration..ExchangeCenterConfig SET
                                        CouponId=@CouponId , -- CouponId - int
                                        CouponName=@CouponName , -- CouponName - nvarchar(50)
                                        --CouponSum=@CouponSum , -- CouponSum - int
                                        --CouponSurplus=@CouponSurplus , -- CouponSurplus - int
                                        Period=@Period , -- Period - int
                                        PointsValue=@PointsValue , -- PointsValue - int                                     
                                        Status=@Status , -- Status - bit
                                        Image=@Image , -- Image - nvarchar(1000)
                                        PointsRules=@PointsRules , -- PointsRules - text
                                        Description=@Description , -- Description - nvarchar(1000)                                          
                                        SmallImage=@SmallImage , -- SmallImage - nvarchar(1000)
                                        CreateTime= GETDATE(),
                                        Sort=@Sort,
                                        CouponEndTime=@CouponEndTime,
                                        EndTime=@EndTime,
                                        CouponDuration=@CouponDuration,
                                        GetRuleGUID=@GetRuleGUID,
                                        Postion=@Postion,
                                        PID=@PID,
                                        UserRank=@UserRank,
                                        EndCouponPrice=@EndCouponPrice,
                                        ExchangeCenterType=@ExchangeCenterType,
                                        Chance=@Chance,
                                        EndVersion=@EndVersion,
                                        StartVersion=@StartVersion
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
                   {
                    new SqlParameter("@CouponId",model.CouponId),
                    new SqlParameter("@CouponName",model.CouponName??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CouponSum",model.CouponSum),
                    new SqlParameter("@CouponSurplus",model.CouponSurplus),
                    new SqlParameter("@Description",model.Description??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@PointsValue",model.PointsValue),
                    new SqlParameter("@Period",model.Period),
                    new SqlParameter("@PointsRules",model.PointsRules),
                    new SqlParameter("@SmallImage",model.SmallImage),
                    new SqlParameter("@Sort",model.Sort),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@CouponEndTime",model.CouponEndTime??null),
                    new SqlParameter("@CouponDuration",model.CouponDuration??null),
                    new SqlParameter("@EndTime",model.EndTime),
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID),
                    new SqlParameter("@Postion",model.Postion),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@UserRank",model.UserRank),
                    new SqlParameter("@EndCouponPrice",model.EndCouponPrice),
                    new SqlParameter("@ExchangeCenterType",model.ExchangeCenterType),
                    new SqlParameter("@Chance",model.Chance),
                    new SqlParameter("@EndVersion",model.EndVersion),
                    new SqlParameter("@StartVersion",model.StartVersion)

                   };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteExchangeCenterConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.ExchangeCenterConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static bool DeletePersonalCenterCouponConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_PersonalCenterCouponConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static int InsertPersonalCenterCouponConfig(PersonalCenterCouponConfig model)
        {
            const string sql = @"  INSERT  INTO Configuration.dbo.[SE_PersonalCenterCouponConfig]
                                              (
                                               [ExchangeCenterId]
                                              ,[HomePageModuleId] 
                                              ,[Sort]                                            
                                              )                                     
                                     VALUES  (  @ExchangeCenterId ,
                                                @HomePageModuleId,
                                                @Sort
                                             )SELECT @@IDENTITY
                                         ";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@HomePageModuleId",model.HomePageModuleId),
                    new SqlParameter("@ExchangeCenterId",model.ExchangeCenterId),
                    new SqlParameter("@Sort",model.Sort),

                };
            return Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
        }

        public static bool UpdatePersonalCenterCouponConfig(PersonalCenterCouponConfig model)
        {
            const string sql = @"DECLARE @Count INT
                                SELECT  @Count = COUNT(0)
                                FROM    Configuration.dbo.[SE_PersonalCenterCouponConfig]
                                WHERE   Sort = @Sort AND Id = @Id

                                IF @Count > 0
                                    BEGIN 
                                        SELECT  0 
                                    END 
                                ELSE
                                    BEGIN
                                        UPDATE  Configuration.dbo.[SE_PersonalCenterCouponConfig]
                                        SET     [Sort] = @Sort
                                        WHERE   Id = @Id 
                                        SELECT  1   
                                    END                 
                                         ";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Sort",model.Sort)
                };
            int i = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return i > 0;
        }
    }
}
