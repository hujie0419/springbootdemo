using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.UnivRedemptionCode
{
    public class RedeemPromotionRecord
    {
        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedemptionCode { get; set; }
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public string PromotionId { get; set; }
    }
}
