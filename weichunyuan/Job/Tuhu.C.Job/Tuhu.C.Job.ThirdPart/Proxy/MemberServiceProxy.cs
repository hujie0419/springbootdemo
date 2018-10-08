using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Member.Request;
using Tuhu.Service.CreatePromotion;

namespace Tuhu.C.Job.ThirdPart.Proxy
{
    class MemberServiceProxy
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MemberServiceProxy));
        /// <summary>
        /// 发券(批量)
        /// </summary>
        /// <param name="models"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static async Task<Tuhu.Service.CreatePromotion.Models.CreatePromotionCodeResult> CreatePromotions(IEnumerable<Tuhu.Service.CreatePromotion.Models.CreatePromotionModel> models)
        {
            Tuhu.Service.CreatePromotion.Models.CreatePromotionCodeResult result = null;
            try
            {
                using (var client = new CreatePromotionClient())
                {
                    var serviceResult = await client.CreatePromotionsNewAsync(models);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"赛券服务报错,Request:{JsonConvert.SerializeObject(models)}", ex);
            }           

            return result;
        }
        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static async Task<PromotionCodeModel> FetchPromotionByPromotionCode(string promotionId)
        {
            PromotionCodeModel result = null;

            try
            {
                using (var client = new PromotionClient())
                {
                    var serviceResult = await client.FetchPromotionByPromotionCodeAsync(promotionId);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"查询优惠券出错,promotionId:{promotionId}", ex);
            }
           
            return result;
        }
        /// <summary>
        /// 更新用户优惠券使用状态（主要改为已作废）(0.未使用，1.已使用，2.已作废
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateUserPromotionCodeStatus(UpdateUserPromotionCodeStatusRequest request)
        {
            bool result = false;

            try
            {
                using (var client = new PromotionClient())
                {
                    var serviceResult = await client.UpdateUserPromotionCodeStatusAsync(request);
                    serviceResult.ThrowIfException(true);
                    result = serviceResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"更新优惠券状态出错,request:{JsonConvert.SerializeObject(request)}", ex);
            }           

            return result;
        }
    }
}
