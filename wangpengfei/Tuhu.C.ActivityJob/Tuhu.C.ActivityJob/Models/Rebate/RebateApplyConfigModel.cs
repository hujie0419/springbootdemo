using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Rebate
{
    public class RebateApplyConfigModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 安装门店id
        /// </summary>
        public int InstallShopId { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// 技师Id
        /// </summary>
        public int? TechId { get; set; }
    }

    public class PkidWithTechId
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 技师Id
        /// </summary>
        public int TechId { get; set; }
    }
}
