using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    public interface ICouponGetRuleAuditRepository
    {
        ValueTask<int> CreateAsync(GetCouponRuleAuditEntity entity, CancellationToken cancellationTok);
        ValueTask<bool> UpdateAsync(GetCouponRuleAuditEntity entity, CancellationToken cancellationToken);
        ValueTask<GetCouponRuleAuditEntity> GetEntityByPKIDAsync(int PKID, CancellationToken cancellationToken);
        ValueTask<List<GetCouponRuleAuditEntity>> GetEntityByWorkOrderIdAsync(int WorkOrderId, CancellationToken cancellationToken);
        ValueTask<List<GetCouponRuleAuditEntity>> GetListByGetCouponRulePKIDAsync(int GetCouponRulePKID, CancellationToken cancellationToken);

        ValueTask<int> GetCouponGetRuleAuditCountAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken);
        ValueTask<List<GetCouponRuleAuditEntity>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 优惠券领取规则 审核 仓储
    /// </summary>
    public class CouponGetRuleAuditRepository : ICouponGetRuleAuditRepository
    {
        private string DBName = "Activity..GetCouponRuleAudit";
        private readonly IDbHelperFactory _factory;
        private readonly ILogger _logger;

        public CouponGetRuleAuditRepository(IDbHelperFactory factory, ILogger<ICouponGetRuleAuditRepository> Logger)
        {
            _factory = factory;
            _logger = Logger;
        } 

        #region 增删改查
        /// <summary>
        /// Create 优惠券审核记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> CreateAsync(GetCouponRuleAuditEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"INSERT INTO {DBName}
                                   ([GetCouponRulePKID]
                                   ,[RuleID]
                                   ,[Description]
                                   ,[PromtionName]
                                   ,Discount
                                   ,Minmoney
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
                                   ,[Applicant]
                                   ,[Auditor]
                                   ,[AuditDateTime]
                                   ,[AuditStatus]
                                   ,[AuditMessage]
                                   ,[WorkOrderId]
                                   ,[CreateDateTime]
                                   ,[LastUpdateDateTime]
                                    )
                             VALUES
                                   (
                                    @GetCouponRulePKID
                                   ,@RuleID
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
                                   ,@Applicant
                                   ,@Auditor
                                   ,getdate()
                                   ,@AuditStatus
                                   ,@AuditMessage
                                   ,@WorkOrderId
                                   ,getdate()
                                   ,getdate()
                                );
                                select SCOPE_IDENTITY();";  //返回pkid
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@GetCouponRulePKID", entity.GetCouponRulePKID));
                    cmd.Parameters.Add(new SqlParameter("@RuleID", entity.RuleID));
                    cmd.Parameters.Add(new SqlParameter("@Description", entity.Description ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@PromtionName", entity.PromtionName ?? ""));
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
                    cmd.Parameters.Add(new SqlParameter("@Channel", entity.Channel ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@SupportUserRange", entity.SupportUserRange));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowStartDate", entity.DetailShowStartDate));
                    cmd.Parameters.Add(new SqlParameter("@DetailShowEndDate", entity.DetailShowEndDate));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentId", entity.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@IntentionId", entity.IntentionId));
                    cmd.Parameters.Add(new SqlParameter("@Creater", entity.Creater ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@DepartmentName", entity.DepartmentName ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@IntentionName", entity.IntentionName ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@CouponType", entity.CouponType));
                    cmd.Parameters.Add(new SqlParameter("@DeadLineDate", entity.DeadLineDate));
                    cmd.Parameters.Add(new SqlParameter("@IsPush", entity.IsPush));
                    cmd.Parameters.Add(new SqlParameter("@PushSetting", entity.PushSetting ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineId", entity.BusinessLineId));
                    cmd.Parameters.Add(new SqlParameter("@BusinessLineName", entity.BusinessLineName ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@RemindQuantity", entity.RemindQuantity));
                    cmd.Parameters.Add(new SqlParameter("@RemindEmails", entity.RemindEmails ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@Applicant", entity.Applicant ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@Auditor", entity.Auditor ?? ""));
                    //cmd.Parameters.Add(new SqlParameter("@AuditDateTime", entity.AuditDateTime));
                    cmd.Parameters.Add(new SqlParameter("@AuditStatus", entity.AuditStatus));
                    cmd.Parameters.Add(new SqlParameter("@AuditMessage", entity.AuditMessage ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@WorkOrderId", entity.WorkOrderId));
                    try
                    {
                        var temp = await dbHelper.ExecuteScalarAsync(cmd,cancellationToken).ConfigureAwait(false);
                        return Convert.ToInt32(temp);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"CouponGetRuleAuditRepository  CreateAsyncException = {JsonConvert.SerializeObject(ex)} & entity= {JsonConvert.SerializeObject(entity)}", ex);
                        return 0;
                    }
                   
                  
                }
            }
        }

        /// <summary>
        ///  Update 优惠券审核记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdateAsync(GetCouponRuleAuditEntity entity, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"UPDATE {DBName}
                               SET [GetCouponRulePKID] = @GetCouponRulePKID              
                                  ,[RuleID] = @RuleID
                                  ,[Description] = @Description
                                  ,[PromtionName] = @PromtionName
                                  ,[Discount] = @Discount
                                  ,[Minmoney] = @Minmoney
                                  ,[AllowChanel] = @AllowChanel
                                  ,[Term] = @Term
                                  ,[ValiStartDate] = @ValiStartDate
                                  ,[ValiEndDate] = @ValiEndDate
                                  ,[Quantity] = @Quantity
                                  ,[GetQuantity] = @GetQuantity
                                  ,[SingleQuantity] = @SingleQuantity
                                  ,[DXStartDate] = @DXStartDate
                                  ,[DXEndDate] = @DXEndDate
                                  ,[GetRuleGUID] = @GetRuleGUID
                                  ,[Channel] = @Channel
                                  ,[SupportUserRange] = @SupportUserRange
                                  ,[DetailShowStartDate] = @DetailShowStartDate
                                  ,[DetailShowEndDate] = @DetailShowEndDate
                                  ,[DepartmentId] = @DepartmentId
                                  ,[IntentionId] = @IntentionId
                                  ,[Creater] = @Creater
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
                                  ,[Applicant] = @Applicant
                                  ,[Auditor] = @Auditor
                                  ,[AuditDateTime] = @AuditDateTime
                                  ,[AuditStatus] = @AuditStatus
                                  ,[AuditMessage] = @AuditMessage
                                  ,[WorkOrderId] = @WorkOrderId
                                  ,[LastUpdateDateTime] = getdate()
                                where PKID =@PKID
                            ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PKID", entity.PKID));
                    cmd.Parameters.Add(new SqlParameter("@GetCouponRulePKID", entity.GetCouponRulePKID));
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
                    cmd.Parameters.Add(new SqlParameter("@Applicant", entity.Applicant));
                    cmd.Parameters.Add(new SqlParameter("@Auditor", entity.Auditor));
                    cmd.Parameters.Add(new SqlParameter("@AuditDateTime", entity.AuditDateTime));
                    cmd.Parameters.Add(new SqlParameter("@AuditStatus", entity.AuditStatus));
                    cmd.Parameters.Add(new SqlParameter("@AuditMessage", entity.AuditMessage??""));
                    cmd.Parameters.Add(new SqlParameter("@WorkOrderId", entity.WorkOrderId));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }

        /// <summary>
        ///  Get 优惠券审核记录 by pkid
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<GetCouponRuleAuditEntity> GetEntityByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                      ,[GetCouponRulePKID]
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
                                      ,[Applicant]
                                      ,[Auditor]
                                      ,[AuditDateTime]
                                      ,[AuditStatus]
                                      ,[AuditMessage]
                                      ,[WorkOrderId]
                                      ,[CreateDateTime]
                                      ,[LastUpdateDateTime]
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
                    var result = (await dbHelper.ExecuteFetchAsync<GetCouponRuleAuditEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result;
                }
            }
        }

        #endregion

        /// <summary>
        ///  Get 优惠券审核记录 by WorkOrderId
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleAuditEntity>> GetEntityByWorkOrderIdAsync(int WorkOrderId, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                      ,[GetCouponRulePKID]
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
                                      ,[Applicant]
                                      ,[Auditor]
                                      ,[AuditDateTime]
                                      ,[AuditStatus]
                                      ,[AuditMessage]
                                      ,[WorkOrderId]
                                      ,[CreateDateTime]
                                      ,[LastUpdateDateTime]
                                  FROM  {DBName} with (nolock)
                                  where WorkOrderId = @WorkOrderId
                                  order by pkid desc
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@WorkOrderId", WorkOrderId));
                    var result = (await dbHelper.ExecuteSelectAsync<GetCouponRuleAuditEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }


        /// <summary>
        ///  Get 优惠券审核记录 by WorkOrderId
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleAuditEntity>> GetListByGetCouponRulePKIDAsync(int GetCouponRulePKID, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PKID]
                                      ,[GetCouponRulePKID]
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
                                      ,[Applicant]
                                      ,[Auditor]
                                      ,[AuditDateTime]
                                      ,[AuditStatus]
                                      ,[AuditMessage]
                                      ,[WorkOrderId]
                                      ,[CreateDateTime]
                                      ,[LastUpdateDateTime]
                                  FROM  {DBName} with (nolock)
                                  where GetCouponRulePKID = @GetCouponRulePKID
                                  order by pkid desc
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Activity", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@GetCouponRulePKID", GetCouponRulePKID));
                    var result = await dbHelper.ExecuteSelectAsync<GetCouponRuleAuditEntity>(cmd, cancellationToken).ConfigureAwait(false);
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 分页获取 数目
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async ValueTask<int> GetCouponGetRuleAuditCountAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken)
        {
            Dictionary<string, List<SqlParameter>> query = CreateCondition(request,true);
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
                    var result = Convert.ToInt32(await dbHelper.ExecuteScalarAsync(cmd).ConfigureAwait(false));
                    return result;
                }
            }
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleAuditEntity>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken)
        {
            Dictionary<string, List<SqlParameter>> query = CreateCondition(request,false);
            string condition = query.Keys.FirstOrDefault();

            #region sql
            string sql = $@"SELECT [PKID]
                                      ,[GetCouponRulePKID]
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
                                      ,[Applicant]
                                      ,[Auditor]
                                      ,[AuditDateTime]
                                      ,[AuditStatus]
                                      ,[AuditMessage]
                                      ,[WorkOrderId]
                                      ,[CreateDateTime]
                                      ,[LastUpdateDateTime]
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
                    var result = await dbHelper.ExecuteSelectAsync<GetCouponRuleAuditEntity>(cmd, cancellationToken).ConfigureAwait(false);
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 获取 分页查询数据 参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string, List<SqlParameter>> CreateCondition(GetCouponGetRuleAuditListRequest request, bool isQueryCount = false)
        {
            Dictionary<string, List<SqlParameter>> dic = new Dictionary<string, List<SqlParameter>>();
            string condition = $" where 1 = 1";
            var sqlParaments = new List<SqlParameter>();

            if (request.RuleID > 0)
            {
                condition += " AND  RuleID = @RuleID ";
                sqlParaments.Add(new SqlParameter("@RuleID", request.RuleID));
            }
            if (request.GetRuleGUID != Guid.Empty)
            {
                condition += "  AND  GetRuleGUID = @GetRuleGUID ";
                sqlParaments.Add(new SqlParameter("@GetRuleGUID", request.GetRuleGUID));
            }
            if (!string.IsNullOrWhiteSpace(request.PromtionName))
            {
                condition += "  AND  PromtionName like @PromtionName ";
                sqlParaments.Add(new SqlParameter("@PromtionName", "%"+ request.PromtionName+"%"));
            }
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                condition += "  AND  Description like @Description ";
                sqlParaments.Add(new SqlParameter("@Description", "%" + request.Description + "%"));
            }
            if (request.SupportUserRange > 0)
            {
                condition += " AND  SupportUserRange = @SupportUserRange ";
                sqlParaments.Add(new SqlParameter("@SupportUserRange", request.SupportUserRange));
            }
            if (request.MinDiscount > 0)
            {
                condition += " AND  Discount >= @MinDiscount ";
                sqlParaments.Add(new SqlParameter("@MinDiscount", request.MinDiscount));
            }
            if (request.MaxDiscount > 0)
            {
                condition += " AND  Discount <= @MaxDiscount ";
                sqlParaments.Add(new SqlParameter("@MaxDiscount", request.MaxDiscount));
            }
            if (request.MinMinmoney > 0)
            {
                condition += " AND Minmoney >= @MinMinmoney ";
                sqlParaments.Add(new SqlParameter("@MinMinmoney", request.MinMinmoney));
            }
            if (request.MaxMinmoney > 0)
            {
                condition += " AND  Minmoney <= @MaxMinmoney ";
                sqlParaments.Add(new SqlParameter("@MaxMinmoney", request.MaxMinmoney));
            }
            if (request.AllowChanel > 0)
            {
                condition += " AND  AllowChanel = @AllowChanel ";
                sqlParaments.Add(new SqlParameter("@AllowChanel", request.AllowChanel));
            }
            if (request.DepartmentId > 0)
            {
                condition += " AND  DepartmentId = @DepartmentId ";
                sqlParaments.Add(new SqlParameter("@DepartmentId", request.DepartmentId));
            }
            if (request.IntentionId > 0)
            {
                condition += " AND  IntentionId = @IntentionId ";
                sqlParaments.Add(new SqlParameter("@IntentionId", request.IntentionId));
            }

            if (request.AuditStatus>0)
            {
                condition += " AND  AuditStatus = @AuditStatus ";
                sqlParaments.Add(new SqlParameter("@IntentionId", request.IntentionId));
            }

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
