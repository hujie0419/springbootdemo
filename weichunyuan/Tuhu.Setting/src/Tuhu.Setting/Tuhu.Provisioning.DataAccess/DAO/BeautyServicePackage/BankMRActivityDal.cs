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
using Tuhu.Provisioning.DataAccess.Entity.BeautyService;

namespace Tuhu.Provisioning.DataAccess.DAO.BankMRActivityDal
{
    public class BankMRActivityDal
    {
        /// <summary>
        /// 根据活动ID获取银行活动配置
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public static BankMRActivityConfig SelectBankMRActivityConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT TOP 1 PKID ,
        ActivityId ,
        BankId ,
        ServiceId ,
        ServiceName ,
        ServiceCount ,
        UserSettlementPrice ,
        BankSettlementPrice ,
        ValidDays ,
        BannerImageUrl ,
        RuleDescription ,
        VerifyType ,
        CreateTime ,
        UpdateTime ,
        ActivityName ,
        CooperateId ,
        StartTime ,
        EndTime ,
        RoundCycleType ,
        ButtonColor ,
        SettleMentMethod ,
        Description ,
        IsActive,
        CustomerType ,
        Card2Length,
          MarketPrice,
          LogoUrl,
          ActivityLimitDescribe,
          ActivityTimeDescribe,
          BuyTips ,
          RoundStartTime ,
          OtherVerifyName 
FROM    Tuhu_groupon..BankMRActivityConfig (NOLOCK) WHERE ActivityId=@ActivityId";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId",activityId)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityConfig>().FirstOrDefault();
        }

        /// <summary>
        /// 根据活动id查询活动广告位
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityAdConfig> SelectBankMRActivityAdConfigByActivityId(SqlConnection conn,
            Guid activityId)
        {
            var sql = @"
SELECT  ActivityId ,
        AdType ,
        CreateTime ,
        ImgUrl ,
        JumpUrl ,
        PKID ,
        UpdateTime
FROM    Tuhu_groupon..BankMRActivityAdConfig WITH ( NOLOCK )
WHERE   ActivityId = @ActivityId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, parameters)
                .ConvertTo<BankMRActivityAdConfig>();
        }

        /// <summary>
        /// 插入或更新银行活动广告位配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpsertBankMRActivityAdConfig(SqlConnection conn, BankMRActivityAdConfig config)
        {
            var sql = @"IF EXISTS ( SELECT  1
            FROM    Tuhu_groupon..BankMRActivityAdConfig WITH ( NOLOCK )
            WHERE   ActivityId = @ActivityId
                    AND AdType = @AdType )
    BEGIN
        UPDATE  Tuhu_groupon..BankMRActivityAdConfig
        SET     ImgUrl = @ImgUrl ,
                JumpUrl = @JumpUrl ,
                UpdateTime = GETDATE()
        WHERE   ActivityId = @ActivityId
                AND AdType = @AdType
    END
ELSE
    BEGIN
        INSERT  Tuhu_groupon..BankMRActivityAdConfig
                ( ActivityId ,
                  AdType ,
                  ImgUrl ,
                  JumpUrl 
                )
        VALUES  ( @ActivityId ,
                  @AdType ,
                  @ImgUrl ,
                  @JumpUrl 
                )
    END";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", config.ActivityId),
                new SqlParameter("@AdType", config.AdType),
                new SqlParameter("@ImgUrl", config.ImgUrl),
                new SqlParameter("@JumpUrl", config.JumpUrl),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, parameters) > 0;
        }

        /// <summary>
        /// 分页查询银行美容活动配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<BankMRActivityConfig>, int> SelectBankMRActivityConfigs(SqlConnection conn, int pageIndex, int pageSize, int cooperateId,
            string activityName, string serviceId, string settleMentMethod)
        {
            #region sql
            var sql_count = @"SELECT  COUNT(1) 
FROM    Tuhu_groupon..BankMRActivityConfig WITH ( NOLOCK )
WHERE   ( CooperateId = @CooperateId
          OR @CooperateId <= 0
        )
        AND ( ActivityName LIKE N'%' + @ActivityName + '%'
              OR @ActivityName = ''
              OR @ActivityName IS NULL
            )
        AND ( ServiceId = @ServiceId
              OR @ServiceId = ''
              OR @ServiceId IS NULL
            )
        AND ( SettleMentMethod = @SettleMentMethod
              OR @SettleMentMethod = ''
              OR @SettleMentMethod IS NULL
            )";
            var sql = @"SELECT  PKID ,
        ActivityId ,
        BankId ,
        ServiceId ,
        ServiceName ,
        ServiceCount ,
        UserSettlementPrice ,
        BankSettlementPrice ,
        ValidDays ,
        BannerImageUrl ,
        RuleDescription ,
        VerifyType ,
        CreateTime ,
        UpdateTime ,
        ActivityName ,
        CooperateId ,
        StartTime ,
        EndTime ,
        RoundCycleType ,
        ButtonColor ,
        SettleMentMethod ,
        Description ,
        IsActive ,
        CustomerType, 
        Card2Length,
          MarketPrice,
          LogoUrl,
          ActivityLimitDescribe,
          ActivityTimeDescribe,
          BuyTips ,
          RoundStartTime ,
          OtherVerifyName 
FROM    Tuhu_groupon..BankMRActivityConfig WITH ( NOLOCK )
WHERE   ( CooperateId = @CooperateId
          OR @CooperateId <= 0
        )
        AND ( ActivityName LIKE N'%' + @ActivityName + '%'
              OR @ActivityName = ''
              OR @ActivityName IS NULL
            )
        AND ( ServiceId = @ServiceId
              OR @ServiceId = ''
              OR @ServiceId IS NULL
            )
        AND ( SettleMentMethod = @SettleMentMethod
              OR @SettleMentMethod = ''
              OR @SettleMentMethod IS NULL
            )
ORDER BY CreateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize  ROWS FETCH NEXT @PageSize
        ROWS ONLY; 
";
            #endregion
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex", pageIndex),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@CooperateId",cooperateId),
                new SqlParameter("@ActivityName",activityName),
                new SqlParameter("@ServiceId",serviceId),
                new SqlParameter("@SettleMentMethod", settleMentMethod)
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql_count, sqlParameters));
            IEnumerable<BankMRActivityConfig> result = null;
            if (count > 0)
                result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityConfig>();
            return Tuple.Create(result, count);
        }
        /// <summary>
        /// 插入银行美容活动配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertBankMRActivityConfig(SqlConnection conn, BankMRActivityConfig config)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BankMRActivityConfig
        ( ActivityId ,
          BankId ,
          ServiceId ,
          ServiceCount ,
          ServiceName ,
          UserSettlementPrice ,
          BankSettlementPrice ,
          ValidDays ,
          BannerImageUrl ,
          RuleDescription ,
          VerifyType ,
          ActivityName ,
          CooperateId ,
          StartTime ,
          EndTime ,
          RoundCycleType ,
          ButtonColor ,
          SettleMentMethod ,
          Description ,
          IsActive ,
          CustomerType,
          Card2Length,
          CodeTypeConfigId ,
          MarketPrice,
          LogoUrl,
          ActivityLimitDescribe,
          ActivityTimeDescribe,
          BuyTips,
          RoundStartTime ,
          OtherVerifyName 
        )
VALUES  ( @ActivityId ,
          @BankId ,
          @ServiceId ,
          @ServiceCount ,
          @ServiceName ,
          @UserSettlementPrice ,
          @BankSettlementPrice ,
          @ValidDays ,
          @BannerImageUrl ,
          @RuleDescription ,
          @VerifyType  ,
          @ActivityName ,
          @CooperateId ,
          @StartTime ,
          @EndTime ,
          @RoundCycleType ,
          @ButtonColor ,
          @SettleMentMethod ,
          @Description ,
          @IsActive ,
          @CustomerType,
          @Card2Length,
          @CodeTypeConfigId ,
          @MarketPrice,
          @LogoUrl,
          @ActivityLimitDescribe,
          @ActivityTimeDescribe,
          @BuyTips ,
          @RoundStartTime ,
          @OtherVerifyName 
        );";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CodeTypeConfigId",config.CodeTypeConfigId),
                new SqlParameter("@ActivityId",config.ActivityId),
                new SqlParameter("@BankId", config.BankId),
                new SqlParameter("@ServiceId", config.ServiceId),
                new SqlParameter("@ServiceCount", config.ServiceCount),
                new SqlParameter("@ServiceName", config.ServiceName),
                new SqlParameter("@UserSettlementPrice", config.UserSettlementPrice),
                new SqlParameter("@BankSettlementPrice", config.BankSettlementPrice),
                new SqlParameter("@ValidDays", config.ValidDays),
                new SqlParameter("@BannerImageUrl", config.BannerImageUrl),
                new SqlParameter("@RuleDescription", config.RuleDescription),
                new SqlParameter("@VerifyType",  config.VerifyType),
                new SqlParameter("@ActivityName", config.ActivityName),
                new SqlParameter("@CooperateId", config.CooperateId),
                new SqlParameter("@StartTime", config.StartTime),
                new SqlParameter("@EndTime", config.EndTime),
                new SqlParameter("@RoundCycleType", config.RoundCycleType),
                new SqlParameter("@ButtonColor", config.ButtonColor),
                new SqlParameter("@SettleMentMethod", config.SettleMentMethod),
                new SqlParameter("@Description", config.Description),
                new SqlParameter("@IsActive", config.IsActive),
                new SqlParameter("@CustomerType", config.CustomerType),
                new SqlParameter("@Card2Length", config.Card2Length),

                new SqlParameter("@MarketPrice", config.MarketPrice),
                new SqlParameter("@LogoUrl", config.LogoUrl),
                new SqlParameter("@ActivityLimitDescribe", config.ActivityLimitDescribe),
                new SqlParameter("@ActivityTimeDescribe", config.ActivityTimeDescribe),
                new SqlParameter("@BuyTips", config.BuyTips),
                new SqlParameter("@RoundStartTime", config.RoundStartTime),
                new SqlParameter("@OtherVerifyName", config.OtherVerifyName),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 更新银行美容配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBankMRActivityConfig(SqlConnection conn, BankMRActivityConfig config)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankMRActivityConfig
SET     BankId = @BankId ,
        ServiceId = @ServiceId ,
        ServiceCount = @ServiceCount ,
        ServiceName = @ServiceName ,
        UserSettlementPrice = @UserSettlementPrice ,
        BankSettlementPrice = @BankSettlementPrice ,
        ValidDays = @ValidDays ,
        BannerImageUrl = @BannerImageUrl ,
        RuleDescription = @RuleDescription ,
        VerifyType = @VerifyType ,
        ActivityName = @ActivityName ,
        CooperateId = @CooperateId ,
        StartTime = @StartTime ,
        EndTime = @EndTime ,
        RoundCycleType = @RoundCycleType ,
        ButtonColor = @ButtonColor ,
        SettleMentMethod = @SettleMentMethod ,
        Description = @Description ,
        IsActive = @IsActive ,
        CustomerType = @CustomerType ,
        Card2Length = @Card2Length ,
        CodeTypeConfigId=@CodeTypeConfigId ,
        MarketPrice=@MarketPrice,
        LogoUrl=@LogoUrl,
        ActivityLimitDescribe=@ActivityLimitDescribe,
        ActivityTimeDescribe=@ActivityTimeDescribe,
        BuyTips=@BuyTips ,
        RoundStartTime=IsNull(@RoundStartTime,RoundStartTime),
        OtherVerifyName=@OtherVerifyName 
WHERE   ActivityId = @ActivityId";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CodeTypeConfigId",config.CodeTypeConfigId),
                new SqlParameter("@ActivityId",config.ActivityId),
                new SqlParameter("@BankId", config.BankId),
                new SqlParameter("@ServiceId", config.ServiceId),
                new SqlParameter("@ServiceCount", config.ServiceCount),
                new SqlParameter("@ServiceName", config.ServiceName),
                new SqlParameter("@UserSettlementPrice", config.UserSettlementPrice),
                new SqlParameter("@BankSettlementPrice", config.BankSettlementPrice),
                new SqlParameter("@ValidDays", config.ValidDays),
                new SqlParameter("@BannerImageUrl", config.BannerImageUrl),
                new SqlParameter("@RuleDescription", config.RuleDescription),
                new SqlParameter("@VerifyType", config.VerifyType),
                new SqlParameter("@ActivityName", config.ActivityName),
                new SqlParameter("@CooperateId", config.CooperateId),
                new SqlParameter("@StartTime", config.StartTime),
                new SqlParameter("@EndTime", config.EndTime),
                new SqlParameter("@RoundCycleType", config.RoundCycleType),
                new SqlParameter("@ButtonColor", config.ButtonColor),
                new SqlParameter("@SettleMentMethod", config.SettleMentMethod),
                new SqlParameter("@Description", config.Description),
                new SqlParameter("@IsActive", config.IsActive),
                new SqlParameter("@CustomerType", config.CustomerType),
                new SqlParameter("@Card2Length", config.Card2Length),

                new SqlParameter("@MarketPrice", config.MarketPrice),
                new SqlParameter("@LogoUrl", config.LogoUrl),
                new SqlParameter("@ActivityLimitDescribe", config.ActivityLimitDescribe),
                new SqlParameter("@ActivityTimeDescribe", config.ActivityTimeDescribe),
                new SqlParameter("@BuyTips", config.BuyTips),
                new SqlParameter("@RoundStartTime", config.RoundStartTime),
                new SqlParameter("@OtherVerifyName", config.OtherVerifyName),
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 根据活动id查询美容活动场次
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityRoundConfig> SelectBankMRActivityRoundConfigsByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT PKID, ActivityId ,
        LimitCount ,
        StartTime ,
        EndTime ,
        IsActive,
        UserLimitCount 
FROM    Tuhu_groupon..BankMRActivityRoundConfig WITH ( NOLOCK )
WHERE   ActivityId = @ActivityId
ORDER BY CreateTime DESC";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId",activityId)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityRoundConfig>();
        }
        /// <summary>
        /// 根据活动id查询美容活动限购周时间设置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityLimitTimeConfig> SelectBankMRActivityLimitTimeConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT [PKID]
      ,[ActivityId]
      ,[DayOfWeek]
      ,[BeginTime]
      ,[EndTime]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [Tuhu_groupon].[dbo].[BankMRActivityLimitTimeConfig] WITH(NOLOCK) 
  WHERE ActivityId=@ActivityId";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId",activityId)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityLimitTimeConfig>();
        }
        /// <summary>
        /// 将当前活动的活动场次置为无效
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static bool SetBankMRActivityRoundDisable(SqlConnection conn, Guid activityId)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankMRActivityRoundConfig
SET     IsActive = 0
WHERE   ActivityId = @ActivityId;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId",activityId)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        /// <summary>
        /// 根据活动场次ID查询活动场次
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BankMRActivityRoundConfig SelectBankMRActivityRoundConfigByPKID(SqlConnection conn, int pkid)
        {
            var sql = @"SELECT  PKID ,
        ActivityId ,
        LimitCount ,
        StartTime ,
        EndTime ,
        IsActive
FROM    Tuhu_groupon..BankMRActivityRoundConfig (NOLOCK)
WHERE   PKID=@PKID";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",pkid)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityRoundConfig>().FirstOrDefault();
        }
        /// <summary>
        /// 插入银行美容活动场次配置
        /// </summary>
        /// <param name="roundConfig"></param>
        /// <returns></returns>
        public static bool InsertBankMRActivityRoundConfig(SqlConnection conn, BankMRActivityRoundConfig roundConfig)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BankMRActivityRoundConfig
        ( ActivityId ,
          LimitCount ,
          StartTime ,
          EndTime ,
          IsActive 
        )
VALUES  ( @ActivityId ,
          @LimitCount ,
          @StartTime ,
          @EndTime ,
          @IsActive 
        )";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", roundConfig.ActivityId),
                new SqlParameter("@LimitCount", roundConfig.LimitCount),
                new SqlParameter("@StartTime", roundConfig.StartTime),
                new SqlParameter("@EndTime", roundConfig.EndTime),
                new SqlParameter("@IsActive", roundConfig.IsActive)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 更新银行美容活动场次配置
        /// </summary>
        /// <param name="roundConfig"></param>
        /// <returns></returns>
        public static bool UpdateBankMRActivityRoundConfig(SqlConnection conn, BankMRActivityRoundConfig roundConfig)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankMRActivityRoundConfig
SET     ActivityId = @ActivityId ,
        LimitCount = @LimitCount ,
        StartTime = @StartTime ,
        EndTime = @EndTime ,
        IsActive = @IsActive
WHERE   PKID = @PKID";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", roundConfig.ActivityId),
                new SqlParameter("@LimitCount", roundConfig.LimitCount),
                new SqlParameter("@StartTime", roundConfig.StartTime),
                new SqlParameter("@EndTime", roundConfig.EndTime),
                new SqlParameter("@IsActive", roundConfig.IsActive),
                new SqlParameter("@PKID", roundConfig.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }

        public static bool UpdateBankMRActivityRoundLimitCount(SqlConnection conn, Guid activityId, int limitCount, int userLimitCount)
        {
            var sql = @"
    UPDATE  Tuhu_groupon..BankMRActivityRoundConfig
    SET     LimitCount = @LimitCount ,
            UserLimitCount=@UserLimitCount,
            UpdateTime = GETDATE()
    WHERE   ActivityId = @ActivityId
            AND IsActive = 1;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@LimitCount", limitCount),
                new SqlParameter("@UserLimitCount", userLimitCount)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        /// <summary>
        /// 根据服务码查询银行美容服务码记录
        /// </summary>
        /// <param name="serviceCodes"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityCodeRecord> SelectBankMRActivityCodeRecordByServiceCode(SqlConnection conn, IEnumerable<string> serviceCodes)
        {
            var sql = @"SELECT  ActivityRoundId ,
        UserId ,
        ServiceCode ,
        OrderId ,
        CreateTime ,
        UpdateTime ,
        BankCardNum ,
        OtherNum ,
        Mobile ,
        RuleId
FROM    Tuhu_groupon..BankMRActivityCodeRecord AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@ServiceCodes, ',', 1) AS B ON A.ServiceCode = B.Item;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ServiceCodes",string.Join(",",serviceCodes))
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityCodeRecord>();
        }

        /// <summary>
        /// 根据userId查询服务码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityCodeRecord> SelectBankMRActivityCodeRecordByUserId(SqlConnection conn, Guid userId)
        {
            var sql = @"SELECT  ActivityRoundId ,
        UserId ,
        ServiceCode ,
        OrderId ,
        CreateTime ,
        UpdateTime ,
        BankCardNum ,
        OtherNum ,
        Mobile ,
        RuleId
FROM    Tuhu_groupon..BankMRActivityCodeRecord AS A WITH ( NOLOCK )
WHERE   A.UserId = @UserId;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserId",userId)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityCodeRecord>();
        }
        /// <summary>
        /// 分页查询合作用户配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<MrCooperateUserConfig>, int> SelectMrCooperateUserConfigs(SqlConnection conn, int pageIndex, int pageSize)
        {
            var sql = @"SELECT  A.PKID ,
        A.CooperateName ,
        A.VipUserId ,
        A.Description ,
        A.CreateUser ,
        A.CreateTime ,
        A.UpdateTime
FROM    Tuhu_groupon..MrCooperateUserConfig AS A WITH ( NOLOCK )
ORDER BY CreateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY; 
SELECT  @TotalCount = COUNT(1)
FROM    Tuhu_groupon..MrCooperateUserConfig AS A WITH ( NOLOCK )";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@TotalCount",SqlDbType.Int){ Direction=ParameterDirection.Output}
            };

            var config = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<MrCooperateUserConfig>();
            return new Tuple<IEnumerable<MrCooperateUserConfig>, int>(config, Convert.ToInt32(sqlParameters.Last().Value));
        }
        /// <summary>
        /// 获取所有合作用户配置
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static IEnumerable<MrCooperateUserConfig> SelectAllMrCooperateUserConfigs(SqlConnection conn)
        {
            var sql = @"SELECT  A.PKID ,
        A.CooperateName ,
        A.VipUserId ,
        A.Description ,
        A.CreateUser ,
        A.CreateTime ,
        A.UpdateTime
FROM    Tuhu_groupon..MrCooperateUserConfig AS A WITH ( NOLOCK )";

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql).ConvertTo<MrCooperateUserConfig>();
        }


        public static MrCooperateUserConfig FetchMrCooperateUserConfigByPKID(SqlConnection conn, int pkid)
        {
            var sql = @"SELECT TOP 1 A.PKID ,
        A.CooperateName ,
        A.VipUserId ,
        A.Description ,
        A.CreateUser ,
        A.CreateTime ,
        A.UpdateTime
FROM    Tuhu_groupon..MrCooperateUserConfig AS A WITH ( NOLOCK )
WHERE   A.PKID = @PKID;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PKID",pkid)
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<MrCooperateUserConfig>()?.FirstOrDefault();
        }
        /// <summary>
        /// 插入合作用户配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertMrCooperateUserConfig(SqlConnection conn, MrCooperateUserConfig config)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..MrCooperateUserConfig
        ( CooperateName ,
          VipUserId ,
          Description ,
          CreateUser 
        )
VALUES  ( @CooperateName ,
          @VipUserId ,
          @Description ,
          @CreateUser  
        );";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CooperateName",config.CooperateName),
                new SqlParameter("@VipUserId",config.VipUserId),
                new SqlParameter("@Description",config.Description),
                new SqlParameter("@CreateUser",config.CreateUser)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }
        /// <summary>
        /// 更新合作用户配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateMrCooperateUserConfig(SqlConnection conn, MrCooperateUserConfig config)
        {
            var sql = @"UPDATE  Tuhu_groupon..MrCooperateUserConfig
        SET     CooperateName = @CooperateName ,
                VipUserId = @VipUserId ,
                Description = @Description ,
                UpdateTime = GETDATE()
        WHERE   PKID = @PKID;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@CooperateName",config.CooperateName),
                new SqlParameter("@VipUserId",config.VipUserId),
                new SqlParameter("@Description",config.Description),
                new SqlParameter("@PKID", config.PKID)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 获取限购配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static BankMRActivityLimitConfig FetchBankMRActivityLimitConfig(SqlConnection conn, Guid activityId)
        {
            var sql = @"SELECT TOP 1 PKID ,
        ActivityId ,
        DaysOfMonth ,
        DaysOfWeek ,
        DayLimit ,
        ProvinceIds ,
        CityIds ,
        DistrictIds ,
        CreateTime ,
        UpdateTime ,
        CardLimitValue ,
        CardLimitType ,
        StartTime ,
        UserDayLimit 
FROM    Tuhu_groupon..BankMRActivityLimitConfig WITH ( NOLOCK )
WHERE   ActivityId = @ActivityId;";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@ActivityId",activityId),
            };

            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityLimitConfig>().FirstOrDefault();
        }
        /// <summary>
        /// 插入限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertBankMRActivityLimitConfig(SqlConnection conn, BankMRActivityLimitConfig config)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BankMRActivityLimitConfig
        ( ActivityId ,
          DaysOfMonth ,
          DaysOfWeek ,
          DayLimit ,
          ProvinceIds ,
          CityIds ,
          DistrictIds ,
          CardLimitType ,
          CardLimitValue ,
          StartTime ,
          UserDayLimit 
        )
VALUES  ( @ActivityId ,
          @DaysOfMonth ,
          @DaysOfWeek ,
          @DayLimit ,
          @ProvinceIds ,
          @CityIds ,
          @DistrictIds ,
          @CardLimitType ,
          @CardLimitValue ,
          @StartTime ,
          @UserDayLimit 
        );";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@ActivityId",config.ActivityId),
               new SqlParameter("@DaysOfMonth",config.DaysOfMonth),
               new SqlParameter("@DaysOfWeek",config.DaysOfWeek),
               new SqlParameter("@DayLimit",config.DayLimit),
               new SqlParameter("@ProvinceIds",config.ProvinceIds),
               new SqlParameter("@CityIds",config.CityIds),
               new SqlParameter("@DistrictIds",config.DistrictIds),
               new SqlParameter("@CardLimitType",config.CardLimitType),
               new SqlParameter("@CardLimitValue",config.CardLimitValue),
               new SqlParameter("@StartTime",config.StartTime),
               new SqlParameter("@UserDayLimit",config.UserDayLimit)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 插入限购周时间配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertBankMRActivityLimitTimeConfig(SqlConnection conn, List<BankMRActivityLimitTimeConfig> configs)
        {
            var sql_count = @"SELECT COUNT(1) From Tuhu_groupon.dbo.BankMRActivityLimitTimeConfig WITH(NOLOCK) 
WHERE ActivityId=@ActivityId;";
            var sql_del = @"Delete From Tuhu_groupon.dbo.BankMRActivityLimitTimeConfig
WHERE ActivityId=@ActivityId;";
            var sql = @"
INSERT INTO Tuhu_groupon.dbo.BankMRActivityLimitTimeConfig
(ActivityId, DayOfWeek, BeginTime, EndTime, CreateTime, UpdateTime)
VALUES(@ActivityId, @DayOfWeek, @BeginTime, @EndTime, GetDate(), GetDate());";
            var result = true;
            var sqlParametersDelOrCount = new SqlParameter[]
            {
                new SqlParameter("@ActivityId",configs[0].ActivityId),
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql_count, sqlParametersDelOrCount));
            var deleteResult = true;
            if (count > 0)
                deleteResult = (SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql_del, sqlParametersDelOrCount) > 0);
            if (deleteResult)
                foreach (var config in configs)
                {

                    var sqlParameters = new SqlParameter[]
                    {
                    new SqlParameter("@ActivityId",config.ActivityId),
                    new SqlParameter("@DayOfWeek",config.DayOfWeek),
                    new SqlParameter("@BeginTime",config.BeginTime),
                    new SqlParameter("@EndTime", config.EndTime)
                    };
                    if (SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) <= 0)
                    {
                        result = false;
                        break;
                    }
                }
            else
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 更新限购配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBankMRActivityLimitConfig(SqlConnection conn, BankMRActivityLimitConfig config)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankMRActivityLimitConfig
SET     DaysOfMonth = @DaysOfMonth ,
        DaysOfWeek = @DaysOfWeek ,
        DayLimit = @DayLimit ,
        ProvinceIds = @ProvinceIds ,
        CityIds = @CityIds ,
        DistrictIds = @DistrictIds ,
        UpdateTime = GETDATE(),
        CardLimitType=@CardLimitType,
        CardLimitValue=@CardLimitValue ,
        StartTime=@StartTime ,
        UserDayLimit=@UserDayLimit 
WHERE   ActivityId = @ActivityId;";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@ActivityId",config.ActivityId),
               new SqlParameter("@DaysOfMonth",config.DaysOfMonth),
               new SqlParameter("@DaysOfWeek",config.DaysOfWeek),
               new SqlParameter("@DayLimit",config.DayLimit),
               new SqlParameter("@ProvinceIds",config.ProvinceIds),
               new SqlParameter("@CityIds",config.CityIds),
               new SqlParameter("@DistrictIds",config.DistrictIds),
               new SqlParameter("@CardLimitType",config.CardLimitType),
               new SqlParameter("@CardLimitValue",config.CardLimitValue),
               new SqlParameter("@StartTime",config.StartTime),
               new SqlParameter("@UserDayLimit",config.UserDayLimit)
            };

            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;

        }

        public static bool IsExistBankMRActivityUsersByRoundIds(SqlConnection conn, IEnumerable<int> roundIds)
        {
            var sql = @"SELECT TOP 1
        A.ActivityRoundId ,
        A.BankCardNum ,
        A.Mobile ,
        A.UserId ,
        A.CreateTime ,
        A.UpdateTime ,
        A.CardETC ,
        A.LimitCount ,
        A.OtherNum ,
        A.DayLimit
FROM    Tuhu_groupon..BankMRActivityUsers AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@RoundIds, ',', 1) AS B ON A.ActivityRoundId = B.Item;";
            var sqlParameter = new SqlParameter[]
            {
                new SqlParameter("@RoundIds",string.Join(",", roundIds))
            };

            var dt = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameter);
            return dt != null && dt.Rows != null && dt.Rows.Count > 0;
        }

        //public static bool DeleteBankMRActivityUsersByRoundIds(SqlConnection conn, IEnumerable<int> roundIds)
        //{
        //    var sql = @"";
        //}

        /// <summary>
        /// 批量导入银行美容活动用户规则
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool BatchImportBankMRActivityUsers(SqlConnection conn, DataTable table)
        {

            using (var sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = table.TableName;
                foreach (DataColumn cloumn in table.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(cloumn.ColumnName, cloumn.ColumnName);
                }
                sqlBulkCopy.WriteToServer(table);
                return true;
            }

        }

        public static IEnumerable<ImportBankMRActivityRecordModel> GetImportBankMRActivityUsers(SqlConnection conn, string batchCode)
        {
            const string sql = @"SELECT 
BU.BatchCode,
BU.ActivityRoundId,
BU.BankCardNum,
BU.Mobile,
BU.CreateTime,
BU.LimitCount,
BU.DayLimit,
BC.ActivityId,
BC.StartTime,
BC.EndTime ,
BU.OtherNum ,
OperateUser=(SELECT TOP 1 [OperateUser] FROM [Tuhu_log].[dbo].[BeautyOprLog] WITH(NOLOCK) WHERE [IdentityID]=Convert(nvarchar,BU.ActivityRoundId)),
VerifyType=(SELECT TOP 1 VerifyType FROM Tuhu_groupon..BankMRActivityConfig  WITH(NOLOCK) WHERE ActivityId=BC.ActivityId) 
From Tuhu_groupon.dbo.BankMRActivityUsers AS BU WITH(NOLOCK)
left join Tuhu_groupon..BankMRActivityRoundConfig AS BC WITH(NOLOCK) 
ON BC.PKID=BU.ActivityRoundId
WHERE  BU.BatchCode=@batchCode  
ORDER BY BU.CreateTime desc";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@batchCode",batchCode),
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<ImportBankMRActivityRecordModel>();
        }

        public static Tuple<int, IEnumerable<ImportBankMRActivityRecordModel>> GetImportBankMRActivityRecords(SqlConnection conn, Guid activityId)
        {
            const string sql_count = @"SELECT COUNT(Distinct(BU.batchcode)) 
From Tuhu_groupon.dbo.BankMRActivityUsers AS BU WITH(NOLOCK)
left join Tuhu_groupon..BankMRActivityRoundConfig AS BC WITH(NOLOCK) 
ON BC.PKID=BU.ActivityRoundId
WHERE  BC.ActivityId=@activityId  AND IsNull(BU.IsDeleted,0)=0 ";
            const string sql = @"SELECT distinct 
BU.BatchCode,
BC.StartTime,
BC.EndTime ,
BU.CreateTime,
Remarks=(SELECT TOP 1 [Remarks] FROM [Tuhu_log].[dbo].[BeautyOprLog] WITH(NOLOCK) WHERE [IdentityID]=BU.BatchCode),
OperateUser=(SELECT TOP 1 [OperateUser] FROM [Tuhu_log].[dbo].[BeautyOprLog] WITH(NOLOCK) WHERE [IdentityID]=BU.BatchCode)
From Tuhu_groupon.dbo.BankMRActivityUsers AS BU WITH(NOLOCK)
left join Tuhu_groupon..BankMRActivityRoundConfig AS BC WITH(NOLOCK) 
ON BC.PKID=BU.ActivityRoundId
WHERE  BC.ActivityId=@activityId  AND IsNull(BU.IsDeleted,0)=0 
ORDER BY BU.CreateTime desc";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@activityId",activityId),
            };
            var count = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sql_count, sqlParameters));
            IEnumerable<ImportBankMRActivityRecordModel> result = null;
            if (count > 0)
                result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<ImportBankMRActivityRecordModel>();
            return Tuple.Create(count, result);
        }
        public static bool DeleteImportBankMRActivityByBatchCode(SqlConnection conn, string batchCode)
        {
            const string sql = @"Update  Tuhu_groupon.dbo.BankMRActivityUsers  WITH(RowLock) 
 SET IsDeleted=1,UpdateTime=GetDate() 
  WHERE  BatchCode=@batchCode  AND IsNull(IsDeleted,0) =0;";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@batchCode",batchCode),
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        public static SignleBankMRActivityUserModel GetSignleBankMRActivityUser(SqlConnection conn, Guid activityId, string mobile, string card, string otherNo)
        {
            string sql = @"SELECT DISTINCT BU.BankCardNum,
BU.Mobile,
BU.LimitCount,
BU.DayLimit,
BU.OtherNum,
BatchCode,
BU.CreateTime,
BC.StartTime,
BC.EndTime,
OperateUser=(SELECT TOP 1 [OperateUser] FROM [Tuhu_log].[dbo].[BeautyOprLog] WITH(NOLOCK) WHERE [IdentityID]=BU.BatchCode) 
From Tuhu_groupon.dbo.BankMRActivityUsers AS BU WITH(NOLOCK)
left join Tuhu_groupon..BankMRActivityRoundConfig AS BC WITH(NOLOCK) 
ON BC.PKID=BU.ActivityRoundId
WHERE  BC.ActivityId=@activityId  AND IsNull(BU.IsDeleted,0)=0 
 {0}  
AND BC.StartTime<GetDate() 
AND BC.EndTime>GetDate() 
Order By BU.CreateTime desc";
            string whereCon = "";
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@activityId",activityId)
            };
            if (!string.IsNullOrEmpty(mobile))
            {
                whereCon = " AND @mobile like  BU.Mobile+'%' ";
                sqlParameters.Add(new SqlParameter("@mobile", mobile));
            }
            else if (!string.IsNullOrEmpty(card))
            {
                whereCon = " AND @card like  BU.BankCardNum+'%' ";
                sqlParameters.Add(new SqlParameter("@card", card));
            }
            else if (!string.IsNullOrEmpty(otherNo))
            {
                whereCon = " AND @otherNo like  BU.OtherNum+'%' ";
                sqlParameters.Add(new SqlParameter("@otherNo", otherNo));
            }
            sql = string.Format(sql, whereCon);
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters.ToArray())
                .ConvertTo<SignleBankMRActivityUserModel>().FirstOrDefault();
        }
        /// <summary>
        /// 根据活动Id查询银行美容活动
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static BankMRActivityConfig FetchBankMRActivityConfigByActivityId(SqlConnection conn, Guid activityId)
        {
            string sql = @"SELECT TOP 1
        PKID ,
        ActivityId ,
        BankId ,
        ServiceId ,
        ServiceName ,
        UserSettlementPrice ,
        BankSettlementPrice ,
        ServiceCount ,
        ValidDays ,
        CreateTime ,
        UpdateTime ,
        BannerImageUrl ,
        RuleDescription ,
        VerifyType ,
        ActivityName ,
        CooperateId ,
        StartTime ,
        EndTime ,
        RoundCycleType ,
        ButtonColor ,
        SettleMentMethod ,
        Description ,
        IsActive ,
        Card2Length ,
        CustomerType ,
        CodeTypeConfigId
FROM    Tuhu_groupon..BankMRActivityConfig WITH(NOLOCK)
WHERE   ActivityId = @ActivityId";
            var sqlParameters = new[] { new SqlParameter("@ActivityId", activityId) };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankMRActivityConfig>()?.FirstOrDefault();
        }
        /// <summary>
        /// 插入银行活动组配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool InsertBankActivityGroupConfig(SqlConnection conn, BankActivityGroupConfig config)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..BankActivityGroupConfig
        ( GroupId ,
          GroupName,
          RegionId ,
          ActivityId ,
          CreateUser
        )
VALUES  ( @GroupId ,
          @GroupName,
          @RegionId ,
          @ActivityId ,
          @CreateUser
        );";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@GroupId",config.GroupId),
                new SqlParameter("@GroupName",config.GroupName),
                new SqlParameter("@RegionId",config.RegionId),
                new SqlParameter("@ActivityId", config.ActivityId),
                new SqlParameter("@CreateUser", config.CreateUser)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 更新银行活动组名
        /// </summary>
        /// <returns></returns>
        public static bool UpdateBankActivityGroupNameByGroupId(SqlConnection conn, Guid groupId, string groupName)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankActivityGroupConfig
SET     GroupName= @GroupName,
        UpdatedDateTime = GETDATE() 
WHERE   GroupId = @GroupId";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@GroupId",groupId),
                new SqlParameter("@GroupName",groupName)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }
        /// <summary>
        /// 根据组名获取银行活动组配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static IEnumerable<BankActivityGroupConfig> SelectBankActivityGroupConfigsByGroupName(SqlConnection conn, string groupName)
        {
            var sql = @"SELECT
        PKID ,
        GroupId ,
        GroupName ,
        RegionId ,
        ActivityId ,
        CreateUser ,
        CreatedDateTime ,
        UpdatedDateTime
FROM    Tuhu_groupon..BankActivityGroupConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0 AND GroupName = @GroupName";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@GroupName", groupName)).ConvertTo<BankActivityGroupConfig>();
        }

        /// <summary>
        /// 更新银行活动组
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateBankActivityGroupConfigByPKID(SqlConnection conn, BankActivityGroupConfig config)
        {
            var sql = @"UPDATE  Tuhu_groupon..BankActivityGroupConfig
SET     RegionId = @RegionId,
        ActivityId = @ActivityId,
        UpdatedDateTime = GETDATE() 
WHERE   PKID = @PKID";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@RegionId",config.RegionId),
                new SqlParameter("@ActivityId",config.ActivityId),
                new SqlParameter("@PKID",config.PKID)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
        }

        /// <summary>
        /// 分页获取银行活动组配置
        /// </summary>
        /// <returns></returns>
        public static Tuple<IEnumerable<BankActivityGroupConfig>, int> SelectBankActivityGroupConfigs(SqlConnection conn, int pageIndex, int pageSize)
        {
            var sql = @"
SET @TotalCount = (SELECT  COUNT(A.GroupId)
FROM    ( SELECT    GroupId
          FROM      Tuhu_groupon..BankActivityGroupConfig WITH ( NOLOCK )
          WHERE     IsDeleted = 0
          GROUP BY  GroupId
        ) AS A);
SELECT  A.PKID ,
        A.GroupId ,
        A.GroupName ,
        A.RegionId ,
        B.RegionName ,
        A.ActivityId ,
        A.CreateUser ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..BankActivityGroupConfig AS A WITH ( NOLOCK )
        LEFT JOIN Gungnir..tbl_region AS B WITH ( NOLOCK ) ON A.RegionId = B.PKID
WHERE   A.PKID IN ( SELECT  MIN(PKID)
                    FROM    Tuhu_groupon..BankActivityGroupConfig
                    WHERE   IsDeleted = 0
                    GROUP BY GroupId )
ORDER BY A.CreatedDateTime DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
            ROWS ONLY";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters)
                .ConvertTo<BankActivityGroupConfig>();
            return new Tuple<IEnumerable<BankActivityGroupConfig>, int>(result, (int)sqlParameters[2].Value);
        }

        /// <summary>
        /// 根据ID删除银行活动组配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteBankActivityGroupConfigByPKID(SqlConnection conn, int pkid)
        {
            //            string sql1 = @"UPDATE  Tuhu_groupon..BankActivityGroupConfig SET IsDeleted = 1
            //WHERE GroupId = @ID";
            string sql = @"UPDATE  Tuhu_groupon..BankActivityGroupConfig SET IsDeleted = 1
WHERE PKID = @PKID";
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, new SqlParameter("@PKID", pkid)) > 0;
        }
        /// <summary>
        /// 根据组Id分页获取银行活动组配置
        /// </summary>
        /// <returns></returns>
        public static Tuple<IEnumerable<BankActivityGroupConfig>, int> SelectBankActivityGroupConfigsByGroupId(SqlConnection conn, Guid groupId, int pageIndex, int pageSize)
        {
            string sql = @"
SET @TotalCount = ( SELECT  COUNT(1)
                    FROM    Tuhu_groupon..BankActivityGroupConfig WITH ( NOLOCK )
                    WHERE   IsDeleted = 0 AND GroupId = @GroupId
                  );
SELECT  A.PKID ,
        A.GroupId ,
        A.GroupName ,
        A.RegionId ,
        B.RegionName ,
        A.ActivityId ,
        A.CreateUser ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..BankActivityGroupConfig AS A WITH ( NOLOCK )
        LEFT JOIN Gungnir..tbl_region AS B WITH ( NOLOCK ) ON A.RegionId = B.PKID
WHERE   A.IsDeleted = 0 AND GroupId = @GroupId
ORDER BY A.CreatedDateTime DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize
            ROWS ONLY";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@GroupId",groupId),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                }
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlParameters).ConvertTo<BankActivityGroupConfig>();
            return new Tuple<IEnumerable<BankActivityGroupConfig>, int>(result, (int)sqlParameters[3].Value);
        }

        /// <summary>
        /// 根据PKID获取银行活动组配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BankActivityGroupConfig SelectBankActivityGroupConfigByPKID(SqlConnection conn,
            int pkid)
        {
            string sql = @"SELECT TOP 1
        PKID ,
        GroupId ,
        GroupName ,
        RegionId ,
        ActivityId ,
        CreateUser ,
        CreatedDateTime ,
        UpdatedDateTime
FROM    Tuhu_groupon..BankActivityGroupConfig WITH ( NOLOCK )
WHERE   IsDeleted = 0 AND PKID = @PKID";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@PKID", pkid)).ConvertTo<BankActivityGroupConfig>().FirstOrDefault();
        }

        /// <summary>
        /// 根据组配置Id获取导入的银行活动白名单
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupConfigId"></param>
        /// <returns></returns>
        public static IEnumerable<BankActivityWhiteUsers> SelectImportBankActivityWhiteUsersByGroupConfigId(
            SqlConnection conn, int groupConfigId)
        {
            string sql = @"SELECT  *
    FROM    Tuhu_groupon..BankActivityWhiteUsers WITH ( NOLOCK )
    WHERE   GroupConfigId = @GroupConfigId";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@GroupConfigId", groupConfigId)).ConvertTo<BankActivityWhiteUsers>();
        }

        /// <summary>
        /// 根据组Id获取已导入银行活动白名单
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static IEnumerable<BankActivityWhiteUsers> SelectImportBankActivityWhiteUsersByGroupId(
            SqlConnection conn, Guid groupId)
        {
            string sql = @"  SELECT    A.PKID ,
            A.GroupConfigId ,
            A.CardNum ,
            A.Mobile ,
            A.CreatedDateTime ,
            A.UpdatedDateTime
  FROM      Tuhu_groupon..BankActivityWhiteUsers AS A WITH ( NOLOCK )
            JOIN Tuhu_groupon..BankActivityGroupConfig AS B WITH ( NOLOCK ) ON A.GroupConfigId = B.PKID
  WHERE     B.IsDeleted = 0 AND B.GroupId = @GroupId";
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, new SqlParameter("@GroupId", groupId)).ConvertTo<BankActivityWhiteUsers>();
        }
        /// <summary>
        /// 分页查询银行活动展示列表配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        public static Tuple<int, IEnumerable<BankMrActivityDisplayConfigEntity>> SelectBankMrActivityDisplayConfigs(
    SqlConnection conn, int pageIndex, int pageSize, int active)
        {
            #region sql
            string sqlCount = @"SELECT COUNT(1) 
  FROM [Tuhu_groupon].[dbo].[BankMRActivityDisplayConfig] WITH(NOLOCK) 
  WHERE 1=1   {0} ";
            string sql = @"SELECT [PKID]
      ,[ActivityId]
      ,[Title]
      ,[Description]
      ,[AppJumpUrl]
      ,[H5JumpUrl]
      ,[ImageUrl]
      ,[DisplayBeginTime]
      ,[DisplayEndTime]
      ,[Sort]
      ,[IsActive]
      ,[UpdateDateTime]
      ,[CreateDateTime]
      ,[CreatedUser]
  FROM [Tuhu_groupon].[dbo].[BankMRActivityDisplayConfig] WITH(NOLOCK) 
  WHERE 1=1   {0} 
  Order by PKID DESC 
  OFFSET @Begin Rows Fetch NEXT @PageSize Rows Only";
            #endregion

            IEnumerable<BankMrActivityDisplayConfigEntity> result = null;
            string whereCond = string.Empty;
            var sqlparas = new List<SqlParameter>
            {
                new SqlParameter("@Begin",(pageIndex-1)*pageSize),
                new SqlParameter("@PageSize",pageSize),
            };
            if (active == 0 || active == 1)
            {
                whereCond = $" AND IsActive=@isActive ";
                sqlparas.Add(new SqlParameter("@isActive", active));
            }
            sqlCount = string.Format(sqlCount, whereCond);
            sql = string.Format(sql, whereCond);
            var total = Convert.ToInt32(SqlHelper.ExecuteScalar(conn, CommandType.Text, sqlCount, sqlparas.ToArray()));
            if (total > 0)
                result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlparas.ToArray()).ConvertTo<BankMrActivityDisplayConfigEntity>();
            return Tuple.Create(total, result);
        }
        /// <summary>
        /// 查询银行活动展示列表配置详情
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BankMrActivityDisplayConfigEntity GetBankMrActivityDisplayConfigsDetail(
SqlConnection conn, int pkid)
        {
            #region sql
            string sql = @"SELECT [PKID]
      ,[ActivityId]
      ,[Title]
      ,[Description]
      ,[AppJumpUrl]
      ,[H5JumpUrl]
      ,[ImageUrl]
      ,[DisplayBeginTime]
      ,[DisplayEndTime]
      ,[Sort]
      ,[IsActive]
      ,[UpdateDateTime]
      ,[CreateDateTime]
      ,[CreatedUser]
  FROM [Tuhu_groupon].[dbo].[BankMRActivityDisplayConfig] WITH(NOLOCK) 
  WHERE PKID=@PKID";
            #endregion
            var sqlparas = new List<SqlParameter>
            {
                new SqlParameter("@PKID",pkid),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlparas.ToArray()).ConvertTo<BankMrActivityDisplayConfigEntity>();
            return result.FirstOrDefault();
        }
        /// <summary>
        /// 保存银行活动展示列表配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="regionEntitys"></param>
        /// <returns></returns>
        public static bool SaveBankMrActivityDisplayConfig(SqlConnection conn, BankMrActivityDisplayConfigEntity entity, IEnumerable<BankMRActivityDisplayRegionEntity> regionEntitys)
        {
            #region sql
            string sql = @"Insert into [Tuhu_groupon].[dbo].[BankMRActivityDisplayConfig]
 ([ActivityId]
 ,[Title]
 ,[Description]
 ,[AppJumpUrl]
 ,[H5JumpUrl]
 ,[ImageUrl]
 ,[DisplayBeginTime]
 ,[DisplayEndTime]
 ,[Sort]
 ,[IsActive]
 ,[UpdateDateTime]
 ,[CreateDateTime]
 ,[CreatedUser])
  Values(
  @ActivityId
 ,@Title
 ,@Description
 ,@AppJumpUrl
 ,@H5JumpUrl
 ,@ImageUrl
 ,@DisplayBeginTime
 ,@DisplayEndTime
 ,@Sort
 ,1
 ,GetDate()
 ,GetDate()
 ,@CreatedUser
  )
  SELECT @@identity";

            string sqlRegion = @"Insert  into  Tuhu_groupon..BankMRActivityDisplayRegion
(DisplayConfigId,
ProvinceId,
IsAllCitys,
DistrictIds,
IsAllDistrict,
IsDeleted,
CreateDateTime,
UpdateDateTime,
CityId)
Values(
@DisplayConfigId,
@ProvinceId,
@IsAllCitys,
@DistrictIds,
@IsAllDistrict,
@IsDeleted,
GetDate(),
GetDate(),
@CityId
)
";
            #endregion
            var sqlparas = new SqlParameter[] {
                new SqlParameter("@ActivityId",entity.ActivityId),
                new SqlParameter("@Title",entity.Title),
                new SqlParameter("@Description",entity.Description),
                new SqlParameter("@AppJumpUrl",entity.AppJumpUrl),
                new SqlParameter("@H5JumpUrl",entity.H5JumpUrl),
                new SqlParameter("@ImageUrl",entity.ImageUrl),
                new SqlParameter("@DisplayBeginTime",entity.DisplayBeginTime),
                new SqlParameter("@DisplayEndTime",entity.DisplayEndTime),
                new SqlParameter("@CreatedUser",entity.CreatedUser),
                new SqlParameter("@Sort",entity.Sort),
            };
            var tran = conn.BeginTransaction();
            var result = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlparas.ToArray());
            bool success = result > 0;
            if (success && regionEntitys != null)
            {
                foreach (var item in regionEntitys)
                {
                    var sqlparas2 = new SqlParameter[] {
                new SqlParameter("@DisplayConfigId",result),
                new SqlParameter("@ProvinceId",item.ProvinceId),
                new SqlParameter("@IsAllCitys",item.IsAllCitys),
                new SqlParameter("@DistrictIds",item.DistrictIds),
                new SqlParameter("@IsAllDistrict",item.IsAllDistrict),
                new SqlParameter("@IsDeleted",0),
                new SqlParameter("@CityId",item.CityId),
                     };
                    if (SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sqlRegion, sqlparas2.ToArray()) <= 0)
                    {
                        success = false;
                        break;
                    }
                }
            }
            if (success)
            {
                tran.Commit();
                return true;
            }
            else
            {
                tran.Rollback();
                return false;
            }
        }
        /// <summary>
        /// 更新银行活动展示列表配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="regionEntitys"></param>
        /// <returns></returns>
        public static bool UpdateBankMrActivityDisplayConfig(SqlConnection conn, BankMrActivityDisplayConfigEntity entity, IEnumerable<BankMRActivityDisplayRegionEntity> regionEntitys)
        {
            #region sql
            string sql = @"UPDATE  [Tuhu_groupon].[dbo].[BankMRActivityDisplayConfig] WITH(ROWLOCK)
 SET [ActivityId]=@ActivityId
 ,[Title]=@Title
 ,[Description]=@Description
 ,[AppJumpUrl]=@AppJumpUrl
 ,[H5JumpUrl]=@H5JumpUrl
 ,[ImageUrl]=@ImageUrl
 ,[DisplayBeginTime]=@DisplayBeginTime
 ,[DisplayEndTime]=@DisplayEndTime
 ,[Sort]=@Sort
 ,[IsActive]=@IsActive
 ,[UpdateDateTime]=GetDate()
 WHERE PKID=@PKID";
            string sqlDelRegion = @"Update Tuhu_groupon..BankMRActivityDisplayRegion WITH(ROWLOCK) 
SET IsDeleted=1,UpdateDateTime=Getdate()  WHERE DisplayConfigId=@DisplayConfigId AND IsDeleted=0";
            string sqlRegion = @"Insert  into  Tuhu_groupon..BankMRActivityDisplayRegion
(DisplayConfigId,
ProvinceId,
IsAllCitys,
DistrictIds,
IsAllDistrict,
IsDeleted,
CreateDateTime,
UpdateDateTime,
CityId)
Values(
@DisplayConfigId,
@ProvinceId,
@IsAllCitys,
@DistrictIds,
@IsAllDistrict,
@IsDeleted,
GetDate(),
GetDate(),
@CityId
)
";
            #endregion
            var sqlparas = new SqlParameter[] {
                new SqlParameter("@ActivityId",entity.ActivityId),
                new SqlParameter("@Title",entity.Title),
                new SqlParameter("@Description",entity.Description),
                new SqlParameter("@AppJumpUrl",entity.AppJumpUrl),
                new SqlParameter("@H5JumpUrl",entity.H5JumpUrl),
                new SqlParameter("@ImageUrl",entity.ImageUrl),
                new SqlParameter("@DisplayBeginTime",entity.DisplayBeginTime),
                new SqlParameter("@DisplayEndTime",entity.DisplayEndTime),
                new SqlParameter("@IsActive",entity.IsActive),
                new SqlParameter("@PKID",entity.PKID),
                new SqlParameter("@Sort",entity.Sort),
            };
            var tran = conn.BeginTransaction();
            var result = SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sql, sqlparas.ToArray());
            bool success = result > 0;
            if (success && regionEntitys != null)
            {
                SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sqlDelRegion, new SqlParameter("@DisplayConfigId", entity.PKID));
                foreach (var item in regionEntitys)
                {
                    var sqlparas2 = new SqlParameter[] {
                new SqlParameter("@DisplayConfigId",result),
                new SqlParameter("@ProvinceId",item.ProvinceId),
                new SqlParameter("@IsAllCitys",item.IsAllCitys),
                new SqlParameter("@DistrictIds",item.DistrictIds),
                new SqlParameter("@IsAllDistrict",item.IsAllDistrict),
                new SqlParameter("@IsDeleted",0),
                new SqlParameter("@CityId",item.CityId),
                     };
                    if (SqlHelper.ExecuteNonQuery(tran, CommandType.Text, sqlRegion, sqlparas2.ToArray()) <= 0)
                    {
                        success = false;
                        break;
                    }
                }
            }
            if (success)
            {
                tran.Commit();
                return true;
            }
            else
            {
                tran.Rollback();
                return false;
            }
        }
        /// <summary>
        /// 获取银行活动展示列表区域配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="displayConfigId"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityDisplayRegionEntity> GetBankMRActivityDisplayRegionEntity(
SqlConnection conn, int displayConfigId)
        {
            #region sql
            string sql = @"SELECT   [PKID]
      ,[DisplayConfigId]
      ,[ProvinceId]
      ,[IsAllCitys]
      ,[DistrictIds]
      ,[IsAllDistrict]
      ,[IsDeleted]
      ,[CreateDateTime]
      ,[UpdateDateTime]
      ,[CityId]
  FROM [Tuhu_groupon].[dbo].[BankMRActivityDisplayRegion] WITH(NOLOCK) 
  WHERE DisplayConfigId=@DisplayConfigId and IsDeleted=0";
            #endregion
            var sqlparas = new List<SqlParameter>
            {
                new SqlParameter("@DisplayConfigId",displayConfigId),
            };
            var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, sqlparas.ToArray()).ConvertTo<BankMRActivityDisplayRegionEntity>();
            return result;
        }


        #region 银行活动使用记录
        /// <summary>
        /// 根据条件查找使用记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityCodeRecord> GetBankMrAtivityCodeRecord(SqlConnection conn, string code, string mobile, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT  bmcr.PKID ,
            bmcr.ActivityRoundId ,
            bmcr.UserId ,
            bmcr.ServiceCode ,
            bmcr.OrderId ,
            bmcr.CreateTime ,
            bmcr.UpdateTime ,
            bmcr.BankCardNum ,
            bmcr.OtherNum ,
            bmcr.Mobile ,
            bmcr.RuleId ,
            COUNT(1) OVER ( ) AS Total
    FROM    Tuhu_groupon..BankMRActivityCodeRecord AS bmcr WITH ( NOLOCK )
    WHERE   ( @ServiceCode = ''
              OR @ServiceCode IS NULL
              OR bmcr.ServiceCode = @ServiceCode
            )
            AND ( @Mobile = ''
                  OR @Mobile IS NULL
                  OR bmcr.Mobile = @Mobile
                )
            ORDER BY bmcr.PKID
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
            ONLY;";
            var para = new SqlParameter[]
            {
               new SqlParameter("@ServiceCode",code),
               new SqlParameter("@Mobile",mobile),
               new SqlParameter("@PageIndex",pageIndex),
               new SqlParameter("@PageSize",pageSize)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityCodeRecord>();
        }
        /// <summary>
        /// 获取活动场次配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="roundIdList"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityRoundConfig> GetBankMrAtivityRoundConfig(SqlConnection conn, List<int> roundIdList)
        {
            const string sql = @"
            SELECT  bmrc.PKID ,
            bmrc.ActivityId ,
            bmrc.LimitCount ,
            bmrc.StartTime ,
            bmrc.EndTime ,
            bmrc.IsActive ,
            bmrc.CreateTime ,
            bmrc.UpdateTime ,
            bmrc.UserLimitCount
    FROM    Tuhu_groupon..BankMRActivityRoundConfig bmrc WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                     FROM   Tuhu_groupon..SplitString(@RoundIdStr, ',', 1) AS t
                     WHERE  t.Item = PKID )";
            var para = new SqlParameter[]
            {
               new SqlParameter("@RoundIdStr",string.Join(",",roundIdList))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityRoundConfig>();
        }
        /// <summary>
        /// 获取活动配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityIdList"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityLimitConfig> GetBankMrAtivityLimitConfig(SqlConnection conn, List<Guid> activityIdList)
        {
            const string sql = @"
            SELECT  bmlc.PKID ,
            bmlc.ActivityId ,
            bmlc.DaysOfMonth ,
            bmlc.DaysOfWeek ,
            bmlc.DayLimit ,
            bmlc.ProvinceIds ,
            bmlc.CityIds ,
            bmlc.DistrictIds ,
            bmlc.CreateTime ,
            bmlc.UpdateTime ,
            bmlc.CardLimitType ,
            bmlc.CardLimitValue ,
            bmlc.StartTime ,
            bmlc.UserDayLimit
    FROM    Tuhu_groupon..BankMRActivityLimitConfig AS bmlc WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                     FROM   Tuhu_groupon..SplitString(@ActivityIdStr, ',', 1) AS t
                     WHERE  t.Item = bmlc.ActivityId );";
            var para = new SqlParameter[]
            {
               new SqlParameter("@ActivityIdStr",string.Join(",",activityIdList))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityLimitConfig>();
        }
        /// <summary>
        /// 获取用户规则
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="roundIdList"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityUser> GetBankMrAtivityUser(SqlConnection conn, List<int> roundIdList)
        {
            const string sql = @"
            SELECT  bmau.PKID ,
            bmau.ActivityRoundId ,
            bmau.BankCardNum ,
            bmau.Mobile ,
            bmau.UserId ,
            bmau.CreateTime ,
            bmau.UpdateTime ,
            bmau.CardETC ,
            bmau.LimitCount ,
            bmau.OtherNum ,
            bmau.DayLimit ,
            bmau.BatchCode
    FROM    Tuhu_groupon..BankMRActivityUsers AS bmau WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                         FROM   Tuhu_groupon..SplitString(@RoundIdStr, ',', 1) AS t
                         WHERE  t.Item = bmau.ActivityRoundId );";
            var para = new SqlParameter[]
            {
               new SqlParameter("@RoundIdStr",string.Join(",",roundIdList))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityUser>();
        }

        public static BankMRActivityUser GetBankMrAtivityUserByRoundIdAndTime(SqlConnection conn, int roundId,DateTime createTime)
        {
            const string sql = @"
            SELECT TOP 1
            bmau.PKID ,
            bmau.ActivityRoundId ,
            bmau.BankCardNum ,
            bmau.Mobile ,
            bmau.UserId ,
            bmau.CreateTime ,
            bmau.UpdateTime ,
            bmau.CardETC ,
            bmau.LimitCount ,
            bmau.OtherNum ,
            bmau.DayLimit ,
            bmau.BatchCode
    FROM    Tuhu_groupon..BankMRActivityUsers AS bmau WITH ( NOLOCK )
    WHERE   bmau.ActivityRoundId = @RoundId
            AND bmau.CreateTime <= @CreateTime
    ORDER BY bmau.PKID DESC;";
            var para = new SqlParameter[]
            {
               new SqlParameter("@RoundId",roundId),
               new SqlParameter("@CreateTime",createTime)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityUser>().FirstOrDefault();
        }
        /// <summary>
        /// 获取活动配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="activityIdList"></param>
        /// <returns></returns>
        public static IEnumerable<BankMRActivityConfig> GetBankMrActivityConfig(SqlConnection conn, List<Guid> activityIdList)
        {
            const string sql = @"
            SELECT  bmac.PKID ,
            bmac.ActivityId ,
            bmac.BankId ,
            bmac.ServiceId ,
            bmac.ServiceName ,
            bmac.ServiceCount ,
            bmac.UserSettlementPrice ,
            bmac.BankSettlementPrice ,
            bmac.ValidDays ,
            bmac.BannerImageUrl ,
            bmac.RuleDescription ,
            bmac.VerifyType ,
            bmac.CreateTime ,
            bmac.UpdateTime ,
            bmac.ActivityName ,
            bmac.CooperateId ,
            bmac.StartTime ,
            bmac.EndTime ,
            bmac.RoundCycleType ,
            bmac.ButtonColor ,
            bmac.SettleMentMethod ,
            bmac.Description ,
            bmac.IsActive ,
            bmac.CustomerType ,
            bmac.Card2Length ,
            bmac.MarketPrice ,
            bmac.LogoUrl ,
            bmac.ActivityLimitDescribe ,
            bmac.ActivityTimeDescribe ,
            bmac.BuyTips ,
            bmac.RoundStartTime
    FROM    Tuhu_groupon..BankMRActivityConfig AS bmac WITH ( NOLOCK )
    WHERE   EXISTS ( SELECT 1
                     FROM   Tuhu_groupon..SplitString(@ActivityIdStr, ',', 1) AS t
                     WHERE  t.Item = bmac.ActivityId );";
            var para = new SqlParameter[]
            {
               new SqlParameter("@ActivityIdStr",string.Join(",",activityIdList))
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityConfig>();
        }
        /// <summary>
        /// 通过PKID获取使用记录
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static BankMRActivityCodeRecord GetBankMrAtivityCodeRecordByPKID(SqlConnection conn, int pkid)
        {
            const string sql = @"
            SELECT  bmcr.PKID ,
            bmcr.ActivityRoundId ,
            bmcr.UserId ,
            bmcr.ServiceCode ,
            bmcr.OrderId ,
            bmcr.CreateTime ,
            bmcr.UpdateTime ,
            bmcr.BankCardNum ,
            bmcr.OtherNum ,
            bmcr.Mobile ,
            bmcr.RuleId ,
            COUNT(1) OVER ( ) AS Total
    FROM    Tuhu_groupon..BankMRActivityCodeRecord AS bmcr WITH ( NOLOCK )
    WHERE   bmcr.PKID = @PKID";
            var para = new SqlParameter[]
            {
               new SqlParameter("@PKID",pkid)
            };
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, para).ConvertTo<BankMRActivityCodeRecord>().FirstOrDefault(); ;
        }
        #endregion

    }
}
