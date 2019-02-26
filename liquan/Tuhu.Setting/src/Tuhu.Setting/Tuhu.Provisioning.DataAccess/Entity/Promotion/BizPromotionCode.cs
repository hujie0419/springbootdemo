using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizPromotionCode
    {
        /// <summary>PKID// </summary>
        public int PKID { get; set; }

        /// <summary>优惠券code// </summary>
        public string Code { get; set; }

        /// <summary>类型// </summary>
        public int Type { get; set; }

        /// <summary>描述// </summary>
        public string Description { get; set; }

        /// <summary>最小优惠券金额// </summary>
        public int MinMoney { get; set; }

        /// <summary>优惠金额// </summary>
        public int Discount { get; set; }

        /// <summary>开始日期// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>截至日期// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>使用时间// </summary>
        public DateTime? UsedTime { get; set; }

        /// <summary>订单号// </summary>
        public int OrderId { get; set; }

        /// <summary>状态// </summary>
        public int Status { get; set; }
        public int PkId { get; set; }

        /// <summary>优惠券名称Id// </summary>
        public int RuleID { get; set; }

        /// <summary>优惠券名称// </summary>
        public string RuleName { get; set; }

        /// <summary>到家，到点，全部支持/// </summary>
        public string InstallType { get; set; }

        /// <summary>用户Id/// </summary>
        public string UserId { get; set; }

        /// <summary>优惠券来源渠道/// </summary>
        public string CodeChannel { get; set; }

        public string StartTimeValue { get; set; }

        public string EndTimeValue { get; set; }

        public string PromtionName { get; set; }

        public int BatchId { get; set; }

        public int? PromotionType { get; set; }

        /// <summary>
        /// 领取规则优惠券Id
        /// </summary>
        public int CouponRulesId { get; set; }

        public BizPromotionCode DeepCopy()
        {
            return (BizPromotionCode)this.MemberwiseClone();
        }
    }
}