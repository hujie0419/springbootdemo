using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 优惠券领取规则
    /// </summary>
    public class CouponGetRuleModel
    {
        public int PKID { get; set; }

        /// <summary>优惠券规则Id/// </summary>
		public int RuleID { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// 使用规则说明
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>优惠券名称/// </summary>
        public string PromotionName { get; set; }

        /// <summary>优惠金额/// </summary>
		public decimal Discount { get; set; }

        /// <summary>满足最低价/// </summary>
        public decimal MinMoney { get; set; }

        public int AllowChanel { get; set; }

        public int? Term { get; set; }

        /// <summary>开始日期/// </summary>
		public DateTime? ValiStartDate { get; set; }

        /// <summary>截止日期/// </summary>
        public DateTime? ValiEndDate { get; set; }

        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途名称
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 最终有效期
        /// </summary>
        public DateTime? DeadLineDate { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 发行量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 已领取数量
        /// </summary>
        public int GetQuantity { get; set; }
        /// <summary>
        /// 单人限购数量
        /// </summary>
        public int SingleQuantity { get; set; }
        /// <summary>
        /// 优惠券类型 0=满减券，1=后返券，2=实付券，3=抵扣券
        /// </summary>
        public int PromotionType { set; get; }

        /// <summary>
        /// 新用户专享
        /// </summary>
        public int SupportUserRange { get; set; }


        /// <summary>
        ///订单类型:  0所有订单，1仅到家订单，2仅到店订单
        /// </summary>
        public int InstallType { get; set; }

        /// <summary>
        /// 支付方式，0不限，1在线支付，2到店支付
        /// </summary>
        public int OrderPayMethod { get; set; }
    }
}
