using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Product;

namespace Tuhu.Service.Activity.Server
{
    /// <summary>
    /// 途虎众测
    /// </summary>
    public class ZeroActivityService : IZeroActivityService
    {
        /// <summary>
        /// 获取未结束的首页众测活动列表
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectUnfinishedZeroActivitiesForHomepageAsync(bool resetCache = false)
        {
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.SelectUnfinishedZeroActivitiesForHomepageAsync(resetCache));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<IEnumerable<ZeroActivityModel>>(ErrorCode.DataNotExisted, "服务端异常");

            }
        }

        /// <summary>
        /// 获取已结束的首页众测活动列表
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<ZeroActivityModel>>> SelectFinishedZeroActivitiesForHomepageAsync(int pageNumber)
        {
            if (pageNumber <= 0)
                return OperationResult.FromError<IEnumerable<ZeroActivityModel>>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.SelectFinishedZeroActivitiesForHomepageAsync(pageNumber));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<IEnumerable<ZeroActivityModel>>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        /// 获取众测活动详情
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<ZeroActivityDetailModel>> FetchZeroActivityDetailAsync(int period)
        {
            if (period < 0)
                return OperationResult.FromError<ZeroActivityDetailModel>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.FetchZeroActivityDetailAsync(period));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<ZeroActivityDetailModel>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        /// 判断用户是否已提交众测申请
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> HasZeroActivityApplicationSubmittedAsync(Guid userId, int period)
        {
            if (userId == null || userId.Equals(Guid.Empty) || period < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.HasZeroActivityApplicationSubmittedAsync(userId, period));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<bool>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        /// 判断用户是否已触发开测提醒
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> HasZeroActivityReminderSubmittedAsync(Guid userId, int period)
        {
            if (userId == null || userId.Equals(Guid.Empty) || period < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.HasZeroActivityReminderSubmittedAsync(userId, period));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<bool>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        /// 获取特定众测活动的入选用户与其报告概况
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<SelectedTestReport>>> SelectChosenUserReportsAsync(int period)
        {
            if (period < 0)
                return OperationResult.FromError<IEnumerable<SelectedTestReport>>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.SelectChosenUserReportsAsync(period));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<IEnumerable<SelectedTestReport>>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        /// 获取众测报告详情
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<SelectedTestReportDetail>> FetchTestReportDetailAsync(int commentId)
        {
            if (commentId < 0)
                return OperationResult.FromError<SelectedTestReportDetail>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.FetchTestReportDetailAsync(commentId));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<SelectedTestReportDetail>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }
        /// <summary>
        /// 获取用户众测活动申请
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<MyZeroActivityApplications>>> SelectMyApplicationsAsync(Guid userId, int applicationStatus)
        {
            if (userId == null || userId.Equals(Guid.Empty))
                return OperationResult.FromError<IEnumerable<MyZeroActivityApplications>>(ErrorCode.ParameterError, "参数错误");
            try
            {
                return await OperationResult.FromResultAsync(ZeroActivityManager.SelectMyApplicationsAsync(userId, applicationStatus));
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<IEnumerable<MyZeroActivityApplications>>(ErrorCode.DataNotExisted, "服务端异常");
            }
        }

        /// <summary>
        ///提交众测申请
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SubmitZeroActivityApplicationAsync(ZeroActivityRequest requestModel)
        {
            if (requestModel == null)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "传入的requestModel不能为空");
            else if (requestModel.Period < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "period参数出错，应该大于0");
            else if (requestModel.UserId == null || requestModel.UserId == Guid.Empty)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "UserId参数不能为空");
            else if (string.IsNullOrWhiteSpace(requestModel.UserName))
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "UserName参数不能为空");

            var result = await ZeroActivityManager.SubmitZeroActivityApplicationAsync(requestModel);
            if (result > 0)
            {
                return OperationResult.FromResult(true);
            }
            else if (result == -2)
            {
                return OperationResult.FromError<bool>("ActivityNotExisted", "该期活动不存在");
            }
            else if (result == -3)
            {
                return OperationResult.FromError<bool>("ActivityNotProceeding", "该期活动不在进行中");
            }
            else if (result == -4)
            {
                return OperationResult.FromError<bool>("ActivityNotProceeding", "该期活动的PID或ProductName为空，无法插入");
            }
            else if (result == -1)
            {
                return OperationResult.FromError<bool>("ApplyExisted", "您已经申请了本期活动");
            }
            else
            {
                return OperationResult.FromError<bool>("InsertFailure", "新建数据失败");
            }
        }

        /// <summary>
        ///触发开测提醒
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SubmitZeroActivityReminderAsync(Guid userId, int period)
        {
            if (userId == null || userId == Guid.Empty || period < 0)
                return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数出错");
            try
            {
                var result = await ZeroActivityManager.SubmitZeroActivityReminderAsync(userId, period);
                if (result > 0)
                {
                    return OperationResult.FromResult(true);
                }
                else if (result == -3)
                {
                    return OperationResult.FromError<bool>("ActivityAlreadyStarted", "该期活动已开始");
                }
                else if (result == -2)
                {
                    return OperationResult.FromError<bool>("ActivityNotExisted", "该期活动不存在");
                }
                else if (result == -1)
                {
                    return OperationResult.FromError<bool>("ApplyExisted", "您已经触发过开测提醒");
                }
                else
                {
                    return OperationResult.FromError<bool>("InsertFailure", "新建数据失败");
                }
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<bool>("ExceptionOccurred", "服务端异常");
            }
        }

        #region APP 首页 0元众测 列表【前2】

        /// <summary>
        /// 刷新众测活动配置缓存
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult<bool>> RefreshZeroActivityCacheAsync()
           => OperationResult.FromResultAsync(ZeroActivityManager.RefreshZeroActivityCache());

        /// <summary>
        ///  获取 APP 首页 0元众测 列表 
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<List<ZeroActivitySimpleRespnseModel>>> SelectHomePageModuleShowZeroActivityAsync()
        {
            return  OperationResult.FromResult(await ZeroActivityManager.SelectHomePageModuleShowZeroActivityAsync());
        }

        #endregion
    }
}
