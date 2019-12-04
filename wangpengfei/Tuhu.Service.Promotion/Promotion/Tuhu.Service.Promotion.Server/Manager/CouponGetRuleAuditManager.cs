using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Utility.Extensions;
using Tuhu.Nosql;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.Repository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Model;
using Tuhu.Service.Promotion.Server.ServiceProxy;
using Tuhu.Service.Promotion.Server.Utility;

namespace Tuhu.Service.Promotion.Server.Manager
{

    public interface ICouponGetRuleAuditManager
    {

        /// <summary>
        /// 创建优惠券审核记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CreateCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken);

        Task<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync(GetCouponGetRuleAuditorRequest request, CancellationToken cancellationToken);
        Task<int> UpdateCouponGetRuleAuditAsync(GetCouponRuleAuditModel request, CancellationToken cancellationToken);
        Task<int> CommitWorkOrderForCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken);
        ///
        ValueTask<List<GetCouponRuleAuditModel>> GetCouponGetRuleAuditByWorkOrderIdAsync(int WorkOrderId, CancellationToken cancellationToken);

        ValueTask<List<GetCouponRuleAuditModel>> GetListByGetCouponRulePKIDAsync(int GetCouponRulePKID, CancellationToken cancellationToken);

        /// <summary>
        /// 获取获优惠券领取规则审核记录 count
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> GetCouponGetRuleAuditCountAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 获取获优惠券领取规则审核记录 list
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<GetCouponRuleAuditModel>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken);

      
        


    }
    /// <summary>
    /// 优惠券领取规则 逻辑层
    /// </summary>
    public class CouponGetRuleAuditManager : ICouponGetRuleAuditManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        /// <summary>
        /// 优惠券领取规则
        /// </summary>
        private readonly ICouponGetRuleRepository _ICouponGetRuleRepository;
        private readonly ICouponUseRuleRepository _ICouponUseRuleRepository;
        /// <summary>
        /// 优惠券领取规则审核
        /// </summary>
        private readonly ICouponGetRuleAuditRepository _ICouponGetRuleAuditRepository;
        private readonly IConfigBaseService _IConfigBaseService;
        private readonly AppSettingOptions _AppSettingOptions;

        private readonly IHttpClientFactory _clientFactory;



        public CouponGetRuleAuditManager(
            ILogger<CouponGetRuleAuditManager> Logger,
            ICacheHelper ICacheHelper,
            ICouponGetRuleRepository ICouponGetRuleRepository,
            ICouponUseRuleRepository ICouponUseRuleRepository,
            ICouponGetRuleAuditRepository ICouponGetRuleAuditRepository,
            IConfigBaseService IConfigBaseService,
            IOptionsSnapshot<AppSettingOptions> AppSettingOptions,
            IHttpClientFactory clientFactory

        )
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _ICouponGetRuleRepository = ICouponGetRuleRepository;
            _ICouponUseRuleRepository = ICouponUseRuleRepository;
            _ICouponGetRuleAuditRepository = ICouponGetRuleAuditRepository;
            _IConfigBaseService = IConfigBaseService;
            _AppSettingOptions = AppSettingOptions.Value;
            _clientFactory = clientFactory;

        }

        #region 优惠券领券记录审核

        /// <summary>
        /// 创建优惠券审核记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CreateCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetCouponRuleAuditEntity entity = ObjectMapper.ConvertTo<CreateCouponGetRuleAuditRequest, GetCouponRuleAuditEntity>(request);
                var pkid = await _ICouponGetRuleAuditRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
                return pkid;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateCouponGetRuleAuditAsync Exception {JsonConvert.SerializeObject(request)}", ex);
                return 0;
            }
        }


        /// <summary>
        /// 获取优惠券领取规则审核人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync(GetCouponGetRuleAuditorRequest request, CancellationToken cancellationToken)
        {
            GetCouponGetRuleAuditorResponse response = new GetCouponGetRuleAuditorResponse() { };
            if (request.Step == 1)//审核第一步
            {
                //1.获取成本归属 的 审核人
                var business = await _ICouponGetRuleRepository.GetPromotionBusinessLineConfigByPKIDAsync(request.BusinessLineId, cancellationToken).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(business?.Auditor))
                {
                    //2.如审核人为空， 则 获取 所属部门的审核人
                    var department = await _ICouponGetRuleRepository.GetCouponDepartmentUseSettingByPKIDAsync(request.DepartmentId, cancellationToken).ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(department?.Auditor))
                    {
                        return OperationResult.FromError<GetCouponGetRuleAuditorResponse>("-3", "未获取到审核人关系");
                    }
                    else
                    {
                        response.Auditors = department.Auditor;
                    }
                }
                else
                {
                    response.Auditors = business.Auditor;
                }
            }
            else if (request.Step == 2)//审核第二步
            {
                //1.根据工单号获取 申请人
                var CouponGetRuleAuditList = await _ICouponGetRuleAuditRepository.GetEntityByWorkOrderIdAsync(request.WorkOrderId, cancellationToken).ConfigureAwait(false);

                var CouponGetRuleAudit = CouponGetRuleAuditList.Where(p => p.AuditStatus == 1).FirstOrDefault();
                if (CouponGetRuleAudit == null || CouponGetRuleAudit.PKID == 0)
                {
                    return OperationResult.FromError<GetCouponGetRuleAuditorResponse>("-4", "未获取 审核记录");
                }
                else
                {
                    //2.获取部门关系的负责人
                    var ConfigResult = await _IConfigBaseService.GetBaseConfigListAsync(GlobalConstant.CouponGetRuleAudit2ndDepartmentRelation, cancellationToken).ConfigureAwait(false);
                    response.Auditors = ConfigResult.Result.Where(p => p.Key == CouponGetRuleAudit.Applicant)?.FirstOrDefault().Value;
                    if (string.IsNullOrWhiteSpace(response.Auditors))
                    {
                        return OperationResult.FromError<GetCouponGetRuleAuditorResponse>("-2", "未获取到审核人关系");
                    }
                }
            }
            else
            {
                return OperationResult.FromError<GetCouponGetRuleAuditorResponse>("-1", "Step只支持1&2");
            }
            return OperationResult.FromResult(response);
        }

        /// <summary>
        /// 更新优惠券审核记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> UpdateCouponGetRuleAuditAsync(GetCouponRuleAuditModel request, CancellationToken cancellationToken)
        {
            GetCouponRuleAuditEntity entity = ObjectMapper.ConvertTo<GetCouponRuleAuditModel, GetCouponRuleAuditEntity>(request);
            int pkid = await _ICouponGetRuleAuditRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false) ? entity.PKID : 0;
            return pkid;
        }

        /// <summary>
        /// 提交领取规则审核到工单系统
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CommitWorkOrderForCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken)
        {
            CommitWorkOrderForCouponGetRuleAuditRequest apiRequest = ObjectMapper.ConvertTo<CreateCouponGetRuleAuditRequest, CommitWorkOrderForCouponGetRuleAuditRequest>(request);
            apiRequest.WorkOrderTypeId = Convert.ToInt32(_AppSettingOptions.WorkOrderTypeIdForCouponGetRuleAudit);
            apiRequest.TaskOwner = request.Auditor;
            apiRequest.UserEmail = request.Applicant;
            apiRequest.Quantity = request.Quantity?.ToString("N0");
            apiRequest.SumDiscount = string.Format("{0:N2}", (request.Quantity * request.Discount));
            //获取使用规则信息
            var useRule = await _ICouponUseRuleRepository.GetByPKIDAsync(request.RuleID, cancellationToken).ConfigureAwait(false);
            apiRequest.PromotionType = useRule.PromotionType;
            apiRequest.EnabledGroupBuy = useRule.EnabledGroupBuy;
            apiRequest.RuleDescription = string.IsNullOrWhiteSpace(useRule.RuleDescription) ? "无" : useRule.RuleDescription;
            apiRequest.CouponUseRuleDetailURL = _AppSettingOptions.SettingHost + "/Promotion/PromotionDetailNew?id=" + request.RuleID;
            try 
            {
                _logger.Info($"CommitWorkOrderForCouponGetRuleAuditAsync result postData= {JsonConvert.SerializeObject(apiRequest)}");
                var httpClient = _clientFactory.CreateClient("HttpTimeoutForCommitWorkOrder");//设置超时时间
                var responseMessage = await httpClient.PostAsJsonAsync(_AppSettingOptions.CommitWorkOrderApiURL, apiRequest).ConfigureAwait(false);
                string response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.Info($"CommitWorkOrderForCouponGetRuleAuditAsync result response ={response}");
                var responseModel = JsonConvert.DeserializeObject<CommitWorkOrderForCouponGetRuleAuditResponse>(response);
                if (responseModel.Code == 1)
                {
                    return Convert.ToInt32(responseModel.ResponseContent);
                }
                _logger.Error($"CommitWorkOrderForCouponGetRuleAuditAsync fail Message ={responseModel?.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"CommitWorkOrderForCouponGetRuleAuditAsync Exception apiRequest ={JsonConvert.SerializeObject(apiRequest)}", ex);
                return 0;
            }
        }


      

        /// <summary>
        /// 创建优惠券领取规则记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> CreateCouponGetRuleAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetCouponRuleAuditEntity entity = ObjectMapper.ConvertTo<CreateCouponGetRuleAuditRequest, GetCouponRuleAuditEntity>(request);
                var pkid = await _ICouponGetRuleAuditRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
                return pkid;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateCouponGetRuleAuditAsync Exception {JsonConvert.SerializeObject(request)}", ex);
                return 0;
            }
        }

        /// <summary>
        /// 根据工单获取 领取规则的审核信息
        /// </summary>
        /// <param name="WorkOrderId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleAuditModel>> GetCouponGetRuleAuditByWorkOrderIdAsync(int WorkOrderId, CancellationToken cancellationToken)
        {
            var entity = await _ICouponGetRuleAuditRepository.GetEntityByWorkOrderIdAsync(WorkOrderId, cancellationToken).ConfigureAwait(false);
            List<GetCouponRuleAuditModel> models = ObjectMapper.ConvertTo<GetCouponRuleAuditEntity, GetCouponRuleAuditModel>(entity).ToList();
            return models;
        }

        public async ValueTask<List<GetCouponRuleAuditModel>> GetListByGetCouponRulePKIDAsync(int GetCouponRulePKID, CancellationToken cancellationToken)
        {
            List<GetCouponRuleAuditModel> models = new List<GetCouponRuleAuditModel>();
            var entities = await _ICouponGetRuleAuditRepository.GetListByGetCouponRulePKIDAsync(GetCouponRulePKID, cancellationToken).ConfigureAwait(false);
            models = ObjectMapper.ConvertTo<GetCouponRuleAuditEntity, GetCouponRuleAuditModel>(entities).ToList();
            return models;
        }


        /// <summary>
        /// 分页查询  领取规则审核  count
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> GetCouponGetRuleAuditCountAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken)
        {
            return await _ICouponGetRuleAuditRepository.GetCouponGetRuleAuditCountAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 分页查询  领取规则审核 list
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<GetCouponRuleAuditModel>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken)
        {
            List<GetCouponRuleAuditEntity> entity = await _ICouponGetRuleAuditRepository.GetCouponGetRuleAuditListAsync(request, cancellationToken).ConfigureAwait(false);
            List<GetCouponRuleAuditModel> models = ObjectMapper.ConvertTo<GetCouponRuleAuditEntity, GetCouponRuleAuditModel>(entity).ToList();
            return models;
        }
        #endregion

        
    }
}
