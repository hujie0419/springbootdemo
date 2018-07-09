using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.MemberPage
{
    public class MemberPageChannelModel
    {
        /// <summary>
        /// 主键标识
        /// </summary>
        public long PKID { get; set; }
        /// <summary>
        /// 渠道，包括IOS、Android、TuhuMiniProgram、GroupBuyMiniProgram、H5、XiaoMiQuickApp、HuaweiQuickApp
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 模块标识
        /// </summary>
        public long MemberPageModuleID { get; set; }
        /// <summary>
        /// 模块内容标识
        /// </summary>
        public long MemberPageModuleContentID { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 目标小程序AppID
        /// </summary>
        public string MiniProgramAppID { get; set; }
    }
}
