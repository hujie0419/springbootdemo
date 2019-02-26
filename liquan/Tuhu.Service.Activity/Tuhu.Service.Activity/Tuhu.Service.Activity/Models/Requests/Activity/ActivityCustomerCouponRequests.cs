using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 验证是否绑券/券码绑定请求实体
    /// </summary>
    public class ActivityCustomerCouponRequests
    {
        /// <summary>
        /// 活动专享ID
        /// </summary>
        ///
        public string ActivityExclusiveId { get; set; }

        /// <summary>
        /// 券码
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }
}
