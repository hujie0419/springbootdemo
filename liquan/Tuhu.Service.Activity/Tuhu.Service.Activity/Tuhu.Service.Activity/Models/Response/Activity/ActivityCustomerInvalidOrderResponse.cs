using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models 
{
    public class ActivityCustomerInvalidOrderResponse
    {

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 异常类型：NOCOUPON无活动券码;COUPONOVERTIME订单不在券码有效期内;OVERACTIVITYLIMIT超出活动限购;OVERPIDLIMIT超出活动商品限购
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
        /// 用户该活动订单号
        /// </summary>
        public List<int> OrderIDs { get; set; }

        /// <summary>
        /// 券码状态 1被删除 0未删除
        /// </summary>
        public int IsCouponDeleted { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Phone { get; set; }

    }
}
