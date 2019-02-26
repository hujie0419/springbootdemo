using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.WashCarCoupon;

namespace Tuhu.Service.Activity.Server.Manager
{
    /// <summary>
    /// 一分钱洗车优惠券 业务层
    /// </summary>
    public class WashCarCouponRecordManager
    {
        /// <summary>
        ///  新增 一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<bool>> CreateWashCarCouponAsync(WashCarCouponRecordModel request)
        {
            try
            {
                #region 新增时 数据验证
                string validMSG = "";
                if (request == null)
                {
                    validMSG = "请求参数不能为空";
                }
                else if (string.IsNullOrWhiteSpace(request.CarNo))
                {
                    validMSG = "车牌号不能为空";
                }
                else if (Guid.Empty == request.UserID)
                {
                    validMSG = "用户不能为空";
                }
                else if (request.PromotionCodeID <= 0)
                {
                    validMSG = "优惠券不能为空";
                }
                if (!string.IsNullOrWhiteSpace(validMSG))
                {
                    return OperationResult.FromError<bool>("-1", validMSG);
                }
                #endregion
                WashCarCouponRecordEntity entity = ObjectMapper.ConvertTo<WashCarCouponRecordModel, WashCarCouponRecordEntity>(request);
                var result  =  await DalWashCarCouponRecord.CreateWashCarCouponAsync(entity);
                return OperationResult.FromResult(result > 0);
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<bool>("500", ex.Message);
            }
        }

        /// <summary>
        /// 根据userid获取  一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<List<WashCarCouponRecordModel>>> GetWashCarCouponListByUseridsAsync(GetWashCarCouponListByUseridsRequest request)
        {
            List<WashCarCouponRecordModel> models = new List<WashCarCouponRecordModel>();
            try
            {
                var result = await DalWashCarCouponRecord.GetWashCarCouponListByUseridsAsync(request.UserID);
                models = ObjectMapper.ConvertTo<WashCarCouponRecordEntity, WashCarCouponRecordModel>(result).ToList();
                return OperationResult.FromResult(models);
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<List<WashCarCouponRecordModel>>("500", ex.Message);
            }
        }

        /// <summary>
        /// 根据优惠券id获取  一分钱洗车优惠券领取记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<OperationResult<WashCarCouponRecordModel>> GetWashCarCouponInfoByPromotionCodeIDAsync(GetWashCarCouponInfoByPromotionCodeIDRequest request)
        {
            WashCarCouponRecordModel models = new WashCarCouponRecordModel();
            try
            {
                var result = await DalWashCarCouponRecord.GetWashCarCouponListByPromotionCodeIDAsync(request.PromotionCodeID);
                models = ObjectMapper.ConvertTo<WashCarCouponRecordEntity, WashCarCouponRecordModel>(result).FirstOrDefault();
                return OperationResult.FromResult(models);
            }
            catch (Exception ex)
            {
                return OperationResult.FromError<WashCarCouponRecordModel>("500", ex.Message);
            }
        }

    }
}
