using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.ProductSearch;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALDefaultSearchConfig
    {
        private static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static string connectionString = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
        private static SqlConnection conn = new SqlConnection(connectionString);


        private static string strConnOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static string connectionStringOnRead = SecurityHelp.IsBase64Formatted(strConnOnRead) ? SecurityHelp.DecryptAES(strConnOnRead) : strConnOnRead;
        private static SqlConnection connOnRead = new SqlConnection(strConnOnRead);

        public static List<DefaultSearchConfig> GetDefaultSearchConfigList(string type, int pageSize, int pageIndex, out int recordCount)
        {
            string strSql = string.Empty;
            if (!string.IsNullOrWhiteSpace(type))
            {
                strSql += " AND Type = @Type";
            }

            string sql = @"
                            SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY CreateTime DESC ) AS ROWNUMBER ,
                                                [Id] ,
                                                [Name] ,
                                                [Keywords] ,
                                                [Image] ,
                                                [Description] ,
                                                [Link] ,
                                                [IOSProcessValue] ,
                                                [AndroidProcessValue] ,
                                                [IOSCommunicationValue] ,
                                                [AndroidCommunicationValue] ,
                                                [WXAPPValue] ,
                                                [H5Value] ,
                                                [Type] ,
                                                [CreateTime]
                                      FROM      [Configuration].[dbo].[DefaultSearchConfig] WITH ( NOLOCK ) 
                                      WHERE     1 = 1   " + strSql + @"
                                    ) AS PG
                            WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                                 AND     STR(@PageIndex * @PageSize)
                            ";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[DefaultSearchConfig] WITH (NOLOCK)  WHERE 1=1  " + strSql;
            var sqlParameters = new SqlParameter[]
           {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@Type",type)
           };
            recordCount = (int)SqlHelper.ExecuteScalar(connOnRead, CommandType.Text, sqlCount, sqlParameters);
           
            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<DefaultSearchConfig>().ToList();

        }


        public static DefaultSearchConfig GetDefaultSearchConfig(int id)
        {
            const string sql = @"
SELECT  [Id] ,
        [Name] ,
        [Keywords] ,
        [Image] ,
        [Description] ,
        [Link] ,
        [IOSProcessValue] ,
        [AndroidProcessValue] ,
        [IOSCommunicationValue] ,
        [AndroidCommunicationValue] ,
        [WXAPPValue] ,
        [H5Value] ,
        [Type] ,
        [CreateTime] ,
        ActivityType ,
        ActivityPromotionId ,
        ActivityStartTime ,
        ActivityEndTime
FROM    [Configuration].[dbo].[DefaultSearchConfig] WITH ( NOLOCK )
WHERE   Id = @id;";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };

            return SqlHelper.ExecuteDataTable(connOnRead, CommandType.Text, sql, sqlParameters).ConvertTo<DefaultSearchConfig>().ToList().FirstOrDefault();

        }

        public static bool InsertDefaultSearchConfig(DefaultSearchConfig model)
        {
            const string sql = @"  
INSERT  INTO Configuration..DefaultSearchConfig
        ( [Name] ,
          [Keywords] ,
          [Image] ,
          [Description] ,
          [Link] ,
          [IOSProcessValue] ,
          [AndroidProcessValue] ,
          [IOSCommunicationValue] ,
          [AndroidCommunicationValue] ,
          [WXAPPValue] ,
          [H5Value] ,
          [Type] ,
          [CreateTime] ,
          ActivityType ,
          ActivityPromotionId ,
          ActivityStartTime ,
          ActivityEndTime
        )
VALUES  ( @Name ,
          @Keywords ,
          @Image ,
          @Description ,
          @Link ,
          @IOSProcessValue ,
          @AndroidProcessValue ,
          @IOSCommunicationValue ,
          @AndroidCommunicationValue ,
          @WXAPPValue ,
          @H5Value ,
          @Type ,
          GETDATE() ,
          @ActivityType ,
          @ActivityPromotionId ,
          @ActivityStartTime ,
          @ActivityEndTime
        );";

            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@Keywords",model.Keywords??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@Description",model.Description??string.Empty),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@WXAPPValue",model.WXAPPValue??string.Empty),
                    new SqlParameter("@H5Value",model.H5Value??string.Empty),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@ActivityType",model.ActivityType),
                    new SqlParameter("@ActivityPromotionId",model.ActivityPromotionId),
                    new SqlParameter("@ActivityStartTime",model.ActivityStartTime),
                    new SqlParameter("@ActivityEndTime",model.ActivityEndTime)

                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool UpdateDefaultSearchConfig(DefaultSearchConfig model)
        {
            const string sql = @"
UPDATE  Configuration..DefaultSearchConfig
SET     Name = @Name ,
        Keywords = @Keywords ,
        Image = @Image ,
        Description = @Description ,
        Link = @Link ,
        IOSProcessValue = @IOSProcessValue ,
        AndroidProcessValue = @AndroidProcessValue ,
        IOSCommunicationValue = @IOSCommunicationValue ,
        AndroidCommunicationValue = @AndroidCommunicationValue ,
        WXAPPValue = @WXAPPValue ,
        H5Value = @H5Value ,
        Type = @Type ,
        CreateTime = GETDATE() ,
        ActivityType = @ActivityType ,
        ActivityPromotionId = @ActivityPromotionId ,
        ActivityStartTime = @ActivityStartTime ,
        ActivityEndTime = @ActivityEndTime
WHERE   Id = @Id;";
            var sqlParameter = new SqlParameter[]
               {
                    new SqlParameter("@Name",model.Name??string.Empty),
                    new SqlParameter("@Keywords",model.Keywords??string.Empty),
                    new SqlParameter("@Image",model.Image??string.Empty),
                    new SqlParameter("@Description",model.Description??string.Empty),
                    new SqlParameter("@Link",model.Link??string.Empty),
                    new SqlParameter("@IOSProcessValue",model.IOSProcessValue??string.Empty),
                    new SqlParameter("@AndroidProcessValue",model.AndroidProcessValue??string.Empty),
                    new SqlParameter("@IOSCommunicationValue",model.IOSCommunicationValue??string.Empty),
                    new SqlParameter("@AndroidCommunicationValue",model.AndroidCommunicationValue??string.Empty),
                    new SqlParameter("@WXAPPValue",model.WXAPPValue??string.Empty),
                    new SqlParameter("@H5Value",model.H5Value??string.Empty),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@ActivityType",model.ActivityType),
                    new SqlParameter("@ActivityPromotionId",model.ActivityPromotionId),
                    new SqlParameter("@ActivityStartTime",model.ActivityStartTime),
                    new SqlParameter("@ActivityEndTime",model.ActivityEndTime)
               };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }

        public static bool DeleteDefaultSearchConfig(int id)
        {
            const string sql = @"DELETE FROM Configuration.dbo.DefaultSearchConfig WHERE Id=@Id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@Id", id)) > 0;
        }

        #region Brands
        public static List<SearchWordBrandsConfig> GetSearchWordBrandsConfigList(string keyword, int pageSize, int pageIndex, out int recordCount)
        {
            var queryStr = string.IsNullOrEmpty(keyword) ? "" : (
                $" (Keywords like N'% {keyword}' OR Keywords like N'{keyword} %' OR Keywords like N'% {keyword} %' OR Keywords=N'{keyword}') AND "
                );

            string sql = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY UpdateTime DESC ) AS ROWNUMBER ,
                    PKID ,
                    Keywords ,
                    Brands ,
                    IsShow ,
                    IsDelete ,
                    CreateTime ,
                    UpdateTime
          FROM      [Configuration].[dbo].[SearchWordBrandsConfig] WITH ( NOLOCK )
          WHERE     " + queryStr + @" IsDelete=0
        ) AS PG
WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                     AND     STR(@PageIndex * @PageSize);";
            string sqlCount = @"SELECT COUNT(1) FROM [Configuration].[dbo].[SearchWordBrandsConfig] WITH (NOLOCK)  
                                WHERE " + queryStr + @" IsDelete=0";
            var sqlParameters = new SqlParameter[]
           {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@keyword",keyword)
           };
            recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlParameters);

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<SearchWordBrandsConfig>().ToList();
        }
        public static bool InsertSearchWordBrandsConfig(SearchWordBrandsConfig model)
        {
            const string sql = @"
INSERT INTO [Configuration].[dbo].[SearchWordBrandsConfig]
        ( [Keywords] ,
          [Brands] ,
          [IsShow] 
        )
VALUES  ( @Keywords , -- Keywords - nvarchar(200)
          @Brands , -- Brands - nvarchar(200)
          @IsShow 
        )";
            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Keywords",model.Keywords),
                    new SqlParameter("@Brands",model.Brands),
                    new SqlParameter("@IsShow",model.IsShow)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
        public static bool UpdateSearchWordBrandsConfig(SearchWordBrandsConfig model)
        {
            const string sql = @"
UPDATE  Configuration..SearchWordBrandsConfig
SET     Keywords = @Keywords ,
        Brands = @Brands ,
        IsShow = @IsShow ,
        UpdateTime = GETDATE()
WHERE   PKID = @Pkid;";
            var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@Keywords",model.Keywords),
                    new SqlParameter("@Brands",model.Brands),
                    new SqlParameter("@IsShow",model.IsShow),
                    new SqlParameter("@Pkid",model.Pkid)
                };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameter) > 0;
        }
        public static bool DeleteSearchWordBrandsConfig(int pkid)
        {
            const string sql = @"
UPDATE  Configuration..SearchWordBrandsConfig
SET     IsDelete = 1
WHERE   PKID = @pkid;";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@pkid", pkid)) > 0;
        }
        public static SearchWordBrandsConfig GetSearchWordBrandsConfig(int pkid)
        {
            const string sql = @"
SELECT  PKID ,
        Keywords ,
        Brands ,
        IsShow ,
        IsDelete ,
        CreateTime ,
        UpdateTime
FROM    [Configuration].[dbo].[SearchWordBrandsConfig] WITH ( NOLOCK )
WHERE   PKID = @pkid";


            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@pkid",pkid)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<SearchWordBrandsConfig>().ToList().FirstOrDefault();

        }
        #endregion
    }
}

