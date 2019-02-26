using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.WashCarCoupon;
using Tuhu.Service.Activity.Server.Manager;

namespace Tuhu.Service.Activity.Server
{
    /// <summary>
    /// 一分钱洗车优惠券 服务层
    /// </summary>
    public class WashCarCouponService : IWashCarCouponService
    {
        /// <summary>
        /// 新增 一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request)
        =>await WashCarCouponRecordManager.CreateWashCarCouponAsync(request);

       
        /// <summary>
        /// 根据userid获取  一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request)
        => await WashCarCouponRecordManager.GetWashCarCouponListByUseridsAsync(request);

        /// <summary>
        /// 根据优惠券id获取  一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request)
        => await WashCarCouponRecordManager.GetWashCarCouponInfoByPromotionCodeIDAsync(request);

    }
}
