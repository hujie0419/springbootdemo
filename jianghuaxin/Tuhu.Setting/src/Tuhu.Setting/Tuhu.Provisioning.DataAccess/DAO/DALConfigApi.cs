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
    public class DALConfigApi
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);

        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(connectionStringOnRead);

        public static List<ConfigApi> GetConfigApi(ConfigApi model)
        {
            string sqlWhere = "";

            if (model.Type != 0)
            {
                sqlWhere += " AND Type=@Type";
            }
            string sql = @"SELECT *  FROM [Configuration].[dbo].[ConfigApi] WITH (NOLOCK) WHERE 1 = 1  " + sqlWhere +  "  ORDER BY  CreateTime  DESC";

            var sqlparams = new SqlParameter[]
            {
                new SqlParameter("@Type",model.Type)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlparams).ConvertTo<ConfigApi>().ToList();
        }

        public static ConfigApi GetConfigApi(int id)
        {
            const string sql = @"SELECT TOP 1 *   FROM [Configuration].[dbo].[ConfigApi] WITH (NOLOCK) WHERE Id=@Id";
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, new SqlParameter("@Id", id)).ConvertTo<ConfigApi>().ToList().FirstOrDefault();
        }
        public static bool UpdateConfigApi(ConfigApi model)
        {
            const string sql = @"UPDATE [Configuration].[dbo].[ConfigApi] SET Value=@Value ,CreateTime=GETDATE(),Remark=@Remark,[Type]=@Type WHERE Id=@Id";

            var sqlParam = new SqlParameter[]
                {
                    new SqlParameter("@Value",model.Value),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Remark",model.Remark),
                    new SqlParameter("@Type",model.Type)
                };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam) > 0;
        }

        public static bool DeleteConfigApi(int id)
        {
            const string sql = @"DELETE  FROM [Configuration].[dbo].[ConfigApi] WHERE Id = @id";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;
        }

        public static int InsertConfigApi(ConfigApi model)
        {
            const string sql1 = @"SELECT  COUNT(0) FROM  [Configuration].[dbo].[ConfigApi] WITH ( NOLOCK ) WHERE [Key]=@Key ";

            var sqlParam1 = new SqlParameter[]
           {
                    new SqlParameter("@Key",model.Key)
           };

            int count = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sql1, sqlParam1);
            if (count > 0)
            {
                return -1;
            }

            const string sql = @"INSERT INTO Configuration..ConfigApi
                                        ([Key], Value ,CreateTime ,Remark,[Type])
                                VALUES  (
                                         @Key, -- Key - varchar(50)
                                         @Value , -- Value - varchar(50)
                                         GETDATE(),
                                         @Remark,@Type
                                          )";

            var sqlParam = new SqlParameter[]
             {
                    new SqlParameter("@Value",model.Value),
                    new SqlParameter("@Key",model.Key),
                    new SqlParameter("@Remark",model.Remark??string.Empty),
                    new SqlParameter("@Type",model.Type)
             };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParam);
        }

        public static ConfigCoupon GetConfigCoupon(SqlConnection conn)
        {
            const string sql = @"SELECT TOP 1  * FROM  [Configuration].[dbo].[ConfigCoupon] WITH ( NOLOCK ) ORDER BY CreateTime DESC";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<ConfigCoupon>().ToList().FirstOrDefault();

        }

        public static bool UpdateConfigCoupon(SqlConnection conn, ConfigCoupon model)
        {
            const string sql = @"UPDATE  [Configuration]..ConfigCoupon
                                SET     BackgroundImage = @BackgroundImage ,
                                        BackgroundSmallImage = @BackgroundSmallImage ,
                                        BackgroundColor = @BackgroundColor ,
                                        ActivityRule = @ActivityRule ,
                                        LinkAvailable = @LinkAvailable ,
                                        LinkImage = @LinkImage ,
                                        LinkSamllImage = @LinkSamllImage ,
                                        AppLink = @AppLink ,
                                        WeixinLink = @WeixinLink ,
                                        Link = @Link ,
                                        CheckLink = @CheckLink ,
                                        CreateTime = GETDATE()
                                WHERE   Id = @Id";
            var sqlParams = new SqlParameter[]
                 {
                    new SqlParameter("@ActivityRule",model.ActivityRule??string.Empty),
                    new SqlParameter("@AppLink",model.AppLink??string.Empty),
                    new SqlParameter("@BackgroundColor",model.BackgroundColor??string.Empty),
                    new SqlParameter("@BackgroundImage",model.BackgroundImage??string.Empty),
                    new SqlParameter("@BackgroundSmallImage",model.BackgroundSmallImage??string.Empty),
                    new SqlParameter("@LinkAvailable",model.LinkAvailable),
                    new SqlParameter("@LinkImage",model.LinkImage??string.Empty),
                    new SqlParameter("@LinkSamllImage",model.LinkSamllImage??string.Empty),
                    new SqlParameter("@WeixinLink",model.WeixinLink??string.Empty),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Link",model.Link),
                    new SqlParameter("@CheckLink",model.CheckLink)
                 };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams) > 0;

        }

        public static bool InsertConfigCoupon(SqlConnection conn, ConfigCoupon model)
        {
            const string sql = @"INSERT INTO [Configuration]..ConfigCoupon
                                        ( BackgroundImage ,
                                          BackgroundSmallImage ,
                                          BackgroundColor ,
                                          ActivityRule ,
                                          LinkAvailable ,
                                          LinkImage ,
                                          LinkSamllImage ,
                                          AppLink ,
                                          WeixinLink ,
                                          Link ,
                                          CheckLink ,
                                          CreateTime
                                        )
                                VALUES  ( @BackgroundImage , -- BackgroundImage - nvarchar(1000)
                                          @BackgroundSmallImage , -- BackgroundSmallImage - nvarchar(1000)
                                          @BackgroundColor , -- BackgroundColor - nvarchar(50)
                                          @ActivityRule , -- ActivityRule - text
                                          @LinkAvailable , -- LinkAvailable - bit
                                          @LinkImage , -- LinkImage - nvarchar(1000)
                                          @LinkSamllImage , -- LinkSamllImage - nvarchar(1000)
                                          @AppLink , -- AppLink - nvarchar(1000)
                                          @WeixinLink  ,-- WeixinLink - nvarchar(1000)
                                          @Link ,
                                          @CheckLink ,
                                          GETDATE()
                                        )";
            var sqlParams = new SqlParameter[]
                    {
                    new SqlParameter("@ActivityRule",model.ActivityRule??string.Empty),
                    new SqlParameter("@AppLink",model.AppLink??string.Empty),
                    new SqlParameter("@BackgroundColor",model.BackgroundColor??string.Empty),
                    new SqlParameter("@BackgroundImage",model.BackgroundImage??string.Empty),
                    new SqlParameter("@BackgroundSmallImage",model.BackgroundSmallImage??string.Empty),
                    new SqlParameter("@LinkAvailable",model.LinkAvailable),
                    new SqlParameter("@LinkImage",model.LinkImage??string.Empty),
                    new SqlParameter("@LinkSamllImage",model.LinkSamllImage??string.Empty),
                    new SqlParameter("@WeixinLink",model.WeixinLink??string.Empty),
                    new SqlParameter("@Link",model.Link),
                    new SqlParameter("@CheckLink",model.CheckLink)

                    };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParams) > 0;

        }
    }
}
