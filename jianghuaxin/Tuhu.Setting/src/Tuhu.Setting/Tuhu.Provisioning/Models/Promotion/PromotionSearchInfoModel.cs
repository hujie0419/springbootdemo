using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PromotionSearchInfoModel
    {
        public int? PromotionTaskId { get; set; }

        public string TaskName { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? TaskStatus { get; set; }

        public int? TaskType { get; set; }

        /// <summary>
        /// 优惠券规则编号
        /// </summary>
        public int? CouponRulesId { get; set; }

        public int? PageNo { get; set; }
    }
}