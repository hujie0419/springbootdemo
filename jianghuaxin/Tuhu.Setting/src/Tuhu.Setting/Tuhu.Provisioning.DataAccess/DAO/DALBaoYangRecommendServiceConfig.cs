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
    public class DALBaoYangRecommendServiceConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<BaoYangRecommendServiceConfig> GetBaoYangRecommendServiceConfigList(string type, int pageSize, int pageIndex, out int recordCount)
        {
            string strSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(type))
            {
                strSql += " AND ServiceType = @ServiceType";
            }
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [Number] ,
                                                [ServiceType] ,
                                                [ServiceName] ,
                                                [GetRuleGUID] ,
                                                [CouponNames] ,
                                                [Recommend] ,
                                                [Status] ,
                                                [Tag] ,
                                                [CreateTime] ,
                                                [UpdateTime]
                                      FROM      [Configuration].[dbo].[BaoYangRecommendServiceConfig] WITH ( NOLOCK )
                                      WHERE     1 = 1    " + strSql + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                             ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[BaoYangRecommendServiceConfig] WITH (NOLOCK)  WHERE 1=1  " + strSql;

            var sqlParameters = new SqlParameter[]
                {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@ServiceType",type),
               };

            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlParameters);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangRecommendServiceConfig>().ToList();

        }


        public static BaoYangRecommendServiceConfig GetBaoYangRecommendServiceConfig(int id)
        {
            const string sql = @"SELECT [Id]
                                      ,[Number]
                                      ,[ServiceType]
                                      ,[ServiceName]
                                      ,[GetRuleGUID]
                                      ,[CouponNames]
                                      ,[Recommend]
                                      ,[Status]
                                      ,[Tag]
                                      ,[CreateTime]
                                      ,[UpdateTime]
                              FROM [Configuration].[dbo].[BaoYangRecommendServiceConfig] WITH (NOLOCK) WHERE Id=@id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangRecommendServiceConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            const string sql = @"INSERT INTO Configuration..BaoYangRecommendServiceConfig
                                          (                                       
                                           [Number]
                                          ,[ServiceType]
                                          ,[ServiceName]
                                          ,[GetRuleGUID]
                                          ,[CouponNames]
                                          ,[Recommend]
                                          ,[Status]
                                          ,[CreateTime]
                                          ,[UpdateTime]
                                          ,[Tag]
                                          )
                                  VALUES(  
                                           NEWID()
                                          ,@ServiceType
                                          ,@ServiceName
                                          ,@GetRuleGUID
                                          ,@CouponNames
                                          ,@Recommend
                                          ,@Status
                                          ,GETDATE()
                                          ,GETDATE()
                                          ,@Tag
                                          )";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty),
                    new SqlParameter("@CouponNames",model.CouponNames??string.Empty),
                    new SqlParameter("@Recommend",model.Recommend),
                    new SqlParameter("@ServiceName",model.ServiceName??string.Empty),
                    new SqlParameter("@ServiceType",model.ServiceType??string.Empty),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Tag",model.Tag??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        /// <summary>
        /// 服务不能重复
        /// </summary>      
        /// <returns></returns>
        public static bool CheckService(BaoYangRecommendServiceConfig model)
        {
            const string sql = @"SELECT COUNT(0)                                    
                              FROM [Configuration].[dbo].[BaoYangRecommendServiceConfig] WITH (NOLOCK) WHERE ServiceType=@ServiceType";
            return (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sql, new SqlParameter("@ServiceType", model.ServiceType)) > 0;
        }
        public static bool UpdateBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            const string sql = @"UPDATE Configuration..BaoYangRecommendServiceConfig SET                                      
                                           [ServiceType]=@ServiceType
                                          ,[ServiceName]=@ServiceName
                                          ,[GetRuleGUID]=@GetRuleGUID
                                          ,[CouponNames]=@CouponNames
                                          ,[Recommend]=@Recommend
                                          ,Tag=@Tag
                                          ,[Status]=@Status                                        
                                          ,[UpdateTime]=GETDATE()                                        
                                WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@ServiceType",model.ServiceType??string.Empty),
                    new SqlParameter("@ServiceName",model.ServiceName??string.Empty),
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty),
                    new SqlParameter("@CouponNames",model.CouponNames??string.Empty),
                    new SqlParameter("@Recommend",model.Recommend),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Tag",model.Tag??string.Empty)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteBaoYangRecommendServiceConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.BaoYangRecommendServiceConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }
    }
}

