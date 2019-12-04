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
    public class ActivityController : ActivityService
    {
        public IRegionManager _IRegionManager;
        public IActivityManager _IActivityManager;
        public IUserActivityApplyManager _IUserActivityApplyManager;
        public IUserManager _IUserManager;
        public ActivityController(IRegionManager RegionManager,
            IActivityManager ActivityManager,
            IUserActivityApplyManager UserActivityApplyManager,
            IUserManager UserManager)
        {
            _IRegionManager = RegionManager;
            _IActivityManager = ActivityManager;
            _IUserActivityApplyManager = UserActivityApplyManager;
            _IUserManager = UserManager;
        }

        /// <summary>
        /// 获取所有活动列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns>活动List</returns>
        public override async ValueTask<OperationResult<PagedModel<GetActivityResponse>>> GetActivityListAsync(
            [FromBody]GetActivityListRequest request)
        {
            if (request.PageSize <= 0 || request.CurrentPage <= 0)
            {
                return OperationResult.FromError<PagedModel<GetActivityResponse>>("-1", "页长页码传参不正确");
            }
            var result = await _IActivityManager.GetActivityListAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <param name="ActivityID">活动ID</param>
        /// <returns>活动Model</returns>
        public override async ValueTask<OperationResult<GetActivityResponse>> GetActivityInfoAsync(int ActivityID)
        {
            var result = await _IActivityManager.GetActivityInfoAsync(ActivityID, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="IsALL">是否全部(默认否)</param>
        /// <returns>区域List</returns>
        public override async ValueTask<OperationResult<List<GetRegionListResponse>>> GetRegionListAsync(bool IsALL = false)
        {
            var result = await _IRegionManager.GetRegionListAsync(HttpContext.RequestAborted, IsALL).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 活动申请信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns>活动申请信息List</returns>
        public override async ValueTask<OperationResult<PagedModel<GetUserActivityApplyResponse>>> GetUserActivityApplyListAsync(
            [FromBody]GetUserActivityApplyListRequest request)
        {
            var result = await _IUserActivityApplyManager.GetUserActivityApplyListAsync(request,
                HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 人员信息查询
        /// </summary>
        /// <param name="request">查询条件</param>
        /// <returns>人员List</returns>
        public override async ValueTask<OperationResult<PagedModel<GetUserListResponse>>> GetUserListAsync([FromBody] GetUserListRequest request)
        {
            if (request.PageSize <= 0 || request.CurrentPage <= 0)
            {
                return OperationResult.FromError<PagedModel<GetUserListResponse>>("-1", "页长页码传参不正确");
            }
            var result = await _IUserManager.GetUserListAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 批量通过活动申请
        /// </summary>
        /// <param name="PKIDs"></param>
        /// <returns>成功/失败</returns>
        public override async ValueTask<OperationResult<bool>> BatchPassUserActivityApplyByPKIDsAsync([FromBody]List<int> PKIDs)
        {
            if (PKIDs == null || PKIDs.Count == 0)
            {
                return OperationResult.FromError<bool>("-1", "传参不正确");
            }
            var result = await _IUserActivityApplyManager.BatchPassUserActivityApplyByPKIDsAsync(PKIDs,
                HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 新增活动申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns>成功/失败</returns>
        public override async ValueTask<OperationResult<bool>> CreateUserActivityApplyAsync([FromBody] CreateUserActivityApplyRequest request)
        {
            if (request.UserId <= 0 || request.ActivityId <= 0 || string.IsNullOrEmpty(request.Remark))
            {
                return OperationResult.FromError<bool>("-1", "传参不正确");
            }
            if (string.IsNullOrEmpty(request.CreateUser))
            {
                request.CreateUser = "sys";
            }
            var result = await _IUserActivityApplyManager.CreateUserActivityApplyAsync(request,
            HttpContext.RequestAborted).ConfigureAwait(false);
            return result;
        }


        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public override async ValueTask<OperationResult<bool>> DeleteUserActivityApplyByPKIDAsync(int PKID)
        {
            if (PKID <= 0)
            {
                return OperationResult.FromError<bool>("-1", "传参不正确");
            }
            var result = await _IUserActivityApplyManager.DeleteUserActivityApplyByPKIDAsync(PKID,
               HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }


        /// <summary>
        /// 获取可自动审核的活动申请数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override async ValueTask<OperationResult<List<int>>> GetAutoPassUserActivityApplyPKIDsAsync([FromBody]GetAutoPassUserActivityApplyPKIDsRequest request)
        {
            if (request.CurrentPage <= 0 || request.PageSize <= 0 || string.IsNullOrEmpty(request.AreaIDs))
            {
                return OperationResult.FromError<List<int>>("-1", "传参不正确");
            }
            var result = await _IUserActivityApplyManager.GetAutoPassUserActivityApplyPKIDsAsync(request,
               HttpContext.RequestAborted).ConfigureAwait(false);
            return OperationResult.FromResult(result);
        }
    }
}
