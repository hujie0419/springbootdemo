using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.GroupBuying.Models.ResultModel;

namespace Tuhu.C.Job.ThirdPart.Proxy
{
    public class GroupBuyingServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GroupBuyingServiceProxy));
        /// <summary>
        /// 生成通用兑换码接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ResultModel<RedemptionResult>> GenerateUSRedemptionCode(GenerateUSRedemptionCodeRequest request)
        {
            var result = new ResultModel<RedemptionResult>() { IsSuccess = false };
            try
            {
                using (var client = new RedemptionCodeClient())
                {
                    var serviceResult = await client.GenerateUSRedemptionCodeAsync(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GroupBuying发通用兑换码接口异常", ex);
            }
            
            return result;
        }
        /// <summary>
        /// 兑换兑换码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<RedeemRedemptionCodeResult> RedeemRedemptionCodeByChoice(ReedemRedemptionRequest request)
        {
            var result = new RedeemRedemptionCodeResult() { Success = false };
            try
            {
                using (var client = new RedemptionCodeClient())
                {
                    var serviceResult = await client.RedeemRedemptionCodeByChoiceAsync(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GroupBuying兑换通用兑换码接口异常", ex);
            }

            return result;
        }
        /// <summary>
        /// 查询通用兑换码
        /// </summary>
        /// <param name="redemptionCode"></param>
        /// <returns></returns>
        public static async Task<RedemptionResult> GetRedemptionCodeDetail(string redemptionCode)
        {
            RedemptionResult result = null;
            try
            {
                using (var client = new RedemptionCodeClient())
                {
                    var serviceResult = await client.GetRedemptionCodeDetailAsync(redemptionCode);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GroupBuying查询通用兑换码接口异常", ex);
            }

            return result;
        }
        /// <summary>
        /// 作废通用兑换码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<InvalidateRedemptionCodeResult> InvalidateRedemptionCode(InvalidateRedemptionCodeRequest request)
        {
            InvalidateRedemptionCodeResult result = null;

            try
            {
                using (var client = new RedemptionCodeClient())
                {
                    var serviceResult = await client.InvalidateRedemptionCodeAsync(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GroupBuying作废通用兑换码接口异常", ex);
            }

            return result;
        }
    }
}
