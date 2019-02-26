using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 砍价黑名单接口请求实体
    /// </summary>
    public class BargainBlackListRequest
    {

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

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
