using Dapper;
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
using Tuhu.Provisioning.DataAccess.Entity.NewCoupon;

namespace Tuhu.Provisioning.DataAccess.DAO.NewCoupon
{
    public static class DALNewCoupon
    {
        public static List<NewCouponActivity> SelectNewCouponConfig(SqlConnection conn, Guid activityId, string activityName, int pageIndex, int pageSize)
        {
            return conn.Query<NewCouponActivity>(@"SELECT  * ,COUNT(1) OVER ( ) AS Total
            FROM    Activity..CouponActivityConfig WITH ( NOLOCK )
            WHERE   ( @ActivityId = '00000000-0000-0000-0000-000000000000'
                      OR @ActivityId  IS NULL
                      OR ActivityId = @ActivityId
                    )
                    AND ( @ActivityName = ''
                          OR @ActivityName IS NULL
                          OR ActivityName LIKE '%' + @ActivityName + '%'
                        )
            ORDER BY PKID DESC
                    OFFSET @PageSize * ( @PageIndex - 1 ) ROWS FETCH NEXT @PageSize ROWS
                    ONLY;", new
            {
                ActivityId = activityId,
                ActivityName = activityName,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, commandType: CommandType.Text).ToList();
        }

        public static List<RecommendActivityConfig> SelectRecommendActivityConfig(SqlConnection conn, Guid activityId)
        {
            return conn.Query<RecommendActivityConfig>(@"SELECT * FROM Activity..RecommendActivityConfig WITH (NOLOCK) WHERE ActivityId=@ActivityId;",
                new { ActivityId = activityId },
                commandType: CommandType.Text).ToList();
        }

        public static int InsertCouponRulesConfig(SqlConnection conn, CouponRulesConfig model)
        {
            return conn.Execute(@"INSERT INTO Activity..CouponRulesConfig
                    (ActivityId,
                      RulesGUID,
                      CreateDateTime,
                      LastUpdateDateTime
                    )
            VALUES(@ActivityId,
                      @RulesGUID,
                      GETDATE(),
                      GETDATE()
            )", new
            {
                ActivityId = model.ActivityId,
                RulesGUID = model.RulesGUID
            }, commandType: CommandType.Text);
        }

        public static int DeleteCouponRulesConfig(SqlConnection conn, Guid activityId)
        {
            return conn.Execute(@"DELETE FROM Activity..CouponRulesConfig WHERE ActivityId=@ActivityId", new { ActivityId = activityId }, commandType: CommandType.Text);
        }

        public static List<CouponRulesConfig> SelectCouponRulesConfig(SqlConnection conn, Guid activityId)
        {
            return conn.Query<CouponRulesConfig>(@"SELECT * FROM Activity..CouponRulesConfig WITH (NOLOCK) WHERE ActivityId=@ActivityId", new { ActivityId = activityId }, commandType: CommandType.Text).ToList();
        }


        public static int DeleteNewCouponConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            return conn.Execute(@"DELETE FROM Activity..CouponActivityConfig WHERE ActivityId=@ActivityId", new { ActivityId = activityId }, commandType: CommandType.Text);
        }

        public static int DeleteRecommendActivityByActivityId(SqlConnection conn, Guid activityId)
        {
            return conn.Execute(@"DELETE FROM Activity..RecommendActivityConfig WHERE ActivityId=@ActivityId", new { ActivityId = activityId }, commandType: CommandType.Text);
        }

        public static int InsertRecommendActivityConfig(SqlConnection conn, RecommendActivityConfig data)
        {
            return conn.Execute(@"INSERT INTO Activity..RecommendActivityConfig
                (ActivityId,
                  ActivityType,
                  ImgUrl,
                  ActivityUrl,
                  CreateDateTime,
                  LastUpdateDateTime
                )
        VALUES(@ActivityId,
                  @ActivityType,
                  @ImgUrl,
                  @ActivityUrl,
                  GETDATE(),
                  GETDATE()
                )", new
            {
                ActivityId = data.ActivityId,
                ActivityType = data.ActivityType.ToString(),
                ImgUrl = data.ImgUrl,
                ActivityUrl = data.ActivityUrl
            }, commandType: CommandType.Text);
        }

        public static int InsertNewCouponConfig(SqlConnection conn, NewCouponActivity data, string user)
        {
            var sql = @"INSERT  INTO Activity..CouponActivityConfig
                ( IsEnabled ,
                  ActivityId ,
                  ActivityName ,
                  Channel ,
                  StartTime ,
                  EndTime ,
                  ValidDays ,
                  QuantityPerUser ,
                  DailyQuantityPerUser ,
                  PageQuantity ,
                  IsNewUser ,
                  HeadImgUrl ,
                  BottomImgUrl ,
                  Description ,
                  VerifyCodeNum ,
                  VerifyImgNum ,
                  AutoEndNum ,
                  VerifyCodeErrorNum ,
                  MobileLockTime ,
                  IsDefaultPage ,
                  IsShowLuckFriends ,
                  SuccessHeadImgUrl ,
                  SuccessBottomImgUrl ,
                  ButtonText ,
                  ButtonUrl ,
                  DefaultFailText ,
                  DefaultFailUrl ,
                  SuccessButtonUrl ,
                  FailButtonUrl ,
                  SuccessMsg ,
                  NoStartMsg ,
                  OverdueMsg ,
                  NoCouponMsg ,
                  PageClosedMsg ,
                  DuplicateAttemptMsg ,
                  ServiceExceptionsMsg ,
                  BlackHouseMsg ,
                  CreateDateTime ,
                  CreateUser ,
                  LastUpdateDateTime ,
                  Owner ,
                  PageType ,
                  IsRandom ,
                  RandomGroupId ,
                  AppShareId ,
                  RandomBigGroupId,RandomMinPos,RandomMaxPos,FinishCount,ActivityDesc                  
                )
        VALUES  ( @IsEnabled ,
                  @ActivityId ,
                  @ActivityName ,
                  @Channel ,
                  @StartTime ,
                  @EndTime ,
                  @ValidDays ,
                  @QuantityPerUser ,
                  @DailyQuantityPerUser ,
                  @PageQuantity ,
                  @IsNewUser ,
                  @HeadImgUrl ,
                  @BottomImgUrl ,
                  @Description ,
                  @VerifyCodeNum ,
                  @VerifyImgNum ,
                  @AutoEndNum ,
                  @VerifyCodeErrorNum ,
                  @MobileLockTime ,
                  @IsDefaultPage ,
                  @IsShowLuckFriends ,
                  @SuccessHeadImgUrl ,
                  @SuccessBottomImgUrl ,
                  @ButtonText ,
                  @ButtonUrl ,
                  @DefaultFailText ,
                  @DefaultFailUrl ,
                  @SuccessButtonUrl ,
                  @FailButtonUrl ,
                  @SuccessMsg ,
                  @NoStartMsg ,
                  @OverdueMsg ,
                  @NoCouponMsg ,
                  @PageClosedMsg ,
                  @DuplicateAttemptMsg ,
                  @ServiceExceptionsMsg ,
                  @BlackHouseMsg ,
                  GETDATE() ,
                  @CreateUser ,
                  GETDATE() ,
                  @Owner ,
                  @PageType ,
                  @IsRandom ,
                  @RandomGroupId ,
                  @AppShareId ,            
                 @RandomBigGroupId,@RandomMinPos,@RandomMaxPos,@FinishCount,@ActivityDesc
                );";

            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                IsEnabled = data.IsEnabled,
                ActivityName = data.ActivityName,
                Channel = data.Channel,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                ValidDays = data.ValidDays,
                QuantityPerUser = data.QuantityPerUser,
                DailyQuantityPerUser = data.DailyQuantityPerUser,
                PageQuantity = data.PageQuantity,
                IsNewUser = data.IsNewUser,
                HeadImgUrl = data.HeadImgUrl,
                BottomImgUrl = data.BottomImgUrl,
                Description = data.Description,
                VerifyCodeNum = data.VerifyCodeNum,
                VerifyImgNum = data.VerifyImgNum,
                AutoEndNum = data.AutoEndNum,
                VerifyCodeErrorNum = data.VerifyCodeErrorNum,
                MobileLockTime = data.MobileLockTime,
                IsDefaultPage = data.IsDefaultPage,
                IsShowLuckFriends = data.IsShowLuckFriends,
                SuccessHeadImgUrl = data.SuccessHeadImgUrl,
                SuccessBottomImgUrl = data.SuccessBottomImgUrl,
                ButtonText = data.ButtonText,
                ButtonUrl = data.ButtonUrl,
                DefaultFailText = data.DefaultFailText,
                DefaultFailUrl = data.DefaultFailUrl,
                SuccessButtonUrl = data.SuccessButtonUrl,
                FailButtonUrl = data.FailButtonUrl,
                SuccessMsg = data.SuccessMsg,
                NoStartMsg = data.NoStartMsg,
                OverdueMsg = data.OverdueMsg,
                NoCouponMsg = data.NoCouponMsg,
                PageClosedMsg = data.PageClosedMsg,
                DuplicateAttemptMsg = data.DuplicateAttemptMsg,
                ServiceExceptionsMsg = data.ServiceExceptionsMsg,
                BlackHouseMsg = data.BlackHouseMsg,
                CreateUser = user,
                Owner = data.Owner,
                PageType = data.PageType,
                IsRandom = data.IsRandom,
                RandomGroupId = data.RandomGroupId,
                AppShareId = data.AppShareId,

                RandomBigGroupId = data.RandomBigGroupId,
                RandomMinPos = data.RandomMinPos,
                RandomMaxPos =data.RandomMaxPos,
                FinishCount= data.FinishCount,
                ActivityDesc = data.ActivityDesc
            }, commandType: CommandType.Text);
        }

        public static int UpdateNewCouponConfig(SqlConnection conn, NewCouponActivity data, string user)
        {
            var sql = @"UPDATE  Activity..CouponActivityConfig
            SET     IsEnabled = @IsEnabled ,
            ActivityName = @ActivityName ,
            Channel = @Channel ,
            StartTime = @StartTime ,
            EndTime = @EndTime ,
            ValidDays = @ValidDays ,
            QuantityPerUser = @QuantityPerUser ,
            DailyQuantityPerUser = @DailyQuantityPerUser ,
            PageQuantity = @PageQuantity ,
            IsNewUser = @IsNewUser ,
            HeadImgUrl = @HeadImgUrl ,
            BottomImgUrl = @BottomImgUrl ,
            Description = @Description ,
            VerifyCodeNum = @VerifyCodeNum ,
            VerifyImgNum = @VerifyImgNum ,
            AutoEndNum = @AutoEndNum ,
            VerifyCodeErrorNum = @VerifyCodeErrorNum ,
            MobileLockTime = @MobileLockTime ,
            IsDefaultPage = @IsDefaultPage ,
            IsShowLuckFriends = @IsShowLuckFriends ,
            SuccessHeadImgUrl = @SuccessHeadImgUrl ,
            SuccessBottomImgUrl = @SuccessBottomImgUrl ,
            ButtonText = @ButtonText ,
            ButtonUrl = @ButtonUrl ,
            DefaultFailText = @DefaultFailText,
            DefaultFailUrl = @DefaultFailUrl,
            SuccessButtonUrl = @SuccessButtonUrl ,
            FailButtonUrl = @FailButtonUrl ,
            SuccessMsg = @SuccessMsg ,
            NoStartMsg = @NoStartMsg ,
            OverdueMsg = @OverdueMsg ,
            NoCouponMsg = @NoCouponMsg ,
            PageClosedMsg = @PageClosedMsg ,
            DuplicateAttemptMsg = @DuplicateAttemptMsg ,
            ServiceExceptionsMsg = @ServiceExceptionsMsg ,
            BlackHouseMsg = @BlackHouseMsg ,
            LastUpdateUser = @LastUpdateUser ,
            LastUpdateDateTime = GETDATE() ,
            Owner = @Owner ,
            PageType = @PageType ,
            IsRandom = @IsRandom ,
            RandomGroupId = @RandomGroupId ,
            AppShareId = @AppShareId ,

            RandomBigGroupId =@RandomBigGroupId,
            RandomMinPos = @RandomMinPos,
            RandomMaxPos = @RandomMaxPos,
            FinishCount = @FinishCount,
            ActivityDesc =@ActivityDesc

            WHERE  ActivityId = @ActivityId ;";
            return conn.Execute(sql, new
            {
                ActivityId = data.ActivityId,
                IsEnabled = data.IsEnabled,
                ActivityName = data.ActivityName,
                Channel = data.Channel,
                StartTime = data.StartTime,
                EndTime = data.EndTime,
                ValidDays = data.ValidDays,
                QuantityPerUser = data.QuantityPerUser,
                DailyQuantityPerUser = data.DailyQuantityPerUser,
                PageQuantity = data.PageQuantity,
                IsNewUser = data.IsNewUser,
                HeadImgUrl = data.HeadImgUrl,
                BottomImgUrl = data.BottomImgUrl,
                Description = data.Description,
                VerifyCodeNum = data.VerifyCodeNum,
                VerifyImgNum = data.VerifyImgNum,
                AutoEndNum = data.AutoEndNum,
                VerifyCodeErrorNum = data.VerifyCodeErrorNum,
                MobileLockTime = data.MobileLockTime,
                IsDefaultPage = data.IsDefaultPage,
                IsShowLuckFriends = data.IsShowLuckFriends,
                SuccessHeadImgUrl = data.SuccessHeadImgUrl,
                SuccessBottomImgUrl = data.SuccessBottomImgUrl,
                ButtonText = data.ButtonText,
                ButtonUrl = data.ButtonUrl,
                DefaultFailText = data.DefaultFailText,
                DefaultFailUrl = data.DefaultFailUrl,
                SuccessButtonUrl = data.SuccessButtonUrl,
                FailButtonUrl = data.FailButtonUrl,
                SuccessMsg = data.SuccessMsg,
                NoStartMsg = data.NoStartMsg,
                OverdueMsg = data.OverdueMsg,
                NoCouponMsg = data.NoCouponMsg,
                PageClosedMsg = data.PageClosedMsg,
                DuplicateAttemptMsg = data.DuplicateAttemptMsg,
                ServiceExceptionsMsg = data.ServiceExceptionsMsg,
                BlackHouseMsg = data.BlackHouseMsg,
                LastUpdateUser = user,
                Owner = data.Owner,
                PageType = data.PageType,
                IsRandom = data.IsRandom,
                RandomGroupId = data.RandomGroupId,
                AppShareId = data.AppShareId,

                RandomBigGroupId = data.RandomBigGroupId,
                RandomMinPos = data.RandomMinPos,
                RandomMaxPos = data.RandomMaxPos,
                FinishCount = data.FinishCount,
                ActivityDesc = data.ActivityDesc

            }, commandType: CommandType.Text);
        }

        public static List<SE_GetPromotionActivityCouponInfoConfig> SelectActivityCouponInfo(SqlConnection conn,string rulesIdStr)
        {
            var sql = @"WITH    rulesIdList
          AS ( SELECT   *
               FROM     Gungnir..SplitString(@RulesIdStr, ',', 1)
             )
    SELECT  PKID AS GetRuleID ,
            GetRuleGUID AS GetRuleGUID ,
            CASE Term
              WHEN NULL THEN 1
              ELSE 0
            END AS VerificationMode ,
            Term AS ValidDays ,
            ValiStartDate AS ValidStartDateTime ,
            ValiEndDate AS ValidEndDateTime ,
            SupportUserRange AS UserType ,
            [Description] AS [Description] ,
            Discount ,
            Minmoney ,
            SingleQuantity ,
            Quantity
    FROM    Activity.dbo.tbl_GetCouponRules (NOLOCK)
    WHERE   EXISTS ( SELECT 1
                     FROM   rulesIdList
                     WHERE  item = GetRuleGUID );";

            return conn.Query<SE_GetPromotionActivityCouponInfoConfig>(sql, new { RulesIdStr = rulesIdStr }, commandType: CommandType.Text).ToList();
        }

        public static List<CouponActivityLog> SelectOperationLog(string objectId, string type)
        {
            var conn = ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbHelper = new Tuhu.Component.Common.SqlDbHelper(conn))
            {
                var cmd = new SqlCommand(@"SELECT * FROM Tuhu_log..CouponActivityLog WITH (NOLOCK) WHERE ObjectId=@ObjectId AND Type=@Type ORDER BY CreatedTime DESC");
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ObjectId", objectId);
                cmd.Parameters.AddWithValue("@Type", type.ToString());
                return dbHelper.ExecuteDataTable(cmd).ConvertTo<CouponActivityLog>().ToList();
            }
        }

        //public static int SelectGetCouponCountByActivityId(SqlConnection conn, Guid ActivityId)
        //{
        //    const string sql = @"  SELECT    COUNT(1) FROM      SystemLog.dbo.tbl_GetPromotionActivityLog (NOLOCK) WHERE     ActivityID = @ActivityID; ";
        //    return Convert.ToInt32(conn.ExecuteScalar(sql, new { ActivityID = ActivityId }, commandType: CommandType.Text));
        //}
        public static int IsExistRandomId(SqlConnection conn, string randomGroupId)
        {
            const string sql = @"SELECT COUNT(1) FROM Configuration..BigBrandRewardList WITH (NOLOCK) WHERE HashKeyValue=@HashKeyValue";
            return Convert.ToInt32(conn.ExecuteScalar(sql, new { HashKeyValue = randomGroupId }, commandType: CommandType.Text));
        }
    }
}
