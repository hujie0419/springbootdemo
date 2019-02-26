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
using Tuhu.Provisioning.DataAccess.Entity.GeneralBeautyServerCode;
using Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode;

namespace Tuhu.Provisioning.DataAccess.DAO.UnivRedemptionCode
{
    public class DALUnivRedemptionCode
    {
        #region MyRegion

        /// <summary>
        /// 分页获取配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Tuple<int, List<RedemptionConfig>> SelectRedemptionCodeConfigs(SqlConnection conn, SearchRedemptionConfigRequest request)
        {
            var sql = @"SELECT  @Total = COUNT(1)
FROM    Tuhu_groupon..RedemptionConfig AS rc WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK ) ON rc.CooperateId = mcuc.PKID
WHERE   rc.GenerateType = @GenerateType
        AND ( @CooperateId <= 0
              OR mcuc.PKID = @CooperateId
            )
        AND ( CASE WHEN @SettlementMethodType = 0 THEN 1
                   WHEN @SettlementMethodType = 1
                        AND ( rc.SettlementMethod IS NULL
                              OR rc.SettlementMethod = N''
                            ) THEN 1
                   WHEN @SettlementMethodType = 2
                        AND rc.SettlementMethod = N'BatchPreSettled' THEN 1
                   ELSE 0
              END ) = 1
        AND ( @CodeTypeConfigId <= 0
              OR EXISTS ( SELECT    1
                          FROM      Tuhu_groupon..RedeemMrCodeConfig AS rmcc
                                    WITH ( NOLOCK )
                          WHERE     rmcc.CodeTypeConfigId = @CodeTypeConfigId
                                    AND rc.ConfigId = rmcc.RedemptionConfigId )
            );
SELECT  rc.ConfigId ,
        rc.PKID ,
        mcuc.CooperateName ,
        rc.Name ,
        rc.EffectiveDay ,
        rc.AtLeastNum ,
        rc.SettlementMethod ,
        rc.GenerateType ,
        rc.Description ,
        rc.IsActive ,
        rc.CreateUser ,
        ( SELECT    COUNT(1)
          FROM      Tuhu_groupon..RedemptionCodeRecord AS rcr WITH ( NOLOCK )
          WHERE     rcr.RedemptionConfigId = rc.ConfigId
        ) AS SumQuantity ,
        rc.AuditStatus ,
        rc.Auditor,
        rc.GroupId 
FROM    Tuhu_groupon..RedemptionConfig AS rc WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK ) ON rc.CooperateId = mcuc.PKID
WHERE   rc.GenerateType = @GenerateType
        AND ( @CooperateId <= 0
              OR mcuc.PKID = @CooperateId
            )
        AND ( CASE WHEN @SettlementMethodType = 0 THEN 1
                   WHEN @SettlementMethodType = 1
                        AND ( rc.SettlementMethod IS NULL
                              OR rc.SettlementMethod = N''
                            ) THEN 1
                   WHEN @SettlementMethodType = 2
                        AND rc.SettlementMethod = N'BatchPreSettled' THEN 1
                   ELSE 0
              END ) = 1
        AND ( @CodeTypeConfigId <= 0
              OR EXISTS ( SELECT    1
                          FROM      Tuhu_groupon..RedeemMrCodeConfig AS rmcc
                                    WITH ( NOLOCK )
                          WHERE     rmcc.CodeTypeConfigId = @CodeTypeConfigId
                                    AND rc.ConfigId = rmcc.RedemptionConfigId )
            )
ORDER BY rc.PKID DESC
        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
            var parameters = new DynamicParameters();
            parameters.Add("@CooperateId", request.CooperateId);
            parameters.Add("@CodeTypeConfigId", request.CodeTypeConfigId);
            parameters.Add("@SettlementMethodType", request.SettlementMethodType);
            parameters.Add("@GenerateType", request.GenerateType);
            parameters.Add("@Total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Skip", (request.PageIndex - 1) * request.PageSize);
            parameters.Add("@Take", request.PageSize);
            var result = conn.Query<RedemptionConfig>(sql, parameters,
                commandType: CommandType.Text)?.ToList() ?? new List<RedemptionConfig>();
            var total = parameters.Get<int>("@Total");
            return Tuple.Create(total, result);
        }

        public static List<RedemptionConfig> SelectRedemptionCodeConfigsByGenerateType(SqlConnection conn, string generateType)
        {
            var sql = @"SELECT  rc.ConfigId ,
        rc.Name ,
        rc.EffectiveDay ,
        rc.AtLeastNum ,
        rc.SettlementMethod ,
        rc.GenerateType ,
        rc.Description
FROM    Tuhu_groupon..RedemptionConfig AS rc WITH ( NOLOCK )
WHERE   rc.GenerateType = @GenerateType
        AND rc.AuditStatus = N'Accepted'
        AND rc.IsActive = 1
        AND EXISTS ( SELECT 1
                     FROM   Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK )
                     WHERE  mcuc.PKID = rc.CooperateId );";
            var parameters = new DynamicParameters();
            parameters.Add("@GenerateType", generateType);
            var result = conn.Query<RedemptionConfig>(sql, new { GenerateType = generateType },
                commandType: CommandType.Text)?.ToList() ?? new List<RedemptionConfig>();
            return result;
        }


        /// <summary>
        /// 是否存在配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool IsExistsRedemptionConfig(SqlConnection conn, RedemptionConfig config)
        {
            var sql = @"SELECT  COUNT(1)
FROM    Tuhu_groupon..RedemptionConfig AS t WITH ( NOLOCK )
WHERE   t.Name = @Name
        AND ( @ConfigId IS NULL
              OR t.ConfigId <> @ConfigId
            );";
            var count = Convert.ToInt32(conn.ExecuteScalar(sql, new { config.Name, config.ConfigId }, commandType: System.Data.CommandType.Text));
            return count > 0;
        }

        /// <summary>
        /// 添加兑换码模板配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool AddRedemptionCodeConfig(SqlConnection conn, RedemptionConfig config)
        {
            const string sql = @"INSERT  INTO Tuhu_groupon..RedemptionConfig
        ( ConfigId ,
          CooperateId ,
          Name ,
          EffectiveDay ,
          AtLeastNum ,
          AtMostNum ,
          SettlementMethod ,
          GenerateType ,
          Description ,
          IsActive ,
          AuditStatus ,
          CreateUser ,
          Auditor ,
          CreateTime ,
          UpdateTime ,
          GroupId 
        )
VALUES  ( @ConfigId ,
          @CooperateId ,
          @Name ,
          @EffectiveDay ,
          @AtLeastNum ,
          @AtMostNum ,
          @SettlementMethod ,
          @GenerateType ,
          @Description ,
          @IsActive ,
          @AuditStatus ,
          @CreateUser ,
          @Auditor ,
          GETDATE() ,
          GETDATE() ,
          @GroupId 
        );";
            var result = conn.Execute(sql, new
            {
                config.GroupId,
                config.ConfigId,
                config.CooperateId,
                config.Name,
                config.EffectiveDay,
                config.AtLeastNum,
                config.AtMostNum,
                config.SettlementMethod,
                config.GenerateType,
                config.Description,
                config.IsActive,
                config.AuditStatus,
                config.CreateUser,
                config.Auditor
            }, commandType: System.Data.CommandType.Text);
            return result > 0;
        }

        /// <summary>
        /// 根据ConfigId获取配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static RedemptionConfig GetRedemptionCodeConfig(SqlConnection conn, Guid configId)
        {
            const string sql = @"SELECT  rc.ConfigId ,
        mcuc.CooperateName ,
        rc.Name ,
        rc.EffectiveDay ,
        rc.AtLeastNum ,
        rc.SettlementMethod ,
        rc.GenerateType ,
        rc.Description ,
        rc.IsActive ,
        rc.CreateUser ,
        ( SELECT    COUNT(1)
          FROM      Tuhu_groupon..RedemptionCodeRecord AS rcr WITH ( NOLOCK )
          WHERE     rcr.RedemptionConfigId = rc.ConfigId
        ) AS SumQuantity ,
        rc.AuditStatus ,
        rc.Auditor ,
        rc.CooperateId,
        mcuc.VipUserId,
        rc.GroupId 
FROM    Tuhu_groupon..RedemptionConfig AS rc WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK ) ON rc.CooperateId = mcuc.PKID
WHERE   rc.ConfigId = @ConfigId;";
            return conn.Query<RedemptionConfig>(sql, new { ConfigId = configId }, commandType: System.Data.CommandType.Text).FirstOrDefault();
        }
        /// <summary>
        /// 根据GroupId获取配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static IEnumerable<RedemptionConfig> GetRedemptionConfigsGroupId(SqlConnection conn, Guid groupId)
        {
            const string sql = @"SELECT  ConfigId ,
        Name ,
        EffectiveDay ,
        AtLeastNum ,
        SettlementMethod ,
        GenerateType ,
        Description ,
        IsActive ,
        CreateUser ,
        AuditStatus ,
        Auditor ,
        CooperateId,
        GroupId 
FROM    Tuhu_groupon..RedemptionConfig  WITH ( NOLOCK )
WHERE   groupId = @groupId and IsActive=1;";
            return conn.Query<RedemptionConfig>(sql, new { groupId = groupId }, commandType: System.Data.CommandType.Text);
        }
        /// <summary>
        /// 审核配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="configId"></param>
        /// <param name="auditStatus"></param>
        /// <returns></returns>
        public static bool AuditRedemptionCodeConfig(SqlConnection conn, Guid configId, string auditStatus, string auditor)
        {
            const string sql = @"UPDATE  Tuhu_groupon..RedemptionConfig
SET     AuditStatus = @AuditStatus ,
        Auditor = @Auditor ,
        UpdateTime = GETDATE()
WHERE   ConfigId = @ConfigId;";
            return conn.Execute(sql, new
            {
                ConfigId = configId,
                AuditStatus = auditStatus,
                Auditor = auditor
            }, commandType: System.Data.CommandType.Text) > 0;
        }

        /// <summary>
        /// 更新兑换码配置
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool UpdateRedemptionCodeConfig(SqlConnection conn, RedemptionConfig config)
        {
            const string sql = @"UPDATE  Tuhu_groupon..RedemptionConfig
SET     CooperateId = @CooperateId ,
        Name = @Name ,
        EffectiveDay = @EffectiveDay ,
        AtLeastNum = @AtLeastNum ,
        AtMostNum = @AtMostNum ,
        SettlementMethod = @SettlementMethod ,
        GenerateType = @GenerateType ,
        Description = @Description ,
        IsActive = @IsActive ,
        AuditStatus = @AuditStatus,
        UpdateTime = GETDATE(),
        GroupId=@GroupId 
WHERE   ConfigId = @ConfigId;";
            var result = conn.Execute(sql, new
            {
                config.GroupId,
                config.CooperateId,
                config.ConfigId,
                config.Name,
                config.EffectiveDay,
                config.AtLeastNum,
                config.AtMostNum,
                config.SettlementMethod,
                config.GenerateType,
                config.Description,
                config.IsActive,
                AuditStatus = AudioStatus.Pending.ToString(),
            }, commandType: CommandType.Text);
            return result > 0;
        }

        public static bool DeleteRedemptionCodeConfig(SqlConnection conn, Guid configId)
        {
            const string sql = @"DELETE  FROM Tuhu_groupon..RedemptionConfig
WHERE   ConfigId = @ConfigId;";
            return conn.Execute(sql, new { ConfigId = configId }, commandType: System.Data.CommandType.Text) > 0;
        }

        #endregion

        #region RedeemMrCodeConfig

        public static RedeemMrCodeConfig SelectRedeemMrCodeConfig(SqlConnection conn, int mrCodeConfigId)
        {
            const string sql = @"SELECT  PKID ,
        RedemptionConfigId ,
        Name ,
        SettlementMethod ,
        SettlementPrice ,
        ShopCommission ,
        Num ,
        StartTime ,
        EndTime ,
        EffectiveDay ,
        CodeTypeConfigId ,
        IsRequired ,
        IsActive ,
        Description ,
        CreateTime ,
        UpdateTime
FROM    Tuhu_groupon..RedeemMrCodeConfig AS t WITH ( NOLOCK )
WHERE   t.PKID = @PKID;";
            return conn.Query<RedeemMrCodeConfig>(sql, new { PKID = mrCodeConfigId }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static bool UpdateRedeemMrCodeConfig(SqlConnection conn, RedeemMrCodeConfig config)
        {
            const string sql = @"UPDATE  Tuhu_groupon..RedeemMrCodeConfig
SET     Name = @Name ,
        SettlementMethod = @SettlementMethod ,
        SettlementPrice = @SettlementPrice ,
        ShopCommission = @ShopCommission ,
        Num = @Num ,
        StartTime = @StartTime ,
        EndTime = @EndTime ,
        EffectiveDay = @EffectiveDay ,
        CodeTypeConfigId = @CodeTypeConfigId ,
        IsRequired = @IsRequired ,
        IsActive = @IsActive ,
        Description = @Description ,
        UpdateTime = GETDATE()
WHERE   PKID = @PKID;";
            return conn.Execute(sql, new
            {
                config.PKID,
                config.RedemptionConfigId,
                config.Name,
                config.SettlementMethod,
                config.SettlementPrice,
                config.ShopCommission,
                config.Num,
                config.StartTime,
                config.EndTime,
                config.EffectiveDay,
                config.CodeTypeConfigId,
                config.IsRequired,
                config.IsActive,
                config.Description
            }, commandType: CommandType.Text) > 0;
        }

        public static int AddRedeemMrCodeConfig(SqlConnection conn, RedeemMrCodeConfig config)
        {
            const string sql = @"INSERT  Tuhu_groupon..RedeemMrCodeConfig
        ( RedemptionConfigId ,
          Name ,
          SettlementMethod ,
          SettlementPrice ,
          ShopCommission ,
          Num ,
          StartTime ,
          EndTime ,
          EffectiveDay ,
          CodeTypeConfigId ,
          IsRequired ,
          IsActive ,
          Description ,
          CreateTime ,
          UpdateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @RedemptionConfigId ,
          @Name ,
          @SettlementMethod ,
          @SettlementPrice ,
          @ShopCommission ,
          @Num ,
          @StartTime ,
          @EndTime ,
          @EffectiveDay ,
          @CodeTypeConfigId ,
          @IsRequired ,
          @IsActive ,
          @Description ,
          GETDATE() ,
          GETDATE()
        );";
            var result = conn.ExecuteScalar(sql, new
            {
                config.RedemptionConfigId,
                config.Name,
                config.SettlementMethod,
                config.SettlementPrice,
                config.ShopCommission,
                config.Num,
                config.StartTime,
                config.EndTime,
                config.EffectiveDay,
                config.CodeTypeConfigId,
                config.IsRequired,
                config.IsActive,
                config.Description
            }, commandType: CommandType.Text);
            return Convert.ToInt32(result);
        }

        public static bool DeleteRedeemMrCodeConfig(SqlConnection conn, int id)
        {
            const string sql = @"DELETE  FROM Tuhu_groupon..RedeemMrCodeConfig WHERE   PKID = @PKID;";
            return conn.Execute(sql, new { PKID = id }, commandType: CommandType.Text) > 0;
        }


        public static List<RedeemMrCodeConfig> SelectRedeemMrCodeConfigs(SqlConnection conn, Guid configId)
        {
            const string sql = @"SELECT  t.PKID ,
        t.RedemptionConfigId ,
        t.Name ,
        t.SettlementMethod ,
        t.SettlementPrice ,
        t.ShopCommission ,
        t.Num ,
        t.StartTime ,
        t.EndTime ,
        t.EffectiveDay ,
        t.CodeTypeConfigId ,
        t.IsRequired ,
        t.IsActive ,
        t.Description ,
        cc.PID ,
        cc.Name AS ServiceName
FROM    Tuhu_groupon..RedeemMrCodeConfig AS t WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..BeautyServiceCodeTypeConfig AS cc WITH ( NOLOCK ) ON cc.PKID = t.CodeTypeConfigId
WHERE   t.RedemptionConfigId = @RedemptionConfigId;";
            return conn.Query<RedeemMrCodeConfig>(sql, new { RedemptionConfigId = configId }, commandType: CommandType.Text)?.ToList() ?? new List<RedeemMrCodeConfig>();
        }

        public static bool IsExistsRedeemMrCodeConfig(SqlConnection conn, RedeemMrCodeConfig config)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    Tuhu_groupon..RedeemMrCodeConfig AS t WITH ( NOLOCK )
WHERE   t.RedemptionConfigId = @RedemptionConfigId
        AND t.PKID <> @PKID
        AND t.Name = @Name; ";
            var count = Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                config.RedemptionConfigId,
                config.Name,
                config.PKID
            }, commandType: CommandType.Text));
            return count > 0;
        }

        #region RedeemMrCodeLimitConfig

        public static RedeemMrCodeLimitConfig SelectRedeemMrCodeLimitConfig(SqlConnection conn, int mrCodeConfigId)
        {
            const string sql = @"SELECT  t.PKID ,
        t.MrCodeConfigId ,
        t.CycleType ,
        t.CycleLimit ,
        t.ProvinceIds ,
        t.CityIds ,
        t.CreateUser ,
        t.CreateTime ,
        t.UpdateTime
FROM    Tuhu_groupon..RedeemMrCodeLimitConfig AS t WITH ( NOLOCK )
WHERE   MrCodeConfigId = @MrCodeConfigId;";
            return conn.Query<RedeemMrCodeLimitConfig>(sql, new
            {
                MrCodeConfigId = mrCodeConfigId
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static bool DeleteRedeemMrCodeLimitConfig(SqlConnection conn, int mrCodeConfigId)
        {
            const string sql = @"DELETE  FROM Tuhu_groupon..RedeemMrCodeLimitConfig
WHERE   MrCodeConfigId = @MrCodeConfigId;";
            return conn.Execute(sql, new
            {
                MrCodeConfigId = mrCodeConfigId
            }, commandType: CommandType.Text) > 0;
        }

        public static bool UpdateRedeemMrCodeLimitConfig(SqlConnection conn, RedeemMrCodeLimitConfig config)
        {
            const string sql = @"IF EXISTS ( SELECT  1
            FROM    Tuhu_groupon..RedeemMrCodeLimitConfig AS t WITH ( NOLOCK )
            WHERE   t.MrCodeConfigId = @MrCodeConfigId )
    BEGIN
        UPDATE  Tuhu_groupon..RedeemMrCodeLimitConfig
        SET     CycleType = @CycleType ,
                CycleLimit = @CycleLimit ,
                CityIds = @CityIds ,
                ProvinceIds = @ProvinceIds ,
                UpdateTime = GETDATE() 
        WHERE   MrCodeConfigId=@MrCodeConfigId;
    END;
ELSE
    BEGIN 
        INSERT  INTO Tuhu_groupon..RedeemMrCodeLimitConfig
                ( MrCodeConfigId ,
                  CycleType ,
                  CycleLimit ,
                  ProvinceIds ,
                  CityIds ,
                  CreateUser ,
                  CreateTime ,
                  UpdateTime
	            )
        VALUES  ( @MrCodeConfigId ,
                  @CycleType ,
                  @CycleLimit ,
                  @ProvinceIds ,
                  @CityIds ,
                  @CreateUser ,
                  GETDATE() ,
                  GETDATE()
                );
    END;";
            return conn.Execute(sql, new
            {
                config.CityIds,
                config.ProvinceIds,
                config.MrCodeConfigId,
                config.CycleType,
                config.CycleLimit,
                config.CreateUser,
            }, commandType: CommandType.Text) > 0;
        }

        #endregion

        #endregion

        #region PromotionBusinessTypeConfig

        public static PromotionBusinessTypeConfig SelectCouponBusinessConfig(SqlConnection conn, int id)
        {
            const string sql = @"SELECT  t.PKID ,
        t.BusinessType ,
        t.Name ,
        t.GetRuleGuid ,
        t.CreateUser ,
        t.CreateTime ,
        t.UpdateTime
FROM    Tuhu_groupon..PromotionBusinessTypeConfig AS t WITH ( NOLOCK )
WHERE   PKID = @PKID;";
            return conn.Query<PromotionBusinessTypeConfig>(sql, new { PKID = id }, commandType: CommandType.Text).FirstOrDefault();
        }

        public static bool RemoveCouponBusinessConfig(SqlConnection conn, int id)
        {
            const string sql = @"DELETE  FROM Tuhu_groupon..PromotionBusinessTypeConfig
WHERE   PKID = @PKID;";
            return conn.Execute(sql, new { PKID = id }, commandType: CommandType.Text) > 0;
        }

        public static int AddCouponBusinessConfig(SqlConnection conn, PromotionBusinessTypeConfig config)
        {
            const string sql = @"INSERT  INTO Tuhu_groupon..PromotionBusinessTypeConfig
        ( BusinessType ,
          Name ,
          GetRuleGuid ,
          CreateUser ,
          CreateTime ,
          UpdateTime
        )
OUTPUT  Inserted.PKID
VALUES  ( @BusinessType ,
          @Name ,
          @GetRuleGuid ,
          @CreateUser ,
          GETDATE() ,
          GETDATE()
        );";
            var result = conn.ExecuteScalar(sql, new
            {
                config.BusinessType,
                config.Name,
                config.GetRuleGuid,
                config.CreateUser
            }, commandType: CommandType.Text);
            return Convert.ToInt32(result);
        }

        public static IEnumerable<PromotionBusinessTypeConfig> GetCouponBusinessConfigs(SqlConnection conn, string businessType)
        {
            const string sql = @"
SELECT  t.PKID ,
        t.BusinessType ,
        t.Name ,
        t.GetRuleGuid ,
        t.CreateUser ,
        t.CreateTime ,
        t.UpdateTime
FROM    Tuhu_groupon..PromotionBusinessTypeConfig AS t WITH ( NOLOCK )
WHERE   t.BusinessType = @BusinessType
        OR @BusinessType = ''
        OR @BusinessType IS NULL;";
            var parameters = new DynamicParameters();
            parameters.Add("@BusinessType", businessType);
            var list = conn.Query<PromotionBusinessTypeConfig>(sql, parameters, commandType: CommandType.Text);
            return list;
        }

        public static bool IsExistsCouponBusinessConfig(SqlConnection conn, PromotionBusinessTypeConfig config)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    Tuhu_groupon..PromotionBusinessTypeConfig AS t WITH ( NOLOCK )
WHERE   t.PKID <> @PKID
        AND t.BusinessType = @BusinessType
        AND ( t.Name = @Name
              OR t.GetRuleGuid = @GetRuleGuid
            );";
            var count = conn.ExecuteScalar(sql, new
            {
                config.PKID,
                config.Name,
                config.BusinessType,
                config.GetRuleGuid,
            }, commandType: CommandType.Text);
            return Convert.ToInt32(count) > 0;
        }

        #endregion

        #region RedeemPromotionConfig

        public static RedeemPromotionConfig SelectRedeemPromotionConfig(SqlConnection conn, int id)
        {
            const string sql = @"SELECT  c.PKID ,
        c.RedemptionConfigId ,
        c.Name ,
        c.GetRuleGuid ,
        c.BusinessType ,
        c.PackageId ,
        c.Num ,
        c.SettlementMethod ,
        c.SettlementPrice ,
        c.IsRequired ,
        c.IsActive ,
        c.Description ,
        c.CreateTime ,
        c.UpdateTime
FROM    Tuhu_groupon..RedeemPromotionConfig AS c WITH ( NOLOCK )
WHERE   c.PKID = @PKID;";
            var result = conn.Query<RedeemPromotionConfig>(sql, new { PKID = id }, commandType: CommandType.Text).FirstOrDefault();
            return result;
        }

        public static int AddRedeemPromotionConfig(SqlConnection conn, RedeemPromotionConfig config)
        {
            const string sql = @"INSERT  INTO Tuhu_groupon..RedeemPromotionConfig
        ( RedemptionConfigId ,
          Name ,
          GetRuleGuid ,
          BusinessType ,
          PackageId ,
          Num ,
          SettlementMethod ,
          SettlementPrice ,
          IsRequired ,
          IsActive ,
          Description ,
          CreateTime ,
          UpdateTime
        )
VALUES  ( @RedemptionConfigId ,
          @Name ,
          @GetRuleGuid ,
          @BusinessType ,
          @PackageId ,
          @Num ,
          @SettlementMethod ,
          @SettlementPrice ,
          @IsRequired ,
          @IsActive ,
          @Description ,
          GETDATE() ,
          GETDATE()
        );";
            var pkid = conn.Execute(sql, new
            {
                config.PKID,
                config.GetRuleGuid,
                config.BusinessType,
                config.PackageId,
                config.Num,
                config.Name,
                config.SettlementMethod,
                config.SettlementPrice,
                config.IsRequired,
                config.IsActive,
                config.Description,
                config.RedemptionConfigId,
            }, commandType: CommandType.Text);
            return pkid;
        }

        public static bool DeleteRedeemPromotionConfig(SqlConnection conn, RedeemPromotionConfig config)
        {
            const string sql = @"DELETE  FROM Tuhu_groupon..RedeemPromotionConfig WHERE   PKID = @PKID;";
            var count = conn.Execute(sql, new { config.PKID, config.Name }, commandType: CommandType.Text);
            return count > 0;
        }

        public static bool IsExistsRedeemPromotionConfig(SqlConnection conn, RedeemPromotionConfig config)
        {
            const string sql = @"SELECT  COUNT(1)
FROM    Tuhu_groupon..RedeemPromotionConfig AS c WITH ( NOLOCK )
WHERE   c.PKID <> @PKID
        AND c.Name = @Name;";
            var count = conn.ExecuteScalar(sql, new { config.PKID, config.Name }, commandType: CommandType.Text);
            return Convert.ToInt32(count) > 0;
        }

        public static bool UpdateRedeemPromotionConfig(SqlConnection conn, RedeemPromotionConfig config)
        {
            const string sql = @"UPDATE  Tuhu_groupon..RedeemPromotionConfig
SET     Name = @Name ,
        GetRuleGuid = @GetRuleGuid ,
        BusinessType = @BusinessType ,
        PackageId = @PackageId ,
        Num = @Num ,
        SettlementMethod = @SettlementMethod ,
        SettlementPrice = @SettlementPrice ,
        IsRequired = @IsRequired ,
        IsActive = @IsActive ,
        Description = @Description ,
        UpdateTime = GETDATE()
WHERE   PKID = @PKID;";
            var count = conn.Execute(sql, new
            {
                config.PKID,
                config.GetRuleGuid,
                config.BusinessType,
                config.PackageId,
                config.Num,
                config.Name,
                config.SettlementMethod,
                config.SettlementPrice,
                config.IsRequired,
                config.IsActive,
                config.Description
            }, commandType: CommandType.Text);
            return count > 0;
        }

        public static List<RedeemPromotionConfig> SelectRedeemPromotionConfigs(SqlConnection conn, Guid id)
        {
            const string sql = @"SELECT  t.PKID ,
        t.RedemptionConfigId ,
        t.Name ,
        t.GetRuleGuid ,
        t.BusinessType ,
        t.PackageId ,
        t.Num ,
        t.SettlementMethod ,
        t.SettlementPrice ,
        t.IsRequired ,
        t.IsActive ,
        t.Description ,
        t.CreateTime ,
        t.UpdateTime
FROM    Tuhu_groupon..RedeemPromotionConfig AS t WITH ( NOLOCK )
WHERE   RedemptionConfigId = @RedemptionConfigId;";
            var result = conn.Query<RedeemPromotionConfig>(sql, new
            {
                RedemptionConfigId = id,
            }, commandType: CommandType.Text);
            return result?.ToList() ?? new List<RedeemPromotionConfig>();
        }

        #endregion

        #region RedeemCodeRecord

        public static bool AddRedeemCodeRecords(SqlConnection conn, List<RedemptionCodeRecord> records)
        {
            var table = new DataTable("Tuhu_groupon..RedemptionCodeRecord");
            table.Columns.Add("RedemptionCode", typeof(string));
            table.Columns.Add("RedemptionConfigId", typeof(Guid));
            table.Columns.Add("BatchCode", typeof(string));
            table.Columns.Add("CreateUser", typeof(string));
            table.Columns.Add("CreateTime", typeof(DateTime));
            records.ForEach(record =>
            {
                var row = table.NewRow();
                row["RedemptionCode"] = record.RedemptionCode;
                row["RedemptionConfigId"] = record.RedemptionConfigId;
                row["BatchCode"] = record.BatchCode;
                row["CreateUser"] = record.CreateUser;
                row["CreateTime"] = record.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                table.Rows.Add(row);
            });
            using (var sbc = new SqlBulkCopy(conn))
            {
                sbc.DestinationTableName = table.TableName;
                foreach (DataColumn column in table.Columns)
                {
                    sbc.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }
                sbc.WriteToServer(table);
                return true;
            }
        }

        public static Tuple<int, IEnumerable<RedemptionCodeRecordResult>> SelectRedemptionCodeRecords(
            SqlConnection conn, SearchRedemptionConfigRequest request)
        {
            const string sql = @"SELECT  @Total = COUNT(1)
FROM    ( SELECT    t.RedemptionConfigId ,
                    t.BatchCode ,
                    t.CreateUser ,
                    t.CreateTime ,
                    COUNT(1) AS Num
          FROM      Tuhu_groupon..RedemptionCodeRecord AS t WITH ( NOLOCK )
          GROUP BY  t.RedemptionConfigId ,
                    t.BatchCode ,
                    t.CreateUser ,
                    t.CreateTime
        ) AS a
        INNER JOIN Tuhu_groupon..RedemptionConfig AS b WITH ( NOLOCK ) ON a.RedemptionConfigId = b.ConfigId
        INNER JOIN Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK ) ON mcuc.PKID = b.CooperateId
WHERE   b.GenerateType = @GenerateType
        AND ( @SettlementMethod IS NULL
              OR b.SettlementMethod = @SettlementMethod
            )
        AND ( @CooperateId <= 0
              OR b.CooperateId = @CooperateId
            )
        AND ( @CodeTypeConfigId <= 0
              OR EXISTS ( SELECT    1
                          FROM      Tuhu_groupon..RedeemMrCodeConfig AS mr
                                    WITH ( NOLOCK )
                          WHERE     mr.CodeTypeConfigId = @CodeTypeConfigId
                                    AND b.ConfigId = mr.RedemptionConfigId )
            );

SELECT  a.RedemptionConfigId ,
        a.BatchCode ,
        a.CreateUser ,
        a.CreateTime ,
        a.Num ,
        b.PKID AS ConfigId,
        b.CooperateId ,
        b.Name ,
        mcuc.CooperateName ,
        b.EffectiveDay
FROM    ( SELECT    t.RedemptionConfigId ,
                    t.BatchCode ,
                    t.CreateUser ,
                    t.CreateTime ,
                    COUNT(1) AS Num
          FROM      Tuhu_groupon..RedemptionCodeRecord AS t WITH ( NOLOCK )
          GROUP BY  t.RedemptionConfigId ,
                    t.BatchCode ,
                    t.CreateUser ,
                    t.CreateTime
        ) AS a
        INNER JOIN Tuhu_groupon..RedemptionConfig AS b WITH ( NOLOCK ) ON a.RedemptionConfigId = b.ConfigId
        INNER JOIN Tuhu_groupon..MrCooperateUserConfig AS mcuc WITH ( NOLOCK ) ON mcuc.PKID = b.CooperateId
WHERE   GenerateType = @GenerateType
        AND ( @SettlementMethod IS NULL
              OR b.SettlementMethod = @SettlementMethod
            )
        AND ( @CooperateId <= 0
              OR b.CooperateId = @CooperateId
            )
        AND ( @CodeTypeConfigId <= 0
              OR EXISTS ( SELECT    1
                          FROM      Tuhu_groupon..RedeemMrCodeConfig AS mr
                                    WITH ( NOLOCK )
                          WHERE     mr.CodeTypeConfigId = @CodeTypeConfigId
                                    AND b.ConfigId = mr.RedemptionConfigId )
            )
ORDER BY a.CreateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            var parameters = new DynamicParameters();

            parameters.Add("@GenerateType", request.GenerateType);
            parameters.Add("@CooperateId", request.CooperateId);
            parameters.Add("@CodeTypeConfigId", request.CodeTypeConfigId);
            parameters.Add("@SettlementMethod", request.SettlementMethod);
            parameters.Add("@PageIndex", request.PageIndex);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@Total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var list = conn.Query<RedemptionCodeRecordResult>(sql, parameters, commandType: CommandType.Text);
            var total = parameters.Get<int>("@Total");
            return Tuple.Create(total, list);
        }

        public static IEnumerable<T> SelectRedemptionBatchStatus<T>(SqlConnection conn, IEnumerable<string> batchCodes, T obj = default(T))
        {
            string sql = @"SELECT DISTINCT
        R.BatchCode ,
        R.Status
FROM    Tuhu_groupon..RedemptionCode AS R WITH ( NOLOCK )
        JOIN ( SELECT   A.BatchCode
               FROM     Tuhu_groupon..RedemptionCode AS A WITH ( NOLOCK )
                        JOIN Tuhu_groupon..SplitString(@BatchCodes, ',', 1) AS B ON B.Item = A.BatchCode
               GROUP BY A.BatchCode
               HAVING   COUNT(DISTINCT A.Status) = 1
             ) AS T ON T.BatchCode = R.BatchCode;";
            return conn.Query<T>(sql, new { BatchCodes = string.Join(",", batchCodes) }, commandType: CommandType.Text);
        }

        public static List<RedemptionCodeRecord> SelectRedemptionCodeRecords(SqlConnection conn, string batchCode)
        {
            const string sql = @"SELECT  t.PKID ,
        t.RedemptionCode ,
        t.RedemptionConfigId ,
        t.BatchCode ,
        t.UserId ,
        t.MobileNum ,
        t.OrderId ,
        t.CreateUser ,
        t.CreateTime ,
        t.UpdateTime ,
        c.EffectiveDay
FROM    Tuhu_groupon..RedemptionCodeRecord AS t WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..RedemptionConfig AS c WITH ( NOLOCK ) ON t.RedemptionConfigId = c.ConfigId
WHERE   t.BatchCode = @BatchCode;";
            var list = conn.Query<RedemptionCodeRecord>(sql, new { BatchCode = batchCode }, commandType: CommandType.Text);
            return list?.ToList();
        }
        public static Tuple<List<RedeemGroupSetting>, int> SelectRedeemGroupSetting(SqlConnection conn, int pageIndex, int pageSize, string keyWord)
        {
            string sqlCount = @"SELECT COUNT(1) 
 From Tuhu_groupon..RedeemGroupSetting with(nolock) 
 Where IsDeleted=0 {0}";
            string sql = @" SELECT PKID, 
 GroupId, 
 GroupName,
 CreateTime,
 UpdateTime,
 BusinessType,
 SendCodeType,
AppId 
 From Tuhu_groupon..RedeemGroupSetting with(nolock) 
 Where IsDeleted=0 {0}
 order by CreateTime Desc
 offset @begin rows Fetch next @pageSize rows only";
            var result = new List<RedeemGroupSetting>();
            string whereCon = "";
            if (!string.IsNullOrEmpty(keyWord))
            {
                whereCon = " AND GroupName Like @GroupName ";
            }
            sqlCount = string.Format(sqlCount, whereCon);
            sql = string.Format(sql, whereCon);
            int count = Convert.ToInt32(conn.ExecuteScalar(sqlCount, new { GroupName = $"{keyWord}%" }));
            if (count > 0)
                result = conn.Query<RedeemGroupSetting>(sql, new { begin = (pageIndex - 1) * pageSize, pageSize = pageSize, GroupName = $"{keyWord}%" }, commandType: CommandType.Text)?.ToList();
            return Tuple.Create(result, count); ;
        }
        public static bool InsertRedeemGroupSetting(SqlConnection conn, RedeemGroupSetting model)
        {
            const string sql = @" Insert into Tuhu_groupon..RedeemGroupSetting(
GroupId, 
GroupName,
BusinessType,
SendCodeType,
 CreateTime,
 UpdateTime,
AppId)
values(
@GroupId, 
@GroupName,
@BusinessType,
@SendCodeType,
 GETDATE(),
 GETDATE(),
@AppId
)";
            return conn.Execute(
                    sql,
                    new
                    {
                        GroupId = model.GroupId,
                        GroupName = model.GroupName,
                        BusinessType = model.BusinessType,
                        SendCodeType = model.SendCodeType,
                        AppId = model.AppId
                    },
                    commandType: CommandType.Text) > 0;
        }
        public static bool UpdateRedeemGroupSetting(SqlConnection conn, RedeemGroupSetting model)
        {
            const string sql = @" Update  Tuhu_groupon..RedeemGroupSetting
 SET GroupName=@GroupName,
BusinessType=@BusinessType,
SendCodeType=@SendCodeType,
 UpdateTime=GETDATE(),
AppId=@AppId 
 WHERE GroupId=@GroupId";
            return conn.Execute(
                sql,
                new
                {
                    GroupName = model.GroupName,
                    GroupId = model.GroupId,
                    BusinessType = model.BusinessType,
                    SendCodeType = model.SendCodeType,
                    AppId = model.AppId
                },
            commandType: CommandType.Text) > 0;
        }

        public static bool DeleteRedeemGroupSetting(SqlConnection conn, RedeemGroupSetting model)
        {
            const string sql = @" Update  Tuhu_groupon..RedeemGroupSetting
 SET IsDeleted=1, 
 UpdateTime=GETDATE()
 WHERE GroupId=@GroupId";
            return conn.Execute(
                sql,
                new
                {
                    GroupId = model.GroupId
                },
            commandType: CommandType.Text) > 0;
        }
        public static int GetRedemptionConfigCountByGroupId(SqlConnection conn, Guid? groupId)
        {
            const string sql = @" Select Count(1) From   Tuhu_groupon..RedemptionConfig WITH(NOLOCK) 
 WHERE GroupId=@GroupId and IsActive=1";
            return Convert.ToInt32(conn.ExecuteScalar(
                sql,
                new
                {
                    GroupId = groupId
                },
            commandType: CommandType.Text));
        }
        public static RedeemGroupSetting GetRedeemGroupSetting(SqlConnection conn, Guid? groupId)
        {
            const string sql = @" SELECT top 1 PKID, 
 GroupId, 
 GroupName,
 CreateTime,
 UpdateTime,
 BusinessType,
 SendCodeType,
 AppId 
 From Tuhu_groupon..RedeemGroupSetting with(nolock) 
 Where GroupId=@GroupId";


            var result = conn.QueryFirstOrDefault<RedeemGroupSetting>(sql, new { GroupId = groupId }, commandType: CommandType.Text);
            return result;
        }

        public static IEnumerable<OpenAppModel> GetAllBigCustomerOpenAppIds(SqlConnection conn, string channel)
        {
            const string sql = @" SELECT [Id]
      ,[AppId]
      ,[AppName]
      ,[CompanyId]
      ,[CompanyUserId]
  FROM [Gungnir].[dbo].[tbl_OpenApp] with(nolock) 
  WHERE OrderChannel=@OrderChannel 
  ORDER BY CreateTime DESC";


            var result = conn.Query<OpenAppModel>(sql, new { OrderChannel = channel }, commandType: CommandType.Text);
            return result;
        }

        public static Tuple<List<OpenAppModel>, int> GetThirdAppSettings(SqlConnection conn, int pageIndex, int pageSize, string keyWord)
        {
            string sqlCount = @"SELECT COUNT(1) 
 From [Gungnir].[dbo].[tbl_OpenApp] with(nolock) 
 Where IsDeleted=0 {0}";
            string sql = @" SELECT Id, 
 AppId, 
 AppName,
 OrderChannel,
 CompanyId,
 CompanyUserId,
 CreateTime 
From [Gungnir].[dbo].[tbl_OpenApp] with(nolock) 
 Where IsDeleted=0 {0}
 order by CreateTime Desc
 offset @begin rows Fetch next @pageSize rows only";
            var result = new List<OpenAppModel>();
            string whereCon = "";
            if (!string.IsNullOrEmpty(keyWord))
            {
                whereCon = " AND AppName Like @AppName ";
            }
            sqlCount = string.Format(sqlCount, whereCon);
            sql = string.Format(sql, whereCon);
            int count = Convert.ToInt32(conn.ExecuteScalar(sqlCount, new { AppName = $"{keyWord}%" }));
            if (count > 0)
                result = conn.Query<OpenAppModel>(sql, new { begin = (pageIndex - 1) * pageSize, pageSize = pageSize, AppName = $"{keyWord}%" }, commandType: CommandType.Text)?.ToList();
            return Tuple.Create(result, count); ;
        }
        public static OpenAppModel GetThirdAppSettingsDetail(SqlConnection conn, int id)
        {
            string sql = @" SELECT * 
From [Gungnir].[dbo].[tbl_OpenApp] with(nolock) 
 Where Id=@Id";
            var result = conn.QueryFirstOrDefault<OpenAppModel>(sql, new { Id = id }, commandType: CommandType.Text);
            return result;
        }
        public static OpenAppModel GetThirdAppSettingsDetailByAppId(SqlConnection conn, string appId)
        {
            string sql = @" SELECT * 
From [Gungnir].[dbo].[tbl_OpenApp] with(nolock) 
 Where appId=@appId";
            var result = conn.QueryFirstOrDefault<OpenAppModel>(sql, new { appId = appId }, commandType: CommandType.Text);
            return result;
        }
        public static bool InsertThirdAppSetting(SqlConnection conn, OpenAppModel model)
        {
            string sql = @" Insert into [Gungnir].[dbo].[tbl_OpenApp](
  [AppId]
 ,[AppName]
 ,[AppSecret]
 ,[OrderChannel]
 ,[CreateTime]
 ,[UpdateTime]
 ,[IsDeleted]
 ,[InputCharset]
 ,[SignType]
 ,[EncryptType]
 ,[PrivateKey]
 ,[BigCustomerUrl]
 ,[CompanyId]
 ,[CompanyUserId]
 ,[BigCustomerStatus]
  )
  values(
  @AppId
 ,@AppName
 ,@AppSecret
 ,@OrderChannel
 ,GetDate()
 ,GetDate()
 ,0
 ,@InputCharset
 ,@SignType
 ,@EncryptType
 ,@PrivateKey
 ,@BigCustomerUrl
 ,@CompanyId
 ,@CompanyUserId
 ,@BigCustomerStatus
 )";
            var result = conn.Execute(sql, new
            {
                AppId = model.AppId
                ,
                AppName = model.AppName
                ,
                AppSecret = model.AppSecret
                ,
                OrderChannel = model.OrderChannel
                ,
                InputCharset = string.IsNullOrEmpty(model.InputCharset) ? "utf-8" : model.InputCharset
                ,
                SignType = model.SignType
                ,
                EncryptType = model.EncryptType
                ,
                PrivateKey = model.PrivateKey
                ,
                BigCustomerUrl = model.BigCustomerUrl
                ,
                BigCustomerStatus = model.BigCustomerStatus
                ,
                CompanyId = model.CompanyId
                ,
                CompanyUserId = model.CompanyUserId
            }, commandType: CommandType.Text);
            return result > 0;
        }
        public static bool UpdateThirdAppSetting(SqlConnection conn, OpenAppModel model)
        {
            string sql = @" Update [Gungnir].[dbo].[tbl_OpenApp]
SET  [AppId]=@AppId
 ,[AppName]=@AppName
 ,[AppSecret]=@AppSecret
 ,[OrderChannel]=@OrderChannel
 ,[UpdateTime]=GetDate()
 ,[InputCharset]=@InputCharset
 ,[SignType]=@SignType
 ,[EncryptType]=@EncryptType
 ,[PrivateKey]=@PrivateKey
 ,[BigCustomerUrl]=@BigCustomerUrl
 ,[CompanyId]=@CompanyId
 ,[CompanyUserId]=@CompanyUserId
 ,[BigCustomerStatus]=@BigCustomerStatus
  WHERE Id=@Id";
            var result = conn.Execute(sql, new
            {
                Id = model.Id
                ,
                AppId = model.AppId
                ,
                AppName = model.AppName
                ,
                AppSecret = model.AppSecret
                ,
                OrderChannel = model.OrderChannel
                ,
                InputCharset = string.IsNullOrEmpty(model.InputCharset) ? "utf-8" : model.InputCharset
                ,
                SignType = model.SignType
                ,
                EncryptType = model.EncryptType
                ,
                PrivateKey = model.PrivateKey
                ,
                BigCustomerUrl = model.BigCustomerUrl
                ,
                BigCustomerStatus = model.BigCustomerStatus
                ,
                CompanyId = model.CompanyId
                ,
                CompanyUserId = model.CompanyUserId
            }, commandType: CommandType.Text);
            return result > 0;
        }
        public static bool DeleteThirdAppSetting(SqlConnection conn, int id)
        {
            string sql = @" UPDATE  
 [Gungnir].[dbo].[tbl_OpenApp] with(rowlock) 
 SET IsDeleted=1 
 Where Id=@Id";
            var result = conn.Execute(sql, new { Id = id }, commandType: CommandType.Text);
            return result > 0;
        }

        public static bool CheckAppIdIsExist(SqlConnection conn, string appId)
        {
            string sql = @"Select Count(1) 
From [Gungnir].[dbo].[tbl_OpenApp] with(rowlock) 
 Where AppId=@AppId";
            var result = conn.ExecuteScalar(sql, new {AppId = appId}, commandType: CommandType.Text);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 根据兑换码查询兑换码信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="redemptionCode"></param>
        /// <returns></returns>
        public static UnivBeautyRedemptionCodeResult FetchRedemptionCodeRecordByRedemptionCode(SqlConnection conn, string redemptionCode)
        {
            string sql = @"SELECT TOP 1
        A.PKID ,
        A.UserId ,
        B.PKID AS ConfigId ,
        B.ConfigId AS ConfigGuid ,
        A.MobileNum AS Mobile ,
        B.CooperateId ,
        D.CooperateName ,
        A.RedemptionCode ,
        C.StartTime ,
        C.EndTime ,
        C.RedeemedTime AS ExchangeTime ,
        A.CreateTime ,
        A.CreateUser ,
        'RedemptionCodeRecord' AS RedemptionCodeType ,
        C.Status ,
        C.BatchCode
FROM    Tuhu_groupon..RedemptionCodeRecord A WITH ( NOLOCK )
        JOIN Tuhu_groupon..RedemptionConfig B WITH ( NOLOCK ) ON A.RedemptionConfigId = B.ConfigId
        JOIN Tuhu_groupon..RedemptionCode C WITH ( NOLOCK ) ON A.RedemptionCode = C.Code
        JOIN Tuhu_groupon..MrCooperateUserConfig D WITH ( NOLOCK ) ON B.CooperateId = D.PKID
WHERE   A.RedemptionCode = @RedemptionCode;";

            return conn.QueryFirstOrDefault<UnivBeautyRedemptionCodeResult>(sql, new { RedemptionCode = redemptionCode }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 根据手机号分页查询兑换码
        /// </summary>
        /// <returns></returns>
        public static Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>, int> SelectRedemptionCodeRecordsByUserId(SqlConnection conn, Guid userId, int pageIndex, int pageSize)
        {
            string sql = @"SELECT  @Total = COUNT(1)
FROM    Tuhu_groupon..RedemptionCodeRecord A WITH ( NOLOCK )
        JOIN Tuhu_groupon..RedemptionConfig B WITH ( NOLOCK ) ON A.RedemptionConfigId = B.ConfigId
        JOIN Tuhu_groupon..RedemptionCode C WITH ( NOLOCK ) ON A.RedemptionCode = C.Code
        JOIN Tuhu_groupon..MrCooperateUserConfig D WITH ( NOLOCK ) ON B.CooperateId = D.PKID
WHERE   A.UserId = @UserId;
SELECT  A.PKID ,
        A.UserId ,
        B.PKID AS ConfigId ,
        B.ConfigId AS ConfigGuid ,
        A.MobileNum AS Mobile ,
        B.CooperateId ,
        D.CooperateName ,
        A.RedemptionCode ,
        C.StartTime ,
        C.EndTime ,
        C.RedeemedTime AS ExchangeTime ,
        A.CreateTime ,
        A.CreateUser ,
        'RedemptionCodeRecord' AS RedemptionCodeType ,
        C.Status ,
        C.BatchCode
FROM    Tuhu_groupon..RedemptionCodeRecord A WITH ( NOLOCK )
        JOIN Tuhu_groupon..RedemptionConfig B WITH ( NOLOCK ) ON A.RedemptionConfigId = B.ConfigId
        JOIN Tuhu_groupon..RedemptionCode C WITH ( NOLOCK ) ON A.RedemptionCode = C.Code
        JOIN Tuhu_groupon..MrCooperateUserConfig D WITH ( NOLOCK ) ON B.CooperateId = D.PKID
WHERE   A.UserId = @UserId
ORDER BY A.CreateTime DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PageIndex", pageIndex);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@Total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            var list = conn.Query<UnivBeautyRedemptionCodeResult>(sql, parameters, commandType: CommandType.Text);
            int total = parameters.Get<int>("@Total");
            return new Tuple<IEnumerable<UnivBeautyRedemptionCodeResult>, int>(list ?? new List<UnivBeautyRedemptionCodeResult>(), total);
        }
        /// <summary>
        /// 查询兑换服务码信息
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="serviceCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<GeneralBeautyServerCodes>> GetRedeemMrServerCodes(string phone, string serviceCode, Guid userId, int pageIndex, int pageSize)
        {
            const string sql = @"
            SELECT 
            rcr.MobileNum AS Phone ,
            mcr.ServiceCode ,
            rcr.RedemptionCode ,
            rmc.SettlementPrice ,
            rmc.ShopCommission ,
            sctc.PID ,
            sctc.AdapterVehicle ,
            rmc.SettlementMethod ,
            rc.ConfigId AS PackageId ,
            muc.CooperateName
     FROM   Tuhu_groupon..RedemptionCodeRecord AS rcr WITH ( NOLOCK )
            LEFT JOIN Tuhu_groupon..RedeemMrCodeRecord AS mcr WITH ( NOLOCK ) ON mcr.RedemptionCode = rcr.RedemptionCode
            LEFT JOIN Tuhu_groupon..RedeemMrCodeConfig AS rmc WITH ( NOLOCK ) ON rmc.PKID = mcr.MrCodeConfigId
            LEFT JOIN Tuhu_groupon..BeautyServiceCodeTypeConfig AS sctc WITH ( NOLOCK ) ON rmc.CodeTypeConfigId = sctc.PKID
            LEFT JOIN Tuhu_groupon..RedemptionConfig AS rc WITH ( NOLOCK ) ON rcr.RedemptionConfigId = rc.ConfigId
            LEFT JOIN Tuhu_groupon..MrCooperateUserConfig AS muc WITH ( NOLOCK ) ON rc.CooperateId = muc.PKID
     WHERE  ( @Mobile = ''
              OR @Mobile IS NULL
              OR rcr.MobileNum = @Mobile
            )
            AND ( @UserId = '00000000-0000-0000-0000-000000000000'
                  OR @UserId IS NULL
                  OR rcr.UserId = @UserId
                )
            AND ( @ServiceCode = ''
                  OR @ServiceCode IS NULL
                  OR mcr.ServiceCode = @ServiceCode
                )
     ORDER BY rcr.PKID DESC
            OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
            ONLY;";
            var parameters = new[]
            {
                new SqlParameter("@Mobile", phone),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ServiceCode", serviceCode),
                new SqlParameter("@PageIndex",pageIndex),
                new SqlParameter("@PageSize",pageSize)
            };
            using (var dbHelper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["Tuhu_Groupon_AlwaysOnRead"].ConnectionString))
            {
                return (await dbHelper.ExecuteDataTableAsync(sql, CommandType.Text, parameters))?.ConvertTo<GeneralBeautyServerCodes>()?.ToList();
            }
        }
        #endregion
    }
}
