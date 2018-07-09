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
          RoundStartTime 
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
          RoundStartTime 
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
          RoundStartTime 
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
          @RoundStartTime 
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
        RoundStartTime=@RoundStartTime 
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
        IsActive
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

        public static bool UpdateBankMRActivityRoundLimitCount(SqlConnection conn, Guid activityId, int limitCount)
        {
            var sql = @"
    UPDATE  Tuhu_groupon..BankMRActivityRoundConfig
    SET     LimitCount = @LimitCount ,
            UpdateTime = GETDATE()
    WHERE   ActivityId = @ActivityId
            AND IsActive = 1;";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ActivityId", activityId),
                new SqlParameter("@LimitCount", limitCount)
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
        CreateTime ,
        UpdateTime ,
        CardLimitValue ,
        CardLimitType ,
        StartTime 
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
          CardLimitType ,
          CardLimitValue ,
          StartTime 
        )
VALUES  ( @ActivityId ,
          @DaysOfMonth ,
          @DaysOfWeek ,
          @DayLimit ,
          @ProvinceIds ,
          @CityIds ,
          @CardLimitType ,
          @CardLimitValue ,
          @StartTime 
        );";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@ActivityId",config.ActivityId),
               new SqlParameter("@DaysOfMonth",config.DaysOfMonth),
               new SqlParameter("@DaysOfWeek",config.DaysOfWeek),
               new SqlParameter("@DayLimit",config.DayLimit),
               new SqlParameter("@ProvinceIds",config.ProvinceIds),
               new SqlParameter("@CityIds",config.CityIds),
               new SqlParameter("@CardLimitType",config.CardLimitType),
               new SqlParameter("@CardLimitValue",config.CardLimitValue),
               new SqlParameter("@StartTime",config.StartTime)
            };
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, sqlParameters) > 0;
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
        UpdateTime = GETDATE(),
        CardLimitType=@CardLimitType,
        CardLimitValue=@CardLimitValue ,
        StartTime=@StartTime 
WHERE   ActivityId = @ActivityId;";
            var sqlParameters = new SqlParameter[]
            {
               new SqlParameter("@ActivityId",config.ActivityId),
               new SqlParameter("@DaysOfMonth",config.DaysOfMonth),
               new SqlParameter("@DaysOfWeek",config.DaysOfWeek),
               new SqlParameter("@DayLimit",config.DayLimit),
               new SqlParameter("@ProvinceIds",config.ProvinceIds),
               new SqlParameter("@CityIds",config.CityIds),
               new SqlParameter("@CardLimitType",config.CardLimitType),
               new SqlParameter("@CardLimitValue",config.CardLimitValue),
               new SqlParameter("@StartTime",config.StartTime)
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
    }
}
