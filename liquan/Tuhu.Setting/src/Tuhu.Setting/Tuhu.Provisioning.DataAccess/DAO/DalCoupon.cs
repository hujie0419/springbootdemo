using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public static class DalCoupon
    {
        #region CouponCategory
        public static List<CouponCategory> GetAllCouponCategory(SqlConnection connection)
        {
            var sql = "SELECT * FROM tbl_CouponCategory WITH (NOLOCK) ORDER BY IsActive DESC,PKID";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql).ConvertTo<CouponCategory>().ToList();
        }

        public static void DeleteCouponCategory(SqlConnection connection, int id)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",id)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_CouponCategory WHERE PKID=@PKID;DELETE FROM tbl_Coupon WHERE CategoryID=@PKID", sqlParamters);
        }

        public static void AddCouponCategory(SqlConnection connection, CouponCategory couponCategory)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@EnID",couponCategory.EnID),
                new SqlParameter("@Name",couponCategory.Name??string.Empty),
                new SqlParameter("@Description",couponCategory.Description??string.Empty),
                new SqlParameter("@Title",couponCategory.Title??string.Empty),
                new SqlParameter("@Text",couponCategory.Text??string.Empty),
                new SqlParameter("@Picture",couponCategory.Picture??string.Empty),
                new SqlParameter("@ValidDays",couponCategory.ValidDays),
                new SqlParameter("@IsActive",couponCategory.IsActive),
                new SqlParameter("@Percentage",couponCategory.Percentage),
                new SqlParameter("@CouponType",couponCategory.CouponType??null),
                new SqlParameter("@RuleID",couponCategory.RuleID??null),
                new SqlParameter("@Remark",couponCategory.Remark??string.Empty),
                new SqlParameter("@Platform",couponCategory.Platform??string.Empty),
                new SqlParameter("@BackImage",couponCategory.BackImage??string.Empty),
                new SqlParameter("@ContentImage",couponCategory.ContentImage??string.Empty),
                new SqlParameter("@BackgroundImage",couponCategory.BackgroundImage??string.Empty),
                new SqlParameter("@SpinImage",couponCategory.SpinImage??string.Empty),
                new SqlParameter("@AndroidCommunicationValue",couponCategory.AndroidCommunicationValue??string.Empty),
                new SqlParameter("@AndroidDisposeValue",couponCategory.AndroidDisposeValue??string.Empty),
                new SqlParameter("@AppLink",couponCategory.AppLink??string.Empty),
                new SqlParameter("@CouponDescriptionStatus",couponCategory.CouponDescriptionStatus),
                new SqlParameter("@GetCouponDescription",couponCategory.GetCouponDescription??string.Empty),
                new SqlParameter("@IOSCommunicationValue",couponCategory.IOSCommunicationValue??string.Empty),
                new SqlParameter("@IOSDisposeValue",couponCategory.IOSDisposeValue??string.Empty),
                new SqlParameter("@RoamingDescription",couponCategory.RoamingDescription??string.Empty)
            };

            const string sql = @"INSERT   INTO tbl_CouponCategory
                                        ( EnID ,
                                          Name ,
                                          Description ,
                                          Title ,
                                          Text ,
                                          Picture ,
                                          ValidDays ,
                                          IsActive ,
                                          Percentage ,
                                          CouponType ,
                                          RuleID ,
                                          Remark ,
                                          Platform ,
                                          BackImage ,
                                          ContentImage ,
                                          BackgroundImage ,
                                          SpinImage ,
                                          IOSDisposeValue ,
                                          AndroidDisposeValue ,
                                          IOSCommunicationValue ,
                                          AndroidCommunicationValue ,
                                          AppLink ,
                                          GetCouponDescription ,
                                          CouponDescriptionStatus ,
                                          RoamingDescription		                    
                                        )
                               VALUES   ( @EnID ,
                                          @Name ,
                                          @Description ,
                                          @Title ,
                                          @Text ,
                                          @Picture ,
                                          @ValidDays ,
                                          @IsActive ,
                                          @Percentage ,
                                          @CouponType ,
                                          @RuleID ,
                                          @Remark ,
                                          @Platform ,
                                          @BackImage ,
                                          @ContentImage ,
                                          @BackgroundImage ,
                                          @SpinImage ,
                                          @IOSDisposeValue ,
                                          @AndroidDisposeValue ,
                                          @IOSCommunicationValue ,
                                          @AndroidCommunicationValue ,
                                          @AppLink ,
                                          @GetCouponDescription ,
                                          @CouponDescriptionStatus ,
                                          @RoamingDescription		
                                        )";

            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }

        public static void UpdateCouponCategory(SqlConnection connection, CouponCategory couponCategory)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",couponCategory.PKID),
                new SqlParameter("@EnID",couponCategory.EnID),
                new SqlParameter("@Name",couponCategory.Name??string.Empty),
                new SqlParameter("@Description",couponCategory.Description??string.Empty),
                new SqlParameter("@Title",couponCategory.Title??string.Empty),
                new SqlParameter("@Text",couponCategory.Text??string.Empty),
                new SqlParameter("@Picture",couponCategory.Picture??string.Empty),
                new SqlParameter("@ValidDays",couponCategory.ValidDays),
                new SqlParameter("@IsActive",couponCategory.IsActive),
                new SqlParameter("@Percentage",couponCategory.Percentage),
                new SqlParameter("@CouponType",couponCategory.CouponType??null),
                new SqlParameter("@RuleID",couponCategory.RuleID??null),
                new SqlParameter("@Remark",couponCategory.Remark??string.Empty),
                new SqlParameter("@Platform",couponCategory.Platform??string.Empty),
                new SqlParameter("@BackImage",couponCategory.BackImage??string.Empty),
                new SqlParameter("@ContentImage",couponCategory.ContentImage??string.Empty),
                new SqlParameter("@BackgroundImage",couponCategory.BackgroundImage??string.Empty),
                new SqlParameter("@SpinImage",couponCategory.SpinImage??string.Empty),
                new SqlParameter("@AndroidCommunicationValue",couponCategory.AndroidCommunicationValue??string.Empty),
                new SqlParameter("@AndroidDisposeValue",couponCategory.AndroidDisposeValue??string.Empty),
                new SqlParameter("@AppLink",couponCategory.AppLink??string.Empty),
                new SqlParameter("@CouponDescriptionStatus",couponCategory.CouponDescriptionStatus),
                new SqlParameter("@GetCouponDescription",couponCategory.GetCouponDescription??string.Empty),
                new SqlParameter("@IOSCommunicationValue",couponCategory.IOSCommunicationValue??string.Empty),
                new SqlParameter("@IOSDisposeValue",couponCategory.IOSDisposeValue??string.Empty),
                new SqlParameter("@RoamingDescription",couponCategory.RoamingDescription??string.Empty)
            };
            const string sql = @"UPDATE  tbl_CouponCategory
                                SET     EnID = @EnID ,
                                        Name = @Name ,
                                        Description = @Description ,
                                        Title = @Title ,
                                        Text = @Text ,
                                        Picture = @Picture ,
                                        ValidDays = @ValidDays ,
                                        IsActive = @IsActive ,
                                        Percentage = @Percentage ,
                                        CouponType = @CouponType ,
                                        RuleID = @RuleID ,
                                        Remark = @Remark ,
                                        Platform = @Platform ,
                                        BackImage = @BackImage ,
                                        ContentImage = @ContentImage ,
                                        BackgroundImage = @BackgroundImage ,
                                        SpinImage = @SpinImage,
                                        IOSDisposeValue = @IOSDisposeValue,
                                        AndroidDisposeValue = @AndroidDisposeValue ,
                                        IOSCommunicationValue = @IOSCommunicationValue,
                                        AndroidCommunicationValue= @AndroidCommunicationValue ,
                                        AppLink = @AppLink  ,
                                        GetCouponDescription= @GetCouponDescription ,
                                        CouponDescriptionStatus= @CouponDescriptionStatus ,
                                        RoamingDescription= @RoamingDescription	
                                WHERE   PKID = @PKID";
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }
        public static void UpdateCouponCategoryPercentage(SqlConnection connection, int id, int perc)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",id),
                new SqlParameter("@Percentage",perc)
            };
            const string sql = @"UPDATE  tbl_CouponCategory
                                SET     Percentage = @Percentage
                                WHERE   PKID = @PKID ";
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }
        public static CouponCategory GetCouponCategoryByID(SqlConnection connection, int id)
        {
            var sqlParameters = new[]
            {
                new SqlParameter("@PKID", id)
            };
            const string sql = @"SELECT TOP 1 * FROM tbl_CouponCategory WHERE PKID = @PKID";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, sqlParameters).ConvertTo<CouponCategory>().ToList().FirstOrDefault();
        }

        public static int GetPKIDByEnID(SqlConnection connection, string EnID)
        {
            var parameters = new[]
            {
                new SqlParameter("@EnID", EnID??string.Empty)
            };

            const string sql = @"   SELECT TOP 1
                                            PKID
                                    FROM    tbl_CouponCategory WITH ( NOLOCK )
                                    WHERE   EnID = @EnID";
            var _PKID = SqlHelper.ExecuteScalar(connection, CommandType.Text, sql, parameters);
            if (_PKID != null && !string.IsNullOrEmpty(_PKID.ToString()))
            {
                return int.Parse(_PKID.ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Coupon
        public static Coupon GetCouponByID(SqlConnection connection, int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@PKID", id)
            };
            const string sql = @"SELECT TOP 1 *  FROM tbl_Coupon WHERE PKID=@PKID";

            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters).ConvertTo<Coupon>().ToList().FirstOrDefault();

        }
        public static List<Coupon> GetCouponByCategoryID(SqlConnection connection, int CategoryID)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoryID", CategoryID)
            };
            var sql = "SELECT * FROM tbl_Coupon WITH (NOLOCK) where CategoryID=@CategoryID ORDER BY IsActive desc,PKID";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql, parameters).ConvertTo<Coupon>().ToList();
        }
        public static string GetCountByCategoryID(SqlConnection connection, int CategoryID)
        {
            var sqlParameter = new SqlParameter("@CategoryID", CategoryID);
            var _Count = SqlHelper.ExecuteScalar(connection, CommandType.Text, "SELECT COUNT(1) FROM tbl_Coupon where CategoryID=@CategoryID", sqlParameter);
            return _Count == null ? "0" : _Count.ToString();
        }
        public static void DeleteCoupon(SqlConnection connection, int PKID)
        {
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",PKID)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, "DELETE FROM tbl_Coupon WHERE PKID=@PKID", sqlParamters);
        }

        public static void UpdateCoupon(SqlConnection connection, Coupon coupon)
        {
            const string sql = @"UPDATE  tbl_Coupon
                            SET CategoryID = @CategoryID,
                                    ParValue = @ParValue,
                                    MinValue = @MinValue,
                                    IsActive = @IsActive,
                                    Remark = @Remark,
                                    ContentImage = @ContentImage,
                                    ContentSmallImage = @ContentSmallImage,
                                    BackgroundImage = @BackgroundImage,
                                    BackgroundSmallImage = @BackgroundSmallImage,
                                    SpinImage = @SpinImage,
                                    SpinSmallImage = @SpinSmallImage
                            WHERE PKID = @PKID";
            var sqlParamters = new[]
            {
                new SqlParameter("@PKID",coupon.PKID),
                new SqlParameter("@CategoryID",coupon.CategoryID),
                new SqlParameter("@ParValue",coupon.ParValue),
                new SqlParameter("@MinValue",coupon.MinValue),
                new SqlParameter("@IsActive",coupon.IsActive),
                new SqlParameter("@Remark",coupon.Remark??string.Empty),
                new SqlParameter("@ContentImage",coupon.ContentImage??string.Empty),
                new SqlParameter("@ContentSmallImage",coupon.ContentSmallImage??string.Empty),
                new SqlParameter("@BackgroundImage",coupon.BackgroundImage??string.Empty),
                new SqlParameter("@BackgroundSmallImage",coupon.BackgroundSmallImage??string.Empty),
                new SqlParameter("@SpinImage",coupon.SpinImage??string.Empty),
                new SqlParameter("@SpinSmallImage",coupon.SpinSmallImage??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }

        public static void AddCoupon(SqlConnection connection, Coupon coupon)
        {
            const string sql = @"  INSERT INTO Gungnir..tbl_Coupon
			          ( CategoryID ,
			            ParValue ,
			            MinValue ,
			            IsActive ,
			            Remark ,
			            ContentImage ,
			            ContentSmallImage ,
			            BackgroundImage ,
			            BackgroundSmallImage ,
			            SpinImage ,
			            SpinSmallImage
			          )
			  VALUES  ( @CategoryID , -- CategoryID - int
			            @ParValue , -- ParValue - int
			            @MinValue , -- MinValue - int
			            @IsActive , -- IsActive - int
			            @Remark , -- Remark - nvarchar(200)
			            @ContentImage , -- ContentImage - nvarchar(2000)
			            @ContentSmallImage , -- ContentSmallImage - nvarchar(2000)
			            @BackgroundImage , -- BackgroundImage - nvarchar(2000)
			            @BackgroundSmallImage , -- BackgroundSmallImage - nvarchar(2000)
			            @SpinImage , -- SpinImage - nvarchar(2000)
			            @SpinSmallImage  -- SpinSmallImage - nvarchar(2000)
			          )";
            var sqlParamters = new[]
            {
                new SqlParameter("@CategoryID",coupon.CategoryID),
                new SqlParameter("@ParValue",coupon.ParValue),
                new SqlParameter("@MinValue",coupon.MinValue),
                new SqlParameter("@IsActive",coupon.IsActive),
                new SqlParameter("@Remark",coupon.Remark??string.Empty),
                new SqlParameter("@ContentImage",coupon.ContentImage??string.Empty),
                new SqlParameter("@ContentSmallImage",coupon.ContentSmallImage??string.Empty),
                new SqlParameter("@BackgroundImage",coupon.BackgroundImage??string.Empty),
                new SqlParameter("@BackgroundSmallImage",coupon.BackgroundSmallImage??string.Empty),
                new SqlParameter("@SpinImage",coupon.SpinImage??string.Empty),
                new SqlParameter("@SpinSmallImage",coupon.SpinSmallImage??string.Empty)
            };
            SqlHelper.ExecuteNonQuery(connection, CommandType.Text, sql, sqlParamters);
        }
        #endregion

        #region  Activity..tbl_CouponRules
        /// <summary>
        /// 查询优惠券下拉列表
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectDropDownList(SqlConnection connection)
        {
            string sql = @"SELECT PKID,Name,Type FROM Activity..tbl_CouponRules WHERE ParentID=0";
            return SqlHelper.ExecuteDataTable(connection, CommandType.Text, sql);
        }
        #endregion
    }
}
