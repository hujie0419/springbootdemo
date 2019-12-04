using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Models;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;

namespace Tuhu.Service.Promotion.Server.Manager.IManager
{
    public interface ICouponGetRuleManager
    {
        /// <summary>
        ///  批量获取领取规则
        /// </summary>
        /// <param name="GetRuleGUIDs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<IEnumerable<CouponGetRuleModel>> GetCouponGetRuleListAsync(IEnumerable<Guid> GetRuleGUIDs, CancellationToken cancellationToken);
        /// <summary>
        /// 审核成功 同步到领取规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> SaveAuditToGetRuleAsync(GetCouponRuleAuditModel request, CancellationToken cancellationToken);

        /// <summary>
        /// 根据pkid 获取实体
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<CouponGetRuleModel> GetByPKIDAsync(int PKID, CancellationToken cancellationToken);

        ValueTask<List<PromotionBusinessLineConfigResponse>> GetPromotionBusinessLineConfigListAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 查询优惠券领取规则 - 分页
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ValueTask<PagedModel<GetCouponRuleListResponse>> GetCouponRuleListAsync(GetCouponRuleListRequest request, CancellationToken cancellationToken);


    }
}
