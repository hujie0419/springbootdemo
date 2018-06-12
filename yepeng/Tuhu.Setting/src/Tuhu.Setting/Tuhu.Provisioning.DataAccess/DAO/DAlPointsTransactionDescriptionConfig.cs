using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess
{
    public class DALPointsTransactionDescriptionConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<PointsTransactionDescriptionConfig> GetPointsTransactionDescriptionConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @" SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY [IsActive] DESC ) AS ROWNUMBER ,
                                                [IntegralRuleID] ,
                                                [IntegralType] ,
                                                [IntegralConditions] ,
                                                [IntegralDescribe] ,
                                                [Remark] ,
                                                [NeedeKeys] ,
                                                [IsActive]
                                      FROM      [Tuhu_profiles].[dbo].[tbl_UserIntegralRule] WITH ( NOLOCK )
                                      WHERE     1 = 1  " + sqlStr + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                               ";
            string sqlCount = @"SELECT COUNT(1) FROM [Tuhu_profiles].[dbo].[tbl_UserIntegralRule] WITH (NOLOCK)  WHERE 1=1  " + sqlStr;
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount);

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsTransactionDescriptionConfig>().ToList();

        }


        public static PointsTransactionDescriptionConfig GetPointsTransactionDescriptionConfig(string id)
        {
            const string sql = @"SELECT [IntegralRuleID]
                                      ,[IntegralType]
                                      ,[IntegralConditions]
                                      ,[IntegralDescribe]
                                      ,[Remark]
                                      ,[NeedeKeys]
                                      ,[IsActive]
                                  FROM [Tuhu_profiles].[dbo].[tbl_UserIntegralRule] WITH (NOLOCK) WHERE IntegralRuleID=@IntegralRuleID";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@IntegralRuleID",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<PointsTransactionDescriptionConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertPointsTransactionDescriptionConfig(PointsTransactionDescriptionConfig model)
        {
            const string sql = @"INSERT INTO [Tuhu_profiles].[dbo].[tbl_UserIntegralRule]
                                          ([IntegralRuleID]
                                          ,[IntegralType]
                                          ,[IntegralConditions]
                                          ,[IntegralDescribe]
                                          ,[Remark]
                                          ,[NeedeKeys]
                                          ,[IsActive]
                                          )
                                  VALUES(  NEWID()
                                          ,@IntegralType
                                          ,@IntegralConditions
                                          ,@IntegralDescribe
                                          ,@Remark
                                          ,@NeedeKeys
                                          ,@IsActive                                        
                                        )";

            var sqlParameter = new SqlParameter[]
                {
                  new SqlParameter("@IntegralConditions",model.IntegralConditions??string.Empty),
                  new SqlParameter("@IntegralDescribe",model.IntegralDescribe??string.Empty),
                  new SqlParameter("@IntegralType",model.IntegralType),
                  new SqlParameter("@IsActive",model.IsActive),
                  new SqlParameter("@NeedeKeys",model.NeedeKeys??string.Empty),
                  new SqlParameter("@Remark",model.Remark??string.Empty)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdatePointsTransactionDescriptionConfig(PointsTransactionDescriptionConfig model)
        {
            const string sql = @"UPDATE [Tuhu_profiles].[dbo].[tbl_UserIntegralRule] SET                                      
                                           [IntegralType]=@IntegralType
                                          ,[IntegralConditions]=@IntegralConditions
                                          ,[IntegralDescribe]=@IntegralDescribe
                                          ,[Remark]=@Remark
                                          ,[NeedeKeys]=@NeedeKeys
                                          ,[IsActive]=@IsActive
                                WHERE IntegralRuleID=@IntegralRuleID";
            var sqlParameter = new SqlParameter[]
               {
                 new SqlParameter("@IntegralConditions",model.IntegralConditions??string.Empty),
                 new SqlParameter("@IntegralDescribe",model.IntegralDescribe??string.Empty),
                 new SqlParameter("@IntegralRuleID",model.IntegralRuleID),
                 new SqlParameter("@IntegralType",model.IntegralType),
                 new SqlParameter("@IsActive",model.IsActive),
                 new SqlParameter("@NeedeKeys",model.NeedeKeys??string.Empty),
                 new SqlParameter("@Remark",model.Remark??string.Empty)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeletePointsTransactionDescriptionConfig(string id)
        {
            const string sql = @"DELETE FROM [Tuhu_profiles].[dbo].[tbl_UserIntegralRule] WHERE IntegralRuleID=@IntegralRuleID";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@IntegralRuleID", id)) > 0;
        }
    }
}
