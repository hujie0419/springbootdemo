using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models 
{

    /// <summary>
    /// 用户领取砍价优惠券接口请求实体
    /// </summary>
    public class ReceiveBargainCouponRequest
    {
        /// <summary>
        /// 砍价发起人UserId
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 砍价活动商品配置Id
        /// </summary>
        public int ActivityProductId { get; set; }

        /// <summary>
        /// 商品PID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 操作ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 操作设备号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 收货手机号
        /// </summary>
        public string ReceivePhone { get; set; }

        /// <summary>
        /// 支付账号
        /// </summary>
        public string PayAccount { get; set; }

    }
}
