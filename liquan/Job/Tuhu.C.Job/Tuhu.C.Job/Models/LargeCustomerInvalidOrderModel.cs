using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Models
{
    /// <summary>
    /// 大客户员工异常订单记录表实体  [Activity].[dbo].[LargeCustomerInvalidOrder]
    /// </summary>
    public class LargeCustomerInvalidOrderModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 异常类型：NOCOUPON:无活动券码; OVERACTIVITYLIMIT超过活动限购;OVERPIDMILIT超过活动商品限购
        /// </summary>
        public string InvalidType { get; set; }

        /// <summary>
        /// 异常详细信息
        /// </summary>
        public string DetailInfo { get; set; }

        /// <summary>
        /// 限时抢购活动id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 用户该活动订单号拼接
        /// </summary>
        public string OrderIDs { get; set; }

        /// <summary>
        /// 邮件发送次数
        /// </summary>
        public int EmailSendCount { get; set; }

        /// <summary>
        /// 券码是否被删除 1 已删除 0 未删除
        /// </summary>
        public int IsCouponDeleted { get; internal set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; internal set; }

    }
}
