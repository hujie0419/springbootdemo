using System;
using Microsoft.ApplicationBlocks.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALActivity
    {
        private static readonly string GungnirConnStr = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        // private static readonly string  GungnirReadConnStr = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        public static bool InsertActivityBuild(SqlConnection conn, ActivityBuild model)
        {

            string sql = @" INSERT INTO Activity..ActivityBuild
                          ( Title ,
                            Content ,
                            BgImageUrl,
                            SBgImageUrl,
                            ActivityUrl ,
                            BgColor,
                            CreateTime,
                            TireBrand,
                          ActivityType,
                            EndDate,
                            DataParames,
                            BigActivityHome,
                            ActivityHome,
                            MenuType,
                            ActivityMenu,
                            SelKeyName,SelKeyImage,IsShowDate,IsTireSize,TireSizeConfig,CreatetorUser,UpdateUser,ActivityConfigType,StartDT,PersonWheel 
                          )
                  VALUES  ( @Title , 
                            @Content , 
                            @BgImageUrl,
                            @SBgImageUrl,
                            @ActivityUrl , 
                            @BgColor,
                            GETDATE(),
                            @TireBrand,
                            @ActivityType,
                            @EndDate,
                            @DataParames,
                           @BigActivityHome,
                            @ActivityHome ,
                            @MenuType,
                            @ActivityMenu,@SelKeyName,@SelKeyImage,@IsShowDate,@IsTireSize,@TireSizeConfig,@CreatetorUser,@UpdateUser,@ActivityConfigType,@StartDT,@PersonWheel                 
                          )";

            var sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@Title",model.Title??string.Empty),
               new SqlParameter("@Content",model.Content??string.Empty),
               new SqlParameter("@BgImageUrl",model.BgImageUrl??string.Empty),
               new SqlParameter("@SBgImageUrl",model.SBgImageUrl??string.Empty),
               new SqlParameter("@ActivityUrl",model.ActivityUrl??string.Empty),
               new SqlParameter("@BgColor",model.BgColor??string.Empty),
               new SqlParameter("@TireBrand",model.TireBrand??string.Empty),
               new SqlParameter("@ActivityType",model.ActivityType),
               new SqlParameter("@EndDate",model.EndDate),
               new SqlParameter("@DataParames",model.DataParames),
               new SqlParameter("@BigActivityHome",model.BigActivityHome??string.Empty),
               new SqlParameter("@ActivityHome",model.ActivityHome??string.Empty),
               new SqlParameter("@MenuType",model.MenuType),
               new SqlParameter("@ActivityMenu",model.ActivityMenu??string.Empty),
               new SqlParameter("@SelKeyName",model.SelKeyName),
               new SqlParameter("@SelKeyImage",model.SelKeyImage),
               new SqlParameter("@IsShowDate",model.IsShowDate),
               new SqlParameter("@IsTireSize",model.IsTireSize),
               new SqlParameter("@TireSizeConfig",model.TireSizeConfig),
               new SqlParameter("@CreatetorUser",model.CreatetorUser),
               new SqlParameter("@UpdateUser",model.UpdateUser),
               new SqlParameter("@ActivityConfigType",model.ActivityConfigType),
               new SqlParameter("@StartDT",model.StartDT),
               new SqlParameter("@PersonWheel",model.PersonWheel)

            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
        }
        public static bool DeleteActivityBuild(SqlConnection conn, int id)
        {
            string sql = @"DELETE FROM [Activity].[dbo].[ActivityBuild] WHERE id=@id";

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@id", id)) > 0;
        }

        public static int CreateActivity(SqlConnection conn, string title)
        {
            string sql = @"INSERT INTO Activity..ActivityBuild
                        ( Title,CreateTime  )
                VALUES  ( @Title,GETDATE()  )
                SELECT @@IDENTITY";
            SqlParameter[] collection = new SqlParameter[] {
          new SqlParameter("@Title", title)};
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql, collection);
            if (obj != null)
                return Convert.ToInt32(obj);
            else
                return 0;

        }

        public static bool UpdateActivityBuild(SqlConnection conn, ActivityBuild model, int id)
        {
            string sql = @"   UPDATE    Activity..ActivityBuild
                              SET    Title=@Title,
                                      BgImageUrl=@BgImageUrl,
                                      SBgImageUrl=@SBgImageUrl,
                                      BgColor=@BgColor,
                                        Content = @Content ,                                  
                                        UpdateTime = GETDATE(),
                                        TireBrand=@TireBrand,
                                        ActivityType=@ActivityType,
                                     EndDate=@EndDate,
                                    DataParames=@DataParames,
                                    BigActivityHome=@BigActivityHome,
                                    ActivityHome=@ActivityHome,
                                    MenuType=@MenuType,
                                    ActivityMenu=@ActivityMenu,SelKeyName=@SelKeyName,SelKeyImage=@SelKeyImage,IsShowDate=@IsShowDate,IsTireSize=@IsTireSize,TireSizeConfig=@TireSizeConfig,UpdateUser=@UpdateUser,ActivityConfigType=@ActivityConfigType,StartDT=@StartDT,PersonWheel=@PersonWheel
                              WHERE     id = @id";
            var sqlPrams = new SqlParameter[]
            {
               new SqlParameter("@Title",model.Title??string.Empty),
               new SqlParameter("@BgImageUrl",model.BgImageUrl??string.Empty),
               new SqlParameter("@SBgImageUrl",model.SBgImageUrl??string.Empty),
               new SqlParameter("@BgColor",model.BgColor??string.Empty),
               new SqlParameter("@Content",model.Content??string.Empty),
               new SqlParameter("@ActivityUrl",model.ActivityUrl??string.Empty),
               new SqlParameter("@id",id),
               new SqlParameter("@TireBrand",model.TireBrand??string.Empty),
               new SqlParameter("@ActivityType",model.ActivityType),
               new SqlParameter("@EndDate",model.EndDate),
               new SqlParameter("@DataParames",model.DataParames),
               new SqlParameter("@BigActivityHome",model.BigActivityHome),
               new SqlParameter("@ActivityHome",model.ActivityHome),
               new SqlParameter("@MenuType",model.MenuType),
               new SqlParameter("@ActivityMenu",model.ActivityMenu),
               new SqlParameter("@SelKeyName",model.SelKeyName),
               new SqlParameter("@SelKeyImage",model.SelKeyImage),
               new SqlParameter("@IsShowDate",model.IsShowDate),
               new SqlParameter("@IsTireSize",model.IsTireSize),
               new SqlParameter("@TireSizeConfig",model.TireSizeConfig),
               new SqlParameter("@UpdateUser",model.UpdateUser),
               new SqlParameter("@ActivityConfigType",model.ActivityConfigType),
               new SqlParameter("@PersonWheel",model.PersonWheel),
               new SqlParameter("@StartDT",model.StartDT)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlPrams) > 0;
        }

        public static int GetMaxID(SqlConnection conn)
        {
            string sql = " SELECT MAX(ID)FROM Activity..ActivityBuild";
            object obj = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if (obj != null)
            {
                int number = 0;
                if (int.TryParse(obj.ToString(), out number))
                    return number;
                else
                    return 0;
            }
            else
                return 0;

        }


        public static ActivityBuild GetActivityBuildById(SqlConnection conn, int id)
        {
            string sql = @"SELECT *
                      FROM [Activity].[dbo].[ActivityBuild] WITH(NOLOCK) WHERE id=@id";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@id", id)).ConvertTo<ActivityBuild>().FirstOrDefault();
        }

        public static List<ActivityBuild> GetActivityBuild(SqlConnection conn, string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string sql = @"SELECT  *
                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY [CreateTime] DESC ) AS ROWNUMBER ,
                                               [id]
                                              ,[Title]
                                              ,[Content]
                                              ,[ActivityUrl]
                                              ,[CreateTime]
                                              ,[UpdateTime]
                                              ,[TireBrand]
                                               ,[ActivityType],[EndDate],CreatetorUser,UpdateUser,ActivityConfigType
                          FROM [Activity].[dbo].[ActivityBuild] WITH(NOLOCK)  WHERE 1=1 " + sqlStr + @") AS PG
                        WHERE   PG.ROWNUMBER BETWEEN STR(( @PageIndex - 1 ) * @PageSize + 1)
                                             AND     STR(@PageIndex * @PageSize)";
            string sqlCount = @"SELECT COUNT(1) FROM [Activity].[dbo].[ActivityBuild] WITH(NOLOCK)  WHERE 1=1  " + sqlStr;
            //  recordCount = (int)SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount);

            var sqlPrams = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageIndex",pageIndex)
            };

            //return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPrams).ConvertTo<ActivityBuild>().ToList();
            using (var connString = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ToString()))
            {
                connString.Open();
                recordCount = (int)SqlHelper.ExecuteScalar(connString, CommandType.Text, sqlCount);
                return SqlHelper.ExecuteDataTable(connString, CommandType.Text, sql, sqlPrams).ConvertTo<ActivityBuild>().ToList();
            }


        }

        public static int SaveCouponActivityConfig(SqlConnection conn, CouponActivity configObject, string userName)
        {
            var sql = "";
            if (configObject.ActivityId == -1)
            {
                sql = "INSERT  INTO [Gungnir].[dbo].[PromotionActivityConfig](SmallImageUrl,BigImageUrl,ButtonBackGroundColor,ButtonTextColor,ButtonText,Url,IosJson,AndroidJson, IsSendCoupon, LayerButtonText, LayerButtonBackGroundColor,LayerButtonTextColor,PromotionRuleId,PromotionMinMoney,PromotionDiscount,StartDate,  EndDate,PromotionDescription,PromotionCodeChannel,CreatedUser,ActivityKey) VALUES (@SmallImageUrl,@BigImageUrl,@ButtonBackGroundColor,@ButtonTextColor, @ButtonText,@Url,@IosJson,@AndroidJson,@IsSendCoupon, @LayerButtonText,@LayerButtonBackGroundColor,@LayerButtonTextColor,@PromotionRuleId,@PromotionMinMoney,@PromotionDiscount, @StartDate,@EndDate,@PromotionDescription,@PromotionCodeChannel,@CreatedUser,@ActivityKey) ";
            }
            else
            {
                sql = "INSERT  INTO [Gungnir].[dbo].[PromotionActivityConfig](SmallImageUrl,BigImageUrl,ButtonBackGroundColor,ButtonTextColor,ButtonText,Url,IosJson,AndroidJson, IsSendCoupon, LayerButtonText, LayerButtonBackGroundColor,LayerButtonTextColor,PromotionRuleId,PromotionMinMoney,PromotionDiscount,StartDate,  EndDate,PromotionDescription,PromotionCodeChannel,CreatedUser,ActivityId) VALUES (@SmallImageUrl,@BigImageUrl,@ButtonBackGroundColor,@ButtonTextColor, @ButtonText,@Url,@IosJson,@AndroidJson,@IsSendCoupon, @LayerButtonText,@LayerButtonBackGroundColor,@LayerButtonTextColor,@PromotionRuleId,@PromotionMinMoney,@PromotionDiscount, @StartDate,@EndDate,@PromotionDescription,@PromotionCodeChannel,@CreatedUser,@ActivityId )";
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("@SmallImageUrl", configObject.SmallImageUrl),
                new SqlParameter("@BigImageUrl", configObject.BigImageUrl),
                new SqlParameter("@ButtonBackGroundColor", configObject.ButtonBackGroundColor),
                new SqlParameter("@ButtonTextColor", configObject.ButtonTextColor),
                new SqlParameter("@ButtonText", configObject.ButtonText),
                new SqlParameter("@IosJson", configObject.IosJson),
                new SqlParameter("@AndroidJson", configObject.AndroidJson),
                new SqlParameter("@Url", configObject.Url),
                new SqlParameter("@IsSendCoupon", configObject.IsSendCoupon),
                new SqlParameter("@LayerButtonText", configObject.LayerButtonText),
                new SqlParameter("@LayerButtonBackGroundColor", configObject.LayerButtonBackGroundColor),
                new SqlParameter("@LayerButtonTextColor", configObject.LayerButtonTextColor),
                //new SqlParameter("@PromotionType", configObject.PromotionType),
                new SqlParameter("@PromotionRuleId", configObject.PromotionRuleId),
                new SqlParameter("@PromotionMinMoney", configObject.PromotionMinMoney),
                new SqlParameter("@PromotionDiscount", configObject.PromotionDiscount),
                new SqlParameter("@StartDate", configObject.StartDate==DateTime.MinValue?DateTime.Today:configObject.StartDate),
                new SqlParameter("@EndDate", configObject.EndDate==DateTime.MinValue?DateTime.Today:configObject.EndDate),
                new SqlParameter("@PromotionDescription", configObject.PromotionDescription),
                new SqlParameter("@PromotionCodeChannel", configObject.PromotionCodeChannel),
                new SqlParameter("@CreatedUser",userName),
                new SqlParameter("@ActivityId",configObject.ActivityId),
                new SqlParameter("@ActivityKey",configObject.ActivityKey)

            };

            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);

            return rows;
        }

        public static int SaveCouponActivityWebConfig(SqlConnection conn, WebCouponActivity configObject, string userName)
        {
            var sql = @"INSERT  INTO [Gungnir].[dbo].[PromotionActivityWebConfig]
        ( SmallBannerImageUrl ,
          BigBannerImageUrl ,
          SmallContentImageUrl ,
          BigContentImageUrl ,
          ButtonBackGroundColor ,
          ButtonTextColor ,
          ButtonText ,
          Url ,
          IsSendCoupon ,
          LayerButtonText ,
          LayerButtonBackGroundColor ,
          LayerButtonTextColor ,
          PromotionRuleId ,
          PromotionMinMoney ,
          PromotionDiscount ,
          StartDate ,
          EndDate ,
          PromotionDescription ,
          PromotionCodeChannel ,
          CreatedUser ,
          ActivityId ,
          ActivityKey ,
          PromotionRuleGUID
        )
VALUES  ( @SmallBannerImageUrl ,
          @BigBannerImageUrl ,
          @SmallContentImageUrl ,
          @BigContentImageUrl ,
          @ButtonBackGroundColor ,
          @ButtonTextColor ,
          @ButtonText ,
          @Url ,
          @IsSendCoupon ,
          @LayerButtonText ,
          @LayerButtonBackGroundColor ,
          @LayerButtonTextColor ,
          @PromotionRuleId ,
          @PromotionMinMoney ,
          @PromotionDiscount ,
          @StartDate ,
          @EndDate ,
          @PromotionDescription ,
          @PromotionCodeChannel ,
          @CreatedUser ,
          @ActivityId ,
          @ActivityKey ,
          @PromotionRuleGUID
        );";
            SqlParameter[] parameters =
            {
                new SqlParameter("@ActivityId",configObject.ActivityId == -1 ? (int?)null : configObject.ActivityId),
                new SqlParameter("@SmallBannerImageUrl", configObject.SmallBannerImageUrl),
                new SqlParameter("@BigBannerImageUrl", configObject.BigBannerImageUrl),
                new SqlParameter("@SmallContentImageUrl", configObject.SmallContentImageUrl),
                new SqlParameter("@BigContentImageUrl", configObject.BigContentImageUrl),
                new SqlParameter("@ButtonBackGroundColor", configObject.ButtonBackGroundColor),
                new SqlParameter("@ButtonTextColor", configObject.ButtonTextColor),
                new SqlParameter("@ButtonText", configObject.ButtonText),
                new SqlParameter("@Url", configObject.Url),
                new SqlParameter("@IsSendCoupon", configObject.IsSendCoupon),
                new SqlParameter("@LayerButtonText", configObject.LayerButtonText),
                new SqlParameter("@LayerButtonBackGroundColor", configObject.LayerButtonBackGroundColor),
                new SqlParameter("@LayerButtonTextColor", configObject.LayerButtonTextColor),
                //new SqlParameter("@PromotionType", configObject.PromotionType),
                new SqlParameter("@PromotionRuleId", configObject.PromotionRuleId),
                new SqlParameter("@PromotionMinMoney", configObject.PromotionMinMoney),
                new SqlParameter("@PromotionDiscount", configObject.PromotionDiscount),
                new SqlParameter("@StartDate", configObject.StartDate==DateTime.MinValue?DateTime.Today:configObject.StartDate),
                new SqlParameter("@EndDate", configObject.EndDate==DateTime.MinValue?DateTime.Today:configObject.EndDate),
                new SqlParameter("@PromotionDescription", configObject.PromotionDescription),
                new SqlParameter("@PromotionCodeChannel", configObject.PromotionCodeChannel),
                new SqlParameter("@CreatedUser",userName),
                new SqlParameter("@ActivityKey",configObject.ActivityId == -1? configObject.ActivityKey: null),
                new SqlParameter("@PromotionRuleGUID", configObject.PromotionRuleGUID),
            };

            var rows = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters);

            return rows;
        }
        public static CouponActivity GetCurrentCouponActivity(SqlConnection conn, string id)
        {
            SqlParameter parameter = new SqlParameter("@ActivityId", id);
            var result =
                SqlHelper.ExecuteDataTable(conn, CommandType.StoredProcedure,
                    "[Gungnir].[dbo].[Activity_GetCurrentCouponActivity]", parameter)
                    .AsEnumerable()
                    .Select(x => new CouponActivity()
                    {
                        ActivityId = x.IsNull(0) ? -1 : Convert.ToInt32(x["ActivityId"]),
                        ActivityKey = x.IsNull(1) ? Guid.Empty : Guid.Parse(x["ActivityKey"].ToString()),
                        SmallImageUrl = (string)(x["SmallImageUrl"] ?? string.Empty),
                        BigImageUrl = (string)(x["BigImageUrl"] ?? string.Empty),
                        ButtonBackGroundColor = (string)(x["ButtonBackGroundColor"] ?? string.Empty),
                        ButtonTextColor = (string)(x["ButtonTextColor"] ?? string.Empty),
                        ButtonText = (string)(x["ButtonText"] ?? string.Empty),
                        Url = (string)(x["Url"] ?? string.Empty),
                        IosJson = (string)(x["IosJson"] ?? string.Empty),
                        AndroidJson = (string)(x["AndroidJson"] ?? string.Empty),
                        IsSendCoupon = (bool)(x["IsSendCoupon"] ?? false),
                        LayerButtonText = (string)(x["LayerButtonText"] ?? string.Empty),
                        LayerButtonBackGroundColor = (string)(x["LayerButtonBackGroundColor"] ?? string.Empty),
                        LayerButtonTextColor = (string)(x["LayerButtonTextColor"] ?? string.Empty),
                        // PromotionType = (int) (x["PromotionType"] ?? 0),
                        PromotionRuleId = (int)(x["PromotionRuleId"] ?? 0),
                        PromotionMinMoney = (int)(x["PromotionMinMoney"] ?? 0),
                        PromotionDiscount = (int)(x["PromotionDiscount"] ?? 0),
                        StartDate = (DateTime)(x["StartDate"] ?? 0),
                        EndDate = (DateTime)(x["EndDate"] ?? 0),
                        PromotionDescription = (string)(x["PromotionDescription"] ?? string.Empty),
                        PromotionCodeChannel = (string)(x["PromotionCodeChannel"] ?? string.Empty),
                    }).FirstOrDefault();

            return result;
        }


        public static WebCouponActivity GetCurrentWebCouponActivityByActivityKey(SqlConnection conn, string acitvityKey)
        {
            var sql = @"SELECT TOP 1
        ActivityId ,
        ActivityKey ,
        SmallBannerImageUrl ,
        BigBannerImageUrl ,
        SmallContentImageUrl ,
        BigContentImageUrl ,
        ButtonBackGroundColor ,
        ButtonTextColor ,
        ButtonText ,
        Url ,
        IsSendCoupon ,
        LayerButtonText ,
        LayerButtonBackGroundColor ,
        LayerButtonTextColor ,
        -- PromotionCodeType ,
        PromotionRuleId ,
        PromotionMinMoney ,
        PromotionDiscount ,
        StartDate ,
        EndDate ,
        PromotionDescription ,
        PromotionCodeChannel ,
        PromotionRuleGUID
FROM    Gungnir.dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   ActivityKey = @ActivityKey
        AND IsDeleted = 0
ORDER BY CreateTime DESC;";
            SqlParameter parameter = new SqlParameter("@ActivityKey", acitvityKey);
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter)
                    .AsEnumerable()
                    .Select(x => new WebCouponActivity()
                    {
                        ActivityId = x.IsNull(0) ? -1 : Convert.ToInt32(x["ActivityId"]),
                        ActivityKey = x.IsNull(1) ? Guid.Empty : Guid.Parse(x["ActivityKey"].ToString()),
                        SmallBannerImageUrl = (string)(x["SmallBannerImageUrl"] ?? string.Empty),
                        BigBannerImageUrl = (string)(x["BigBannerImageUrl"] ?? string.Empty),
                        SmallContentImageUrl = (string)(x["SmallContentImageUrl"] ?? string.Empty),
                        BigContentImageUrl = (string)(x["BigContentImageUrl"] ?? string.Empty),
                        ButtonBackGroundColor = (string)(x["ButtonBackGroundColor"] ?? string.Empty),
                        ButtonTextColor = (string)(x["ButtonTextColor"] ?? string.Empty),
                        ButtonText = (string)(x["ButtonText"] ?? string.Empty),
                        Url = (string)(x["Url"] ?? string.Empty),
                        IsSendCoupon = (bool)(x["IsSendCoupon"] ?? false),
                        LayerButtonText = (string)(x["LayerButtonText"] ?? string.Empty),
                        LayerButtonBackGroundColor = (string)(x["LayerButtonBackGroundColor"] ?? string.Empty),
                        LayerButtonTextColor = (string)(x["LayerButtonTextColor"] ?? string.Empty),
                        // PromotionType = (int) (x["PromotionCodeType"] ?? 0),
                        PromotionRuleId = (int)(x["PromotionRuleId"] ?? 0),
                        PromotionMinMoney = (int)(x["PromotionMinMoney"] ?? 0),
                        PromotionDiscount = (int)(x["PromotionDiscount"] ?? 0),
                        StartDate = (DateTime)(x["StartDate"] ?? 0),
                        EndDate = (DateTime)(x["EndDate"] ?? 0),
                        PromotionDescription = x["PromotionDescription"].ToString(),
                        PromotionCodeChannel = x["PromotionCodeChannel"].ToString(),
                        PromotionRuleGUID = (Object.Equals(x["PromotionRuleGUID"], DBNull.Value) ? (Guid?)null : (Guid)x["PromotionRuleGUID"]),
                    }).FirstOrDefault();

            return result;
        }

        public static WebCouponActivity GetCurrentWebCouponActivityByActivityId(SqlConnection conn, int id)
        {
            var sql = @"SELECT TOP 1
        ActivityId ,
        ActivityKey ,
        SmallBannerImageUrl ,
        BigBannerImageUrl ,
        SmallContentImageUrl ,
        BigContentImageUrl ,
        ButtonBackGroundColor ,
        ButtonTextColor ,
        ButtonText ,
        Url ,
        IsSendCoupon ,
        LayerButtonText ,
        LayerButtonBackGroundColor ,
        LayerButtonTextColor ,
        -- PromotionCodeType ,
        PromotionRuleId ,
        PromotionMinMoney ,
        PromotionDiscount ,
        StartDate ,
        EndDate ,
        PromotionDescription ,
        PromotionCodeChannel 
FROM    Gungnir.dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   ActivityId = @ActivityId
        AND IsDeleted = 0
ORDER BY CreateTime DESC;";
            SqlParameter parameter = new SqlParameter("@ActivityId", id);
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameter)
                    .AsEnumerable()
                    .Select(x => new WebCouponActivity()
                    {
                        ActivityId = x.IsNull(0) ? -1 : Convert.ToInt32(x["ActivityId"]),
                        ActivityKey = x.IsNull(1) ? Guid.Empty : Guid.Parse(x["ActivityKey"].ToString()),
                        SmallBannerImageUrl = (string)(x["SmallBannerImageUrl"] ?? string.Empty),
                        BigBannerImageUrl = (string)(x["BigBannerImageUrl"] ?? string.Empty),
                        SmallContentImageUrl = (string)(x["SmallContentImageUrl"] ?? string.Empty),
                        BigContentImageUrl = (string)(x["BigContentImageUrl"] ?? string.Empty),
                        ButtonBackGroundColor = (string)(x["ButtonBackGroundColor"] ?? string.Empty),
                        ButtonTextColor = (string)(x["ButtonTextColor"] ?? string.Empty),
                        ButtonText = (string)(x["ButtonText"] ?? string.Empty),
                        Url = (string)(x["Url"] ?? string.Empty),
                        IsSendCoupon = (bool)(x["IsSendCoupon"] ?? false),
                        LayerButtonText = (string)(x["LayerButtonText"] ?? string.Empty),
                        LayerButtonBackGroundColor = (string)(x["LayerButtonBackGroundColor"] ?? string.Empty),
                        LayerButtonTextColor = (string)(x["LayerButtonTextColor"] ?? string.Empty),
                        // PromotionType = (int) (x["PromotionCodeType"] ?? 0),
                        PromotionRuleId = (int)(x["PromotionRuleId"] ?? 0),
                        PromotionMinMoney = (int)(x["PromotionMinMoney"] ?? 0),
                        PromotionDiscount = (int)(x["PromotionDiscount"] ?? 0),
                        StartDate = (DateTime)(x["StartDate"] ?? 0),
                        EndDate = (DateTime)(x["EndDate"] ?? 0),
                        PromotionDescription = x["PromotionDescription"].ToString(),
                        PromotionCodeChannel = x["PromotionDescription"].ToString(),
                        PromotionRuleGUID = (Object.Equals(x["PromotionRuleGUID"], DBNull.Value) ? (Guid?)null : (Guid)x["IntentionId"]),
                    }).FirstOrDefault();

            return result;
        }

        public static Tuple<int, List<CouponActivity>> GetActivityListForApp(SqlConnection conn, int pageSize, int pageIndex)
        {
            int totalCount;
            var parameters = new[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int) { Direction=ParameterDirection.Output}
            };
            var sql = @"DECLARE @CountId INT;
DECLARE @CountKey INT;
SELECT  @CountId = COUNT(DISTINCT ActivityId)
FROM    PromotionActivityConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NOT NULL;	

SELECT  @CountKey = COUNT(DISTINCT ActivityKey)
FROM    dbo.PromotionActivityConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NULL
        AND ActivityKey IS NOT NULL;

SELECT  @TotalCount = @CountId + @CountKey;

SELECT  *
FROM    ( SELECT    * ,
                    ROW_NUMBER() OVER ( ORDER BY PKID DESC ) AS rownum
          FROM      ( SELECT    pac.PKID ,
                                pac.ActivityId ,
                                pac.SmallImageUrl ,
                                pac.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY pac.ActivityKey ORDER BY pac.CreateTime DESC ) rown
                      FROM      PromotionActivityConfig pac WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND pac.ActivityId IS NULL
                                AND pac.ActivityKey IS NOT NULL
                      UNION
                      SELECT    pac.PKID ,
                                pac.ActivityId ,
                                pac.SmallImageUrl ,
                                pac.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY pac.ActivityId ORDER BY pac.CreateTime DESC ) rown
                      FROM      PromotionActivityConfig pac WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND pac.ActivityId IS NOT NULL
                    ) T
          WHERE     T.rown = 1
        ) Y
WHERE   Y.rownum BETWEEN ( @PageIndex - 1 ) * @PageSize + 1
                 AND     @PageIndex * @PageSize;";
            var activityList =
                SqlHelper.ExecuteDataTable(conn, CommandType.Text,
                sql, parameters)
                .AsEnumerable()
                .Select(x => new CouponActivity()
                {
                    ActivityId = x.IsNull(1) ? -1 : Convert.ToInt32(x[1]),
                    SmallImageUrl = (string)(x["SmallImageUrl"] ?? string.Empty),
                    ActivityKey = x.IsNull(3) ? Guid.Empty : Guid.Parse(x[3].ToString())
                }).ToList();

            int.TryParse(parameters.LastOrDefault().Value.ToString(), out totalCount);

            Tuple<int, List<CouponActivity>> result = new Tuple<int, List<CouponActivity>>(totalCount, activityList);

            return result;
        }

        public static Tuple<int, List<CouponActivity>> GetActivityListForWeb(SqlConnection conn, int pageSize, int pageIndex)
        {
            int totalCount;

            var parameters = new[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int) { Direction=ParameterDirection.Output}
            };
            var sql = @"DECLARE @CountId INT;
DECLARE @CountKey INT;

SELECT  @CountId = COUNT(DISTINCT ActivityId)
FROM    dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NOT NULL;

SELECT  @CountKey = COUNT(DISTINCT ActivityKey)
FROM    dbo.PromotionActivityWebConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0
        AND ActivityId IS NULL
        AND ActivityKey IS NOT NULL;

SELECT  @TotalCount = @CountId + @CountKey;

SELECT  *
FROM    ( SELECT    * ,
                    ROW_NUMBER() OVER ( ORDER BY T.PKID DESC ) AS rownum
          FROM      ( SELECT    paw.PKID ,
                                paw.ActivityId ,
                                paw.SmallBannerImageUrl AS SmallImageUrl ,
                                paw.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY paw.ActivityKey ORDER BY paw.CreateTime DESC ) rown
                      FROM      PromotionActivityWebConfig paw WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND paw.ActivityId IS NULL
                                AND paw.ActivityKey IS NOT NULL
                      UNION
                      SELECT    paw.PKID ,
                                paw.ActivityId ,
                                paw.SmallBannerImageUrl AS SmallImageUrl ,
                                paw.ActivityKey ,
                                ROW_NUMBER() OVER ( PARTITION BY paw.ActivityId ORDER BY paw.CreateTime DESC ) rown
                      FROM      PromotionActivityWebConfig paw WITH ( NOLOCK )
                      WHERE     IsDeleted = 0
                                AND paw.ActivityId IS NOT NULL
                    ) T
          WHERE     T.rown = 1
        ) Y
WHERE   Y.rownum BETWEEN ( @PageIndex - 1 ) * @PageSize + 1
                 AND     @PageIndex * @PageSize;";
            var activityList =
                SqlHelper.ExecuteDataTable(conn, CommandType.Text,
                sql, parameters)
                .AsEnumerable()
                .Select(x => new CouponActivity()
                {
                    ActivityId = x.IsNull(1) ? -1 : Convert.ToInt32(x[1]),
                    SmallImageUrl = (string)(x["SmallImageUrl"] ?? string.Empty),
                    ActivityKey = x.IsNull(3) ? Guid.Empty : Guid.Parse(x[3].ToString())
                }).ToList();

            int.TryParse(parameters.LastOrDefault().Value.ToString(), out totalCount);

            Tuple<int, List<CouponActivity>> result = new Tuple<int, List<CouponActivity>>(totalCount, activityList);

            return result;
        }

        public static bool DeleteActivityConfig(SqlConnection conn, string type, string id, string userName)
        {
            var parameters = new[]
            {
                new SqlParameter("@Type",type),
                new SqlParameter("@Id",id),
                new SqlParameter("@UserName",userName)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "[Gungnir].[dbo].[Activity_DeleteActivityConfig]", parameters) > 0;
        }


        public static Dictionary<string, string> GetProductImageUrl(string pid)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string connString = ConnectionHelper.GetDecryptConn("Tuhu_productcatalog_AlwaysOnRead");
            //ConfigurationManager.ConnectionStrings["Tuhu_productcatalog_AlwaysOnRead"].ConnectionString
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            string sql = "select CP_ShuXing5, [image],[DisplayName],cy_list_price as ActivityPrice,CP_Tire_Width,CP_Tire_AspectRatio,CP_Tire_Rim from Tuhu_productcatalog.dbo.vw_Products WITH(NOLOCK)  where PID=@PID ";
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlParameter parameter = new SqlParameter("@PID", pid);
            DataRow dr = SqlHelper.ExecuteDataRow(conn, CommandType.Text, sql, parameter);
            if (dr != null)
            {
                dic.Add("image", dr["image"].ToString());
                dic.Add("name", dr["DisplayName"].ToString());
                dic.Add("ActivityPrice", dr["ActivityPrice"].ToString());
                dic.Add("CP_Tire_Width", dr["CP_Tire_Width"].ToString());
                dic.Add("CP_Tire_AspectRatio", dr["CP_Tire_AspectRatio"].ToString());
                dic.Add("CP_Tire_Rim", dr["CP_Tire_Rim"].ToString());
                dic.Add("CP_ShuXing5", dr["CP_ShuXing5"].ToString());
            }
            return dic;
        }

        /// <summary>
        /// 根据活动GUID获得该优惠卷的信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static DataTable GetActivity_Coupon(string activityID)
        {
            string connString = ConnectionHelper.GetDecryptConn("Gungnir");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            DataTable dt = null;
            string sql = "select Instructions,ISNULL(PKID,0) as PKID  from Activity.dbo.tbl_CouponActivity_Coupon where ActivityID=@ActivityID AND Deleted=0 ";

            SqlParameter parameters = new SqlParameter("@ActivityID", activityID);
            dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            conn.Close();
            conn.Dispose();
            return dt;

        }


        /// <summary>
        /// 校验优惠券
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        public static DataTable CouponVlidate(string couponRulePKID)
        {
            Guid guid;
            if (!Guid.TryParse(couponRulePKID, out guid))
                return null;
            DataTable dt = null;
            if (string.IsNullOrEmpty(couponRulePKID))
                return dt;
            string sql = "SELECT * FROM  Activity..tbl_GetCouponRules WITH(NOLOCK) WHERE GetRuleGUID=@GetRuleGUID";
            string connString = ConnectionHelper.GetDecryptConn("Gungnir");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlParameter parameters = new SqlParameter("@GetRuleGUID", couponRulePKID);
            dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            conn.Close();
            conn.Dispose();
            return dt;
        }


        /// <summary>
        /// 校验优惠券
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        public static DataTable CouponVlidateForPKID(int pkid)
        {
            if (pkid > 0)
            {
                DataTable dt = new DataTable();
                string sql = "SELECT * FROM  Activity..tbl_GetCouponRules WITH(NOLOCK) WHERE PKID=@PKID";
                string connString = ConnectionHelper.GetDecryptConn("Gungnir");
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                SqlParameter parameters = new SqlParameter("@PKID", pkid);
                dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
                conn.Close();
                conn.Dispose();
                return dt;
            }
            return null;
        }

        public static DataTable CouponVlidateForPKIDS(IEnumerable<int> pkids)
        {
            if (pkids.Any())
            {
                using (var cmd =
                    new SqlCommand(
                        @"SELECT * FROM  Activity..tbl_GetCouponRules WITH(NOLOCK) WHERE PKID IN ( SELECT Item from Activity..SplitString(@PKIDS,',',1))")
                )
                {
                    cmd.Parameters.AddWithValue("@PKIDS", string.Join(",", pkids));
                    return DbHelper.ExecuteDataTable(cmd);
                }
            }
            return null;
        }

        public static DataTable CouponVlidate1(string couponRulePKID)
        {

            DataTable dt = null;
            if (string.IsNullOrEmpty(couponRulePKID))
                return dt;
            string sql = "SELECT * FROM  Activity..tbl_GetCouponRules WITH(NOLOCK) WHERE PKID=@PKID";
            string connString = ConnectionHelper.GetDecryptConn("Gungnir");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlParameter parameters = new SqlParameter("@PKID", couponRulePKID);
            dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters);
            conn.Close();
            conn.Dispose();
            return dt;
        }

        /// <summary>
        /// 查询当天的秒杀
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMiaoSha()
        {
            using (var cmd = new SqlCommand(@"Activity..Action_SelectMiaoShaProduct"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return DbHelper.ExecuteDataTable(cmd);
            }
        }

        #region 活动看板
        public static List<ActivityBuild> SelectActivityDetailsForActivityBoard(SqlConnection conn, DateTime start, DateTime end, string title, string createdUser, string owner)
        {
            const string sql = @" SELECT  
                id ,
                Title ,
                ActivityConfigType ,
                StartDT ,
                EndDate ,
                0 AS IsNew ,
                '' AS HashKey
        FROM    Activity..ActivityBuild WITH ( NOLOCK )
        WHERE   ActivityConfigType IS NOT NULL
                AND StartDT IS NOT NULL
                AND EndDate IS NOT NULL
                AND EndDate >= @StartTime
                AND StartDT <= @EndTime
                AND ( @Title = ''
                      OR @Title IS NULL
                      OR Title LIKE '%' + @Title + '%'
                    )
                AND ( @CreatedUser = ''
                      OR @CreatedUser IS NULL
                      OR CreatetorUser LIKE '%' + @CreatedUser + '%'
                    )
                AND ( @Owner = ''
                      OR @Owner IS NULL
                      OR PersonWheel LIKE '%' + @Owner + '%'
                    )
                UNION ALL
                SELECT  PKID AS Id ,
                        Title ,
                        ActivityType AS ActivityConfigType ,
                        StartDate AS StartDT ,
                        EndDate AS EndDate ,
                        1 AS IsNew ,
                        HashKey
                FROM    Configuration..ActivePageList WITH ( NOLOCK )
                WHERE   IsEnabled = 1
                        AND StartDate IS NOT NULL
                        AND EndDate IS NOT NULL
                        AND EndDate >= @StartTime
                        AND StartDate <= @EndTime
                        AND ( @Title = ''
                              OR @Title IS NULL
                              OR Title LIKE '%' + @Title + '%'
                            )
                        AND ( @CreatedUser = ''
                              OR @CreatedUser IS NULL
                              OR CreateorUser LIKE '%' + @CreatedUser + '%'
                            )
                        AND ( @Owner = ''
                              OR @Owner IS NULL
                              OR PersonCharge LIKE '%' + @Owner + '%'
                            )
                  ";

            var sqlPrams = new SqlParameter[]
            {
                new SqlParameter("@StartTime",start),
                new SqlParameter("@EndTime",end),
                new SqlParameter("@Title",title),
                new SqlParameter("@CreatedUser",createdUser),
                new SqlParameter("@Owner",owner)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlPrams).ConvertTo<ActivityBuild>().ToList();
        }
        #endregion

        #region 查询

        /// <summary>
        /// 查询优惠券规则
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="GetRuleGUID"></param>
        /// <returns></returns>
        public static WebCouponActivityRuleModel SelectCouponRule(SqlConnection conn, Guid GetRuleGUID)
        {
            var sql = @"SELECT  GCR.IntentionName ,
        GCR.DepartmentName ,
        GCR.Creater ,
        GCR.GetRuleGUID ,
        GCR.ValiEndDate ,
        GCR.ValiStartDate ,
        GCR.DeadLineDate ,
        GCR.Term ,
        GCR.Minmoney ,
        GCR.Discount ,
        GCR.PromtionName AS PromotionName ,
        GCR.Description ,
        GCR.RuleID ,
        GCR.Channel
FROM    Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
        JOIN Activity..tbl_GetCouponRules AS GCR WITH ( NOLOCK ) ON GCR.RuleID = CR.PKID
WHERE   GetRuleGUID = @GetRuleGUID;";
            var param = new SqlParameter[] {
                new SqlParameter("@GetRuleGUID", GetRuleGUID),
            };
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, param);

            var result = dt.Rows.Cast<DataRow>().Select(row => new WebCouponActivityRuleModel
            {
                IntentionName = row["IntentionName"].ToString(),
                DepartmentName = row["DepartmentName"].ToString(),
                Creater = row["Creater"].ToString(),
                GetRuleGUID = (Guid)row["GetRuleGUID"],
                DeadLineDate = Equals(row["DeadLineDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["DeadLineDate"],
                ValiEndDate = Equals(row["ValiEndDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["ValiEndDate"],
                ValiStartDate = Equals(row["ValiStartDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["ValiStartDate"],
                Term = Equals(row["Term"], DBNull.Value) ? (int?)null : (int)row["Term"],
                MinMoney = Equals(row["MinMoney"], DBNull.Value) ? 0 : (decimal)row["MinMoney"],
                Discount = Equals(row["Discount"], DBNull.Value) ? 0 : (decimal)row["Discount"],
                PromotionName = row["PromotionName"].ToString(),
                Description = row["Description"].ToString(),
                RuleID = (int)row["RuleID"],
                Channel = row["Channel"].ToString(),
            }).FirstOrDefault();
            return result;
        }


        /// <summary>
        /// 根据 大优惠券的 pkid 获取 所有的 可用的小优惠券
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="GetRuleGUID"></param>
        /// <returns></returns>
        public static List<WebCouponActivityRuleModel> SelectCouponRuleByCouponRulesPKID(SqlConnection conn,List<int> pkids)
        {
            string queryparam = string.Join(",", pkids.ConvertAll<string>(x => x.ToString())); 
            var sql = @"SELECT  GCR.IntentionName ,
                    GCR.DepartmentName ,
                    GCR.Creater ,
                    GCR.GetRuleGUID ,
                    GCR.ValiEndDate ,
                    GCR.ValiStartDate ,
                    GCR.DeadLineDate ,
                    GCR.Term ,
                    GCR.Minmoney ,
                    GCR.Discount ,
                    GCR.PromtionName AS PromotionName ,
                    GCR.Description ,
                    GCR.RuleID ,
                    GCR.Channel
            FROM    Activity..tbl_CouponRules AS CR WITH ( NOLOCK )
                    JOIN Activity..tbl_GetCouponRules AS GCR WITH ( NOLOCK ) ON GCR.RuleID = CR.PKID
            WHERE   CR.pkid in ("+ queryparam + ");";
            //var param = new SqlParameter[] {
            //    new SqlParameter("@queryparam", queryparam),
            //};
            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql);

            var result = dt.Rows.Cast<DataRow>().Select(row => new WebCouponActivityRuleModel
            {
                IntentionName = row["IntentionName"].ToString(),
                DepartmentName = row["DepartmentName"].ToString(),
                Creater = row["Creater"].ToString(),
                GetRuleGUID = (Guid)row["GetRuleGUID"],
                DeadLineDate = Equals(row["DeadLineDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["DeadLineDate"],
                ValiEndDate = Equals(row["ValiEndDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["ValiEndDate"],
                ValiStartDate = Equals(row["ValiStartDate"], DBNull.Value) ? (DateTime?)null : (DateTime)row["ValiStartDate"],
                Term = Equals(row["Term"], DBNull.Value) ? (int?)null : (int)row["Term"],
                MinMoney = Equals(row["MinMoney"], DBNull.Value) ? 0 : (decimal)row["MinMoney"],
                Discount = Equals(row["Discount"], DBNull.Value) ? 0 : (decimal)row["Discount"],
                PromotionName = row["PromotionName"].ToString(),
                Description = row["Description"].ToString(),
                RuleID = (int)row["RuleID"],
                Channel = row["Channel"].ToString(),
            }).ToList();
            return result;
        }

        #endregion
    }
}
