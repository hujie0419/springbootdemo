using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 用户的优惠券
    /// </summary>
    public class GetCouponByIDResponse
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public int Pkid { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 优惠券券号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建优惠券时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 优惠券面值
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 实际抵扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>
        /// 允许使用最小金额
        /// </summary>
        public decimal? MinMoney { get; set; }
        /// <summary>
        /// 优惠券有效期（开始时间）
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 优惠券有效期（结束时间）
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 优惠券使用状态 0:未使用  1:已使用  2&3：作废
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 优惠券描述 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 优惠券使用时间
        /// </summary>
        public string UsedTime { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string CodeChannel { get; set; }
        /// <summary>
        /// 返券来源主键id
        /// </summary>
        public int BatchID { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string PromtionName { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途Id
        /// </summary>
        public int IntentionId { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }

        /// <summary>
        /// 发放渠道
        /// </summary>
        public string IssueChannle { get; set; }
        /// <summary>
        /// 发放渠道ID
        /// </summary>
        public string IssueChannleId { get; set; }
        /// <summary>
        /// 优惠券的 起用金额 MinMoney的小数类型
        /// </summary>
        public decimal LeastCost { get; set; }

        /// <summary>
        /// 优惠券的 减免金额 Discount的小数类型
        /// </summary>
        public decimal ReduceCost{ get; set; }

        /// <summary>
        /// PromotionCodeModel新增CouponType字段  规则类型，空和0 为途虎券，1门店券
        /// </summary>
        public int CouponType { get; set; }

        /// <summary>
        /// 领取数目
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 已领取数目
        /// </summary>
        public int GetQuantity { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }


        #region 领取规则信息
        /// <summary>
        /// 领取规则id
        /// </summary>
        public int GetRuleID { get; set; }
        /// <summary>
        /// 领取规则guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }
        /// <summary>
        /// 领券规则的优惠券名称
        /// </summary>
        public string GetRuleName { get; set; }
        #endregion


        #region 使用规则信息
        /// <summary>
        /// 使用规则id
        /// </summary>
        public int RuleId { get; set; }
        /// <summary>
        /// 优惠券类型 0=满减券，1=后返券，2=实付券，3=抵扣券
        /// </summary>
        public int? PromotionType { set; get; }

        /// <summary>
        /// 使用规则名称
        /// </summary>
        public string RuleName { get; set; }


        /// <summary>
        /// 使用规则说明
        /// </summary>
        public string RuleDescription { get; set; }

          /// <summary>
        /// 订单支付方式
        /// </summary>
        public int OrderPayMethod { get; set; }
        /// <summary>
        /// 拼团是否可用
        /// </summary>
        public int EnabledGroupBuy { get; set; }
        /// <summary>
        /// 安装类型
        /// </summary>
        public string InstallType { get; set; }
        #endregion
    }
}
