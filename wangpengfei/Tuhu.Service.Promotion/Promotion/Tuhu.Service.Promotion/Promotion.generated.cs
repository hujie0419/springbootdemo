
#if TUHU4
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif
#if SERVER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
#else
using JetBrains.Annotations;
using System.ServiceModel;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Models;


//不要在Promotion.generated.cs文件里加任何代码，此文件内容为自动生成。需要加接口请在Promotion.tt或Promotion.cs中添加
namespace Tuhu.Service.Promotion
{
#pragma warning disable 618
#if SERVER
        /// <summary>优惠券</summary>
    [ApiController, Route("Promotion/Promotion")]
    public abstract partial class PromotionService : ControllerBase
    {
           /// <summary>获取用户的优惠券</summary>
/// <param name="request">用户id和类型</param>
/// <returns>用户的优惠券</returns>
               [AcceptVerbs("POST"), Route("GetCouponByUserID")]
        [TuhuServiceActionFilter("/Promotion/Promotion/GetCouponByUserID", "GetCouponByUserIDAsync", "request")]
                public abstract ValueTask<OperationResult<List<CouponModel>>> GetCouponByUserIDAsync([FromBody] GetCouponByUserIDRequest request);
	       /// <summary>根据pkid获取用户的优惠券</summary>
/// <param name="request">用户优惠券pkid</param>
/// <returns>用户的优惠券</returns>
               [AcceptVerbs("POST"), Route("GetCouponByID")]
        [TuhuServiceActionFilter("/Promotion/Promotion/GetCouponByID", "GetCouponByIDAsync", "request")]
                public abstract ValueTask<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync([FromBody] GetCouponByIDRequest request);
	       /// <summary>延期优惠券时间</summary>
/// <param name="request"></param>
/// <returns>是否成功</returns>
               [AcceptVerbs("POST"), Route("DelayCouponEndTime")]
        [TuhuServiceActionFilter("/Promotion/Promotion/DelayCouponEndTime", "DelayCouponEndTimeAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> DelayCouponEndTimeAsync([FromBody] DelayCouponEndTimeRequest request);
	       /// <summary>修改优惠券金额</summary>
/// <param name="request"></param>
/// <returns>是否成功</returns>
               [AcceptVerbs("POST"), Route("UpdateCouponReduceCost")]
        [TuhuServiceActionFilter("/Promotion/Promotion/UpdateCouponReduceCost", "UpdateCouponReduceCostAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> UpdateCouponReduceCostAsync([FromBody] UpdateCouponReduceCostRequest request);
	       /// <summary>作废优惠券-单张</summary>
/// <param name="request"></param>
/// <returns>是否成功</returns>
               [AcceptVerbs("POST"), Route("ObsoleteCoupon")]
        [TuhuServiceActionFilter("/Promotion/Promotion/ObsoleteCoupon", "ObsoleteCouponAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> ObsoleteCouponAsync([FromBody] ObsoleteCouponRequest request);
	       /// <summary>批量作废优惠券 - 根据用户id</summary>
/// <param name="request"></param>
/// <returns>作废信息</returns>
               [AcceptVerbs("POST"), Route("ObsoleteCouponList")]
        [TuhuServiceActionFilter("/Promotion/Promotion/ObsoleteCouponList", "ObsoleteCouponListAsync", "request")]
                public abstract ValueTask<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListAsync([FromBody] ObsoleteCouponListRequest request);
	       /// <summary>批量作废优惠券 - 根据渠道</summary>
/// <param name="request"></param>
/// <returns>作废信息</returns>
               [AcceptVerbs("POST"), Route("ObsoleteCouponListByChannel")]
        [TuhuServiceActionFilter("/Promotion/Promotion/ObsoleteCouponListByChannel", "ObsoleteCouponListByChannelAsync", "request")]
                public abstract ValueTask<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListByChannelAsync([FromBody] ObsoleteCouponsByChannelRequest request);
	}
    /// <summary>使用规则</summary>
    [ApiController, Route("Promotion/CouponUseRule")]
    public abstract partial class CouponUseRuleService : ControllerBase
    {
           /// <summary>通过使用规则ID查询使用规则</summary>
/// <param name="ID"></param>
/// <returns>使用规则</returns>
               [AcceptVerbs("POST"), Route("GetRuleByID")]
        [TuhuServiceActionFilter("/Promotion/CouponUseRule/GetRuleByID", "GetRuleByIDAsync", "ID")]
                public abstract ValueTask<OperationResult<CouponModel>> GetRuleByIDAsync([FromBody] int ID);
	}
    /// <summary>优惠券任务</summary>
    [ApiController, Route("Promotion/PromotionTask")]
    public abstract partial class PromotionTaskService : ControllerBase
    {
           /// <summary>获取所有有效的优惠券任务</summary>
/// <param name="request">有效的优惠券任务筛选项</param>
/// <returns>优惠券任务列表</returns>
               [AcceptVerbs("POST"), Route("GetPromotionTaskList")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/GetPromotionTaskList", "GetPromotionTaskListAsync", "request")]
                public abstract ValueTask<OperationResult<List<PromotionTaskModel>>> GetPromotionTaskListAsync([FromBody] GetPromotionTaskListRequest request);
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request">订单号和优惠券任务列表</param>
/// <returns>优惠券任务列表</returns>
               [AcceptVerbs("POST"), Route("GetPromotionTaskListByOrderId")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/GetPromotionTaskListByOrderId", "GetPromotionTaskListByOrderIdAsync", "request")]
                public abstract ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> GetPromotionTaskListByOrderIdAsync([FromBody] GetPromotionTaskListByOrderIdRequest request);
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request"></param>
/// <returns>优惠券任务列表</returns>
               [AcceptVerbs("POST"), Route("CheckSendCouponByOrderId")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/CheckSendCouponByOrderId", "CheckSendCouponByOrderIdAsync", "request")]
                public abstract ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponByOrderIdAsync([FromBody] CheckSendCouponByOrderIdRequest request);
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request"></param>
/// <returns>优惠券任务列表</returns>
               [AcceptVerbs("POST"), Route("CheckSendCoupon")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/CheckSendCoupon", "CheckSendCouponAsync", "request")]
                public abstract ValueTask<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponAsync([FromBody] CheckSendCouponRequest request);
	       /// <summary>发放优惠券</summary>
/// <param name="request">发放优惠券</param>
/// <returns>优惠券id</returns>
               [AcceptVerbs("POST"), Route("SendCoupon")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/SendCoupon", "SendCouponAsync", "request")]
                public abstract ValueTask<OperationResult<SendCouponResponse>> SendCouponAsync([FromBody] SendCouponRequest request);
	       /// <summary>根据任务id获取发券规则</summary>
/// <param name="request">发放优惠券</param>
/// <returns>发券规则</returns>
               [AcceptVerbs("POST"), Route("GetCouponRuleByTaskID")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/GetCouponRuleListByTaskID", "GetCouponRuleListByTaskIDAsync", "request")]
                public abstract ValueTask<OperationResult<List<PromotionTaskPromotionListModel>>> GetCouponRuleListByTaskIDAsync([FromBody] GetCouponRuleByTaskIDRequest request);
	       /// <summary>根据任务id 关闭任务</summary>
/// <param name="request">任务id</param>
/// <returns>是否成功</returns>
               [AcceptVerbs("POST"), Route("ClosePromotionTaskByPkid")]
        [TuhuServiceActionFilter("/Promotion/PromotionTask/ClosePromotionTaskByPKID", "ClosePromotionTaskByPKIDAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> ClosePromotionTaskByPKIDAsync([FromBody] ClosePromotionTaskByPkidRequest request);
	}
    /// <summary>优惠券领取规则</summary>
    [ApiController, Route("Promotion/CouponGetRule")]
    public abstract partial class CouponGetRuleService : ControllerBase
    {
           /// <summary>创建优惠券审核记录</summary>
/// <param name="request">优惠券审核内容</param>
/// <returns>优惠券审核的pkid</returns>
               [AcceptVerbs("POST"), Route("CreateCouponGetRuleAudit")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/CreateCouponGetRuleAudit", "CreateCouponGetRuleAuditAsync", "request")]
                public abstract ValueTask<OperationResult<int>> CreateCouponGetRuleAuditAsync([FromBody] CreateCouponGetRuleAuditRequest request);
	       /// <summary>根据多个GetRuleId获取优惠券领取规则</summary>
/// <param name="GetRuleGUIDs">优惠券领取规则Guid</param>
/// <returns>优惠券规则</returns>
               [AcceptVerbs("POST"), Route("GetCouponGetRuleList")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/GetCouponGetRuleList", "GetCouponGetRuleListAsync", "GetRuleGUIDs")]
                public abstract ValueTask<OperationResult<IEnumerable<CouponGetRuleModel>>> GetCouponGetRuleListAsync([FromBody] IEnumerable<Guid> GetRuleGUIDs);
	       /// <summary>更新优惠券审核状态</summary>
/// <param name="request">优惠券审核内容</param>
/// <returns>更新结果</returns>
               [AcceptVerbs("POST"), Route("UpdateCouponGetRuleAudit")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/UpdateCouponGetRuleAudit", "UpdateCouponGetRuleAuditAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> UpdateCouponGetRuleAuditAsync([FromBody] UpdateCouponGetRuleAuditRequest request);
	       /// <summary>获取优惠券领取规则审核人</summary>
/// <param name="Step">审核步骤</param>
/// <param name="WorkOrderId">工单号</param>
/// <returns>审核人</returns>
               [AcceptVerbs("GET"), Route("GetCouponGetRuleAuditor")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/GetCouponGetRuleAuditor", "GetCouponGetRuleAuditorAsync", "Step", "WorkOrderId")]
                public abstract ValueTask<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync([FromQuery(Name = "Step")] int Step, [FromQuery(Name = "WorkOrderId")] int WorkOrderId);
	       /// <summary>获取获优惠券领取规则审核记录 - 分页</summary>
/// <param name="request">获取获优惠券领取规则审核记录 - 分页</param>
/// <returns>优惠券领取规则审核记录</returns>
               [AcceptVerbs("POST"), Route("GetCouponGetRuleAuditList")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/GetCouponGetRuleAuditList", "GetCouponGetRuleAuditListAsync", "request")]
                public abstract ValueTask<OperationResult<PagedModel<GetCouponRuleAuditModel>>> GetCouponGetRuleAuditListAsync([FromBody] GetCouponGetRuleAuditListRequest request);
	       /// <summary>获取所有业务线 [不包括已删除]</summary>
/// <returns>业务线</returns>
               [AcceptVerbs("GET"), Route("GetPromotionBusinessLineConfigList")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/GetPromotionBusinessLineConfigList", "GetPromotionBusinessLineConfigListAsync")]
                public abstract ValueTask<OperationResult<List<PromotionBusinessLineConfigResponse>>> GetPromotionBusinessLineConfigListAsync();
	       /// <summary>查询优惠券领取规则 - 分页</summary>
/// <param name="request"></param>
/// <returns>领取规则</returns>
               [AcceptVerbs("POST"), Route("GetCouponRuleList")]
        [TuhuServiceActionFilter("/Promotion/CouponGetRule/GetCouponRuleList", "GetCouponRuleListAsync", "request")]
                public abstract ValueTask<OperationResult< PagedModel<GetCouponRuleListResponse>>> GetCouponRuleListAsync([FromBody] GetCouponRuleListRequest request);
	}
    /// <summary>活动</summary>
    [ApiController, Route("Promotion/Activity")]
    public abstract partial class ActivityService : ControllerBase
    {
           /// <summary>获取所有活动列表</summary>
/// <param name="request"></param>
/// <returns>活动信息列表</returns>
               [AcceptVerbs("POST"), Route("GetActivityList")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetActivityList", "GetActivityListAsync", "request")]
                public abstract ValueTask<OperationResult<PagedModel<GetActivityResponse>>> GetActivityListAsync([FromQuery(Name = "request")] GetActivityListRequest request);
	       /// <summary>获取活动信息</summary>
/// <param name="ActivityID">活动ID</param>
/// <returns>活动信息</returns>
               [AcceptVerbs("POST"), Route("GetActivityInfo")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetActivityInfo", "GetActivityInfoAsync", "ActivityID")]
                public abstract ValueTask<OperationResult<GetActivityResponse>> GetActivityInfoAsync([FromBody] int ActivityID);
	       /// <summary>获取区域信息</summary>
/// <param name="IsALL"></param>
/// <returns>区域信息</returns>
               [AcceptVerbs("POST"), Route("GetRegionList")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetRegionList", "GetRegionListAsync", "IsALL")]
                public abstract ValueTask<OperationResult<List<GetRegionListResponse>>> GetRegionListAsync([FromBody] bool IsALL);
	       /// <summary>获取活动申请信息</summary>
/// <param name="request"></param>
/// <returns>活动申请信息</returns>
               [AcceptVerbs("POST"), Route("GetUserActivityApplyList")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetUserActivityApplyList", "GetUserActivityApplyListAsync", "request")]
                public abstract ValueTask<OperationResult<PagedModel<GetUserActivityApplyResponse>>> GetUserActivityApplyListAsync([FromQuery(Name = "request")] GetUserActivityApplyListRequest request);
	       /// <summary>获取活动报名人员信息列表</summary>
/// <param name="request">人员model</param>
/// <returns>人员列表</returns>
               [AcceptVerbs("POST"), Route("GetUserList")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetUserList", "GetUserListAsync", "request")]
                public abstract ValueTask<OperationResult<PagedModel<GetUserListResponse>>> GetUserListAsync([FromQuery(Name = "request")] GetUserListRequest request);
	       /// <summary>批量通过活动申请信息</summary>
/// <param name="PKIDs"></param>
/// <returns>成功/失败</returns>
               [AcceptVerbs("POST"), Route("BatchPassUserActivityApplyByPKIDs")]
        [TuhuServiceActionFilter("/Promotion/Activity/BatchPassUserActivityApplyByPKIDs", "BatchPassUserActivityApplyByPKIDsAsync", "PKIDs")]
                public abstract ValueTask<OperationResult<bool>> BatchPassUserActivityApplyByPKIDsAsync([FromQuery(Name = "PKIDs")] List<int> PKIDs);
	       /// <summary>批量通过活动申请信息</summary>
/// <param name="PKID"></param>
/// <returns>成功/失败</returns>
               [AcceptVerbs("POST"), Route("DeleteUserActivityApplyByPKID")]
        [TuhuServiceActionFilter("/Promotion/Activity/DeleteUserActivityApplyByPKID", "DeleteUserActivityApplyByPKIDAsync", "PKID")]
                public abstract ValueTask<OperationResult<bool>> DeleteUserActivityApplyByPKIDAsync([FromQuery(Name = "PKID")] int PKID);
	       /// <summary>新增活动申请信息</summary>
/// <param name="request"></param>
/// <returns>成功/失败</returns>
               [AcceptVerbs("POST"), Route("CreateUserActivityApply")]
        [TuhuServiceActionFilter("/Promotion/Activity/CreateUserActivityApply", "CreateUserActivityApplyAsync", "request")]
                public abstract ValueTask<OperationResult<bool>> CreateUserActivityApplyAsync([FromQuery(Name = "request")] CreateUserActivityApplyRequest request);
	       /// <summary>取得自动通过的活动数据</summary>
/// <param name="request"></param>
/// <returns>成功/失败</returns>
               [AcceptVerbs("POST"), Route("GetAutoPassUserActivityApplyPKIDs")]
        [TuhuServiceActionFilter("/Promotion/Activity/GetAutoPassUserActivityApplyPKIDs", "GetAutoPassUserActivityApplyPKIDsAsync", "request")]
                public abstract ValueTask<OperationResult<List<int>>> GetAutoPassUserActivityApplyPKIDsAsync([FromQuery(Name = "request")] GetAutoPassUserActivityApplyPKIDsRequest request);
	}
#else
#pragma warning disable CS0472
    /// <summary>优惠券</summary>
#if !TUHU4
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
#endif
    public partial interface IPromotionClient : ITuhuServiceClient
    {
           /// <summary>获取用户的优惠券</summary>
/// <param name="request">用户id和类型</param>
/// <param name="cancellationToken"></param>
/// <returns>用户的优惠券</returns>
                Task<OperationResult<List<CouponModel>>> GetCouponByUserIDAsync(GetCouponByUserIDRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据pkid获取用户的优惠券</summary>
/// <param name="request">用户优惠券pkid</param>
/// <param name="cancellationToken"></param>
/// <returns>用户的优惠券</returns>
                Task<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync(GetCouponByIDRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>延期优惠券时间</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>是否成功</returns>
                Task<OperationResult<bool>> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>修改优惠券金额</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>是否成功</returns>
                Task<OperationResult<bool>> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>作废优惠券-单张</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>是否成功</returns>
                Task<OperationResult<bool>> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>批量作废优惠券 - 根据用户id</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>作废信息</returns>
                Task<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListAsync(ObsoleteCouponListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>批量作废优惠券 - 根据渠道</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>作废信息</returns>
                Task<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListByChannelAsync(ObsoleteCouponsByChannelRequest request, CancellationToken cancellationToken = default(CancellationToken));
	}

#if TUHU4
    [Obsolete("此为兼容4.0以下版本升级，不要直接使用，请使用依赖注入IPromotionClient")]
    public partial class PromotionClient : TuhuWebApiClient<IPromotionClient>, IPromotionClient
    {
        public PromotionClient() : base() { }
        public PromotionClient(IServiceProvider provider) : base(provider) { }
#else
	public partial class PromotionClient : TuhuServiceClient<IPromotionClient>, IPromotionClient
    {
#endif
            /// <inherit />
        public async Task<OperationResult<List<CouponModel>>> GetCouponByUserIDAsync(GetCouponByUserIDRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/GetCouponByUserID");
                                 return await SendAsync<List<CouponModel>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync(GetCouponByIDRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/GetCouponByID");
                                 return await SendAsync<GetCouponByIDResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/DelayCouponEndTime");
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/UpdateCouponReduceCost");
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/ObsoleteCoupon");
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListAsync(ObsoleteCouponListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/ObsoleteCouponList");
                                 return await SendAsync<ObsoleteCouponListReponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListByChannelAsync(ObsoleteCouponsByChannelRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/Promotion/ObsoleteCouponListByChannel");
                                 return await SendAsync<ObsoleteCouponListReponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	    }
    /// <summary>使用规则</summary>
#if !TUHU4
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
#endif
    public partial interface ICouponUseRuleClient : ITuhuServiceClient
    {
           /// <summary>通过使用规则ID查询使用规则</summary>
/// <param name="ID"></param>
/// <param name="cancellationToken"></param>
/// <returns>使用规则</returns>
                Task<OperationResult<CouponModel>> GetRuleByIDAsync(int ID, CancellationToken cancellationToken = default(CancellationToken));
	}

#if TUHU4
    [Obsolete("此为兼容4.0以下版本升级，不要直接使用，请使用依赖注入ICouponUseRuleClient")]
    public partial class CouponUseRuleClient : TuhuWebApiClient<ICouponUseRuleClient>, ICouponUseRuleClient
    {
        public CouponUseRuleClient() : base() { }
        public CouponUseRuleClient(IServiceProvider provider) : base(provider) { }
#else
	public partial class CouponUseRuleClient : TuhuServiceClient<ICouponUseRuleClient>, ICouponUseRuleClient
    {
#endif
            /// <inherit />
        public async Task<OperationResult<CouponModel>> GetRuleByIDAsync(int ID, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(ID);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponUseRule/GetRuleByID");
                                 return await SendAsync<CouponModel>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	    }
    /// <summary>优惠券任务</summary>
#if !TUHU4
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
#endif
    public partial interface IPromotionTaskClient : ITuhuServiceClient
    {
           /// <summary>获取所有有效的优惠券任务</summary>
/// <param name="request">有效的优惠券任务筛选项</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券任务列表</returns>
                Task<OperationResult<List<PromotionTaskModel>>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request">订单号和优惠券任务列表</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券任务列表</returns>
                Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> GetPromotionTaskListByOrderIdAsync(GetPromotionTaskListByOrderIdRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券任务列表</returns>
                Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponByOrderIdAsync(CheckSendCouponByOrderIdRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据订单号匹配有效的优惠券任务</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券任务列表</returns>
                Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponAsync(CheckSendCouponRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>发放优惠券</summary>
/// <param name="request">发放优惠券</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券id</returns>
                Task<OperationResult<SendCouponResponse>> SendCouponAsync(SendCouponRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据任务id获取发券规则</summary>
/// <param name="request">发放优惠券</param>
/// <param name="cancellationToken"></param>
/// <returns>发券规则</returns>
                Task<OperationResult<List<PromotionTaskPromotionListModel>>> GetCouponRuleListByTaskIDAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据任务id 关闭任务</summary>
/// <param name="request">任务id</param>
/// <param name="cancellationToken"></param>
/// <returns>是否成功</returns>
                Task<OperationResult<bool>> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken = default(CancellationToken));
	}

#if TUHU4
    [Obsolete("此为兼容4.0以下版本升级，不要直接使用，请使用依赖注入IPromotionTaskClient")]
    public partial class PromotionTaskClient : TuhuWebApiClient<IPromotionTaskClient>, IPromotionTaskClient
    {
        public PromotionTaskClient() : base() { }
        public PromotionTaskClient(IServiceProvider provider) : base(provider) { }
#else
	public partial class PromotionTaskClient : TuhuServiceClient<IPromotionTaskClient>, IPromotionTaskClient
    {
#endif
            /// <inherit />
        public async Task<OperationResult<List<PromotionTaskModel>>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/GetPromotionTaskList");
                                 return await SendAsync<List<PromotionTaskModel>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> GetPromotionTaskListByOrderIdAsync(GetPromotionTaskListByOrderIdRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/GetPromotionTaskListByOrderId");
                                 return await SendAsync<GetPromotionTaskListByOrderIdResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponByOrderIdAsync(CheckSendCouponByOrderIdRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/CheckSendCouponByOrderId");
                                 return await SendAsync<GetPromotionTaskListByOrderIdResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetPromotionTaskListByOrderIdResponse>> CheckSendCouponAsync(CheckSendCouponRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/CheckSendCoupon");
                                 return await SendAsync<GetPromotionTaskListByOrderIdResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<SendCouponResponse>> SendCouponAsync(SendCouponRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/SendCoupon");
                                 return await SendAsync<SendCouponResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<List<PromotionTaskPromotionListModel>>> GetCouponRuleListByTaskIDAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/GetCouponRuleByTaskID");
                                 return await SendAsync<List<PromotionTaskPromotionListModel>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/PromotionTask/ClosePromotionTaskByPkid");
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	    }
    /// <summary>优惠券领取规则</summary>
#if !TUHU4
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
#endif
    public partial interface ICouponGetRuleClient : ITuhuServiceClient
    {
           /// <summary>创建优惠券审核记录</summary>
/// <param name="request">优惠券审核内容</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券审核的pkid</returns>
                Task<OperationResult<int>> CreateCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>根据多个GetRuleId获取优惠券领取规则</summary>
/// <param name="GetRuleGUIDs">优惠券领取规则Guid</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券规则</returns>
                Task<OperationResult<IEnumerable<CouponGetRuleModel>>> GetCouponGetRuleListAsync(IEnumerable<Guid> GetRuleGUIDs, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>更新优惠券审核状态</summary>
/// <param name="request">优惠券审核内容</param>
/// <param name="cancellationToken"></param>
/// <returns>更新结果</returns>
                Task<OperationResult<bool>> UpdateCouponGetRuleAuditAsync(UpdateCouponGetRuleAuditRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取优惠券领取规则审核人</summary>
/// <param name="Step">审核步骤</param>
/// <param name="WorkOrderId">工单号</param>
/// <param name="cancellationToken"></param>
/// <returns>审核人</returns>
                Task<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync(int Step, int WorkOrderId, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取获优惠券领取规则审核记录 - 分页</summary>
/// <param name="request">获取获优惠券领取规则审核记录 - 分页</param>
/// <param name="cancellationToken"></param>
/// <returns>优惠券领取规则审核记录</returns>
                Task<OperationResult<PagedModel<GetCouponRuleAuditModel>>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取所有业务线 [不包括已删除]</summary>
/// <param name="cancellationToken"></param>
/// <returns>业务线</returns>
                Task<OperationResult<List<PromotionBusinessLineConfigResponse>>> GetPromotionBusinessLineConfigListAsync(CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>查询优惠券领取规则 - 分页</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>领取规则</returns>
                Task<OperationResult< PagedModel<GetCouponRuleListResponse>>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	}

#if TUHU4
    [Obsolete("此为兼容4.0以下版本升级，不要直接使用，请使用依赖注入ICouponGetRuleClient")]
    public partial class CouponGetRuleClient : TuhuWebApiClient<ICouponGetRuleClient>, ICouponGetRuleClient
    {
        public CouponGetRuleClient() : base() { }
        public CouponGetRuleClient(IServiceProvider provider) : base(provider) { }
#else
	public partial class CouponGetRuleClient : TuhuServiceClient<ICouponGetRuleClient>, ICouponGetRuleClient
    {
#endif
            /// <inherit />
        public async Task<OperationResult<int>> CreateCouponGetRuleAuditAsync(CreateCouponGetRuleAuditRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/CreateCouponGetRuleAudit");
                                 return await SendAsync<int>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<IEnumerable<CouponGetRuleModel>>> GetCouponGetRuleListAsync(IEnumerable<Guid> GetRuleGUIDs, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(GetRuleGUIDs);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/GetCouponGetRuleList");
                                 return await SendAsync<IEnumerable<CouponGetRuleModel>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> UpdateCouponGetRuleAuditAsync(UpdateCouponGetRuleAuditRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/UpdateCouponGetRuleAudit");
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetCouponGetRuleAuditorResponse>> GetCouponGetRuleAuditorAsync(int Step, int WorkOrderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("GET");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("Step", Step == null ? "" : Step.ToString());
                                                                _query_.Add("WorkOrderId", WorkOrderId == null ? "" : WorkOrderId.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/GetCouponGetRuleAuditor", _query_);
                                 return await SendAsync<GetCouponGetRuleAuditorResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<PagedModel<GetCouponRuleAuditModel>>> GetCouponGetRuleAuditListAsync(GetCouponGetRuleAuditListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/GetCouponGetRuleAuditList");
                                 return await SendAsync<PagedModel<GetCouponRuleAuditModel>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<List<PromotionBusinessLineConfigResponse>>> GetPromotionBusinessLineConfigListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("GET");
                                    _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/GetPromotionBusinessLineConfigList");
                                 return await SendAsync<List<PromotionBusinessLineConfigResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult< PagedModel<GetCouponRuleListResponse>>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(request);
                                                  _request_.RequestUri = GetUri($"/Promotion/CouponGetRule/GetCouponRuleList");
                                 return await SendAsync< PagedModel<GetCouponRuleListResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	    }
    /// <summary>活动</summary>
#if !TUHU4
    [ServiceContract(Namespace = TuhuSerivce.TuhuSerivceNamespace)]
#endif
    public partial interface IActivityClient : ITuhuServiceClient
    {
           /// <summary>获取所有活动列表</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>活动信息列表</returns>
                Task<OperationResult<PagedModel<GetActivityResponse>>> GetActivityListAsync(GetActivityListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取活动信息</summary>
/// <param name="ActivityID">活动ID</param>
/// <param name="cancellationToken"></param>
/// <returns>活动信息</returns>
                Task<OperationResult<GetActivityResponse>> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取区域信息</summary>
/// <param name="IsALL"></param>
/// <param name="cancellationToken"></param>
/// <returns>区域信息</returns>
                Task<OperationResult<List<GetRegionListResponse>>> GetRegionListAsync(bool IsALL, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取活动申请信息</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>活动申请信息</returns>
                Task<OperationResult<PagedModel<GetUserActivityApplyResponse>>> GetUserActivityApplyListAsync(GetUserActivityApplyListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>获取活动报名人员信息列表</summary>
/// <param name="request">人员model</param>
/// <param name="cancellationToken"></param>
/// <returns>人员列表</returns>
                Task<OperationResult<PagedModel<GetUserListResponse>>> GetUserListAsync(GetUserListRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>批量通过活动申请信息</summary>
/// <param name="PKIDs"></param>
/// <param name="cancellationToken"></param>
/// <returns>成功/失败</returns>
                Task<OperationResult<bool>> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>批量通过活动申请信息</summary>
/// <param name="PKID"></param>
/// <param name="cancellationToken"></param>
/// <returns>成功/失败</returns>
                Task<OperationResult<bool>> DeleteUserActivityApplyByPKIDAsync(int PKID, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>新增活动申请信息</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>成功/失败</returns>
                Task<OperationResult<bool>> CreateUserActivityApplyAsync(CreateUserActivityApplyRequest request, CancellationToken cancellationToken = default(CancellationToken));
	       /// <summary>取得自动通过的活动数据</summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns>成功/失败</returns>
                Task<OperationResult<List<int>>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request, CancellationToken cancellationToken = default(CancellationToken));
	}

#if TUHU4
    [Obsolete("此为兼容4.0以下版本升级，不要直接使用，请使用依赖注入IActivityClient")]
    public partial class ActivityClient : TuhuWebApiClient<IActivityClient>, IActivityClient
    {
        public ActivityClient() : base() { }
        public ActivityClient(IServiceProvider provider) : base(provider) { }
#else
	public partial class ActivityClient : TuhuServiceClient<IActivityClient>, IActivityClient
    {
#endif
            /// <inherit />
        public async Task<OperationResult<PagedModel<GetActivityResponse>>> GetActivityListAsync(GetActivityListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("request", request == null ? "" : request.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/GetActivityList", _query_);
                                 return await SendAsync<PagedModel<GetActivityResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<GetActivityResponse>> GetActivityInfoAsync(int ActivityID, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(ActivityID);
                                                  _request_.RequestUri = GetUri($"/Promotion/Activity/GetActivityInfo");
                                 return await SendAsync<GetActivityResponse>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<List<GetRegionListResponse>>> GetRegionListAsync(bool IsALL, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                   _request_.Content = SerializeBody(IsALL);
                                                  _request_.RequestUri = GetUri($"/Promotion/Activity/GetRegionList");
                                 return await SendAsync<List<GetRegionListResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<PagedModel<GetUserActivityApplyResponse>>> GetUserActivityApplyListAsync(GetUserActivityApplyListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("request", request == null ? "" : request.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/GetUserActivityApplyList", _query_);
                                 return await SendAsync<PagedModel<GetUserActivityApplyResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<PagedModel<GetUserListResponse>>> GetUserListAsync(GetUserListRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("request", request == null ? "" : request.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/GetUserList", _query_);
                                 return await SendAsync<PagedModel<GetUserListResponse>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> BatchPassUserActivityApplyByPKIDsAsync(List<int> PKIDs, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("PKIDs", PKIDs == null ? "" : PKIDs.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/BatchPassUserActivityApplyByPKIDs", _query_);
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> DeleteUserActivityApplyByPKIDAsync(int PKID, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("PKID", PKID == null ? "" : PKID.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/DeleteUserActivityApplyByPKID", _query_);
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<bool>> CreateUserActivityApplyAsync(CreateUserActivityApplyRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("request", request == null ? "" : request.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/CreateUserActivityApply", _query_);
                                 return await SendAsync<bool>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	        /// <inherit />
        public async Task<OperationResult<List<int>>> GetAutoPassUserActivityApplyPKIDsAsync(GetAutoPassUserActivityApplyPKIDsRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var _request_ = new HttpRequestMessage())
            {
                            _request_.Method = new HttpMethod("POST");
                                                var _query_ = new NameValueCollection();
                                                               _query_.Add("request", request == null ? "" : request.ToString());
                                                    _request_.RequestUri = GetUri($"/Promotion/Activity/GetAutoPassUserActivityApplyPKIDs", _query_);
                                 return await SendAsync<List<int>>(_request_, cancellationToken).ConfigureAwait(false);
            }
        }
	    }
#pragma warning restore CS0472
#if TUHU4
    public static class WcfBuilderExtensions
    {
        public static IWcfBuilder AddPromotion(this IWcfBuilder builder)
        {
            builder.Services.TryAddScoped<IPromotionClient, PromotionClient>();
            builder.Services.TryAddScoped<ICouponUseRuleClient, CouponUseRuleClient>();
            builder.Services.TryAddScoped<IPromotionTaskClient, PromotionTaskClient>();
            builder.Services.TryAddScoped<ICouponGetRuleClient, CouponGetRuleClient>();
            builder.Services.TryAddScoped<IActivityClient, ActivityClient>();

            return builder;
        }
    }
#endif
#endif
#pragma warning restore 618
}
