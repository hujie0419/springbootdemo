using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Manager;
using Tuhu.Service.Promotion.Server.Manager.IManager;

namespace Tuhu.Service.Promotion.Server
{
    /// <summary>
    /// 优惠券查询服务
    /// </summary>
    public class PromotionController : PromotionService
    {
        public ICouponManager _CouponManager;
        public ICouponGetRuleManager _CouponGetRuleManager;
        private readonly IPromotionCodeRepository promotionCodeRepository;
        private readonly ILogger _logger;
        public PromotionController(ICouponManager ICouponManager,
            ILogger<CouponManager> Logger,
            IPromotionCodeRepository IPromotionCodeRepository,
            ICouponGetRuleManager ICouponGetRuleManager
            )
        {
            _CouponManager = ICouponManager;
            _logger = Logger;
            _CouponGetRuleManager = ICouponGetRuleManager;
            promotionCodeRepository = IPromotionCodeRepository;
        }

        /// <summary>
        /// 获取用户的优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<List<CouponModel>>> GetCouponByUserIDAsync([FromBody] GetCouponByUserIDRequest request)
        {
            _logger.Info($"PromotionController GetCouponByUserIDAsync 1");
            return OperationResult.FromResult(await _CouponManager.GetCouponByUserIDAsync(request, HttpContext.RequestAborted).ConfigureAwait(false));
        }

        /// <summary>
        /// 根据pkid获取用户优惠券接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<GetCouponByIDResponse>> GetCouponByIDAsync([FromBody]  GetCouponByIDRequest request)
        {
            return await _CouponManager.GetCouponByIDAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
        }

        /// <summary>
        /// 延期优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<bool>> DelayCouponEndTimeAsync([FromBody] DelayCouponEndTimeRequest request)
        {
            if (!request.IsPassed)
            {
                return OperationResult.FromError<bool>("-1", request.ErrorMsg);
            }
            return await _CouponManager.DelayCouponEndTimeAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
        }


        /// <summary>
        /// 更改优惠券的金额
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<bool>> UpdateCouponReduceCostAsync([FromBody] UpdateCouponReduceCostRequest request)
        {
            if (!request.IsPassed)
            {
                return OperationResult.FromError<bool>("-1", request.ErrorMsg);
            }
            return await _CouponManager.UpdateCouponReduceCostAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
        }

        #region 作废优惠券

        /// <summary>
        /// 作废优惠券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<bool>> ObsoleteCouponAsync([FromBody] ObsoleteCouponRequest request)
        {
            if (!request.IsPassed)
            {
                return OperationResult.FromError<bool>("-1", request.ErrorMsg);
            }
            return await _CouponManager.ObsoleteCouponAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
        }

        /// <summary>
        ///  作废优惠券 - 批量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListAsync([FromBody] ObsoleteCouponListRequest request)
        {
            if (!request.IsPassed)
            {
                return OperationResult.FromError<ObsoleteCouponListReponse>("-1", request.ErrorMsg);
            }
            if (request.IsObsoleteAll)
            {
                GetCouponByUserIDRequest getCouponByUserIDRequest = new GetCouponByUserIDRequest()
                {
                    UserID = request.UserID,
                    IsHistory = 2
                };
                var couponList = await _CouponManager.GetCouponByUserIDAsync(getCouponByUserIDRequest, HttpContext.RequestAborted).ConfigureAwait(false);
                request.PromotionPKIDList = couponList.Where(p=>p.Status == 0).Select(p => p.PKID).ToList() ;
            }
            ObsoleteCouponListReponse response = new ObsoleteCouponListReponse();
            ObsoleteCouponRequest ObsoleteCouponRequest = ObjectMapper.ConvertTo<ObsoleteCouponListRequest, ObsoleteCouponRequest>(request);
            foreach (var item in request.PromotionPKIDList)
            {
                ObsoleteCouponRequest.PromotionPKID = item;
                var  flag = await _CouponManager.ObsoleteCouponAsync(ObsoleteCouponRequest, HttpContext.RequestAborted).ConfigureAwait(false);
                if (flag.Result)
                {
                    response.SuccessedIDs += ObsoleteCouponRequest.PromotionPKID + ",";
                }
                else
                {
                    response.FaidedIDs += ObsoleteCouponRequest.PromotionPKID + ",";
                }
            }
            return OperationResult.FromResult(response);
        }

        /// <summary>
        /// 批量作废优惠券 - 根据优惠券渠道
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async override ValueTask<OperationResult<ObsoleteCouponListReponse>> ObsoleteCouponListByChannelAsync([FromBody] ObsoleteCouponsByChannelRequest request)
        {
            if (!request.IsPassed)
            {
                return OperationResult.FromError<ObsoleteCouponListReponse>("-1", request.ErrorMsg);
            }

            //根据渠道和pkid筛选优惠券
            var couponList = await promotionCodeRepository.GetPKIDByCodeChannelANDPKIDAsync(request, HttpContext.RequestAborted).ConfigureAwait(false);
            couponList = couponList.Where(p => p.Status == 0).ToList();

            ObsoleteCouponListReponse response = new ObsoleteCouponListReponse();
            ObsoleteCouponRequest ObsoleteCouponRequest = ObjectMapper.ConvertTo<ObsoleteCouponsByChannelRequest, ObsoleteCouponRequest>(request);
            foreach (var item in couponList)
            {
                ObsoleteCouponRequest.PromotionPKID = item.PKID;
                ObsoleteCouponRequest.UserID = item.UserId;
                var flag = await _CouponManager.ObsoleteCouponAsync(ObsoleteCouponRequest, HttpContext.RequestAborted).ConfigureAwait(false);
                if (flag.Result)
                {
                    response.SuccessedIDs += ObsoleteCouponRequest.PromotionPKID + ",";
                }
                else
                {
                    response.FaidedIDs += ObsoleteCouponRequest.PromotionPKID + ",";
                }
            }
            return OperationResult.FromResult(response);
        }

        #endregion
    }
}
