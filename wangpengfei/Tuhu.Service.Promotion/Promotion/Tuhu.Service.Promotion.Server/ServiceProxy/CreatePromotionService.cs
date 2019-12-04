using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tuhu.Service.CreatePromotion;
using Tuhu.Service.CreatePromotion.Models;

namespace Tuhu.Service.Promotion.Server.ServiceProxy
{

    public interface ICreatePromotionService
    {
        Task<OperationResult<bool>> RefreshGetCouponRulesCache(Guid getRuleGuid);

        /// <summary>
        /// 批量创建优惠券 无GUID
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task<OperationResult<CreatePromotionCodeResult>> CreatePromotionsForYeWuAsync(IEnumerable<CreatePromotionModel> models);

    }

    public class CreatePromotionService: ICreatePromotionService
    {
        private readonly ILogger _logger;
        private ICreatePromotionClient _Client;

        public CreatePromotionService(ILogger<CreatePromotionService> Logger, ICreatePromotionClient ICreatePromotionClient)
        {
            _logger = Logger;
            _Client = ICreatePromotionClient;
        }

        /// <summary>
        /// 刷新优惠券领取规则缓存
        /// </summary>
        /// <param name="getRuleGuid"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> RefreshGetCouponRulesCache(Guid getRuleGuid)
        {
            var result = await _Client.RefreshGetCouponRulesCacheAsync(getRuleGuid).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"CreatePromotionService RefreshGetCouponRulesCache fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}");
            }
            return result;
        }

        /// <summary>
        /// 批量发送优惠券 - 无guid
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<OperationResult<CreatePromotionCodeResult>> CreatePromotionsForYeWuAsync(IEnumerable<CreatePromotionModel> models)
        {
            var result = await _Client.CreatePromotionsForYeWuAsync(models).ConfigureAwait(false);
            result.ThrowIfException(true);
            if (!result.Success)
            {
                _logger.Error($"CreatePromotionService CreatePromotionsForYeWuAsync fail => ErrorCode ={result.ErrorCode} & ErrorMessage ={result.ErrorMessage}", result.Exception);
            }
            return result;
        }



    }
}
