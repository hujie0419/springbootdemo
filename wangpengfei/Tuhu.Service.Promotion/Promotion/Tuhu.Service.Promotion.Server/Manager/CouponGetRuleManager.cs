using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Promotion.DataAccess.Entity;
using Tuhu.Service.Promotion.DataAccess.Repository;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Config;
using Tuhu.Service.Promotion.Server.Manager.IManager;
using Tuhu.Service.Promotion.Server.ServiceProxy;

namespace Tuhu.Service.Promotion.Server.Manager
{
    /// <summary>
    /// 优惠券领取规则 逻辑层
    /// </summary>
    public class CouponGetRuleManager : ICouponGetRuleManager
    {
        private readonly ILogger _logger;
        private readonly ICacheHelper _ICacheHelper;
        /// <summary>
        /// 优惠券领取规则
        /// </summary>
        private readonly ICouponGetRuleRepository _ICouponGetRuleRepository;
        private readonly ICouponUseRuleRepository _ICouponUseRuleRepository;
        /// <summary>
        /// 优惠券领取规则审核
        /// </summary>
        private readonly ICouponGetRuleAuditRepository _ICouponGetRuleAuditRepository;
        private readonly IConfigBaseService _IConfigBaseService;
        private readonly AppSettingOptions _AppSettingOptions;


        public CouponGetRuleManager(
            ILogger<CouponGetRuleManager> Logger,
            ICacheHelper ICacheHelper,
            ICouponGetRuleRepository ICouponGetRuleRepository,
            ICouponUseRuleRepository ICouponUseRuleRepository,
            ICouponGetRuleAuditRepository ICouponGetRuleAuditRepository,
            IConfigBaseService IConfigBaseService,
            IOptionsSnapshot<AppSettingOptions> AppSettingOptions
        )
        {
            _logger = Logger;
            _ICacheHelper = ICacheHelper;
            _ICouponGetRuleRepository = ICouponGetRuleRepository;
            _ICouponUseRuleRepository = ICouponUseRuleRepository;
            _ICouponGetRuleAuditRepository = ICouponGetRuleAuditRepository;
            _IConfigBaseService = IConfigBaseService;
            _AppSettingOptions = AppSettingOptions.Value;
        }

        /// <summary>
        /// 批量获取领取规则
        /// </summary>
        /// <param name="GetRuleGUIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<CouponGetRuleModel>> GetCouponGetRuleListAsync(IEnumerable<Guid> GetRuleGUIDs, CancellationToken cancellationToken)
        {
            List<CouponGetRuleModel> result = new List<CouponGetRuleModel>();

            if (GetRuleGUIDs != null && GetRuleGUIDs.Any())
            {
                foreach (var guid in GetRuleGUIDs)
                {
                    var rule = await GetCouponGetRuleCahceByGuidAsync(guid, cancellationToken).ConfigureAwait(false);
                    if (rule != null)
                    {
                        result.Add(rule);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据guid获取优惠券领取规则 【加缓存】
        /// </summary>
        /// <param name="GetRuleGUID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CouponGetRuleModel> GetCouponGetRuleCahceByGuidAsync(Guid GetRuleGUID, CancellationToken cancellationToken)
        {
            CouponGetRuleEntity entity = new CouponGetRuleEntity();
            using (var client = _ICacheHelper.CreateCacheClient(GlobalConstant.RedisClient))
            {
                var result = await client.GetOrSetAsync(string.Format(GlobalConstant.RedisKeyForGetRule, GetRuleGUID),
                                                           async () => await _ICouponGetRuleRepository.GetCouponGetRuleByGuidAsync(GetRuleGUID, cancellationToken).ConfigureAwait(false),
                                                           GlobalConstant.RedisTTLForGetRule).ConfigureAwait(false);
                if (result.Success)
                {
                    entity = result.Value;
                }
                else
                {
                    entity = await _ICouponGetRuleRepository.GetCouponGetRuleByGuidAsync(GetRuleGUID, cancellationToken).ConfigureAwait(false);
                }
            }
            CouponGetRuleModel model = ObjectMapper.ConvertTo<CouponGetRuleEntity, CouponGetRuleModel>(entity);
            return model;
        }

        /// <summary>
        /// 审核成功 同步到领取规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<int> SaveAuditToGetRuleAsync(GetCouponRuleAuditModel request, CancellationToken cancellationToken)
        {
            int pkid = 0;
            try
            {
                CouponGetRuleEntity entity = ObjectMapper.ConvertTo<GetCouponRuleAuditModel, CouponGetRuleEntity>(request);
                entity.PKID = request.GetCouponRulePKID;
                if (entity.PKID > 0)
                {
                    pkid = await _ICouponGetRuleRepository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false) ? entity.PKID : 0;

                }
                else
                {
                    pkid = await _ICouponGetRuleRepository.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
                }
                return pkid;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateCouponGetRuleAuditAsync Exception {JsonConvert.SerializeObject(request)}", ex);
                return 0;
            }
        }

        public async ValueTask<CouponGetRuleModel> GetByPKIDAsync(int PKID, CancellationToken cancellationToken)
        {
            CouponGetRuleEntity entity = await _ICouponGetRuleRepository.GetByPKIDAsync(PKID, cancellationToken).ConfigureAwait(false);
            CouponGetRuleModel model = ObjectMapper.ConvertTo<CouponGetRuleEntity, CouponGetRuleModel>(entity);
            return model;
        }

        /// <summary>
        /// 获取所有业务线 [不包括已删除]
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionBusinessLineConfigResponse>> GetPromotionBusinessLineConfigListAsync(CancellationToken cancellationToken)
        {
            var result = await _ICouponGetRuleRepository.GetPromotionBusinessLineConfigListAsync(cancellationToken).ConfigureAwait(false);
            List<PromotionBusinessLineConfigResponse> models = ObjectMapper.ConvertTo<PromotionBusinessLineConfigEntity, PromotionBusinessLineConfigResponse>(result).ToList();
            return models;
        }

        public async ValueTask<PagedModel<GetCouponRuleListResponse>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken)
        {
            PagedModel<GetCouponRuleListResponse> result = new PagedModel<GetCouponRuleListResponse>()
            {
                Pager = new PagerModel()
                {
                    CurrentPage = request.CurrentPage,
                    PageSize = request.PageSize
                }
            };
            int count = await _ICouponGetRuleRepository.GetCouponRuleCountAsync(request, cancellationToken).ConfigureAwait(false);
            result.Pager.Total = count;
            List<GetCouponRuleListResponse> source = await _ICouponGetRuleRepository.GetCouponRuleListAsync(request, cancellationToken).ConfigureAwait(false);
            result.Source = source;
            return result;
        }
    }
}
