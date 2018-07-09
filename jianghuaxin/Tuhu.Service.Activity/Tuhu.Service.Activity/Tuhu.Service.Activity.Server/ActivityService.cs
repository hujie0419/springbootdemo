using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Tuhu.Models;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Activity;
using Tuhu.Service.Activity.Models.Questionnaire;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.FlashSaleSystem;
using Tuhu.Service.Activity.Server.Manager;
using Tuhu.Service.Product;
using Question = Tuhu.Service.Activity.Models.Response.Question;

namespace Tuhu.Service.Activity.Server
{
    public class ActivityService : IActivityService
    {

        private static ILog Logger = LogManager.GetLogger(typeof(ActivityService));
        public Task<OperationResult<DownloadApp>> GetActivityConfigForDownloadAppAsync(int id) =>
            OperationResult.FromResultAsync(ActivityManager.GetActivityConfigForDownloadApp(id));

        public Task<OperationResult<bool>> CleanActivityConfigForDownloadAppCacheAsync(int id) =>
            OperationResult.FromResultAsync(ActivityManager.CleanActivityConfigForDownloadAppCache(id));

        public Task<OperationResult<TireActivityModel>> SelectTireActivityAsync(string vehicleId, string tireSize) =>
            OperationResult.FromResultAsync(ActivityManager.SelectTireActivityAsync(vehicleId, tireSize));

        public Task<OperationResult<TireActivityModel>> SelectTireActivityByActivityIdAsync(Guid activityId)
            => OperationResult.FromResultAsync(ActivityManager.SelectTireActivityByActivityIdAsync(activityId));

        public Task<OperationResult<IEnumerable<string>>> SelectTireActivityPidsAsync(Guid activityId) =>
            OperationResult.FromResultAsync(ActivityManager.SelectTireActivityPidsAsync(activityId));

        public Task<OperationResult<bool>> UpdateTireActivityCacheAsync(string vehicleId, string tireSize) =>
            OperationResult.FromResultAsync(ActivityManager.UpadeTireActivityAsync(vehicleId, tireSize));

        public Task<OperationResult<bool>> UpdateActivityPidsCacheAsync(Guid activityId) =>
            OperationResult.FromResultAsync(ActivityManager.UpadeTireActivityPidsAsync(activityId));


        /// <summary>
        /// 获取车型适配轮胎信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<VehicleAdaptTireTireSizeDetailModel>>> SelectVehicleAaptTiresAsync(
            VehicleAdaptTireRequestModel request)
        {
            if (!string.IsNullOrWhiteSpace(request?.VehicleId) && request?.TireSizes != null && request.TireSizes.Any())
            {
                return OperationResult.FromResult(await ActivityManager.SelectVehicleAaptTiresAsync(request));
            }
            else
            {
                return OperationResult.FromError<List<VehicleAdaptTireTireSizeDetailModel>>(ErrorCode.ParameterError,
                    "参数不能为空");
            }
        }


        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<CarTagCouponConfigModel>>> SelectCarTagCouponConfigsAsync()
        {
            return OperationResult.FromResult(await ActivityManager.SelectCarTagCouponConfigsAsync());
        }

        /// <summary>
        /// 获取车型适配保养信息
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<VehicleAdaptBaoyangModel>>> SelectVehicleAaptBaoyangsAsync(
            string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                return OperationResult.FromError<IEnumerable<VehicleAdaptBaoyangModel>>(ErrorCode.ParameterError,
                    "车型Id不能为空");
            }
            return OperationResult.FromResult(await ActivityManager.SelectVehicleAaptBaoyangsAsync(vehicleId));
        }


        /// <summary>
        /// 获取车型适配车品信息
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<OperationResult<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>>
            SelectVehicleAdaptChepinsAsync(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                return
                    OperationResult.FromError<IDictionary<string, IEnumerable<VehicleAdaptChepinDetailModel>>>(
                        ErrorCode.ParameterError, "车型Id不能为空");
            }
            return OperationResult.FromResult(await ActivityManager.SelectVehicleAdaptChepinsAsync(vehicleId));
        }


        /// <summary>
        /// 获取排序后的轮胎规格
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<VehicleSortedTireSizeModel>>> SelectVehicleSortedTireSizesAsync(
            string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                return OperationResult.FromError<IEnumerable<VehicleSortedTireSizeModel>>(ErrorCode.ParameterError,
                    "车型Id不能为空");
            }
            return OperationResult.FromResult(await ActivityManager.SelectVehicleSortedTireSizesAsync(vehicleId));
        }

        /// <summary>
        /// 插入用户分享信息并返回guid，分享赚钱
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="batchGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<Guid>> GetGuidAndInsertUserShareInfoAsync(string pid, Guid batchGuid,
            Guid userId)
        {
            if (pid == null || batchGuid == Guid.Empty || userId == Guid.Empty)
            {
                return OperationResult.FromError<Guid>(ErrorCode.ParameterError, "参数不能为空");
            }
            else
            {
                var shareId = Guid.NewGuid();
                var result = await ActivityManager.GetGuidAndInsertUserShareInfoAsync(shareId, pid, batchGuid, userId);
                if (result == 0)
                {
                    return OperationResult.FromError<Guid>(ErrorCode.DataNotExisted, "数据插入失败，请检查参数是否有错");
                }
                else
                {
                    return OperationResult.FromResult(shareId);
                }
            }
        }

        /// <summary>
        /// 根据Guid取出写入表中的数据
        /// </summary>
        /// <param name="shareId"></param>
        /// <returns></returns>
        public async Task<OperationResult<ActivityUserShareInfoModel>> GetActivityUserShareInfoAsync(Guid shareId)
        {
            if (shareId == Guid.Empty)
            {
                return OperationResult.FromError<ActivityUserShareInfoModel>(ErrorCode.ParameterError, "参数不能为空");
            }
            else
            {
                return OperationResult.FromResult(await ActivityManager.GetActivityUserShareInfoAsync(shareId));
            }
        }



        public async Task<OperationResult<IEnumerable<PromotionPacketHistoryModel>>> SelectPromotionPacketHistoryAsync(
            Guid userId, Guid luckyWheel)
        {
            return
                OperationResult.FromResult(await ActivityManager.SelectPromotionPacketHistoryAsync(userId, luckyWheel));
        }

        /// <summary>
        /// 根据配置表Guid跟用户id取出生成的新id，推荐有礼
        /// </summary>
        /// <param name="configGuid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<Guid>> GetGuidAndInsertUserForShareAsync(Guid configGuid, Guid userId)
        {
            if (configGuid == Guid.Empty || userId == Guid.Empty)
            {
                return OperationResult.FromError<Guid>(ErrorCode.ParameterError, "参数不能为空");
            }
            else
            {
                var shareId = Guid.NewGuid();
                var result = await ActivityManager.GetGuidAndInsertUserForShareAsync(shareId, configGuid, userId);
                if (result == 0)
                {
                    return OperationResult.FromError<Guid>(ErrorCode.DataNotExisted, "数据插入失败");
                }
                else
                {
                    return OperationResult.FromResult(shareId);
                }
            }
        }

        public async Task<OperationResult<RecommendGetGiftConfigModel>> FetchRecommendGetGiftConfigAsync(
            Guid? number = null, Guid? userId = null)
        {
            return OperationResult.FromResult(await ActivityManager.FetchRecommendGetGiftConfigAsync(number, userId));
        }


        /// <summary>
        /// 查询礼包领取
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult<DataTable>> SelectPacketByUsersAsync()
        {
            return OperationResult.FromResultAsync(ActivityManager.SelectPacketByUsersAsync());
        }

        public Task<OperationResult<RegionActivityPageModel>> GetRegionActivityPageUrlAsync(string city,
            string activityId)
        {
            return OperationResult.FromResultAsync(ActivityManager.GetRegionActivityPageUrlAsync(city, activityId));
        }

        #region 分地区分车型活动页配置
        [Obsolete("请使用GetRegionVehicleIdActivityUrlNewAsync")]
        public async Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlAsync(Guid activityId, int regionId, string vehicleId)
        {
            if (activityId != Guid.Empty)
            {
                var activityChannel = "kH5";
                return OperationResult.FromResult(await ActivityManager.GetRegionVehicleIdTargetUrl(activityId, regionId, vehicleId, activityChannel));
            }
            return OperationResult.FromError<ResultModel<string>>(ErrorCode.ParameterError, "参数错误");
        }

        /// <summary>
        /// 获取分地区分车型活动页地址
        /// 传入渠道 kH5 / WXAPP
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="regionId"></param>
        /// <param name="vehicleId"></param>
        /// <param name="activityChannel"></param>
        /// <returns></returns>
        public async Task<OperationResult<ResultModel<string>>> GetRegionVehicleIdActivityUrlNewAsync(Guid activityId, int regionId, string vehicleId, string activityChannel)
        {
            if (activityId != Guid.Empty && (regionId > 0 || !string.IsNullOrEmpty(vehicleId)) && !string.IsNullOrEmpty(activityChannel))
            {
                return OperationResult.FromResult(await ActivityManager.GetRegionVehicleIdTargetUrl(activityId, regionId, vehicleId, activityChannel));
            }
            return OperationResult.FromError<ResultModel<string>>(ErrorCode.ParameterError, "参数错误");
        }

        /// <summary>
        /// 刷新分地区分车型活动页缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshRegionVehicleIdActivityUrlCacheAsync(Guid activityId)
        {
            if (activityId != Guid.Empty)
            {
                return OperationResult.FromResult(await ActivityManager.RefreshRegionVehicleIdTargetUrlCache(activityId));
            }
            return OperationResult.FromError<bool>(ErrorCode.ParameterError, "参数错误");
        }
        #endregion

        /// <summary>
        /// 取消相同支付账户的订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="paymentAccount"></param>
        /// <returns>
        /// -1:失败 1:取消成功 2:无取消订单
        /// </returns>
        public async Task<OperationResult<int>> CancelActivityOrderOfSamePaymentAccountAsync(int orderId,
            string paymentAccount)
        {
            return await OperationResult.FromResultAsync(Task.Run(() => 1));
        }

        public async Task<OperationResult<TireActivityModel>> SelectTireActivityNewAsync(TireActivityRequest request)
            => OperationResult.FromResult(await ActivityManager.SelectTireActivityNewAsync(request));

        public async Task<OperationResult<ActivePageListModel>> GetActivePageListModelAsync(ActivtyPageRequest request)
        {
            if (!string.IsNullOrEmpty(request.CityId) && !int.TryParse(request.CityId, out int region))
            {
                return OperationResult.FromError<ActivePageListModel>(ErrorCode.ParameterError, "城市id传的不对");
            }
            return OperationResult.FromResult(await ActivityManager.GetActivePageListModel(request.Id, request, request.HashKey));
        }


        /// <summary>
        /// 大翻盘用户可翻盘次数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userGroup"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> GetLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup, string hashkey = null)
        {
            return
                OperationResult.FromResult(await ActivityManager.GetLuckyWheelUserlotteryCountAsync(userid, userGroup, hashkey));
        }

        /// <summary>
        /// 更新大翻盘用户可翻盘次数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="userGroup"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public async Task<OperationResult<int>> UpdateLuckyWheelUserlotteryCountAsync(Guid userid, Guid userGroup, string hashkey = null)
        {
            return
                OperationResult.FromResult(await ActivityManager.UpdateLuckyWheelUserlotteryCountAsync(userid, userGroup, hashkey));
        }

        public async Task<OperationResult<bool>> RefreshActivePageListModelCacheAsync(ActivtyPageRequest request)
        {
            return
                OperationResult.FromResult(await ActivityManager.RefreshActivePageListModelCache(request.Id,
                    request.Channel, request.HashKey));
        }

        public async Task<OperationResult<bool>> RefreshLuckWheelCacheAsync(string id)
        {
            return OperationResult.FromResult(await Task.Run(() => ActivityManager.RefreshLuckWheelCache(id)));
        }
        /// <summary>
        /// 验证是否可以领取轮胎优惠券
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationTiresResponseModel>> VerificationTiresPromotionRuleAsync(VerificationTiresRequestModel requestModel, int ruleId)
        {
            if (ruleId == 122315)
            {
                return await ActivityManager.VerificationByTiresAsync(requestModel);
            }
            else
            {
                return OperationResult.FromResult(new VerificationTiresResponseModel
                {
                    Result = true,
                    HitRules = new List<HitRulesModel>()
                });
            }
        }
        /// <summary>
        /// 验证是否可以购买轮胎
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<OperationResult<VerificationTiresResponseModel>> VerificationByTiresAsync(
            VerificationTiresRequestModel requestModel)
            => await ActivityManager.VerificationByTiresAsync(requestModel);

        /// <summary>
        /// 增加轮胎购买数量
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> InsertTiresOrderRecordAsync(TiresOrderRecordRequestModel requestModel)
            => await ActivityManager.InsertTiresOrderRecordAsync(requestModel);

        /// <summary>
        /// 撤销轮胎购买数量
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RevokeTiresOrderRecordAsync(int orderId)
            => await ActivityManager.RevokeTiresOrderRecordAsync(orderId);

        /// <summary>
        /// 重建Redis数据
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RedisTiresOrderRecordReconStructionAsync(
            TiresOrderRecordRequestModel requestModel)
            => await ActivityManager.RedisTiresOrderRecordReconStructionAsync(requestModel);

        /// <summary>
        /// 查询数据库和Redis中的数据
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task<OperationResult<Dictionary<string, object>>> SelectTiresOrderRecordAsync(
            TiresOrderRecordRequestModel requestModel)
            => await ActivityManager.SelectTiresOrderRecordAsync(requestModel);

        public async Task<OperationResult<LuckyWheelModel>> GetLuckWheelAsync(string id)
        {
            return OperationResult.FromResult(await Task.Run(() => ActivityManager.GetLuckWheel(id)));
        }

        /// <summary>
        /// 分享赚钱
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="BatchGuid"></param>
        /// <returns></returns>
        public async Task<OperationResult<ShareProductModel>> SelectShareActivityProductByIdAsync(string ProductId,
            string BatchGuid = null)
        {
            if (string.IsNullOrEmpty(ProductId))
            {
                return OperationResult.FromError<ShareProductModel>(ErrorCode.ParameterError, "参数不正确");
            }
            else
            {
                var data = await ActivityManager.SelectShareActivityProductById(ProductId, BatchGuid);
                return OperationResult.FromResult<ShareProductModel>(data);

            }
        }

        public Task<OperationResult<BaoYangActivitySetting>> SelectBaoYangActivitySettingAsync(string activityId)
            => OperationResult.FromResultAsync(ActivityManager.SelectBaoYangActivitySetting(activityId));

        public Task<OperationResult<CouponActivityConfigModel>> SelectCouponActivityConfigAsync(string activityNum,
            int type)
            => OperationResult.FromResultAsync(ActivityManager.SelectCouponActivityConfig(activityNum, type));


        public async Task<OperationResult<IEnumerable<ActivityTypeModel>>> SelectActivityTypeByActivityIdsAsync(List<Guid> activityIds)

        {
            if (activityIds == null || !activityIds.Any())
            {
                return null;
            }


            return await OperationResult.FromResultAsync(Task.Run(() => ActivityManager.SelectActivityTypeByActivityIds(activityIds)));

        }

        public async Task<OperationResult<ActivityBuild>> GetActivityBuildWithSelKeyAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;
            return await OperationResult.FromResultAsync(ActivityManager.GetActivityBuildWithSelKeyAsync(keyword));
        }

        public Task<OperationResult<bool>> RecordActivityTypeLogAsync(ActivityTypeRequest request)
            => OperationResult.FromResultAsync(ActivityManager.RecordActivityTypeLogAsync(request));

        /// <summary>
        /// 更新保养活动配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateBaoYangActivityConfigAsync(Guid activityId)
            => OperationResult.FromResultAsync(ActivityCache.UpdateBaoYangActivityConfig(activityId));

        /// <summary>
        /// 获取保养活动状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="userId"></param>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public Task<OperationResult<FixedPriceActivityStatusResult>> GetFixedPriceActivityStatusAsync(Guid activityId, Guid userId, int regionId)
            => OperationResult.FromResultAsync(FlashSaleManager.GetFixedPriceActivityStatus(activityId, userId, regionId));

        /// <summary>
        /// 重置活动计数器
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateBaoYangPurchaseCountAsync(Guid activityId)
            => OperationResult.FromResultAsync(ActivityCache.UpdateBaoYangPurchaseCount(activityId));

        /// <summary>
        /// 根据activityId获取当前活动配置
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public Task<OperationResult<FixedPriceActivityRoundResponse>> GetFixedPriceActivityRoundAsync(Guid activityId)
            => OperationResult.FromResultAsync(ActivityCache.GetFixedPriceActivityRoundCache(activityId));

        #region 轮胎活动
        public async Task<OperationResult<TiresActivityResponse>> FetchRegionTiresActivityAsync(FlashSaleTiresActivityRequest request)
        {
            if (request != null && request.ActivityId != Guid.Empty && request.RegionId > 0)
            {
                return OperationResult.FromResult(await ActivityManager.FetchRegionTiresActivity(request));
            }
            else
            {
                return OperationResult.FromError<TiresActivityResponse>(ErrorCode.ParameterError, "参数无效");
            }
        }

        public async Task<OperationResult<bool>> RefreshRegionTiresActivityCacheAsync(Guid activityId, int regionId)
            => await OperationResult.FromResultAsync(ActivityManager.RefreshRegionTiresActivityCache(activityId, regionId));

        public async Task<OperationResult<bool>> RecordActivityProductUserRemindLogAsync(ActivityProductUserRemindRequest request)
        {
            if (request.UserId == Guid.Empty || string.IsNullOrEmpty(request.ActivityId) ||
                string.IsNullOrEmpty(request.ActivityName) || string.IsNullOrEmpty(request.Pid) ||
                string.IsNullOrEmpty(request.PorductName))
                return OperationResult.FromError<bool>(nameof(Resource.ParameterError), Resource.ParameterError);
            return OperationResult.FromResult(await ActivityManager.RecordActivityProductUserRemindLogAsync(request));
        }

        public async Task<OperationResult<string>> SelectTireChangedActivityAsync(TireActivityRequest request)
        {
            if (string.IsNullOrEmpty(request.VehicleId) || string.IsNullOrEmpty(request.TireSize))
            {
                return OperationResult.FromError<string>(ErrorCode.ParameterError, "参数无效,VehicleId与TireSize必须有值");
            }

            return OperationResult.FromResult(await ActivityManager.SelectTireChangedActivityAsync(request));
        }

        #endregion

        #region  返现申请记录
        /// <summary>
        /// 添加返现申请记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> InsertRebateApplyRecordAsync(RebateApplyRequest request)
        {
            return await OperationResult.FromResultAsync(Task.FromResult(false));
        }

        /// <summary>
        /// 返现申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordNewAsync(RebateApplyRequest request)
        {
            return await OperationResult.FromResultAsync(Task.FromResult(new ResultModel<bool>
            {
                Code = 1001,
                Msg = "每个客户只能参与一次（包含手机号、订单号、微信号）均视为同一客户"
            }));
        }

        /// <summary>
        /// 申请返现
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<ResultModel<bool>>> InsertRebateApplyRecordV2Async(RebateApplyRequest request)
        {
            if (request.OrderId > 0 && !string.IsNullOrEmpty(request.UserPhone) &&
                !string.IsNullOrEmpty(request.OpenId))
            {
                return OperationResult.FromResult(await ActivityManager.InsertRebateApplyRecordNewAsync(request));
            }
            else
            {
                return OperationResult.FromError<ResultModel<bool>>(nameof(Resource.ParameterError), Resource.ParameterError);
            }
        }

        /// <summary>
        /// 获取用户所有返现申请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<RebateApplyResponse>>> SelectRebateApplyByOpenIdAsync(string openId)
        {
            return OperationResult.FromResult(await ActivityManager.SelectRebateApplyByOpenIdAsync(openId));
        }

        /// <summary>
        /// 获取返现申请页面配置
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<RebateApplyPageConfig>> SelectRebateApplyPageConfigAsync()
        {
            return OperationResult.FromResult(await ActivityManager.SelectRebateApplyPageConfigAsync());
        }

        public async Task<OperationResult<bool>> InsertOrUpdateActivityPageWhiteListRecordsAsync(List<ActivityPageWhiteListRequest> requests)
        {
            return OperationResult.FromResult(await ActivityManager.InsertOrUpdateActivityPageWhiteListRecordsAsync(requests));
        }

        public async Task<OperationResult<bool>> GetActivityPageWhiteListByUserIdAsync(Guid userId)
        {
            return OperationResult.FromResult(await ActivityManager.GetActivityPageWhiteListByUserIdAsync(userId));
        }

        public async Task<OperationResult<UserRewardApplicationResponse>> PutUserRewardApplicationAsync(UserRewardApplicationRequest request)
        {
            return await ActivityManager.PutUserRewardApplicationAsync(request);
        }

        public async Task<OperationResult<bool>> PutApplyCompensateRecordAsync(ApplyCompensateRequest request)
        {
            return await ActivityManager.PutApplyCompensateRecordAsync(request);
        }

        public async Task<OperationResult<List<ActivtyValidityResponse>>> GetActivtyValidityResponsesAsync(ActivtyValidityRequest request)
        {
            return OperationResult.FromResult(await ActivityManager.GetActivtyValidityResponsesAsync(request));
        }

        public async Task<OperationResult<List<VipCardSaleConfigDetailModel>>> GetVipCardSaleConfigDetailsAsync(string activityId)
        {
            return OperationResult.FromResult(await ActivityManager.GetVipCardSaleConfigDetailsAsync(activityId));
        }

        public async Task<OperationResult<Dictionary<string, bool>>> VipCardCheckStockAsync(List<string> batchIds)
        {
            return OperationResult.FromResult(await ActivityManager.VipCardCheckStockAsync(batchIds));
        }

        public async Task<OperationResult<bool>> PutVipCardRecordAsync(VipCardRecordRequest request)
        {
            return OperationResult.FromResult(await ActivityManager.PutVipCardRecordAsync(request));
        }

        public async Task<OperationResult<bool>> BindVipCardAsync(int orderId)
        {
            return OperationResult.FromResult(await ActivityManager.BindVipCardAsync(orderId));
        }

        public async Task<OperationResult<bool>> ModifyVipCardRecordByOrderIdAsync(int orderId)
        {
            return OperationResult.FromResult(await ActivityManager.ModifyVipCardRecordByOrderIdAsync(orderId));
        }

        #endregion


        #region 活动

        /// <summary>
        ///     获取2018世界杯的活动对象和积分规则信息
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<ActivityResponse>> GetWorldCup2018ActivityAsync()
        {
            return await ActivityManager.GetWorldCup2018ActivityAsync();
        }

        /// <summary>
        ///      通过用户ID获取兑换券数量接口
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<int>> GetCouponCountByUserIdAsync(Guid userId, long activityId)
        {
            return await ActivityManager.GetCouponCountByUserIdAsync(userId ,activityId);
        }

        /// <summary>
        ///     返回活动兑换券排行排名
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<ActivityCouponRankResponse>>> SearchCouponRankAsync(long activityId, int pageIndex = 1, int pageSize = 20)
        {
            return await ActivityManager.SearchCouponRankAsync(activityId, pageIndex, pageSize);

        }

        /// <summary>
        ///     返回用户的兑换券排名情况
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<ActivityCouponRankResponse>> GetUserCouponRankAsync(Guid userId, long activityId)
        {
            return await ActivityManager.GetUserCouponRankAsync(userId, activityId);
        }


        /// <summary>
        ///     返回兑换物列表
        /// </summary>
        /// <param name="searchPrizeListRequest">请求对象</param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<ActivityPrizeResponse>>> SearchPrizeListAsync(SearchPrizeListRequest searchPrizeListRequest)
        {
            return await ActivityManager.SearchPrizeListAsync(searchPrizeListRequest);
        }

        /// <summary>
        ///      用户兑换奖品
        ///      异常代码：-1 系统异常（请重试） , -2 兑换卡不足  -3 库存不足  -4 已经下架  -5 已经兑换  -6 兑换时间已经截止不能兑换
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="prizeId">兑换物ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> UserRedeemPrizesAsync(Guid userId, long prizeId, long activityId)
        {
            var result = await ActivityManager.UserRedeemPrizesAsync(userId, prizeId, activityId);
            if (!result.Success)
            {
                Logger.Info($"用户兑换奖品失败 {userId} {prizeId} {activityId} {result?.ErrorCode} {result?.ErrorMessage} ");
            }
            return result;
        }

        /// <summary>
        ///     用户已兑换商品列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<ActivityPrizeOrderDetailResponse>>> SearchPrizeOrderDetailListByUserIdAsync(Guid userId, long activityId, int pageIndex = 1, int pageSize = 20)
        {
            return await ActivityManager.SearchPrizeOrderDetailListByUserIdAsync(userId, activityId, pageIndex,
                pageSize);
        }
        
        /// <summary>
        ///     今日竞猜题目
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<Models.Response.Question>>> SearchQuestionAsync(Guid userId , long activityId)
        {
            return await ActivityManager.SearchQuestionAsync(userId, activityId);
        }

        /// <summary>
        ///     提交用户竞猜
        ///      -1 请重试  -2 参数异常   -3 时间已经截止    -4 用户积分不足
        /// </summary>
        /// <param name="submitQuestionAnswerRequest"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> SubmitQuestionAnswerAsync(SubmitQuestionAnswerRequest submitQuestionAnswerRequest)
        {
            var result = await ActivityManager.SubmitQuestionAnswerAsync(submitQuestionAnswerRequest);
            if (!result.Success)
            {
                Logger.Info($"提交用户竞猜失败 {submitQuestionAnswerRequest?.UserId}  {submitQuestionAnswerRequest?.ActivityId} {submitQuestionAnswerRequest?.OptionId} {result?.ErrorCode} {result?.ErrorMessage} ");
            }
            return result;
        }

        /// <summary>
        ///      返回用户答题历史 
        /// </summary>
        /// <param name="searchQuestionAnswerHistoryRequest">请求对象</param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<QuestionUserAnswerHistoryResponse>>> SearchQuestionAnswerHistoryByUserIdAsync(SearchQuestionAnswerHistoryRequest searchQuestionAnswerHistoryRequest)
        {
            return await ActivityManager.SearchQuestionAnswerHistoryByUserIdAsync(searchQuestionAnswerHistoryRequest);
        }

        /// <summary>
        ///     返回用户胜利次数和胜利称号
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public async Task<OperationResult<ActivityVictoryInfoResponse>> GetVictoryInfoAsync(Guid userId, long activityId)
        {
            return await ActivityManager.GetVictoryInfoAsync(userId,activityId);
        }

        /// <summary>
        ///     活动分享赠送积分 
        ///     异常：   -77 活动未开始  -2 今日已经分享   -1 系统异常
        /// </summary>
        /// <param name="shareDetailRequest">分享的对象</param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ActivityShareAsync(ActivityShareDetailRequest shareDetailRequest)
        {
            return await ActivityManager.ActivityShareAsync(shareDetailRequest);
        }

        /// <summary>
        ///     今日是否已经分享了
        ///     true = 今日已经分享
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> ActivityTodayAlreadyShareAsync(Guid userId, long activityId)
        {
            return await ActivityManager.ActivityTodayAlreadyShareAsync(userId, activityId);
        }

        public async Task<OperationResult<List<string>>> GetOrSetActivityPageSortedPidsAsync(SortedPidsRequest request)
        {
            var mResult = await ActivityManager.GetOrSetActivityPageSortedPidsAsync(request);
            if (mResult.Item1.Key==1)
                return OperationResult.FromResult(mResult.Item2);
            else
            {
                return OperationResult.FromError<List<string>> (mResult.Item1.Key.ToString(), mResult.Item1.Value);
            }
        }

        /// <summary>
        ///     保存用户答题数据到数据库
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<SubmitActivityQuestionUserAnswerResponse>> SubmitQuestionUserAnswerAsync(SubmitActivityQuestionUserAnswerRequest request)
        {
            return await ActivityManager.SubmitQuestionUserAnswerAsync(request);
        }



        /// <summary>
        ///     修改或者增加用户兑换券  返回主键
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <param name="couponCount"></param>
        /// <returns></returns>
        public async Task<OperationResult<long>> ModifyActivityCouponAsync(Guid userId, long activityId, int couponCount, string couponName, DateTime? modifyDateTime = null)
        {
            return await ActivityManager.ModifyActivityCouponAsync(userId, activityId, couponCount , couponName, modifyDateTime);
        }

        /// <summary>
        ///      刷新活动题目  缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshActivityQuestionCacheAsync(long activityId)
        {
            return await ActivityManager.RefreshActivityQuestionCacheAsync(activityId);
        }

        /// <summary>
        ///      刷新活动兑换物  缓存
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshActivityPrizeCacheAsync(long activityId)
        {
            return await ActivityManager.RefreshActivityPrizeCacheAsync(activityId);
        }


        /// <summary>
        ///     更新用户答题结果状态
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult<ModifyQuestionUserAnswerResultResponse>> ModifyQuestionUserAnswerResultAsync(ModifyQuestionUserAnswerResultRequest request)
        {
            return await ActivityManager.ModifyQuestionUserAnswerResultAsync(request);
        }

        #region 活动页接口

        public async Task<OperationResult<ActivityPageInfoModel>> GetActivityPageInfoConfigModelAsync(ActivityPageInfoRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoManager().GetActivityPageInfoConfigModelAsync(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoHomeModel>>> GetActivityPageInfoHomeModelsAsync(string hashKey)
        {
            return OperationResult.FromResult(await new ActivityPageInfoManager().GetActivityPageInfoHomeModelsAsync(hashKey));
        }

        public async Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoCpRecommendsAsync(ActivityPageInfoModuleRecommendRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoManager().GetActivityPageInfoCpRecommendsAsync(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRecommend>>> GetActivityPageInfoTireRecommendsAsync(ActivityPageInfoModuleRecommendRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoManager().GetActivityPageInfoTireRecommendsAsync(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowMenuModel>>> GetActivityPageInfoRowMenusAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoMenuManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowProductPool>>> GetActivityPageInfoRowPoolsAsync(ActivityPageInfoModuleProductPoolRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoProductPoolManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowNewProductPool>>> GetActivityPageInfoRowNewPoolsAsync(ActivityPageInfoModuleNewProductPoolRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoNewProductPoolManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowCountDown>>> GetActivityPageInfoRowCountDownsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoCountDownManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowActivityText>>> GetActivityPageInfoRowActivityTextsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoActivityTextManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowJson>>> GetActivityPageInfoRowJsonsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoJsonManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowPintuan>>> GetActivityPageInfoRowPintuansAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoPintuanManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowVideo>>> GetActivityPageInfoRowVideosAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoVideoManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowOtherActivity>>> GetActivityPageInfoRowOtherActivitysAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoOtherActivityManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowOther>>> GetActivityPageInfoRowOthersAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoOtherManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowRule>>> GetActivityPageInfoRowRulesAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoRuleManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowBy>>> GetActivityPageInfoRowBysAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoByManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowCoupon>>> GetActivityPageInfoRowCouponsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoCouponManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowProduct>>> GetActivityPageInfoRowProductsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoProductManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowLink>>> GetActivityPageInfoRowLinksAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoLinkManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowImage>>> GetActivityPageInfoRowImagesAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoImageManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowVehicleBanner>>> GetActivityPageInfoRowVehicleBannersAsync(ActivityPageInfoModuleVehicleBannerRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoVehicleBannerManager().GetActivityPageInfoRowByTypes(request));
        }

        public async Task<OperationResult<List<ActivityPageInfoRowSeckill>>> GetActivityPageInfoRowSeckillsAsync(ActivityPageInfoModuleBaseRequest request)
        {
            return OperationResult.FromResult(await new ActivityPageInfoSeckillManager().GetActivityPageInfoRowByTypes(request));
        }

        #endregion
        #endregion



        #region 新人需求

        /// <summary>
        /// 新增报名信息
        /// </summary>
        /// <param name="infomodel"></param>
        /// <returns></returns>
        public Task<OperationResult<bool>> AddEnrollInfoAsync(TEnrollInfoModel infomodel)
           => OperationResult.FromResultAsync(ActivityManager.AddEnrollInfo(infomodel));

        /// <summary>
        /// 修改报名信息
        /// </summary>
        /// <param name="infomodel"></param>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateEnrollInfoAsync(TEnrollInfoModel infomodel)
           => OperationResult.FromResultAsync(ActivityManager.UpdateEnrollInfo(infomodel));

        /// <summary>
        /// 上海市闵行区用户的报名状态为已通过
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public Task<OperationResult<bool>> UpdateUserEnrollStatusAsync(string area)
           => OperationResult.FromResultAsync(ActivityManager.UpdateUserEnrollStatus(area));

        /// <summary>
        /// 根据id获取model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelAsync(int id)
        {
            return OperationResult.FromResult(await ActivityManager.GetEnrollInfoModel(id));
        }

        /// <summary>
        /// 根据手机号获取model
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        public async Task<OperationResult<TEnrollInfoModel>> GetEnrollInfoModelByTelAsync(string tel)
        {
            return OperationResult.FromResult(await ActivityManager.GetEnrollInfoModelByTel(tel));
        }

        /// <summary>
        /// 根据区域条件查询用户信息
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public async Task<OperationResult<PagedModel<TEnrollInfoModel>>> SelectEnrollInfoByAreaAsync(
            string area, int pageIndex = 1, int pageSize = 20)
        {
            return OperationResult.FromResult(await ActivityManager.SelectEnrollInfoByArea(area, pageIndex, pageSize));
        }
        #endregion
    }
}
