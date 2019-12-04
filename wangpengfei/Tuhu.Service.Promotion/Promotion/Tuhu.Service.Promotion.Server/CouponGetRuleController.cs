using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Manager;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.ServiceProxy;

namespace Tuhu.Service.Promotion.Server
{
    public class CouponGetRuleController : CouponGetRuleService
    {

        public ICouponGetRuleManager _ICouponGetRuleManager;
        public ICouponGetRuleAuditManager _ICouponGetRuleAuditManager;
        public IConfigLogService _IConfigLogService;
        public ICreatePromotionService _ICreatePromotionService;


        public CouponGetRuleController(ICouponGetRuleAuditManager CouponGetRuleAuditManager
            , IConfigLogService IConfigLogService
            , ICreatePromotionService ICreatePromotionService
            , ICouponGetRuleManager ICouponGetRuleManager)
        {
            _ICouponGetRuleAuditManager = CouponGetRuleAuditManager;
            _IConfigLogService = IConfigLogService;
            _ICreatePromotionService = ICreatePromotionService;
            _ICouponGetRuleManager = ICouponGetRuleManager;
        }

        /// <summary>
        /// 根据guid获取优惠券领取规则
        /// </summary>
        /// <param name="GetRuleGUIDs"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<IEnumerable<CouponGetRuleModel>>> GetCouponGetRuleListAsync([FromQuery(Name = "getRuleIds")] IEnumerable<Guid> GetRuleGUIDs)
        {
            return OperationResult.FromResult(await _ICouponGetRuleManager.GetCouponGetRuleListAsync(GetRuleGUIDs, HttpContext.RequestAborted).ConfigureAwait(false));
        }

        #region 优惠券领取规则审核
        /// <summary>
        /// 创建优惠券审核记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<int>> CreateCouponGetRuleAuditAsync([FromBody] CreateCouponGetRuleAuditRequest request)
        {
            if (request.Applicant == "devteam@tuhu.work")
            {
                request.Applicant = "huangxiaodie@tuhu.cn";
            }
            //1.获取审核第一步的 审核人
            GetCouponGetRuleAuditorRequest getCouponGetRuleAuditorRequest = new GetCouponGetRuleAuditorRequest()
            {
                Step = 1,
                Applicant = request.Applicant,
                BusinessLineId = request.BusinessLineId,
                DepartmentId = request.DepartmentId,
            };

            if (request.GetCouponRulePKID > 0)//修改
            {
                //判断是否存在 审核中的 记录
                var list = await _ICouponGetRuleAuditManager.GetListByGetCouponRulePKIDAsync(request.GetCouponRulePKID, HttpContext.RequestAborted).ConfigureAwait(false);
                if (list.Where(p => p.AuditStatus == 1).Any())
                {
                    return OperationResult.FromError<int>("-1", "此优惠券正在审核中不支持编辑，请前往【审核列表】查看审核信息");
                }
                //获取 原领取规则信息
                var originalGetRule = await _ICouponGetRuleManager.GetByPKIDAsync(request.GetCouponRulePKID, HttpContext.RequestAborted).ConfigureAwait(false);
                request.GetRuleGUID = originalGetRule.GetRuleGUID;

            }

            var GetCouponGetRuleAuditorResult = await _ICouponGetRuleAuditManager.GetCouponGetRuleAuditorAsync(getCouponGetRuleAuditorRequest, HttpContext.RequestAborted).ConfigureAwait(false);
            if (GetCouponGetRuleAuditorResult.Success || GetCouponGetRuleAuditorResult.Result != null)
            {
                request.Auditor = GetCouponGetRuleAuditorResult.Result.Auditors;
            }
            else
            {
                return OperationResult.FromError<int>(GetCouponGetRuleAuditorResult.ErrorCode, GetCouponGetRuleAuditorResult.ErrorMessage);
            }

            //2.提交到工单系统 
            var workOrderId = await _ICouponGetRuleAuditManager.CommitWorkOrderForCouponGetRuleAuditAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            request.AuditStatus = (int)WorkOrderAuditStatusEnum.审批中;
            if (workOrderId == 0)
            {
                return OperationResult.FromError<int>("-1", "提交到工单失败,请重试");
            }
            request.WorkOrderId = workOrderId;
            //3.添加审核记录
            var pkid = await _ICouponGetRuleAuditManager.CreateCouponGetRuleAuditAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            if (pkid > 0)
            {
                //4.添加审核的提交日志
                var ConfigResult = await _IConfigLogService.InsertDefaultLogQueue(GlobalConstant.LogTypeCouponGetRuleAudit, JsonConvert.SerializeObject(new
                {
                    ObjectId = pkid,
                    ObjectType = GlobalConstant.LogObjectTypeCouponGetRuleAudit,
                    BeforeValue = "",
                    AfterValue = JsonConvert.SerializeObject(request),
                    Operate = "添加审核记录",
                    Author = request.Applicant
                })).ConfigureAwait(false);
                return OperationResult.FromResult(pkid);
            }
            else
            {
                return OperationResult.FromError<int>("-2", "CreateCouponGetRuleAuditAsync Exception");
            }
        }

        /// <summary>
        /// 更新优惠券审核状态 【工单审核成功通知MQ中调用】
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<bool>> UpdateCouponGetRuleAuditAsync([FromBody] UpdateCouponGetRuleAuditRequest request)
        {
            #region 1.更新审核记录的状态  并 添加日志
            //1.获取优惠券审核的信息
            var couponGetRuleAuditList = await _ICouponGetRuleAuditManager.GetCouponGetRuleAuditByWorkOrderIdAsync(request.WorkOrderId, HttpContext.RequestAborted).ConfigureAwait(false);

            GetCouponRuleAuditModel couponGetRuleAuditModelBefore = couponGetRuleAuditList.Where(p => p.AuditStatus == (int)WorkOrderAuditStatusEnum.审批中).FirstOrDefault();
            if (couponGetRuleAuditModelBefore == null)
            {
                return OperationResult.FromError<bool>("-1", $@"未查询到WorkOrderId ={request.WorkOrderId}审核记录");
            }
            GetCouponRuleAuditModel couponGetRuleAuditModelAfter = ObjectMapper.ConvertTo<GetCouponRuleAuditModel, GetCouponRuleAuditModel>(couponGetRuleAuditModelBefore);
            couponGetRuleAuditModelAfter.Auditor = request.Auditor;
            WorkOrderAuditStatusEnum status;
            Enum.TryParse(request.AuditStatus, out status);
            couponGetRuleAuditModelAfter.AuditStatus = (int)(status);
            couponGetRuleAuditModelAfter.AuditDateTime = request.AuditDateTime == new DateTime() ? DateTime.Now : request.AuditDateTime;
            couponGetRuleAuditModelAfter.AuditMessage = request.AuditMessage;
            couponGetRuleAuditModelAfter.Auditor = request.Auditor;
            //更新审核记录的审核信息
            var UpdateAudit = await _ICouponGetRuleAuditManager.UpdateCouponGetRuleAuditAsync(couponGetRuleAuditModelAfter, HttpContext.RequestAborted).ConfigureAwait(false);
            if (UpdateAudit == 0)
            {
                return OperationResult.FromError<bool>("-1", $@"更新审核记录异常");
            }
            //2.添加日志
            GetCouponGetRuleAuditorResponse response = new GetCouponGetRuleAuditorResponse();
            var ConfigResult = await _IConfigLogService.InsertDefaultLogQueue(GlobalConstant.LogTypeCouponGetRuleAudit, JsonConvert.SerializeObject(new
            {
                ObjectId = couponGetRuleAuditModelBefore.PKID,
                ObjectType = GlobalConstant.LogObjectTypeCouponGetRuleAudit,
                BeforeValue = JsonConvert.SerializeObject(couponGetRuleAuditModelBefore),
                AfterValue = JsonConvert.SerializeObject(couponGetRuleAuditModelAfter),
                Operate = "修改审核记录",
                Author = couponGetRuleAuditModelAfter.Auditor
            })).ConfigureAwait(false);
            #endregion

            #region 2.将审核通过的内容更新到领取规则表 & 推送给申请人 & 添加日志

            if (couponGetRuleAuditModelAfter.AuditStatus == (int)WorkOrderAuditStatusEnum.审批通过) //审核通过
            {
                //审核成功 同步到领取规则
                var pkid = await _ICouponGetRuleManager.SaveAuditToGetRuleAsync(couponGetRuleAuditModelAfter, HttpContext.RequestAborted).ConfigureAwait(false);
                if (pkid == 0)
                {
                    return OperationResult.FromError<bool>("-1", "审核成功，更新优惠券领取规则失败");
                }
                else//更新优惠券领取规则 成功
                {
                    ConfigResult = await _IConfigLogService.InsertDefaultLogQueue(GlobalConstant.LogTypeCouponGetRuleAudit, JsonConvert.SerializeObject(new
                    {
                        ObjectId = pkid,
                        ObjectType = GlobalConstant.LogObjectTypeCouponGetRule,
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(couponGetRuleAuditModelAfter),
                        Operate = "审核成功同步优惠券领取规则",
                        Author = couponGetRuleAuditModelAfter.Auditor
                    })).ConfigureAwait(false);

                    //3.更新优惠券领取规则的缓存
                    var refreshResult = await _ICreatePromotionService.RefreshGetCouponRulesCache(couponGetRuleAuditModelAfter.GetRuleGUID).ConfigureAwait(false);
                }
            }
            #endregion
            return OperationResult.FromResult(UpdateAudit > 0);
        }

        /// <summary>
        /// 获取优惠券领取规则审核人
        /// </summary>
        /// <param name="Step"></param>
        /// <param name="WorkOrderId"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync([FromQuery(Name = "Step")] int Step, [FromQuery(Name = "WorkOrderId")] int WorkOrderId)
        {
            GetCouponGetRuleAuditorRequest request = new GetCouponGetRuleAuditorRequest()
            {
                Step = Step,
                WorkOrderId = WorkOrderId,
            };
            return await _ICouponGetRuleAuditManager.GetCouponGetRuleAuditorAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取获优惠券领取规则审核记录 - 分页
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<PagedModel<GetCouponRuleAuditModel>>> GetCouponGetRuleAuditListAsync([FromBody] GetCouponGetRuleAuditListRequest request)
        {
            try
            {
                PagedModel<GetCouponRuleAuditModel> result = new PagedModel<GetCouponRuleAuditModel>()
                {
                    Pager = new PagerModel()
                    {
                        CurrentPage = request.CurrentPage > 0 ? request.CurrentPage : 1,
                        PageSize = request.PageSize > 0 ? request.PageSize : 10,
                    }
                };

                result.Pager.Total = await _ICouponGetRuleAuditManager.GetCouponGetRuleAuditCountAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
                List<GetCouponRuleAuditModel> source = await _ICouponGetRuleAuditManager.GetCouponGetRuleAuditListAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
                result.Source = source;
                return OperationResult.FromResult(result);
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<PagedModel<GetCouponRuleAuditModel>>("-1", ex.Message);
            }
        }

        /// <summary>
        /// 获取所有业务线 [不包括已删除]
        /// </summary>
        /// <returns></returns>
        public  async override ValueTask<OperationResult<List<PromotionBusinessLineConfigResponse>>> GetPromotionBusinessLineConfigListAsync()
        {
            var result = await _ICouponGetRuleManager.GetPromotionBusinessLineConfigListAsync(HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        #endregion

        /// <summary>
        /// 分页查询 领取规则
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async ValueTask<OperationResult<PagedModel<GetCouponRuleListResponse>>> GetCouponRuleListAsync([FromBody] GetCouponRuleListRequest request)
        {
            var result = await _ICouponGetRuleManager.GetCouponRuleListAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }
    }
}
