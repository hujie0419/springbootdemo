using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Model.GFCard
{
    public class GFBankSmsTask
    {
        /// <summary>
        /// 优惠券任务PKID
        /// </summary>
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 兑换码任务PKID
        /// </summary>
        public int RedemptionCodeTaskId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }
}
