using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     请求类：兑换奖品列表
    /// </summary>
    public class SearchPrizeListRequest
    {
        /// <summary>
        ///     用户ID 没有则是 Guid.Empty
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     活动ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///    true = 当前可兑换
        /// </summary>
        public bool? ShowCanPay { get; set; }

        /// <summary>
        ///     页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        ///     分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        ///     是否拒绝缓存  0 不拒绝 1 直接读库 2 读库并且刷新缓存
        /// </summary>
        public int IgnoreCache { get; set; }
    }
}
