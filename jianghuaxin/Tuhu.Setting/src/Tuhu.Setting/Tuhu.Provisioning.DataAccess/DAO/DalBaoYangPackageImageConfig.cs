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
    public class DalBaoYangPackageImageConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(strConn);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);
        public static List<BaoYangPackageImageConfig> GetBaoYangPackageImageConfig(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            string sqlStr = "";
            if (!string.IsNullOrWhiteSpace(pid))
            {
                sqlStr += " AND R.PID=@PID";
            }

            string sql = @"SELECT  *
                            FROM    ( SELECT    C.PID ,
                                                C.DisplayName ,
                                                B.Image ,
                                                B.Image2 ,
                                                B.CreateTime ,
                                                B.Id                                               
                                      FROM      Tuhu_productcatalog..[CarPAR_zh-CN] AS C WITH ( NOLOCK )
                                                LEFT JOIN [Configuration].[dbo].[SE_BaoYangPackageImageConfig]
                                                AS B WITH ( NOLOCK ) ON C.PID = B.PID
                                      WHERE     1 = 1
                                                AND C.PrimaryParentCategory = 'XBYTC'
                                                AND C.i_ClassType IN ( 2, 4 )
                                    ) AS R
                            WHERE   1 = 1 " + sqlStr + @"
                            ORDER BY R.CreateTime DESC
                                    OFFSET ( @PageIndex - 1 ) * @PageSize ROWS  FETCH NEXT @PageSize ROWS
                                    ONLY ";
            string sqlCount = @"SELECT  COUNT(0)
                            FROM    ( SELECT    C.PID ,
                                                C.PrimaryParentCategory           
                                      FROM      Tuhu_productcatalog..[CarPAR_zh-CN] AS C WITH ( NOLOCK )
                                                LEFT JOIN [Configuration].[dbo].[SE_BaoYangPackageImageConfig]
                                                AS B WITH ( NOLOCK ) ON C.PID = B.PID
                                      WHERE     1 = 1
                                                AND C.PrimaryParentCategory = 'XBYTC'
                                                AND C.i_ClassType IN ( 2, 4 )
                                    ) AS R
                            WHERE   1 = 1 " + sqlStr;
            var sqlParameters = new SqlParameter[]
                  {
                            new SqlParameter("@PID",pid),
                            new SqlParameter("@PageSize",pageSize),
                            new SqlParameter("@PageIndex",pageIndex),

                  };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<BaoYangPackageImageConfig>().ToList();

        }

        public static BaoYangPackageImageConfig GetBaoYangPackageImageConfigById(int id)
        {
            const string sql = @"SELECT  [Id]
                                          ,[PID]
                                          ,[Image]
                                          ,[Image2]
                                          ,[CreateTime]
                              FROM [Configuration].[dbo].[SE_BaoYangPackageImageConfig] WITH (NOLOCK)  WHERE Id = @Id ";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<BaoYangPackageImageConfig>().ToList().FirstOrDefault();
        }

        public static bool UpdateBaoYangPackageImageConfigNew(BaoYangPackageImageConfig model)
        {
            const string sql = @"IF EXISTS ( SELECT  1
            FROM    Configuration..SE_BaoYangPackageImageConfig WITH ( NOLOCK )
            WHERE   PID = @PID )
    BEGIN
        UPDATE  [Configuration].[dbo].[SE_BaoYangPackageImageConfig]
        SET     [Image] = @Image ,
                [Image2] = @Image2 ,
                [CreateTime] = GETDATE()
        WHERE   PID = @PID;
    END;
ELSE
    BEGIN
        INSERT  INTO [Configuration].[dbo].[SE_BaoYangPackageImageConfig]
                ( [PID] ,
                  [Image] ,
                  [Image2] ,
                  [CreateTime]
                )
        VALUES  ( @PID ,
                  @Image ,
                  @Image2 ,
                  GETDATE()
                );
    END;";

            var sqlParameters = new SqlParameter[]
                   {
                         new SqlParameter("@PID",model.PID),
                         new SqlParameter("@Image",model.Image),
                         new SqlParameter("@Image2",model.Image2),
                   };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        public static bool DeleteBaoYangPackageImageConfig(int id)
        {
            const string sql = @"DELETE FROM [Configuration].[dbo].[SE_BaoYangPackageImageConfig] WHERE Id =@Id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        public static List<BaoYangPackageImageConfig> GetBaoYangPackagePruduct()
        {
            const string sql = @"SELECT  PID ,
        DisplayName
FROM    Tuhu_productcatalog..[CarPAR_zh-CN] WITH ( NOLOCK )
WHERE   PrimaryParentCategory = 'XBYTC'
        AND i_ClassType IN ( 2, 4 );";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql).ConvertTo<BaoYangPackageImageConfig>().ToList();
        }

    }
}
