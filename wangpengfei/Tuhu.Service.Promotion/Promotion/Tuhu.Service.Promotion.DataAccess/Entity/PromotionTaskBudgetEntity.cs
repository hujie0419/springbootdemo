using System;
using System.Collections.Generic;
using System.Text;

namespace Tuhu.Service.Promotion.DataAccess.Entity
{
    public class PromotionTaskBudgetEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 优惠券任务id
        /// </summary>
        public int PromotionTaskID { get; set; }
        /// <summary>
        /// 优惠券使用规则
        /// </summary>
        public int CouponRulesId { get; set; }
        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// 订单匹配的产品id
        /// </summary>
        public string PIDs { get; set; }

        /// <summary>
        /// 发券金额
        /// </summary>
        public decimal DiscountMoney { get; set; }
        /// <summary>
        /// 发券张数
        /// </summary>
        public int CouponNum { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDel { get; set; }
    }
}
