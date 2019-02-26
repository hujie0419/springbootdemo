using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 车友群小程序推送消息的请求参数
    /// </summary>
    public class GetCarFriendsGroupPushInfoRequest
    {
        /// <summary>
        /// 车友群或群主信息主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 消息类型。0=车友群，1=群主
        /// </summary>
        public int InfoType { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 微信OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public Guid RandomNumberKey { get; set; }
    }
}
