using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.CreatePromotion.Models;

namespace Tuhu.Service.Activity.Server.ServiceProxy
{
    /// <summary>
    ///创建 优惠券服务代理
    /// </summary>
    public class CreatePromotionServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigServiceProxy));

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async static Task<OperationResult<CreatePromotionCodeResult>> CreatePromotionNewAsync(CreatePromotionModel model)
        {
            using (var client = new CreatePromotion.CreatePromotionClient())
            {
                var result = await client.CreatePromotionNewAsync(model);
                if (!result.Success)
                {
                    Logger.Error($"CreatePromotionClient CreatePromotionNewAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }
                return result;
            }
        }


        /// <summary>
        /// 创建优惠券 批量
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async static Task<OperationResult<CreatePromotionCodeResult>> CreatePromotionsNewAsync(List<CreatePromotionModel> models)
        {
            using (var client = new CreatePromotion.CreatePromotionClient())
            {
                var result = await client.CreatePromotionsNewAsync(models);
                if (!result.Success)
                {
                    Logger.Error($"CreatePromotionClient CreatePromotionsNewAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage} ");
                }
                return result;
            }
        }
    }
}
