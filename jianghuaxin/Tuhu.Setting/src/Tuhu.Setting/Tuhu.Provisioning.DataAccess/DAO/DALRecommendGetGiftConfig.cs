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
    public class DALRecommendGetGiftConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<RecommendGetGiftConfig> GetRecommendGetGiftConfig(RecommendGetGiftConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  [Id]
                                  ,[Number]
                                  ,[Name]
                                  ,[Banner]
                                  ,[AwardLimit]
                                  ,[AwardType]
                                  ,[AwardValue]
                                  ,[GetRuleGUID]
                                  ,[RegisteredText]
                                  ,[AwardedText]
                                  ,[ShareButtonValue]
                                  ,[ShareChannel]
                                  ,[Rules]
                                  ,[TimeLimitCollectRules]
                                  ,[CreateName]
                                  ,[CreateTime]
                                  ,[UpdateName]
                                  ,[UpdateTime]
                                  ,TabName
                                  ,IsSendCode
                                  ,UserGroupId
                                  ,StartTime
                                  ,EndTime
                            FROM    [Configuration].[dbo].[SE_RecommendGetGiftConfig] AS A WITH (NOLOCK)                                 
                                    ORDER BY  A.[UpdateTime] DESC 
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS  ONLY 
                                ";
            string sqlCount = @"SELECT Count(0)
                                FROM    [Configuration].[dbo].[SE_RecommendGetGiftConfig] AS A WITH (NOLOCK)";

            var sqlParameters = new SqlParameter[]
                   {
                    new SqlParameter("@PageSize",pageSize),
                    new SqlParameter("@PageIndex",pageIndex)
                   };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RecommendGetGiftConfig>().ToList();

        }

        public static RecommendGetGiftConfig GetRecommendGetGiftConfigById(int id)
        {
            const string sql = @"SELECT    [Id]
                                          ,[Number]
                                          ,[Name]
                                          ,[Banner]
                                          ,[AwardLimit]
                                          ,[AwardType]
                                          ,[AwardValue]
                                          ,[GetRuleGUID]
                                          ,[RegisteredText]
                                          ,[AwardedText]
                                          ,[ShareButtonValue]
                                          ,[ShareChannel]
                                          ,[Rules]
                                          ,[TimeLimitCollectRules]
                                          ,[CreateName]
                                          ,[CreateTime]
                                          ,[UpdateName]
                                          ,[UpdateTime]
                                          ,TabName
                                          ,IsSendCode
                                          ,UserGroupId
                                          ,StartTime
                                          ,EndTime
                                  FROM [Configuration].[dbo].[SE_RecommendGetGiftConfig] WITH(NOLOCK)  WHERE Id=@Id";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<RecommendGetGiftConfig>().ToList().FirstOrDefault();
        }
        public static bool InsertRecommendGetGiftConfig(RecommendGetGiftConfig model, ref int id)
        {
            const string sql = @"  INSERT INTO Configuration..[SE_RecommendGetGiftConfig]
                                              (  
                                               [Number]
                                              ,[Name]
                                              ,[Banner]
                                              ,[AwardLimit]
                                              ,[AwardType]
                                              ,[AwardValue]
                                              ,[GetRuleGUID]
                                              ,[RegisteredText]
                                              ,[AwardedText]
                                              ,[ShareButtonValue]
                                              ,[ShareChannel]
                                              ,[Rules]
                                              ,[TimeLimitCollectRules]
                                              ,[CreateName]
                                              ,[CreateTime]
                                              ,[UpdateName]
                                              ,[UpdateTime]
                                              ,TabName
                                              ,IsSendCode
                                              ,UserGroupId
                                              ,StartTime
                                              ,EndTime
                                              )
                                      VALUES  ( 
                                               NEWID()
                                              ,@Name
                                              ,@Banner
                                              ,@AwardLimit
                                              ,@AwardType
                                              ,@AwardValue
                                              ,@GetRuleGUID
                                              ,@RegisteredText
                                              ,@AwardedText
                                              ,@ShareButtonValue
                                              ,@ShareChannel
                                              ,@Rules
                                              ,@TimeLimitCollectRules
                                              ,@CreateName
                                              ,GETDATE()
                                              ,@UpdateName
                                              ,GETDATE()
                                              ,@TabName
                                              ,@IsSendCode
                                              ,@UserGroupId
                                              ,@StartTime
                                              ,@EndTime
                                              )SELECT @@IDENTITY";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@AwardedText",model.AwardedText??string.Empty),
                    new SqlParameter("@AwardLimit",model.AwardLimit),
                    new SqlParameter("@AwardType",model.AwardType??string.Empty),
                    new SqlParameter("@AwardValue",model.AwardValue),
                    new SqlParameter("@Banner",model.Banner),
                    new SqlParameter("@CreateName",model.CreateName??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@RegisteredText",model.RegisteredText??string.Empty),
                    new SqlParameter("@Rules",model.Rules??string.Empty),
                    new SqlParameter("@TimeLimitCollectRules",model.TimeLimitCollectRules??string.Empty),
                    new SqlParameter("@ShareButtonValue",model.ShareButtonValue??string.Empty),
                    new SqlParameter("@ShareChannel",model.ShareChannel??string.Empty),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty),
                    new SqlParameter("@TabName",model.TabName??string.Empty),
                    new SqlParameter("@IsSendCode",model.IsSendCode),
                    new SqlParameter("@UserGroupId",model.UserGroupId),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime)
                };
            id = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, sqlParameter));
            return id > 0;
        }


        public static bool UpdateRecommendGetGiftConfig(RecommendGetGiftConfig model)
        {
            const string sql = @"UPDATE Configuration..SE_RecommendGetGiftConfig SET  
                                         Name=@Name
                                        ,Banner=@Banner
                                        ,AwardLimit=@AwardLimit
                                        ,AwardType=@AwardType
                                        ,AwardValue=@AwardValue
                                        ,RegisteredText=@RegisteredText
                                        ,AwardedText=@AwardedText
                                        ,ShareButtonValue=@ShareButtonValue
                                        ,ShareChannel=@ShareChannel
                                        ,Rules=@Rules 
                                        ,TimeLimitCollectRules=@TimeLimitCollectRules 
                                        ,UpdateName=@UpdateName
                                        ,UpdateTime=GETDATE() 
                                        ,GetRuleGUID=@GetRuleGUID 
                                        ,TabName=@TabName 
                                        ,IsSendCode=ISNULL(@IsSendCode,IsSendCode)
                                        ,UserGroupId=ISNULL(@UserGroupId,UserGroupId)
                                        ,StartTime=ISNULL(@StartTime,StartTime)
                                        ,EndTime=ISNULL(@EndTime,EndTime)
                                 WHERE Id=@Id";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@AwardedText",model.AwardedText??string.Empty),
                    new SqlParameter("@AwardLimit",model.AwardLimit),
                    new SqlParameter("@AwardType",model.AwardType??string.Empty),
                    new SqlParameter("@AwardValue",model.AwardValue),
                    new SqlParameter("@Banner",model.Banner),
                    new SqlParameter("@CreateName",model.CreateName??string.Empty),
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@RegisteredText",model.RegisteredText??string.Empty),
                    new SqlParameter("@Rules",model.Rules??string.Empty),
                    new SqlParameter("@TimeLimitCollectRules",model.TimeLimitCollectRules??string.Empty),
                    new SqlParameter("@ShareButtonValue",model.ShareButtonValue??string.Empty),
                    new SqlParameter("@ShareChannel",model.ShareChannel??string.Empty),
                    new SqlParameter("@UpdateName",model.UpdateName??string.Empty),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@GetRuleGUID",model.GetRuleGUID??string.Empty),
                    new SqlParameter("@TabName",model.TabName??string.Empty),
                    new SqlParameter("@IsSendCode",model.IsSendCode),
                    new SqlParameter("@UserGroupId",model.UserGroupId),
                    new SqlParameter("@StartTime",model.StartTime),
                    new SqlParameter("@EndTime",model.EndTime)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteRecommendGetGiftConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.SE_RecommendGetGiftConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static DataTable GetCoupon(string guidOrId)
        {
            Guid guid;
            int id = 0;
            DataTable dt = null;
            if (string.IsNullOrEmpty(guidOrId))
            {
                return dt;
            }

            string connString = ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            if (Guid.TryParse(guidOrId, out guid))
            {
                string sql = "SELECT * FROM  Activity.dbo.tbl_GetCouponRules WITH(NOLOCK) WHERE GetRuleGUID=@GetRuleGUID";
                SqlParameter parameters = new SqlParameter("@GetRuleGUID", guid);
                dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            };

            if (int.TryParse(guidOrId, out id))
            {
                string sql = "SELECT * FROM  Activity.dbo.tbl_GetCouponRules WITH(NOLOCK) WHERE PKID=@PKID";
                SqlParameter parameters = new SqlParameter("@PKID", id);
                dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            };

            conn.Close();
            conn.Dispose();
            return dt;
        }

    }
}

