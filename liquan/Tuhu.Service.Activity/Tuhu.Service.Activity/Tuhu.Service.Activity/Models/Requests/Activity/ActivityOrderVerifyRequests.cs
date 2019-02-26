using System;

namespace Tuhu.Service.Activity.Models.Requests.Activity
{
    /// <summary>
    /// 客户专享活动下单验证(锦湖轮胎)
    /// </summary>
    public class ActivityOrderVerifyRequests
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 限时抢购ID
        /// </summary>
        public string ActivityId { get; set; }

    }
}
