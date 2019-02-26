using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ThirdPartyOrderChannellink
{
    public class ThirdPartyOrderChannelModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 三方订单渠道
        /// </summary>
        public string OrderChannel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastUpdateDateTime { get; set; }
    }
}
