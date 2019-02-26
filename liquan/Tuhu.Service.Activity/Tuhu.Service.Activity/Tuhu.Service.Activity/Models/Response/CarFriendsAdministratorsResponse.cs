using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 车友群途虎管理员
    /// </summary>
    public class CarFriendsAdministratorsResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 微信昵称
        /// </summary>
        public string WeChatNickName { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string WeChatNumber { get; set; }

        /// <summary>
        /// 微信头像
        /// </summary>
        public string WeChatHeadPortrait { get; set; }

        /// <summary>
        /// 微信二维码
        /// </summary>
        public string WeChatQRCode { get; set; }
    }
}
