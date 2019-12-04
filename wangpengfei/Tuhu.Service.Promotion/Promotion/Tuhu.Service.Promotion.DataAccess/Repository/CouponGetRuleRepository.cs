using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 领取规则 仓储
    /// </summary>
    public interface ICouponGetRuleRepository
    {
        ValueTask<CouponGetRuleEntity> GetCouponGetRuleByGuidAsync(Guid getRuleId, CancellationToken cancellationToken);
        /// <summary>
        /// Create 优惠券领取规则
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationTok"></param>
        /// <returns></returns>
        ValueTask<int> CreateAsync(CouponGetRuleEntity entity, CancellationToken cancellationToken);
        ValueTask<bool> UpdateAsync(CouponGetRuleEntity entity, CancellationToken cancellationToken);
        ValueTask<CouponGetRuleEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);

        ValueTask<CouponDepartmentUseSettingEntity> GetCouponDepartmentUseSettingByPKIDAsync(int PKID, CancellationToken cancellationToken);
        ValueTask<PromotionBusinessLineConfigEntity> GetPromotionBusinessLineConfigByPKIDAsync(int PKID, CancellationToken cancellationToken);

        ValueTask<List<PromotionBusinessLineConfigEntity>> GetPromotionBusinessLineConfigListAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 分页查询领取规则 数目
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> GetCouponRuleCountAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken);
        /// <summary>
        /// 分页查询领取规则 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<GetCouponRuleListResponse>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken);

    }

    /// <summary>
    /// 优惠券领取规则  仓储
    /// </summary>
    public class CouponGetRuleRepository : ICouponGetRuleRepository
    {
        private string DBName = "Activity..tbl_GetCouponRules";
        private readonly IDbHelperFactory _factory;

        public CouponGetRuleRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 根据guid回去使用规则详情
        /// </summary>
        /// <param name="GetRuleGUID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<CouponGetRuleEntity> GetCouponGetRuleByGuidAsync(Guid GetRuleGUID, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT
                                    GCR.PKID ,
                                    GCR.RuleID ,
                                    GCR.Description ,
                                    GCR.PromtionName AS PromotionName ,
                                    GCR.Discount ,
                                    GCR.Minmoney ,
                                    GCR.ValiStartDate ,
                                    GCR.ValiEndDate ,
                                    GCR.Term ,
                                    GCR.GetRuleGUID ,
                                    GCR.Creater ,
                                    GCR.DepartmentName ,
                                    GCR.IntentionName ,
                                    GCR.DeadLineDate ,
                                    GCR.Quantity ,
                                    GCR.GetQuantity ,
                                    GCR.SingleQuantity ,
                                    GCR.SupportUserRange,
                                    CR.RuleDescription,
                                    CR.PromotionType,
                                    CR.InstallType,
                                    CR.OrderPayMethod,
                                    CR.EnabledGroupBuy
                            FROM    Activity..tbl_GetCouponRules AS GCR WITH ( NOLOCK )
                                    LEFT JOIN Activity..tbl_CouponRules AS CR WITH ( NOLOCK ) ON GCR.RuleID = CR.PKID
                            WHERE   GCR.GetRuleGUID = @GetRuleGUID;";
            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@GetRuleGUID", GetRuleGUID));
                    var result = (await dbHelper.ExecuteFetchAsync<CouponGetRuleEntity>(cmd, cancellationToken).ConfigureAwait(false)) ?? new CouponGetRuleEntity();
                    return result;
                }
            }

        }

        #region 增删改查
        /// <summary>
        /// Create 优惠券领取规则
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationTok"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(CouponGetRuleEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   (
                                   [RuleID]
                                   ,[Description]
                                   ,[PromtionName]
                                   ,[Discount]
                                   ,[Minmoney]
                                   ,[AllowChanel]
                                   ,[Term]
                                   ,[ValiStartDate]
                                   ,[ValiEndDate]
                                   ,[Quantity]
                                   ,[GetQuantity]
                                   ,[SingleQuantity]
                                   ,[DXStartDate]
                                   ,[DXEndDate]
                                   ,[GetRuleGUID]
                                   ,[Channel]
                                   ,[SupportUserRange]
                                   ,[DetailShowStartDate]
                                   ,[DetailShowEndDate]
                                   ,[DepartmentId]
                                   ,[IntentionId]
                                   ,[Creater]
                                   ,[DepartmentName]
                                   ,[IntentionName]
                                   ,[CouponType]
                                   ,[DeadLineDate]
                                   ,[IsPush]
                                   ,[PushSetting]
                                   ,[BusinessLineId]
                                   ,[BusinessLineName]
                                   ,[RemindQuantity]
                                   ,[RemindEmails]
                                    )
                             VALUES
                                   (
                                    @RuleID
                                   ,@Description
                                   ,@PromtionName
                                   ,@Discount
                                   ,@Minmoney
                                   ,@AllowChanel
                                   ,@Term
                                   ,@ValiStartDate
                                   ,@ValiEndDate
                                   ,@Quantity
                                   ,@GetQuantity
                                   ,@SingleQuantity
                                   ,@DXStartDate
                                   ,@DXEndDate
                                   ,@GetRuleGUID
                                   ,@Channel
                                   ,@SupportUserRange
                                   ,@DetailShowStartDate
                                   ,@DetailShowEndDate
                                   ,@DepartmentId
                                   ,@IntentionId
                                   ,@Creater
                                   ,@DepartmentName
                                   ,@IntentionName
                                   ,@CouponType
                                   ,@DeadLineDate
                                   ,@IsPush
                                   ,@PushSetting
                                   ,@BusinessLineId
                                   ,@BusinessLineName
                                   ,@RemindQuantity
                                   ,@RemindEmails
                                );
                                select  @@IDENTITY ;";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@RuleID", entity.RuleID));
                    cmd.Parameters.Add(new SqlParameter("@Description", entity.Description));
                    cmd.Parameters.Add(new SqlParameter("@PromtionName", entity.PromtionName));
                    cmd.Parameters.Add(new SqlParameter("@Discount", entity.Discount));
                    cmd.Parameters.Add(new SqlParameter("@Minmoney", entity.Minmoney));
                    cmd.Parameters.Add(new SqlParameter("@AllowChanel", entity.AllowChanel));
                    cmd.Parameters.Add(new SqlParameter("@Term", entity.Term));
                    cmd.Parameters.Add(new SqlParameter("@ValiStartDate", entity.ValiStartDate));
                    cmd.Parameters.Add(new SqlParameter("@ValiEndDate", entity.ValiEndDate));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", entity.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@GetQuantity", entity.GetQuantity));
                    cmd.Parameters.Add(new SqlParameter("@SingleQuantity", entity.SingleQuantity));
                    cmd.Parameters.Add(new SqlParameter("@DXStartDate", entity.DXStartDate));
                    cmd.Parameters.Add(new SqlParameter("@DXEndDate", entity.DXEndDate));
                    cmd.Parameters.Add(new SqlParameter("@GetRuleGUID", entity.GetRuleGUID));
                    cmd.Parameters.Add(new SqlParameter("@Channel", entity.Channel));
                    cmd.Parameters.Add(new SqlParameter("@SupportUserRange", entity.SupportUserRange));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowStartDate", entity.DetailShowStartDate));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowEndDate", entity.DetailShowEndDate));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", entity.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@IntentionId", entity.IntentionId));
                    cmd.Parameters.Add(new SqlParameter("@Creater", entity.Creater));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentName", entity.DepartmentName));
                    cmd.Parameters.Add(new SqlParameter("@IntentionName", entity.IntentionName));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));
                    cmd.Parameters.Add(new SqlParameter("@DeadLineDate", entity.DeadLineDate));
                    cmd.Parameters.Add(new SqlParameter("@IsPush", entity.IsPush));
                    cmd.Parameters.Add(new SqlParameter("@PushSetting", entity.PushSetting));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineId", entity.BusinessLineId));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineName", entity.BusinessLineName));
                    cmd.Parameters.Add(new SqlParameter("@RemindQuantity", entity.RemindQuantity));
                    cmd.Parameters.Add(new SqlParameter("@RemindEmails", entity.RemindEmails));
                    var result = (await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return Convert.ToInt32(result);
                }
            }
        }



        /// <summary>
        /// Update 优惠券领取规则
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdateAsync(CouponGetRuleEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"UPDATE {DBName}
                               SET          
                                   
                                   [Description] = @Description
                                  --,[RuleID] = @RuleID
                                  ,[PromtionName] = @PromtionName
                                  ,[Discount] = @Discount
                                  ,[Minmoney] = @Minmoney
                                  ,[AllowChanel] = @AllowChanel
                                  ,[Term] = @Term
                                  ,[ValiStartDate] = @ValiStartDate
                                  ,[ValiEndDate] = @ValiEndDate
                                  ,[Quantity] = @Quantity
                                  --,[GetQuantity] = @GetQuantity
                                  ,[LastDateTime] = getdate()
                                  ,[SingleQuantity] = @SingleQuantity
                                  ,[DXStartDate] = @DXStartDate
                                  ,[DXEndDate] = @DXEndDate
                                  --,[GetRuleGUID] = @GetRuleGUID
                                  ,[Channel] = @Channel
                                  ,[SupportUserRange] = @SupportUserRange
                                  ,[DetailShowStartDate] = @DetailShowStartDate
                                  ,[DetailShowEndDate] = @DetailShowEndDate
                                  ,[DepartmentId] = @DepartmentId
                                  ,[IntentionId] = @IntentionId
                                  --,[Creater] = @Creater
                                  ,[DepartmentName] = @DepartmentName
                                  ,[IntentionName] = @IntentionName
                                  ,[CouponType] = @CouponType
                                  ,[DeadLineDate] = @DeadLineDate
                                  ,[IsPush] = @IsPush
                                  ,[PushSetting] = @PushSetting
                                  ,[BusinessLineId] = @BusinessLineId
                                  ,[BusinessLineName] = @BusinessLineName
                                  ,[RemindQuantity] = @RemindQuantity
                                  ,[RemindEmails] = @RemindEmails
                                where PKID =@PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", entity.PKID));
                    //cmd.Parameters.Add(new SqlParameter("@RuleID", entity.RuleID));
                    cmd.Parameters.Add(new SqlParameter("@Description", entity.Description));
                    cmd.Parameters.Add(new SqlParameter("@PromtionName", entity.PromtionName));
                    cmd.Parameters.Add(new SqlParameter("@Discount", entity.Discount));
                    cmd.Parameters.Add(new SqlParameter("@Minmoney", entity.Minmoney));
                    cmd.Parameters.Add(new SqlParameter("@AllowChanel", entity.AllowChanel));
                    cmd.Parameters.Add(new SqlParameter("@Term", entity.Term));
                    cmd.Parameters.Add(new SqlParameter("@ValiStartDate", entity.ValiStartDate));
                    cmd.Parameters.Add(new SqlParameter("@ValiEndDate", entity.ValiEndDate));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", entity.Quantity));
                    //cmd.Parameters.Add(new SqlParameter("@GetQuantity", entity.GetQuantity));
                    cmd.Parameters.Add(new SqlParameter("@SingleQuantity", entity.SingleQuantity));
                    cmd.Parameters.Add(new SqlParameter("@DXStartDate", entity.DXStartDate));
                    cmd.Parameters.Add(new SqlParameter("@DXEndDate", entity.DXEndDate));
                    //cmd.Parameters.Add(new SqlParameter("@GetRuleGUID", entity.GetRuleGUID));
                    cmd.Parameters.Add(new SqlParameter("@Channel", entity.Channel));
                    cmd.Parameters.Add(new SqlParameter("@SupportUserRange", entity.SupportUserRange));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowStartDate", entity.DetailShowStartDate));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowEndDate", entity.DetailShowEndDate));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", entity.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@IntentionId", entity.IntentionId));
                    //cmd.Parameters.Add(new SqlParameter("@Creater", entity.Creater));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentName", entity.DepartmentName));
                    cmd.Parameters.Add(new SqlParameter("@IntentionName", entity.IntentionName));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));
                    cmd.Parameters.Add(new SqlParameter("@DeadLineDate", entity.DeadLineDate));
                    cmd.Parameters.Add(new SqlParameter("@IsPush", entity.IsPush));
                    cmd.Parameters.Add(new SqlParameter("@PushSetting", entity.PushSetting));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineId", entity.BusinessLineId));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineName", entity.BusinessLineName));
                    cmd.Parameters.Add(new SqlParameter("@RemindQuantity", entity.RemindQuantity));
                    cmd.Parameters.Add(new SqlParameter("@RemindEmails", entity.RemindEmails));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }


        /// <summary>
        /// Get 优惠券领取规则
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<CouponGetRuleEntity> GetByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT       [PKID]
                                      ,[RuleID]
                                      ,[Description]
                                      ,[PromtionName]
                                      ,[Discount]
                                      ,[Minmoney]
                                      ,[AllowChanel]
                                      ,[Term]
                                      ,[ValiStartDate]
                                      ,[ValiEndDate]
                                      ,[Quantity]
                                      ,[GetQuantity]
                                      ,[SingleQuantity]
                                      ,[DXStartDate]
                                      ,[DXEndDate]
                                      ,[GetRuleGUID]
                                      ,[Channel]
                                      ,[SupportUserRange]
                                      ,[DetailShowStartDate]
                                      ,[DetailShowEndDate]
                                      ,[DepartmentId]
                                      ,[IntentionId]
                                      ,[Creater]
                                      ,[DepartmentName]
                                      ,[IntentionName]
                                      ,[CouponType]
                                      ,[DeadLineDate]
                                      ,[IsPush]
                                      ,[PushSetting]
                                      ,[BusinessLineId]
                                      ,[BusinessLineName]
                                      ,[RemindQuantity]
                                      ,[RemindEmails]
                                  FROM  {DBName} with (nolock)
                                  where PKID = @PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));
                    var result = (await dbHelper.ExecuteFetchAsync<CouponGetRuleEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        #endregion

        #region 部门和用途
        /// <summary>
        ///  Get 部门信息 by pkid
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<CouponDepartmentUseSettingEntity> GetCouponDepartmentUseSettingByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                  ,[SettingId]
                                  ,[DisplayName]
                                  ,[ParentSettingId]
                                  ,[Type]
                                  ,[IsDel]
                                  ,[CreateTime]
                                  ,[UpdateTime]
                                  ,[Auditor]
                                 FROM Configuration..CouponDepartmentUseSetting  with (nolock)
                                 where PKID = @PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));
                    var result = (await dbHelper.ExecuteFetchAsync<CouponDepartmentUseSettingEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }


        /// <summary>
        ///  Get 成本归属 by pkid
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<PromotionBusinessLineConfigEntity> GetPromotionBusinessLineConfigByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                  ,[DisplayName]
                                  ,[Creater]
                                  ,[LastUpdater]
                                  ,[IsDel]
                                  ,[UpdateTime]
                                  ,[CreateTime]
                                  ,[Auditor]
                                 FROM Configuration..PromotionBusinessLineConfig with (nolock)
                                 where PKID = @PKID
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", PKID));
                    var result = (await dbHelper.ExecuteFetchAsync<PromotionBusinessLineConfigEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        /// <summary>
        /// 获取所有业务线 [不包括已删除]
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionBusinessLineConfigEntity>> GetPromotionBusinessLineConfigListAsync( CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                  ,[DisplayName]
                                  ,[Creater]
                                  ,[LastUpdater]
                                  ,[IsDel]
                                  ,[UpdateTime]
                                  ,[CreateTime]
                                  ,[Auditor]
                                 FROM Configuration..PromotionBusinessLineConfig  with (nolock)
                                 where IsDel = 0
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionBusinessLineConfigEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        #endregion

        /// <summary>
        /// 分页查询领取规则 数目
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> GetCouponRuleCountAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken)
        {
            Dictionary<string, List<SqlParameter>> query = CreateCondition(request, true);
            string condition = query.Keys.FirstOrDefault();
            #region sql
            var sql = $@"SELECT 
                           Count(1)
                        FROM {DBName} WITH ( NOLOCK )
                        {condition}
                        ";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(query[condition].ToArray());
                    int result = (int)(await dbHelper.ExecuteScalarAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        /// <summary>
        /// 分页查询领取规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleListResponse>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken)
        {
            Dictionary<string, List<SqlParameter>> query = CreateCondition(request, false);
            string condition = query.Keys.FirstOrDefault();

            #region sql
            string sql = $@"SELECT
                                 PKID as GetRuleID
                                ,GetRuleGUID
                                ,PromtionName as PromtionName
                                ,Description as Description
                                FROM  {DBName} with (nolock)
                                {condition}
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(query[condition].ToArray());
                    var result = await dbHelper.ExecuteSelectAsync<GetCouponRuleListResponse>(cmd, cancellationToken).ConfigureAwait(false);
                    return result.ToList();
                }
            }
        }


        /// <summary>
        /// 获取 分页查询数据 参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string, List<SqlParameter>> CreateCondition(GetCouponRuleListRequest request, bool isQueryCount = false)
        {
            Dictionary<string, List<SqlParameter>> dic = new Dictionary<string, List<SqlParameter>>();
            string condition = $" where 1 = 1";
            var sqlParaments = new List<SqlParameter>();

            #region 添加查询条件

            if (request.RuleID > 0)
            {
                condition += $" AND RuleID = @RuleID";
                sqlParaments.Add(new SqlParameter("@RuleID", request.RuleID));
            }

            if (!string.IsNullOrWhiteSpace(request.PromtionName))
            {
                condition += $" AND PromtionName like @PromtionName";
                sqlParaments.Add(new SqlParameter("@PromtionName", "%" + request.PromtionName + "%"));
            }

            if (request.AllowChanel == 1)
            {
                condition += " AND  AllowChanel = @AllowChanel ";
                sqlParaments.Add(new SqlParameter("@AllowChanel", request.AllowChanel));
                if (request.DXDateTime.HasValue)
                {
                    condition += " AND  DXEndDate >= @DXDateTime ";
                    condition += " AND  DXStartDate < @DXDateTime ";
                    sqlParaments.Add(new SqlParameter("@DXDateTime", request.DXDateTime.Value));
                }
            }

            if (request.GetRuleGUID != Guid.Empty)
            {
                condition += "  AND  GetRuleGUID = @GetRuleGUID ";
                sqlParaments.Add(new SqlParameter("@GetRuleGUID", request.GetRuleGUID));
            }

            if (request.SupportUserRange != 0)
            {
                condition += "  AND  SupportUserRange = @SupportUserRange ";
                sqlParaments.Add(new SqlParameter("@SupportUserRange", request.SupportUserRange));
            }
            #endregion

            if (!isQueryCount)
            {
                condition += " ORDER BY PKID DESC ";
                condition += " OFFSET(@PageSize * (@CurrentPage - 1)) ROWS FETCH NEXT @PageSize ROWS ONLY ";
                sqlParaments.Add(new SqlParameter("@CurrentPage", request.CurrentPage));
                sqlParaments.Add(new SqlParameter("@PageSize", request.PageSize));
            }
            dic.Add(condition, sqlParaments);
            return dic;
        }


    }
}



























