using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Product.ModelDefinition.ProductModels;
using Tuhu.Service.Promotion.Request;
using Tuhu.Service.Promotion.Response;
using Tuhu.Service.Promotion.Server.Model;
using Tuhu.Service.Shop.Models;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Service.Promotion.Server.Manager.IManager
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public interface ICouponTaskManager
    {
        /// <summary>
        ///  获取优惠券 任务列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskModel>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken);


        /// <summary>
        ///  获取优惠券 任务列表 [cache]
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskModel>> GetPromotionTaskListMemoryCacheAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken);


        /// <summary>
        ///  发券
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<SendCouponResponse> SendCouponAsync(SendCouponRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 根据任务id获取 发券规则
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<List<PromotionTaskPromotionListModel>> GetCouponRuleByTaskIDAsync(GetCouponRuleByTaskIDRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken);


        /// <summary>
        ///  验证 发送历史
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int> CheckSendHistoryAsync(SendCouponRequest request, User user, CancellationToken cancellationToken);


        /// <summary>
        /// 验证单个订单和优惠券任务是否匹配，并更新不匹配原因
        /// </summary>
        /// <param name="couponTask"></param>
        /// <param name="order"></param>
        /// <param name="productList"></param>
        /// <param name="shop"></param>
        /// <returns></returns>
        Task<bool> CheckPromotionTaskWithOrderAsync(GetPromotionTaskListByOrderIdRequest request,PromotionTaskModel couponTask, OrderModel order, List<ProductBaseInfo> productList, ShopModel shop, List<ChannelDictionariesModel> orderChannelList,CancellationToken cancellationToken);


        /// <summary>
        ///  批量发送优惠券并更新统计次数  ,记录已发日志
        /// </summary>
        /// <param name="couponRuleList"></param>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<string> SendCouponsLogicAsync(List<PromotionTaskPromotionListModel> couponRuleList, User user, SendCouponRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 发送通知 【短信和推送】
        /// </summary>
        /// <param name="couponRuleList"></param>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool> SendNoyificationAsync(List<PromotionTaskPromotionListModel> couponRuleList, User user, SendCouponRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// 获取所有订单渠道
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<ChannelDictionariesModel>> GetAllOrderChannelMemoryCacheAsync(CancellationToken cancellationToken);

    }
}
