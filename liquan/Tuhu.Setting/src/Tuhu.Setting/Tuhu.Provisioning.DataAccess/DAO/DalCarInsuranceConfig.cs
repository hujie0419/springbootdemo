using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalCarInsuranceConfig
    {
        #region banner 
        public static bool UpdateBannerIndex(SqlConnection conn, int bannerId, int displayIndex)
        {
            var sql = @"UPDATE  Configuration.dbo.CarInsuranceBanner
                        SET     DisplayIndex = @DisplayIndex
                        WHERE   PKID = @BannerId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BannerId", bannerId),
                new SqlParameter("@DisplayIndex", displayIndex)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateBanner(SqlConnection conn, int bannerId, string name, string img, string linkUrl, string displayPage)
        {
            var sql = @"UPDATE  Configuration.dbo.CarInsuranceBanner
                        SET     Name = @Name ,
                                Img = @Img ,
                                LinkUrl = @LinkUrl ,
                                DisplayPage = @DisplayPage
                        WHERE   PKID = @BannerId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", name),
                new SqlParameter("@Img", img),
                new SqlParameter("@LinkUrl", linkUrl),
                new SqlParameter("@DisplayPage", displayPage),
                new SqlParameter("@BannerId", bannerId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateBannerIsEnable(SqlConnection conn, int bannerId, int isEnable)
        {
            var sql = @"UPDATE  Configuration.dbo.CarInsuranceBanner
                        SET     IsEnable = @IsEnable
                        WHERE   PKID = @BannerId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IsEnable", isEnable),
                new SqlParameter("@BannerId", bannerId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool CreateBanner(SqlConnection conn, string name, string img, string linkUrl, int displayIndex, string displayPage, int isEnable)
        {
            var sql = @"INSERT  INTO Configuration.dbo.CarInsuranceBanner
                               ( Name ,
                                 Img ,
                                 LinkUrl ,
                                 DisplayIndex ,
                                 DisplayPage ,
                                 IsEnable
                                )
                        VALUES  ( @Name ,
                                  @Img ,
                                  @LinkUrl ,
                                  @DisplayIndex ,
                                  @DisplayPage ,
                                  @IsEnable
                                );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", name),
                new SqlParameter("@Img", img),
                new SqlParameter("@LinkUrl", linkUrl),
                new SqlParameter("@DisplayIndex", displayIndex),
                new SqlParameter("@DisplayPage", displayPage),
                new SqlParameter("@IsEnable", isEnable)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteBanner(SqlConnection conn, int bannerId)
        {
            var sql = @"DELETE  FROM Configuration.dbo.CarInsuranceBanner
                        WHERE   PKID = @BannerId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BannerId", bannerId)
            };

            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        #endregion

        #region insurance
        public static bool UpdateInsuranceIndex(SqlConnection conn, int insurancePartnerId, int displayIndex)
        {
            var sql = @"update Configuration.dbo.CarInsurancePartner set DisplayIndex = @DisplayIndex where PKID = @InsurancePartnerId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@DisplayIndex", displayIndex),
                new SqlParameter("@InsurancePartnerId", insurancePartnerId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateInsurance(SqlConnection conn, CarInsurancePartner insurance)
        {
            var sql = @"UPDATE  Configuration.dbo.CarInsurancePartner
SET     Name = @Name ,
        Img = @Img ,
        LinkUrl = @LinkUrl ,
        InsuranceId = @InsuranceId ,
        Remarks = @MarkText ,
        Title = @Title ,
        SubTitle = @SubTitle ,
        TagText = @TagText ,
        TagColor = @TagColor ,
        DisplayIndex = @DisplayIndex ,
        IsEnable = @IsEnable ,
        ProviderCode = @ProviderCode ,
        RegionCode = @RegionCode ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", insurance.Name),
                new SqlParameter("@Img", insurance.Img),
                new SqlParameter("@LinkUrl", insurance.LinkUrl),
                new SqlParameter("@InsuranceId", insurance.InsuranceId),
                new SqlParameter("@MarkText", insurance.Remarks),
                new SqlParameter("@Title", insurance.Title),
                new SqlParameter("@SubTitle", insurance.SubTitle),
                new SqlParameter("@TagText", insurance.TagText),
                new SqlParameter("@TagColor", insurance.TagColor),
                new SqlParameter("@DisplayIndex", insurance.DisplayIndex),
                new SqlParameter("@IsEnable", insurance.IsEnable),
                new SqlParameter("@ProviderCode", insurance.ProviderCode),
                new SqlParameter("@RegionCode", insurance.RegionCode),
                new SqlParameter("@PKID", insurance.PKID)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool UpdateInsuranceIsEnable(SqlConnection conn, int insurancePartnerId, int isEnable)
        {
            var sql = @"UPDATE  Configuration.dbo.CarInsurancePartner
                        SET     IsEnable = @IsEnable
                        WHERE   PKID = @InsurancePartnerId;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IsEnable", isEnable),
                new SqlParameter("@InsurancePartnerId", insurancePartnerId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static int CreateInsurance(SqlConnection conn, CarInsurancePartner insurance)
        {
            var sql = @"INSERT  INTO Configuration.dbo.CarInsurancePartner
        ( Name ,
          Img ,
          LinkUrl ,
          InsuranceId ,
          Remarks ,
          Title ,
          SubTitle ,
          TagText ,
          TagColor ,
          RegionCode ,
          ProviderCode ,
          DisplayIndex ,
          IsEnable 
        )
VALUES  ( @Name ,
          @Img ,
          @LinkUrl ,
          @InsuranceId ,
          @MarkText ,
          @Title ,
          @SubTitle ,
          @TagText ,
          @TagColor ,
          @RegionCode ,
          @ProviderCode ,
          @DisplayIndex ,
          @IsEnable 
        );
SELECT  SCOPE_IDENTITY();";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Name", insurance.Name),
                new SqlParameter("@Img", insurance.Img),
                new SqlParameter("@LinkUrl", insurance.LinkUrl),
                new SqlParameter("@InsuranceId", insurance.InsuranceId),
                new SqlParameter("@MarkText", insurance.Remarks),
                new SqlParameter("@Title", insurance.Title),
                new SqlParameter("@SubTitle", insurance.SubTitle),
                new SqlParameter("@TagText", insurance.TagText),
                new SqlParameter("@TagColor", insurance.TagColor),
                new SqlParameter("@DisplayIndex", insurance.DisplayIndex),
                new SqlParameter("@ProviderCode", insurance.ProviderCode),
                new SqlParameter("@RegionCode", insurance.RegionCode),
                new SqlParameter("@IsEnable", insurance.IsEnable)
            };
            var pkid = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, parameters);
            if (pkid == DBNull.Value)
                return -1;
            return Convert.ToInt32(pkid);
        }

        public static bool DeleteInsurance(SqlConnection conn, int insurancePartnerId)
        {
            var sql = @"DELETE  FROM Configuration.dbo.CarInsurancePartner
                        WHERE   PKID = @InsurancePartnerId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsurancePartnerId", insurancePartnerId)
            };

            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        #endregion

        #region select 
        public static List<CarInsuranceBanner> SelectBanners(SqlConnection conn)
        {
            var sql = @"SELECT  PKID,
                                Name ,
                                Img ,
                                LinkUrl ,
                                DisplayIndex ,
                                DisplayPage ,
                                IsEnable
                        FROM    Configuration.dbo.CarInsuranceBanner WITH ( NOLOCK )
                        ORDER BY DisplayIndex";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<CarInsuranceBanner>().ToList();
            return result;
        }

        public static List<CarInsurancePartner> SelectInsurance(SqlConnection conn)
        {
            var sql = @"SELECT  PKID ,
        Name ,
        Img ,
        LinkUrl ,
        InsuranceId ,
        Remarks ,
        Title ,
        SubTitle ,
        TagText ,
        TagColor ,
        DisplayIndex ,
        IsEnable ,
        ProviderCode ,
        RegionCode
FROM    Configuration.dbo.CarInsurancePartner WITH ( NOLOCK )
ORDER BY DisplayIndex;";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<CarInsurancePartner>().ToList();
            return result;
        }

        public static List<int> SelectRegionIds(SqlConnection conn)
        {
            var sql = @"SELECT  DISTINCT
        ISNULL(CityId, 0) AS CityId
FROM    Configuration..CarInsurancePartner AS p WITH ( NOLOCK )
        LEFT JOIN Configuration..CarInsuranceRegion AS r WITH ( NOLOCK ) ON p.PKID = r.InsurancePartnerId;";
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);
            var result = dt.Rows.Cast<DataRow>().Select(row => (int)row["CityId"]).ToList();
            return result;
        }

        public static CarInsuranceBanner SelectBannerById(SqlConnection conn, int bannerId)
        {
            var sql = @"SELECT  Name ,
                                Img ,
                                LinkUrl ,
                                DisplayIndex ,
                                DisplayPage ,
                                IsEnable
                        FROM    Configuration.dbo.CarInsuranceBanner WITH ( NOLOCK )
                        WHERE PKID = @BannerId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@BannerId", bannerId)
            };

            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarInsuranceBanner>().FirstOrDefault();
            return result;
        }

        public static CarInsurancePartner SelectInsuranceById(SqlConnection conn, int insurancePartnerId)
        {
            var sql = @"SELECT  Name ,
        Img ,
        LinkUrl ,
        InsuranceId ,
        Remarks ,
        Title ,
        SubTitle ,
        TagText ,
        TagColor ,
        DisplayIndex ,
        IsEnable ,
        ProviderCode ,
        RegionCode
FROM    Configuration.dbo.CarInsurancePartner WITH ( NOLOCK )
WHERE   PKID = @InsurancePartnerId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsurancePartnerId", insurancePartnerId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarInsurancePartner>().FirstOrDefault();
            return result;
        }

        public static int GetMaxBannerIndex(SqlConnection conn)
        {
            var sql = @"SELECT  MAX(DisplayIndex)
                        FROM    Configuration.dbo.CarInsuranceBanner";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (result == DBNull.Value)
                return 0;
            return (int)result;
        }


        public static int GetMaxInsuranceIndex(SqlConnection conn)
        {
            var sql = @"SELECT  MAX(DisplayIndex)
                        FROM    Configuration.dbo.CarInsurancePartner";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (result == DBNull.Value)
                return 0;
            return (int)result;
        }
        #endregion

        #region region
        public static List<CarInsuranceRegion> GetAllRegion(SqlConnection conn)
        {
            var sql = @"SELECT  PKID,
                                ProvinceId ,
                                ProvinceName ,
                                CityId ,
                                CityName ,
                                InsurancePartnerId
                        FROM    Configuration.dbo.CarInsuranceRegion WITH ( NOLOCK )";
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<CarInsuranceRegion>().ToList();
            return result;
        }

        public static List<CarInsuranceRegion> GetRegion(SqlConnection conn, int regionId)
        {
            var sql = @"SELECT  PKID,
                                ProvinceId ,
                                ProvinceName ,
                                CityId ,
                                CityName ,
                                InsurancePartnerId
                        FROM    Configuration.dbo.CarInsuranceRegion WITH ( NOLOCK )
                        WHERE   CityId = @CityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CityId", regionId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarInsuranceRegion>().ToList();
            return result;
        }

        public static List<CarInsuranceRegion> GetRegionByInsurancePartnerId(SqlConnection conn, int insurancePartnerId)
        {
            var sql = @"SELECT  PKID,
                                ProvinceId ,
                                ProvinceName ,
                                CityId ,
                                CityName ,
                                InsurancePartnerId
                        FROM    Configuration.dbo.CarInsuranceRegion WITH ( NOLOCK )
                        WHERE   InsurancePartnerId = @InsurancePartnerId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsurancePartnerId", insurancePartnerId)
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters).ConvertTo<CarInsuranceRegion>().ToList();
            return result;
        }

        public static bool CreateRegion(SqlConnection conn, CarInsuranceRegion region)
        {
            var sql = @"INSERT  INTO Configuration.dbo.CarInsuranceRegion
                                    ( ProvinceId ,
                                      ProvinceName ,
                                      CityId ,
                                      CityName ,
                                      InsurancePartnerId
                                    )
                                VALUES  ( @ProvinceId ,
                                          @ProvinceName ,
                                          @CityId ,
                                          @CityName ,
                                          @InsurancePartnerId
                                        );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ProvinceId", region.ProvinceId),
                new SqlParameter("@ProvinceName", region.ProvinceName),
                new SqlParameter("@CityId", region.CityId),
                new SqlParameter("@CityName", region.CityName),
                new SqlParameter("@InsurancePartnerId", region.InsurancePartnerId)
            };

            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteRegion(SqlConnection conn, int regionId, int insurancePartnerId)
        {
            var sql = @"DELETE FROM   Configuration.dbo.CarInsuranceRegion
                        WHERE     InsurancePartnerId = @InsurancePartnerId
                        AND   CityId = @CityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsurancePartnerId", insurancePartnerId),
                new SqlParameter("@CityId", regionId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteRegionByRegionId(SqlConnection conn, int regionId)
        {
            var sql = @"DELETE FROM   Configuration.dbo.CarInsuranceRegion
                        WHERE    CityId = @CityId;";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@CityId", regionId)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static bool DeleteRegionByInsurancePartnerId(SqlConnection conn, int insurancePartnerId)
        {
            var sql = @"DELETE FROM   Configuration.dbo.CarInsuranceRegion
                        WHERE     InsurancePartnerId = @InsurancePartnerId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@InsurancePartnerId", insurancePartnerId),
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        #endregion

        #region
        public static bool UpdateFAQ(SqlConnection conn, string FAQ)
        {
            var sql = @"UPDATE  Configuration.dbo.SimpleConfig
                        SET     ConfigContent = @ConfigContent,
                                UpdatedTime = GETDATE()
                        WHERE   ConfigName = N'CarInsuranceFAQ';";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigContent", FAQ)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;
        }

        public static string SelectFAQ(SqlConnection conn)
        {
            var sql = @"SELECT  ConfigContent
                        FROM    Configuration.dbo.SimpleConfig WITH ( NOLOCK )
                        WHERE   ConfigName = N'CarInsuranceFAQ';";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (result == DBNull.Value)
                return null;
            return (string)result;
        }

        public static bool CreateFAQ(SqlConnection conn, string FAQ)
        {
            var sql = @"INSERT  INTO Configuration.dbo.SimpleConfig
                                ( ConfigName ,
                                  ConfigContent ,
                                  Description ,
                                  ConfigUrl,
                                  CreatedTime
                                )
                        VALUES  ( N'CarInsuranceFAQ' ,
                                  @ConfigContent ,
                                  N'车险页脚' ,
                                  N' ',
                                  GETDATE()
                                );";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ConfigContent", FAQ)
            };
            var count = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);
            return count > 0;

        }
        #endregion
    }
}
