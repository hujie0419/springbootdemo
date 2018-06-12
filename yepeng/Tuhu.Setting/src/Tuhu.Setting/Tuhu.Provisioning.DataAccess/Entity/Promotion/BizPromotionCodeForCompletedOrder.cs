using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizPromotionCodeForCompletedOrder
    {
        public int PKID { get; set; }
        /// <summary>订单号// </summary>
        public int OrderId { get; set; }

        /// <summary>用户Id/// </summary>
        public string UserId { get; set; }

        /// <summary>开始日期// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>截至日期// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>描述// </summary>
        public string Description { get; set; }

        /// <summary>优惠金额// </summary>
        public int Discount { get; set; }

        /// <summary>最小优惠券金额// </summary>
        public int MinMoney { get; set; }

        /// <summary>优惠券名称Id// </summary>
        public int RuleId { get; set; }

        /// <summary>优惠券名称// </summary>
        public string PromotionName { get; set; }

        /// <summary>创建人// </summary>
        public string CreateBy { get; set; }

        /// <summary>创建时间// </summary>
        public string CreateTime { get; set; }

        /// <summary>是否有效// </summary>
        public int IsActive { get; set; }

        /// <summary>优惠券code// </summary>
        public string Code { get; set; }

        /// <summary>优惠券来源渠道/// </summary>
        public string CodeChannel { get; set; }

        /// <summary>优惠券来源渠道/// </summary>
        public string UpdateBy { get; set; }

        /// <summary>优惠券来源渠道/// </summary>
        public DateTime? UpdateTime { get; set; }

        public int GetRuleID { get; set; }

        public BizPromotionCodeForCompletedOrder DeepCopy()
        {
            return (BizPromotionCodeForCompletedOrder)this.MemberwiseClone();
        }
    }
}
