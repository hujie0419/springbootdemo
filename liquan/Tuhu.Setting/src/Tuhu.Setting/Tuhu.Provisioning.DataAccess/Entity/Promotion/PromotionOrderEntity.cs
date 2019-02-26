using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class PromotionOrderEntity
    {
        public int PKID { get; set; }
        public int OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public int Discount { get; set; }
        public int MinMoney { get; set; }
        public int RuleID { get; set; }
        public string PromotionName { get; set; }
        public string InstallStatus { get; set; }

        public int? InstallShopID { get; set; }

        public string CodeChannel { get; set; }

        public DateTime? DeliveryDatetime { get; set; }

        /// <summary>
        /// 领取规则优惠券Id
        /// </summary>
        public int CouponRulesId { get; set; }

    }
}
