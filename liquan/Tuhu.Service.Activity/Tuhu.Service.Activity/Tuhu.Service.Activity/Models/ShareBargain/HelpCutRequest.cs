using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
   public class HelpCutRequest
    {
        /// <summary>
        /// 用户发起砍价的标识
        /// </summary>
        public Guid IdKey { get; set; } 

        /// <summary>
        /// 砍价配置的主键
        /// </summary>
        public int ActivityProductId { get; set; }

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
    }
}
