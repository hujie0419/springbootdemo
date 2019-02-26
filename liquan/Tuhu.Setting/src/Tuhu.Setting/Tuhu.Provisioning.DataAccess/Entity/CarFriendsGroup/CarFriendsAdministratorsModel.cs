using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CarFriendsGroup
{
    /// <summary>
    /// 车友群途虎管理员
    /// </summary>
    public class CarFriendsAdministratorsModel
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

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Is_Deleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastUpdateBy { get; set; }
    }
}
