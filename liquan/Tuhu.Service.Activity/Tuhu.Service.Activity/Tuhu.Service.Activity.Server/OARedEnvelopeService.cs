using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    /// <summary>
    ///     公众号领红包业务类
    /// </summary>
    public class OARedEnvelopeService : IOARedEnvelopeService
    {
        /// <summary>
        ///     公众号领红包活动详情
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(
            int officialAccountType = 1)
        {
            return await OARedEnvelopeManager.OARedEnvelopeActivityInfoAsync(officialAccountType);
        }

        /// <summary>
        ///     公众号领红包活动详情 - 无缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoNoCacheAsync(
            int officialAccountType = 1)
        {
            return await OARedEnvelopeManager.OARedEnvelopeActivityInfoNoCacheAsync();
        }

        /// <summary>
        ///     公众号领红包 - 用户领取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(OARedEnvelopeUserReceiveRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserReceiveAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(
            OARedEnvelopeUserInfoRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserInfoAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 用户领取 回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(
            OARedEnvelopeUserReceiveCallbackRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserReceiveCallbackAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 用户是否可以领取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(OARedEnvelopeUserVerifyRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserVerifyAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 红包领取动态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>> OARedEnvelopeReceiveUpdatingsAsync(
            OARedEnvelopeReceiveUpdatingsRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeReceiveUpdatingsAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync()
        {
            return await OARedEnvelopeManager.OARedEnvelopeRefreshCacheAsync();
        }

        /// <summary>
        ///     公众号领红包 - 统计更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(
            OARedEnvelopeStatisticsUpdateRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeStatisticsUpdateAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 删除用户数据 为了测试
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="officialAccountType"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,
            int officialAccountType = 1)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserReceiveDeleteAsync(userId, officialAccountType);
        }

        /// <summary>
        ///     公众号领红包 - 删除每日初始化数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="officialAccountType"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,
            int officialAccountType = 1)
        {
            return await OARedEnvelopeManager.OARedEnvelopeDailyDataInitDeleteAsync(date, officialAccountType);
        }

        /// <summary>
        ///     公众号领红包 - 每日数据初始化
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(
            OARedEnvelopeDailyDataInitRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeDailyDataInitAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 分享
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(OARedEnvelopeUserShareRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeUserShareAsync(request);
        }

        /// <summary>
        ///    获取生成的全部红包对象 为了测试
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<OperationResult<IDictionary<string, OARedEnvelopeObjectResponse>>>
            GetAllOARedEnvelopeDailyDataAsync(GetAllOARedEnvelopeDailyDataRequest request)
        {
            return await OARedEnvelopeManager.GetAllOARedEnvelopeDailyDataAsync(request);
        }

        /// <summary>
        ///     公众号领红包 - 设置更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(
            OARedEnvelopeSettingUpdateRequest request)
        {
            return await OARedEnvelopeManager.OARedEnvelopeSettingUpdateAsync(request);
        }
    }
}
