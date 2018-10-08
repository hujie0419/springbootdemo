using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.OARedEnvelope;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.Vehicle;
using Tuhu.Service.Vehicle.Model;
using Tuhu.ZooKeeper;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    ///     公众号领红包 逻辑类
    /// </summary>
    public class OARedEnvelopeManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OARedEnvelopeManager));

        /// <summary>
        ///     公众号领红包 - 刷新缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeRefreshCacheAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.OARedEnvelopeCacheHeader))
                {
                    await Task.WhenAll(
                        // 干掉 公众号领红包活动详情
                        cacheClient.RemoveAsync($"{nameof(OARedEnvelopeActivityInfoAsync)}")
                    );
                }

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error($"OARedEnvelopeManager -> OARedEnvelopeRefreshCacheAsync -> error", e.InnerException ?? e);
                throw;
            }
        }


        /// <summary>
        ///     公众号领红包 - 删除用户数据 为了测试
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="officialAccountType"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeUserReceiveDeleteAsync(Guid userId,
            int officialAccountType = 1)
        {
            try
            {
                await DalOARedEnvelopeDetail.DeleteOARedEnvelopeDetailAsync(userId);

                // 干掉缓存
                var oaRedEnvelopeCacheManager = new OARedEnvelopeCacheManager(officialAccountType, DateTime.Now);

                await oaRedEnvelopeCacheManager.RemoveUserOARedEnvelopeObjectAsync(userId);

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error($"OARedEnvelopeManager -> OARedEnvelopeUserReceiveDeleteAsync -> error -> {userId}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #region 公众号领红包 - 设置更新

        /// <summary>
        ///     公众号领红包 - 设置更新
        ///     返回值：
        ///     -2 获取锁失败
        ///     -3 参数异常
        ///     -4 参数FLAG验证失败
        ///     -5 金额验证失败
        ///     -6 活动时间验证失败
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeSettingUpdateAsync(
            OARedEnvelopeSettingUpdateRequest request)
        {
            try
            {
                await Task.Yield();

                using (var zkLock = new ZooKeeperLock($"{nameof(OARedEnvelopeSettingUpdateAsync)}"))
                {
                    if (!await zkLock.WaitAsync(5000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<bool>("-2", "");
                    }


                    // 判断参数
                    if (request == null)
                    {
                        return OperationResult.FromError<bool>("-3", "");
                    }

                    // 判断条件是否合法
                    if (request.ConditionPriceFlag != 1 && request.ConditionCarModelFlag != 1)
                    {
                        return OperationResult.FromError<bool>("-4", "");
                    }

                    // 判断单笔订单金额是否合法
                    if (request.ConditionPriceFlag == 1 && request.ConditionPrice < 0)
                    {
                        return OperationResult.FromError<bool>("-5", "");
                    }

                    // 活动时间判断
                    if (request.StartTime >= request.EndTime)
                    {
                        return OperationResult.FromError<bool>("-6", "");
                    }

                    Logger.Info(
                        $"OARedEnvelopeDailyDataInitAsync -> OARedEnvelopeSettingUpdateAsync -> start -> {JsonConvert.SerializeObject(request)}");

                    if (request.OfficialAccountType == 0)
                    {
                        request.OfficialAccountType = 1;
                    }

                    // 取上一次的设置
                    var beforeSetting =
                        await DalOARedEnvelopeSetting.GetOARedEnvelopeSettingAsync(request.OfficialAccountType);


                    if (beforeSetting == null)
                    {
                        beforeSetting =
                            ObjectMapper.ConvertTo<OARedEnvelopeSettingUpdateRequest, OARedEnvelopeSettingModel>(
                                request);
                        beforeSetting.PerMaxMoney = 8.88m;
                        beforeSetting.PerMinMoney = 1.00m;
                        beforeSetting.Channel = "WX_APP_OfficialAccount";
                    }
                    else
                    {
                        beforeSetting.ConditionPrice = request.ConditionPrice;
                        beforeSetting.ConditionPriceFlag = request.ConditionPriceFlag;
                        beforeSetting.ConditionCarModelFlag = request.ConditionCarModelFlag;
                        beforeSetting.DayMaxMoney = request.DayMaxMoney;
                        beforeSetting.AvgMoney = request.AvgMoney;
                        beforeSetting.ActivityRuleText = request.ActivityRuleText;
                        beforeSetting.FailTipText = request.FailTipText;
                        beforeSetting.QRCodeUrl = request.QRCodeUrl;
                        beforeSetting.QRCodeTipText = request.QRCodeTipText;
                        beforeSetting.ShareTitleText = request.ShareTitleText;
                        beforeSetting.ShareUrl = request.ShareUrl;
                        beforeSetting.SharePictureUrl = request.SharePictureUrl;
                        beforeSetting.ShareText = request.ShareText;
                        beforeSetting.OfficialAccountType = request.OfficialAccountType;
                        beforeSetting.CreateBy = request.CreateBy;
                        beforeSetting.LastUpdateBy = request.LastUpdateBy;
                        beforeSetting.OpenIdLegalDate = request.OpenIdLegalDate;
                    }

                    // 获取活动对象
                    var activityType = await DalActivity.GetActivityByTypeId(2);
                    activityType.StartTime = request.StartTime;
                    activityType.EndTime = request.EndTime;
                    using (var dbHelper = DbHelper.CreateDbHelper())
                    {
                        // 开启事务
                        dbHelper.BeginTransaction();
                        if (beforeSetting.PKID == 0)
                        {
                            await DalOARedEnvelopeSetting.InsertOARedEnvelopeSettingAsync(dbHelper, beforeSetting);
                        }
                        else
                        {
                            await DalOARedEnvelopeSetting.UpdateOARedEnvelopeSettingAsync(dbHelper, beforeSetting);
                        }

                        // 更新活动
                        await DalActivity.UpdateActivtyAsync(dbHelper, activityType);
                        // 提交事务
                        dbHelper.Commit();
                    }

                    return OperationResult.FromResult(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserReceiveDeleteAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 公众号领红包 - 统计更新

        /// <summary>
        ///     公众号领红包 - 统计更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeStatisticsUpdateAsync(
            OARedEnvelopeStatisticsUpdateRequest request)
        {
            try
            {
                await Task.Yield();
                // 判断参数
                if (request == null)
                {
                    return OperationResult.FromError<bool>("-10", "");
                }

                if (request.OfficialAccountType == 0)
                {
                    request.OfficialAccountType = 1;
                }

                Logger.Info(
                    $"OARedEnvelopeDailyDataInitAsync -> OARedEnvelopeStatisticsUpdateAsync -> start -> {JsonConvert.SerializeObject(request)}");


                var oaRedEnvelopeCacheManager =
                    new OARedEnvelopeCacheManager(request.OfficialAccountType, request.StatisticsDate);

                // 统计结果 - 实时  - 缓存里面
                var statisticsResult = await oaRedEnvelopeCacheManager.GetStatisticsAsync();

                // 设置结果 - 缓存里面
                var settingResult = await oaRedEnvelopeCacheManager.GetDailySettingAsync();

                // 当前日期的数据
                var dbResult =
                    await DalOARedEnvelopeStatistics.GetOARedEnvelopeStatisticsAsync(request.OfficialAccountType,
                        request.StatisticsDate);


                // 判断一下从缓存里面拿到的数据是否真的可用
                if (statisticsResult.Item1 == 0 || statisticsResult.Item2 == 0)
                {
                    Logger.Info(
                        $"OARedEnvelopeManager -> OARedEnvelopeStatisticsUpdateAsync -> No Data -> {JsonConvert.SerializeObject(request)} ");
                    return OperationResult.FromResult(false);
                }

                // 必要数据
                var count = (int)statisticsResult.Item1;
                var redEnvelopeSumMoney = statisticsResult.Item2;
                var redEnvelopAvg = Math.Round(redEnvelopeSumMoney / count, 2);

                using (var dbHelper = DbHelper.CreateDbHelper())
                {
                    // 保存到表中
                    if (dbResult != null)
                    {
                        dbResult.RedEnvelopeCount = count;
                        dbResult.RedEnvelopeSumMoney = redEnvelopeSumMoney;
                        dbResult.RedEnvelopeAvg = redEnvelopAvg;
                        dbResult.DayMaxMoney = settingResult?.DayMaxMoney ?? 0;

                        // 更新数据
                        await DalOARedEnvelopeStatistics.UpdateOARedEnvelopeStatisticsAsync(dbHelper, dbResult);
                    }
                    else
                    {
                        var insertModel = new OARedEnvelopeStatisticsModel
                        {
                            StatisticsDate = request.StatisticsDate.Date,
                            OfficialAccountType = request.OfficialAccountType,
                            RedEnvelopeAvg = redEnvelopAvg,
                            RedEnvelopeCount = count,
                            RedEnvelopeSumMoney = redEnvelopeSumMoney,
                            DayMaxMoney = settingResult?.DayMaxMoney ?? 0
                        };
                        // 添加数据
                        await DalOARedEnvelopeStatistics.InsertOARedEnvelopeStatisticsAsync(dbHelper, insertModel);
                    }
                }

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeStatisticsUpdateAsync -> error -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 公众号领红包 - 个人信息

        /// <summary>
        ///     公众号领红包 - 用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<OARedEnvelopeUserInfoResponse>> OARedEnvelopeUserInfoAsync(
            OARedEnvelopeUserInfoRequest request)
        {
            try
            {
                var userId = request.UserId;
                var requestOfficialAccountType = request.OfficialAccountType;
                // 获取缓存中数据
                var oaRedEnvelopeCacheManager = new OARedEnvelopeCacheManager(requestOfficialAccountType, DateTime.Now);
                var oaRedEnvelopeBuilderModel = await oaRedEnvelopeCacheManager.GetUserOARedEnvelopeObjectAsync(userId);
                // 获取DB中数据
                var oaRedEnvelopeDetailModel = (await DalOARedEnvelopeDetail.GetOARedEnvelopeDetailAsync(
                    userId
                    , string.Empty
                    , requestOfficialAccountType
                    , string.Empty
                )).FirstOrDefault();
                var result = new OARedEnvelopeUserInfoResponse();
                // 可能DB还没有数据 MQ异步操作
                if (oaRedEnvelopeDetailModel != null)
                {
                    result.GetDate = oaRedEnvelopeDetailModel.CreateDatetime;
                    result.GetMoney = oaRedEnvelopeDetailModel.GetMoney;
                }
                else if (oaRedEnvelopeBuilderModel != null)
                {
                    result.GetDate = oaRedEnvelopeBuilderModel.RequestTime;
                    result.GetMoney = oaRedEnvelopeBuilderModel.Money;
                }

                return OperationResult.FromResult(result);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserInfoAsync -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 公众号领红包 - 分享

        /// <summary>
        ///     公众号领红包 - 分享
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeUserShareAsync(
            OARedEnvelopeUserShareRequest request)
        {
            if (request == null)
            {
                return OperationResult.FromResult(false);
            }

            if (string.IsNullOrWhiteSpace(request.SharedOpenId) || string.IsNullOrWhiteSpace(request.ShareingOpenId))
            {
                return OperationResult.FromResult(false);
            }

            try
            {
                var target = new OARedEnvelopeShareModel
                {
                    OfficialAccountType = request.OfficialAccountType,
                    SharedOpenId = request.SharedOpenId,
                    SharedUserId = request.SharedUserId,
                    ShareingOpenId = request.ShareingOpenId,
                    ShareingUserId = request.ShareingUserId
                };
                await DalOARedEnvelopeShare.InsertOARedEnvelopeShareAsync(target);

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserShareAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 公众号领红包  - 红包领取动态

        /// <summary>
        ///     公众号领红包 - 红包领取动态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<OARedEnvelopeReceiveUpdatingsResponse>>
            OARedEnvelopeReceiveUpdatingsAsync(OARedEnvelopeReceiveUpdatingsRequest request)
        {
            try
            {
                if (request.Count > 100)
                {
                    return OperationResult.FromError<OARedEnvelopeReceiveUpdatingsResponse>("-5", "");
                }

                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.OARedEnvelopeCacheHeader))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync(
                        $"{nameof(OARedEnvelopeReceiveUpdatingsAsync)}:{request.OfficialAccountType}:{request.Count}",
                        async () =>
                        {
                            var dbResult =
                                await DalOARedEnvelopeDetail.SearchTopOARedEnvelopeDetailAsync(request.Count,
                                    request.OfficialAccountType);
                            dbResult?.ForEach(item =>
                            {
                                item.NickName = OARedEnvelopeReceiveUpdatingsNickNameDataMasking(item.NickName);
                            });
                            var returnValue = new OARedEnvelopeReceiveUpdatingsResponse();

                            returnValue.Items = dbResult.Select(p => new OARedEnvelopeReceiveUpdatingsItem
                            {
                                GetMoney = p.GetMoney,
                                NickName = p.NickName,
                                WXHeadImgUrl = p.WXHeadImgUrl
                            }).ToList();
                            return returnValue;
                        }, TimeSpan.FromMinutes(5));
                    return OperationResult.FromResult(cacheResult.Value);
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeReceiveUpdatingsAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     数据 脱敏
        /// </summary>
        private static string OARedEnvelopeReceiveUpdatingsNickNameDataMasking(string nickName)
        {
            nickName = nickName?.Trim() ?? "";
            //加密 如果11位 那么隐藏中间 四位
            if (nickName.Length == 11)
            {
                nickName = nickName.Substring(0, 3) + "****" + nickName.Substring(7, 4);
            }

            //写死 临时需求 不想在排行榜显示这些词汇
            var keyNames = new[] { "京东", "淘宝", "测试", "途虎" };
            keyNames.ForEach(name => { nickName = nickName.Replace(name, "*"); });
            return nickName;
        }

        #endregion

        #region 公众号领红包活动详情

        /// <summary>
        ///     私有：公众号领红包活动详情
        /// </summary>
        /// <returns></returns>
        private static async Task<OARedEnvelopeActivityInfoResponse> GetOARedEnvelopeActivityInfoAsync(
            int officialAccountType = 1)
        {
            var returnValue = new OARedEnvelopeActivityInfoResponse();

            if (officialAccountType == 0)
            {
                officialAccountType = 1;
            }

            // 获取设置
            var setting = await DalOARedEnvelopeSetting.GetOARedEnvelopeSettingAsync(officialAccountType);
            if (setting == null)
            {
                return null;
            }

            // 获取当前缓存
            var oaRedEnvelopeCacheManager = new OARedEnvelopeCacheManager(officialAccountType, DateTime.Now);

            // 获取当前设置
            var oaRedEnvelopeSettingModel = await oaRedEnvelopeCacheManager.GetDailySettingAsync();
            // 当前已经领取的红包数量
            var nowRedEnvelopeCount = await oaRedEnvelopeCacheManager.IncrementNumAsync(0);
            var lessCount = oaRedEnvelopeSettingModel?.MathRedEnvelopeCount() - nowRedEnvelopeCount;

            // 获取活动
            var activity = await DalActivity.GetActivityByTypeId(2);
            returnValue.ActivityRuleText = setting.ActivityRuleText;
            returnValue.AvgMoney = setting.AvgMoney;
            returnValue.ConditionCarModelFlag = setting.ConditionCarModelFlag;
            returnValue.ConditionPrice = setting.ConditionPrice;
            returnValue.ConditionPriceFlag = setting.ConditionPriceFlag;
            returnValue.DayMaxMoney = setting.DayMaxMoney;
            returnValue.FailTipText = setting.FailTipText;
            returnValue.OfficialAccountType = setting.OfficialAccountType;
            returnValue.QRCodeTipText = setting.QRCodeTipText;
            returnValue.QRCodeUrl = setting.QRCodeUrl;
            returnValue.SharePictureUrl = setting.SharePictureUrl;
            returnValue.ShareText = setting.ShareText;
            returnValue.ShareUrl = setting.ShareUrl;
            returnValue.ShareTitleText = setting.ShareTitleText;
            returnValue.StartTime = activity?.StartTime ?? DateTime.MinValue;
            returnValue.EndTime = activity?.EndTime ?? DateTime.MinValue;
            returnValue.Channel = setting.Channel;
            returnValue.OpenIdLegalDate = setting.OpenIdLegalDate;
            returnValue.RedEnvelopCount = (int)(lessCount ?? int.MaxValue);

            return returnValue;
        }

        /// <summary>
        ///     公众号领红包活动详情
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<OARedEnvelopeActivityInfoResponse>> OARedEnvelopeActivityInfoAsync(
            int officialAccountType = 1)
        {
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.OARedEnvelopeCacheHeader))
                {
                    var result = await cacheClient.GetOrSetAsync($"{nameof(OARedEnvelopeActivityInfoAsync)}"
                        , async () => await GetOARedEnvelopeActivityInfoAsync(officialAccountType)
                        , TimeSpan.FromMinutes(5));

                    // 返回数据
                    return OperationResult.FromResult(result?.Value);
                }
            }
            catch (Exception e)
            {
                Logger.Error("OARedEnvelopeManager -> OARedEnvelopeActivityInfoAsync -> error", e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     公众号领红包活动详情 - 无缓存
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<OARedEnvelopeActivityInfoResponse>>
            OARedEnvelopeActivityInfoNoCacheAsync(int officialAccountType = 1)
        {
            try
            {
                var result = await GetOARedEnvelopeActivityInfoAsync(officialAccountType);
                return OperationResult.FromResult(result);
            }
            catch (Exception e)
            {
                Logger.Error("OARedEnvelopeManager -> OARedEnvelopeActivityInfoNoCacheAsync -> error",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 用户领取

        /// <summary>
        ///     公众号领红包 - 用户领取 - 验证传参
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static Tuple<bool, string, string> ValidateUserReceiveRequestParameter(
            OARedEnvelopeUserReceiveRequest request)
        {
            if (request == null)
            {
                return Tuple.Create(false, "-5", Resource.ParameterError);
            }

            if (string.IsNullOrWhiteSpace(request.OpenId))
            {
                return Tuple.Create(false, "-5", Resource.ParameterError);
            }

            if (request.UserId == Guid.Empty)
            {
                return Tuple.Create(false, "-5", Resource.ParameterError);
            }

            return Tuple.Create(true, string.Empty, string.Empty);
        }


        /// <summary>
        ///     判断OpenId是否合法
        /// </summary>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, string>> ValidateOARedEnvelopeOpenIdLegalAsync(string openId,
            string channel, DateTime? date)
        {
            if (date == null)
            {
                return Tuple.Create(true, string.Empty, string.Empty);
            }

            using (var userAuthAccountClient = new UserAuthAccountClient())
            {
                var operationResult = await userAuthAccountClient.GetAllUserBindWxOfficialAccountInfoAsync(openId);
                var datas = operationResult.Result?.Where(p => p.Channel == channel).ToList();

                if (datas == null || !datas.Any())
                {
                    return Tuple.Create(false, "-21", Resource.OARedEnvelope_Follow);
                }

                if (datas.Count(p => p.BindingStatus == "Bound" && p.AuthorizationStatus == "Authorized") == 0)
                {
                    return Tuple.Create(false, "-21", Resource.OARedEnvelope_Follow);
                }

                if (datas?.Any(p => p.CreatedTime < date) ?? false)
                {
                    return Tuple.Create(false, "-80", Resource.OARedEnvelope_OldUser);
                }
            }
            return Tuple.Create(true, string.Empty, string.Empty);
        }

        /// <summary>
        ///     判断用户是否已经领取过了
        /// </summary>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, string>> ValidateOARedEnvelopeUserReceiveDoneAsync(Guid userId,
            string openId, int officialAccountType,bool isVerify)
        {
            if (isVerify)
            {
                openId = "";
            }

            var datas = await DalOARedEnvelopeDetail.GetOARedEnvelopeDetailAsync(userId, openId, officialAccountType,
                "");
            var flag = datas?.Count > 0;

            if (flag)
            {
                return Tuple.Create(false, "-30", Resource.OARedEnvelope_UserAlreadRecive);
            }

            return Tuple.Create(true, string.Empty, string.Empty);
        }


        /// <summary>
        ///     判断活动是否可用
        /// </summary>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, string>> ValidateOARedEnvelopeActivityAsync(
            OARedEnvelopeActivityInfoResponse activity)
        {
            await Task.Yield();
            var now = DateTime.Now;
            // 判断结束时间
            if (activity?.EndTime < now)
            {
                return Tuple.Create(false, "-40", Resource.OARedEnvelope_ActivityTimeError);
            }

            if (activity?.StartTime > now)
            {
                return Tuple.Create(false, "-40", Resource.OARedEnvelope_ActivityTimeError);
            }

            return Tuple.Create(true, string.Empty, string.Empty);
        }

        /// <summary>
        ///     获取用户订单列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static async Task<List<OrderModel>> GetUserOrderListAsync(Guid userId)
        {
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.GetOrderListByUserIdAsync(userId.ToString());
                    fetchResult.ThrowIfException();
                    return fetchResult?.Result?.ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error($" {nameof(OARedEnvelopeManager)} -> {nameof(GetUserOrderListAsync)} -> {userId} ");
                throw;
            }
        }

        /// <summary>
        ///     获取用户车型数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static async Task<List<UserVehicleInfoModel>> GetUserVehicleAsync(Guid userId)
        {
            try
            {
                using (var vehicleClient = new VehicleClient())
                {
                    var fetchResult = await vehicleClient.GetAllUserVehiclesAsync(userId);
                    fetchResult.ThrowIfException();
                    return fetchResult?.Result?.ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error($" {nameof(OARedEnvelopeManager)} -> {nameof(GetUserVehicleAsync)} -> {userId} ");
                throw;
            }
        }

        /// <summary>
        ///     获取第三方渠道
        /// </summary>
        /// <returns></returns>
        private static async Task<List<string>> GetThirdPartyOrderChannelAsync()
        {
            try
            {
                using (var cacheClient = CacheHelper.CreateCacheClient(GlobalConstant.OARedEnvelopeCacheHeader))
                {
                    var cacheResult = await cacheClient.GetOrSetAsync(nameof(GetThirdPartyOrderChannelAsync),
                        async () =>
                        {
                            using (var orderClient = new OrderQueryClient())
                            {
                                var fetchResult = await orderClient.Get3rdOrderChannelAsync();
                                fetchResult.ThrowIfException();
                                return fetchResult?.Result?.ToList();
                            }
                        }, TimeSpan.FromMinutes(5));
                    return cacheResult.Value;
                }
            }
            catch (Exception e)
            {
                Logger.Error($" {nameof(OARedEnvelopeManager)} -> {nameof(GetThirdPartyOrderChannelAsync)} ");
                throw;
            }
        }


        /// <summary>
        ///     公众号领红包 - 用户领取 - 验证用户是否满足领取条件
        /// </summary>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, string>> ValidateOARedEnvelopeUserLegalAsync(
            OARedEnvelopeUserReceiveRequest request, OARedEnvelopeActivityInfoResponse activity)
        {
            // 判断领取条件
            // 条件汽车
            var conditionCarModelFlag = activity.ConditionCarModelFlag;
            // 条件价格
            var conditionPriceFlag = activity.ConditionPriceFlag;
            // userid
            var userId = request.UserId;

            var taskList = new List<Task>();
            Task<List<OrderModel>> taskUserOrder = null;
            Task<List<UserVehicleInfoModel>> taskUserVehicle = null;
            Task<List<string>> taskThirdPartyOrderChannel = null;
            if (conditionCarModelFlag == 1 && activity.ConditionPrice > 0)
            {
                taskUserOrder = GetUserOrderListAsync(userId);
                taskThirdPartyOrderChannel = GetThirdPartyOrderChannelAsync();
                taskList.Add(taskUserOrder);
                taskList.Add(taskThirdPartyOrderChannel);
            }

            if (conditionPriceFlag == 1)
            {
                taskUserVehicle = GetUserVehicleAsync(userId);
                taskList.Add(taskUserVehicle);
            }

            await Task.WhenAll(taskList);

            // 订单验证 和 车型验证其中一个通过就可以
            // 订单
            if (taskUserOrder != null && taskThirdPartyOrderChannel != null)
            {
                // 获取第三方渠道
                var thirdPartyChannl = taskThirdPartyOrderChannel.Result;
                var userOrderList = taskUserOrder
                    .Result?
                    .Where(order =>
                    {
                        // 判断类型
                        if (thirdPartyChannl.Contains(order.OrderChannel))
                        {
                            return false;
                        }

                        // 判断订单是否完成
                        // 到店
                        if (order.InstallShopId > 0
                            && order.InstallType == "1ShopInstall"
                            && order.Status == "3Installed"
                            && order.InstallStatus == "2Installed")
                        {
                            return true;
                        }

                        // 到家
                        if ((order.InstallShopId == 0 || order.InstallShopId == null) &&
                            (order.Status == "6Complete" || order.DeliveryStatus == "3.5Signed"))
                        {
                            return true;
                        }

                        return false;
                    });
                var sumMoney = userOrderList?.Sum(p => p.SumPaid);
                // 如果总支付金额
                if (sumMoney >= activity.ConditionPrice)
                {
                    return Tuple.Create(true, string.Empty, string.Empty);
                }
            }

            // 车型
            if (taskUserVehicle != null)
            {
                var userVehicleInfoModels = taskUserVehicle.Result;
                // 取反 = 没有车型认证过
                if (userVehicleInfoModels?.Any(p => p.Status == 1) ?? false)
                {
                    return Tuple.Create(true, string.Empty, string.Empty);
                }
            }

            return Tuple.Create(false, "-50", Resource.OARedEnvelope_UserValidateError);
        }


        /// <summary>
        ///     公众号领红包 - 用户领取 - 验证
        /// </summary>
        /// <returns></returns>
        private static async Task<Tuple<bool, string, string>> ValidateOARedEnvelopeUserReceive(
            OARedEnvelopeUserReceiveRequest request, OARedEnvelopeActivityInfoResponse activityInfo,bool isVerify = false)
        {
            // 验证参数
            var result = ValidateUserReceiveRequestParameter(request);
            if (!result.Item1)
            {
                return result;
            }

            if (activityInfo == null)
            {
                return Tuple.Create(false, "-20", Resource.OARedEnvelope_ActivityError);
            }

            var validateOaRedEnvelopeUserReceiveDoneTask =
                ValidateOARedEnvelopeUserReceiveDoneAsync(request.UserId, request.OpenId, request.OfficialAccountType,isVerify);
            var validateOaRedEnvelopeActivityTask = ValidateOARedEnvelopeActivityAsync(activityInfo);
            var validateOaRedEnvelopeUserLegalTask = ValidateOARedEnvelopeUserLegalAsync(request, activityInfo);
            var validateOaRedEnvelopeOpenIdLegalTask = ValidateOARedEnvelopeOpenIdLegalAsync(request.OpenId,
                activityInfo.Channel,
                activityInfo.OpenIdLegalDate);
            await Task.WhenAll(validateOaRedEnvelopeUserReceiveDoneTask
                , validateOaRedEnvelopeActivityTask
                , validateOaRedEnvelopeUserLegalTask
                , validateOaRedEnvelopeOpenIdLegalTask
            );

            // 判断用户是否领取过
            result = validateOaRedEnvelopeUserReceiveDoneTask.Result;
            if (!result.Item1)
            {
                return result;
            }

            // 验证OpenId是否正常
            result = validateOaRedEnvelopeOpenIdLegalTask.Result;
            if (!result.Item1)
            {
                return result;
            }

            // 判断当前活动是否OK
            result = validateOaRedEnvelopeActivityTask.Result;
            if (!result.Item1)
            {
                return result;
            }

            // 验证是否满足条件
            result = validateOaRedEnvelopeUserLegalTask.Result;
            if (!result.Item1)
            {
                return result;
            }

            return Tuple.Create(true, string.Empty, string.Empty);
        }


        /// <summary>
        ///     公众号领红包 - 判断用户是否可以领取
        ///     返回值
        ///     -5  参数异常
        ///     -10 获取锁失败
        ///     -20 获取活动失败
        ///     -21 请先关注途虎养车公众号
        ///     -30 用户已经领取过了
        ///     -40 活动不可用
        ///     -50 订单或者车型不满足条件
        ///     -60 没有红包了
        ///     -70 用户已经领取过了 v2
        ///     -80 您已经是老用户了
        /// </summary>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeUserVerifyAsync(
            OARedEnvelopeUserVerifyRequest request)
        {
            try
            {
                await Task.Yield();

                // 获取当前活动
                var activityInfo = (await OARedEnvelopeActivityInfoAsync())?.Result;
                
                // 验证
                var result = await ValidateOARedEnvelopeUserReceive(new OARedEnvelopeUserReceiveRequest
                {
                    OfficialAccountType = request.OfficialAccountType,
                    OpenId = request.OpenId,
                    UserId = request.UserId
                }, activityInfo,true);

                if (!result.Item1)
                {
                    Logger.Info($"OARedEnvelopeManager -> OARedEnvelopeUserVerifyAsync -> {result.Item2} {result.Item3} {JsonConvert.SerializeObject(request)}");
                    return OperationResult.FromError<bool>(result.Item2, result.Item3);
                }

                var now = DateTime.Now;
                ;
                var builderModel = new OARedEnvelopeBuilderModel
                {
                    UserId = request.UserId,
                    OpenId = request.OpenId,
                    OfficialAccountType = request.OfficialAccountType,
                };
                var oaRedEnvelopeUserReceiveManager = new OARedEnvelopeUserReceiveManager(builderModel,
                    new OARedEnvelopeCacheManager(request.OfficialAccountType, now), now);

                var validateResult = await oaRedEnvelopeUserReceiveManager.ValidateAsync();

                if (!validateResult.Item1)
                {
                    Logger.Info($"OARedEnvelopeManager -> OARedEnvelopeUserVerifyAsync -> {validateResult.Item2} {validateResult.Item3} {JsonConvert.SerializeObject(request)}");
                    return OperationResult.FromError<bool>(validateResult.Item2, validateResult.Item3);
                }

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserVerifyAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     公众号领红包 - 用户领取
        ///     返回值
        ///     -1,-3  程序异常
        ///     -5  参数异常
        ///     -10 获取锁失败
        ///     -20 获取活动失败
        ///     -21 请先关注途虎养车公众号
        ///     -30 用户已经领取过了
        ///     -40 活动不可用
        ///     -50 订单或者车型不满足条件
        ///     -60 没有红包了
        ///     -70 用户已经领取过了 v2
        ///     -80 您已经是老用户了
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeUserReceiveAsync(
            OARedEnvelopeUserReceiveRequest request)
        {
            try
            {
                await Task.Yield();

                // 当前日期
                var now = DateTime.Now;

                // OpenId 作为唯一锁
                using (var zkLock = new ZooKeeperLock($"/OARedEnvelope/{request.OpenId}"))
                {
                    if (!await zkLock.WaitAsync(20000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<bool>("-10", Resource.Invalid_SystemError);
                    }


                    // 获取当前活动
                    var activityInfo = (await OARedEnvelopeActivityInfoAsync())?.Result;

                    // 验证
                    var result = await ValidateOARedEnvelopeUserReceive(request, activityInfo);

                    if (!result.Item1)
                    {
                        Logger.Info($"OARedEnvelopeManager -> OARedEnvelopeUserReceiveAsync -> {result.Item2} {result.Item3} {JsonConvert.SerializeObject(request)}");
                        return OperationResult.FromError<bool>(result.Item2, result.Item3);
                    }

                    var builderModel = new OARedEnvelopeBuilderModel
                    {
                        UserId = request.UserId,
                        OpenId = request.OpenId,
                        OfficialAccountType = request.OfficialAccountType,
                        WXHeadPicUrl = request.WXHeadPicUrl,
                        WXNickName = request.WXNickName,
                        ReferrerUserId = request.ReferrerUserId,
                        RequestTime = now
                    };
                    var oaRedEnvelopeUserReceiveManager = new OARedEnvelopeUserReceiveManager(builderModel,
                        new OARedEnvelopeCacheManager(request.OfficialAccountType, now), now);

                    try
                    {
                        // 锁定一个红包
                        var lockResult = await oaRedEnvelopeUserReceiveManager.LockRedEnvelopAsync();

                        if (!lockResult.Item1)
                        {

                            Logger.Info($"OARedEnvelopeManager -> OARedEnvelopeUserReceiveAsync ->  LockRedEnvelopAsync -> {lockResult.Item2} {lockResult.Item3} ->  {JsonConvert.SerializeObject(request)}");
                            //  回滚
                            await oaRedEnvelopeUserReceiveManager.RollBackAsync();
                            return OperationResult.FromError<bool>(lockResult.Item2, lockResult.Item3);
                        }

                        // 发送红包
                        var sendResult = await oaRedEnvelopeUserReceiveManager.SendRedEnvelopAsync();

                        if (!sendResult.Item1)
                        {
                            Logger.Info($"OARedEnvelopeManager -> OARedEnvelopeUserReceiveAsync ->  SendRedEnvelopAsync -> {sendResult.Item2} {sendResult.Item3} ->  {JsonConvert.SerializeObject(request)}");

                            //  回滚
                            await oaRedEnvelopeUserReceiveManager.RollBackAsync();
                            return OperationResult.FromError<bool>(lockResult.Item2, lockResult.Item3);
                        }
                    }
                    catch (Exception e)
                    {
                        await oaRedEnvelopeUserReceiveManager.RollBackAsync();
                        throw;
                    }
                }



                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserReceiveAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }


        /// <summary>
        ///     公众号领红包 - 用户领取 回调
        ///     返回值
        ///     -10 领取红包失败
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeUserReceiveCallbackAsync(
            OARedEnvelopeUserReceiveCallbackRequest request)
        {
            await Task.Yield();


            var builderModel = new OARedEnvelopeBuilderModel
            {
                UserId = request.UserId,
                OpenId = request.OpenId,
                OfficialAccountType = request.OfficialAccountType,
                IsRedEnvelopeGet = request.IsRedEnvelopeGet,
                IsTicketGet = request.IsTicketGet,
                Money = request.Money,
                WXHeadPicUrl = request.WXHeadPicUrl,
                WXNickName = request.WXNickName,
                ReferrerUserId = request.ReferrerUserId,
                RequestTime = request.RequestTime
            };
            var oaRedEnvelopeUserReceiveManager = new OARedEnvelopeUserReceiveManager(builderModel,
                new OARedEnvelopeCacheManager(request.OfficialAccountType, request.RequestTime), request.RequestTime);

            try
            {
                // 调用用户领红包回调
                var callbackResult = await oaRedEnvelopeUserReceiveManager.CallbackRedEnvelopAsync();

                if (!callbackResult.Item1)
                {
                    return OperationResult.FromError<bool>(callbackResult.Item2, callbackResult.Item3);
                }

                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                // 回滚
                await oaRedEnvelopeUserReceiveManager.RollBackAsync();
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeUserReceiveCallbackAsync -> error -> {JsonConvert.SerializeObject(request)}",
                    e.InnerException ?? e);
                throw;
            }
        }

        #endregion

        #region 公众号领红包 - 每日数据初始化

        /// <summary>
        ///     公众号领红包 - 删除每日初始化数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="officialAccountType"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeDailyDataInitDeleteAsync(DateTime date,
            int officialAccountType = 1)
        {
            await Task.Yield();

            try
            {
                Logger.Info(
                    $"OARedEnvelopeDailyDataInitAsync -> OARedEnvelopeDailyDataInitDeleteAsync -> start -> {date} {officialAccountType} ");

                if (officialAccountType == 0)
                {
                    officialAccountType = 1;
                }

                var oaRedEnvelopeCacheManager = new OARedEnvelopeCacheManager(officialAccountType, date);

                var incrCount = await oaRedEnvelopeCacheManager.IncrementCountAsync();

                await Task.WhenAll(
                    oaRedEnvelopeCacheManager.DeleteOARedEnvelopeObjectListAsync(),
                    oaRedEnvelopeCacheManager.DeleteDailySettingAsync(),
                    //  归0
                    oaRedEnvelopeCacheManager.IncrementNumAsync((int)-incrCount)
                );
                return OperationResult.FromResult(true);
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeDailyDataInitDeleteAsync -> error -> {date} {officialAccountType} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     公众号领红包 - 每日数据初始化
        ///     此方法一般是 当天 23点 生成 明天的数据 - Job 触发
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> OARedEnvelopeDailyDataInitAsync(
            OARedEnvelopeDailyDataInitRequest request)
        {
            await Task.Yield();
            try
            {
                using (var zkLock = new ZooKeeperLock($"{nameof(OARedEnvelopeDailyDataInitAsync)}"))
                {
                    if (!await zkLock.WaitAsync(30000)) //如果锁释放 会立即执行，正常逻辑不会等待5秒钟
                    {
                        return OperationResult.FromError<bool>("-2", "");
                    }

                    if (request == null)
                    {
                        return OperationResult.FromError<bool>("-10", "");
                    }

                    Logger.Info(
                        $"OARedEnvelopeDailyDataInitAsync -> OARedEnvelopeDailyDataInitAsync -> start -> {JsonConvert.SerializeObject(request)}");


                    // 初始化
                    var date = request.Date.Date;

                    var officialAccountType = request.OfficialAccountType;

                    if (officialAccountType == 0)
                    {
                        officialAccountType = 1;
                    }


                    // 获取当前设置
                    var oaRedEnvelopeSettingModel =
                        await DalOARedEnvelopeSetting.GetOARedEnvelopeSettingAsync(officialAccountType);

                    if (oaRedEnvelopeSettingModel == null)
                    {
                        // 设置为空
                        return OperationResult.FromError<bool>("-3", "");
                    }

                    var oaRedEnvelopeCacheManager = new OARedEnvelopeCacheManager(officialAccountType, date);

                    // 强制生成数据
                    if (!request.IsForce)
                    {
                        // 获取当前缓存数据是否已经生成了
                        var oaRedEnvelopeObjectModel = await oaRedEnvelopeCacheManager.GetOARedEnvelopeObjectAsync(1);
                        if (oaRedEnvelopeObjectModel != null)
                        {
                            return OperationResult.FromError<bool>("-4", "");
                        }
                    }

                    // 设置的变量
                    var dayMaxMoney = oaRedEnvelopeSettingModel.DayMaxMoney;
                    var avgMoney = oaRedEnvelopeSettingModel.AvgMoney;
                    // 不能为空
                    if (avgMoney == 0)
                    {
                        return OperationResult.FromError<bool>("-5", "");
                    }

                    // 红包总数
                    var redEnvelopCount = oaRedEnvelopeSettingModel.MathRedEnvelopeCount();

                    // 生成红包数组
                    var redEnvelopList = CalueHB(dayMaxMoney, oaRedEnvelopeSettingModel.PerMaxMoney,
                            oaRedEnvelopeSettingModel.PerMinMoney, redEnvelopCount)
                        .Select((p, u) => new OARedEnvelopeObjectModel
                        {
                            Position = u + 1,
                            Money = p
                        })
                        .ToList();
                    Logger.Info(
                        $"OARedEnvelopeDailyDataInitAsync -> OARedEnvelopeDailyDataInitAsync -> before SaveOARedEnvelopeObjectListAsync -> {redEnvelopList.Count}");


                    // 怼到CACHE里面
                    await Task.WhenAll(
                        oaRedEnvelopeCacheManager.SaveOARedEnvelopeObjectListAsync(redEnvelopList),
                        oaRedEnvelopeCacheManager.SaveDailySettingAsync(oaRedEnvelopeSettingModel)
                    );

                    return OperationResult.FromResult(true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(
                    $"OARedEnvelopeManager -> OARedEnvelopeDailyDataInitAsync -> error -> {JsonConvert.SerializeObject(request)} ",
                    e.InnerException ?? e);
                throw;
            }
        }

        /// <summary>
        ///     生成红包数组
        /// </summary>
        /// <param name="totalMoney">总金额</param>
        /// <param name="perMax">最大金额</param>
        /// <param name="perMin">最小金额</param>
        /// <param name="totalUser">总人数</param>
        private static decimal[] CalueHB(decimal totalMoney, decimal perMax, decimal perMin, int totalUser)
        {
            var i = 0; //第几人
            var array = new decimal[totalUser]; //分配结果集           
            var ran = new Random();

            for (i = 0; i < totalUser; i++) //保证每个人有最小金额+随机值
            {
                array[i] = perMin + Math.Round(
                               Convert.ToDecimal(ran.NextDouble()) * Math.Abs(totalMoney / totalUser - perMin - 1), 2);
            }

            var yet = array.Sum(); // 已分配的总金额
            var thisM = 0M; //当前随机分配金额

            while (yet < totalMoney)
            {
                thisM = Math.Round(Convert.ToDecimal(ran.NextDouble()) * Math.Abs(perMax - perMin - 1), 2);

                i = ran.Next(0, totalUser); //随机选择人
                if (yet + thisM > totalMoney)
                {
                    thisM = totalMoney - yet;
                }

                if (array[i] + thisM < perMax) //判断是否超出最大金额
                {
                    array[i] += thisM;
                    yet += thisM;
                }
            }

            // 如果生成总额大于总金额
            while (yet > totalMoney)
            {
                {
                    var m = yet - totalMoney;
                    i = ran.Next(0, totalUser); //随机选择人
                    // 减少此人金额
                    thisM = array[i];

                    var lessMoney = thisM - perMin;

                    if (lessMoney > 0)
                    {
                        if (lessMoney > m)
                        {
                            lessMoney = m;
                        }

                        thisM = thisM - lessMoney;
                        array[i] = thisM;
                        yet = yet - lessMoney;
                    }

                }
            }



            return array;
        }

        #endregion
    }
}
