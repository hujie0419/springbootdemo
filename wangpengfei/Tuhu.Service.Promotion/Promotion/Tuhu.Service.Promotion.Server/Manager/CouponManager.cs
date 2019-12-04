using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Nosql;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.DataAccess.Repository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Model;

namespace Tuhu.Service.Promotion.Server.Manager
{
    public interface ICouponManager
    {
        /// <summary>
        /// 根据userid获取优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<CouponModel>> GetCouponByUserIDAsync(GetCouponByUserIDRequest request, CancellationToken cancellationToken);
        /// <summary>
        /// 根据id获取优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync(GetCouponByIDRequest request, CancellationToken cancellationToken);
        /// <summary>
        /// 延期优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<OperationResult<bool>> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 更改优惠券的金额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ValueTask<OperationResult<bool>> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 作废优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<OperationResult<bool>> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken);
    }

    public class CouponManager : ICouponManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        private readonly IPromotionCodeRepository _PromotionCodeRepository;
        private readonly ICouponGetRuleRepository _CouponGetRuleRepository;
        private readonly ICouponUseRuleRepository _CouponUseRuleRepository;
        private readonly IPromotionOprLogRepository _IPromotionOprLogRepository;


        /// <summary>
        /// 用户优惠券 逻辑层
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="ICacheHelper"></param>
        /// <param name="IPromotionCodeRepository"></param>
        public CouponManager(
            IPromotionOprLogRepository IPromotionOprLogRepository,
            ILogger<CouponManager> Logger,
            ICacheHelper ICacheHelper,
            IPromotionCodeRepository IPromotionCodeRepository,
            ICouponGetRuleRepository ICouponGetRuleRepository,
            ICouponUseRuleRepository ICouponUseRuleRepository
        )
        {
            _IPromotionOprLogRepository = IPromotionOprLogRepository;
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _CouponGetRuleRepository = ICouponGetRuleRepository;
            _CouponUseRuleRepository = ICouponUseRuleRepository;
            _PromotionCodeRepository = IPromotionCodeRepository;
        }
        /// <summary>
        /// 查询用户的优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<CouponModel>> GetCouponByUserIDAsync(GetCouponByUserIDRequest request, CancellationToken cancellationToken)
        {

            List<CouponModel> result = new List<CouponModel>();
            List<CouponModel> resultHistorysList = new List<CouponModel>();//归档用户优惠券
            List<CouponModel> resultNowList = new List<CouponModel>();//正在使用的用户优惠券

            try
            {
                if (request.IsHistory == 1 || request.IsHistory == 0)
                {
                    var entities = await _PromotionCodeRepository.GetHistoryCouponByUserIDAsync(request.UserID, cancellationToken).ConfigureAwait(false);
                    resultHistorysList = ObjectMapper.ConvertTo<PromotionCodeEntity, CouponModel>(entities).ToList();
                    foreach (var item in resultHistorysList)
                    {
                        item.IsHistory = 1;
                    }
                    result.AddRange(resultHistorysList);
                }
                if (request.IsHistory == 2 || request.IsHistory == 0)
                {
                    var entities = await _PromotionCodeRepository.GetCouponByUserIDAsync(request.UserID, cancellationToken).ConfigureAwait(false);
                    resultNowList = ObjectMapper.ConvertTo<PromotionCodeEntity, CouponModel>(entities).ToList();
                    result.AddRange(resultNowList);
                }
            }
            catch (Exception ex)
            {
                _logger.Info($"CouponManager GetCouponByUserIDAsync Exception", ex);
                throw;
            }

            return result;
        }

        /// <summary>
        /// 根据pkid获取用户优惠券接口
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync(GetCouponByIDRequest request, CancellationToken cancellationToken)
        {
            PromotionCodeEntity entity = await _PromotionCodeRepository.GetCouponByIDAsync(request.pkid, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                return OperationResult.FromError<GetCouponByIDResponse>("-1", "未获取到信息");
            }
            GetCouponByIDResponse result = ObjectMapper.ConvertTo<PromotionCodeEntity, GetCouponByIDResponse>(entity);
            result.ReduceCost = (Math.Abs(result.Discount - result.ReduceCost) < 1) ? result.ReduceCost : result.Discount;
            result.LeastCost = (Math.Abs(result.MinMoney ?? 0 - result.LeastCost) < 1) ? result.LeastCost : (result.MinMoney ?? 0);
            if (result.GetRuleID > 0)
            {
                var getRuleModel = await _CouponGetRuleRepository.GetByPKIDAsync(result.GetRuleID, cancellationToken).ConfigureAwait(false);
                result.GetRuleGUID = getRuleModel.GetRuleGUID;
                result.Quantity = getRuleModel.Quantity??0;
                result.GetQuantity = getRuleModel.GetQuantity;
                result.GetRuleName = getRuleModel.PromtionName;
            }
            if (result.RuleId > 0)
            {
                var useRuleModel = await _CouponUseRuleRepository.GetByPKIDAsync(result.RuleId, cancellationToken).ConfigureAwait(false);
                result.PromotionType = useRuleModel.PromotionType;
                result.RuleName = useRuleModel.Name;
                result.RuleDescription = useRuleModel.RuleDescription;
                result.OrderPayMethod = useRuleModel.OrderPayMethod;
                result.EnabledGroupBuy = useRuleModel.EnabledGroupBuy ? 1 : 0;
                result.InstallType = useRuleModel.InstallType.ToString();
            }
            return OperationResult.FromResult(result);
        }


        /// <summary>
        /// 延期优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<OperationResult<bool>> DelayCouponEndTimeAsync(DelayCouponEndTimeRequest request, CancellationToken cancellationToken)
        {
            var coupon = await _PromotionCodeRepository.GetCouponByIDAsync(request.PromotionPKID, cancellationToken).ConfigureAwait(false);
            if (coupon == null)
            {
                return OperationResult.FromError<bool>("-1", "优惠券不存在");
            }
            if (coupon.UserId != request.UserID)
            {
                return OperationResult.FromError<bool>("-2", "优惠券所属人不匹配 请检查入参userid");
            }
            if (coupon.Status != 0)
            {
                return OperationResult.FromError<bool>("-2", "优惠券已使用或者已作废");
            }

            var result = await _PromotionCodeRepository.DelayCouponEndTimeAsync(request, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                PromotionOprLogEntity entity = ObjectMapper.ConvertTo<DelayCouponEndTimeRequest, PromotionOprLogEntity> (request);
                entity.Referer = request.Message;
                entity.UserID = coupon.UserId;
                entity.Operation = CouponOperateEnum.延期.ToString();
                entity.OperationDetail= string.Format("OldEndTime={0};NewEndTime={1}", coupon.EndTime, request.EndTime);

                await _IPromotionOprLogRepository.CreateAsync(entity,cancellationToken).ConfigureAwait(false);
            }
            return OperationResult.FromResult(result);
        }

        public async ValueTask<OperationResult<bool>> UpdateCouponReduceCostAsync(UpdateCouponReduceCostRequest request, CancellationToken cancellationToken)
        {
            var coupon = await _PromotionCodeRepository.GetCouponByIDAsync(request.PromotionPKID, cancellationToken).ConfigureAwait(false);
            if (coupon == null)
            {
                return OperationResult.FromError<bool>("-1", "优惠券不存在");
            }
            if (coupon.UserId != request.UserID)
            {
                return OperationResult.FromError<bool>("-2", "优惠券所属人不匹配 请检查入参userid");
            }
            if (coupon.Status != 0)
            {
                return OperationResult.FromError<bool>("-2", "优惠券已使用或者已作废");
            }

            var result = await _PromotionCodeRepository.UpdateCouponReduceCostAsync(request, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                PromotionOprLogEntity entity = ObjectMapper.ConvertTo<UpdateCouponReduceCostRequest, PromotionOprLogEntity>(request);
                entity.Referer = request.Message;
                entity.UserID = coupon.UserId;
                entity.Operation = CouponOperateEnum.修改金额.ToString();
                entity.OperationDetail = string.Format("OldReduceCost={0};NewReduceCost={1}", coupon.ReduceCost, request.ReduceCost);

                await _IPromotionOprLogRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            }
            return OperationResult.FromResult(result);
        }

        /// <summary>
        /// 作废优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<OperationResult<bool>> ObsoleteCouponAsync(ObsoleteCouponRequest request, CancellationToken cancellationToken)
        {
            var coupon = await _PromotionCodeRepository.GetCouponByIDAsync(request.PromotionPKID, cancellationToken).ConfigureAwait(false);
            if (coupon == null)
            {
                return OperationResult.FromError<bool>("-1", "优惠券不存在");
            }
            if (coupon.UserId != request.UserID)
            {
                return OperationResult.FromError<bool>("-2", "优惠券所属人不匹配 请检查入参userid");
            }
            if (coupon.Status != 0)
            {
                return OperationResult.FromError<bool>("-2", "优惠券已使用或者已作废");
            }

            var result = await _PromotionCodeRepository.ObsoleteCouponAsync(request, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                PromotionOprLogEntity entity = ObjectMapper.ConvertTo<ObsoleteCouponRequest, PromotionOprLogEntity>(request);
                entity.Referer = request.Message;
                entity.UserID = coupon.UserId;
                entity.Operation = CouponOperateEnum.作废.ToString();
                entity.OperationDetail = string.Format("OldStatus={0};NewStatus={1}", coupon.Status, (int)CouponOperateEnum.作废);
                await _IPromotionOprLogRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            }
            return OperationResult.FromResult(result);
        }
    }
}
