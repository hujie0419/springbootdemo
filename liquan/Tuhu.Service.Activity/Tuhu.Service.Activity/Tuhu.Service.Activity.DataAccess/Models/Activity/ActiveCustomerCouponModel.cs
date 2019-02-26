using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    /// 表实体 [Activity].[dbo].[CustomerExclusiveCoupon]
    /// 大客户专享券信息
    /// </summary>
    public class ActiveCustomerCouponModel
    {

        public int PKID { get; set; }

        /// <summary>
        /// 外键关联CustomerExclusiveSetting表PKID
        /// </summary>
        public int CustomerExclusiveSettingPkId { get; set; }

        /// <summary>
        /// 活动专享ID
        /// </summary>
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
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 限时抢购活动ID
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 券码状态 0有效 -1被删除
        /// </summary>
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateDateTime { get; set; }

    }
}
