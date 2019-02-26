using Dapper;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DALCompanyClient
    {
        public static List<CompanyClientConfig> SelectAllCompanyClientConfig(SqlConnection conn, int pageIndex, int pageSize)
        {
            var sql = @"   SELECT  *,COUNT(1) OVER() AS Total
    FROM    Configuration..CompanyClientConfig WITH ( NOLOCK )
    ORDER BY  PKID DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
            ROWS ONLY";
            return conn.Query<CompanyClientConfig>(sql, new { PageIndex = pageIndex, PageSize = pageSize }, commandType: CommandType.Text).ToList();
        }

        public static CompanyClientConfig SelectCompanyClientConfigByPkid(SqlConnection conn, int pkid)
        {
            var sql = @"   SELECT  * FROM    Configuration..CompanyClientConfig WITH ( NOLOCK ) WHERE PKID=@PKID";
            return conn.Query<CompanyClientConfig>(sql, new { PKID=pkid }, commandType: CommandType.Text).SingleOrDefault();
        }

        public static List<ClientCouponCode> SelectCouponCodeByParentId(SqlConnection conn, int parentId, int isBind)
        {
            return conn.Query<ClientCouponCode>(@"SELECT  * ,
                COUNT(1) OVER ( ) AS Total
        FROM    Configuration..CompanyClientCouponCode WITH ( NOLOCK )
        WHERE   ParentId = @ParentId
                AND ( @IsBind = -1
                      OR ( @IsBind = 1
                           AND Telephone IS NOT NULL
                         )
                      OR ( @IsBind = 0
                           AND Telephone IS NULL
                         )
                    )
        ORDER BY PKID DESC",
                new { ParentId = parentId, IsBind = isBind }, commandType: CommandType.Text).ToList();
        }

        public static int InsertCompanyClientConfig(SqlConnection conn, string channel, string url)
        {
            return (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, @"INSERT Configuration..CompanyClientConfig
            (Channel,ActivityUrl )
			OUTPUT Inserted.PKID
			VALUES
            ( @Channel,@ActivityUrl )", new SqlParameter[] { new SqlParameter("@Channel", channel), new SqlParameter("@ActivityUrl", url) });
        }

        public static int UpdateCompanyClientConfig(SqlConnection conn, string channel, string url, int pkid)
        {
            return conn.Execute(@"UPDATE Configuration..CompanyClientConfig SET Channel = @Channel, ActivityUrl = @ActivityUrl , UpdatedTime=GETDATE() WHERE PKID = @PKID", 
                new { PKID = pkid, Channel = channel, ActivityUrl = url }, commandType: CommandType.Text);
        }

        public static int DeletedCompanyClientConfig(SqlConnection conn,  int pkid)
        {
            return conn.Execute(@"DELETE FROM Configuration..CompanyClientConfig  WHERE PKID = @PKID",
                new { PKID = pkid }, commandType: CommandType.Text);
        }

        public static int DeletedCouponCodeByPkid(SqlConnection conn, int pkid)
        {
            return conn.Execute(@"DELETE FROM Configuration..CompanyClientCouponCode WHERE ParentId=@PKID",
                new { PKID = pkid }, commandType: CommandType.Text);
        }

        public static int GenerateCouponCode(SqlConnection conn, int count, int parentId)
        {
            var sql = @"DECLARE @BatchSize INT;
   DECLARE @InsertedCount INT;
   SET @BatchSize = @BatchCount;
   DECLARE @Codes TABLE
    (
      PKID INT IDENTITY(1, 1)
               PRIMARY KEY ,
      Code NVARCHAR(9)
    ); 
   DECLARE @Inserted TABLE ( Code NVARCHAR(9) ); 

   WHILE @BatchCount > 0
    BEGIN TRY 
        DECLARE @Index INT; 
        SET @Index = 1;
			  
        WHILE @Index <= @BatchCount
            AND @Index <= @BatchSize
            BEGIN
                DECLARE @code VARCHAR(9)= '';
		
                SET @code = RIGHT(1000000000
                                  + CONVERT(BIGINT, ABS(CHECKSUM(NEWID()))), 9);
                INSERT  INTO @Codes
                        ( Code )
                VALUES  ( @code );
                SET @Index += 1;
            END;
        INSERT  INTO Configuration..CompanyClientCouponCode
                ( ParentId ,
                  CouponCode,
				  CreatedTime
                )
        OUTPUT  Inserted.CouponCode
                INTO @Inserted
                SELECT  @ParentId ,
                        Code,
						GETDATE()
                FROM    @Codes;

        SELECT  @InsertedCount = COUNT(1)
        FROM    @Inserted;

        DELETE  @Codes;
        DELETE  @Inserted;
        SET @BatchCount -= @InsertedCount;
    END TRY
    BEGIN CATCH
        SELECT  @InsertedCount = COUNT(1)
        FROM    @Inserted;

        DELETE  @Codes;
        DELETE  @Inserted;
        SET @BatchCount -= @InsertedCount;
    END CATCH;";
            return conn.Execute(sql, new { BatchCount = count, ParentId = parentId }, commandType: CommandType.Text);
        }

        public static List<CompanyClientConfigLog> SelectOperationLog(string objectId, string type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Tuhu_log..CompanyClientConfigLog WITH (NOLOCK) WHERE ( @Type = '' OR Type=@Type ) AND ObjectId=@ObjectId  ORDER BY CreatedTime DESC");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectId", objectId);
                cmd.Parameters.AddWithValue("@Type", type );
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<CompanyClientConfigLog>().ToList();
            }
        }
    }
}
