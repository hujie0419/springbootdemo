using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Response
{
    /// <summary>
    /// 用户优惠券
    /// </summary>
    public class CouponModel
    {
        public int PKID { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 券的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 券的结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 	使用时间
        /// </summary>
        public DateTime UsedTime { get; set; }
        /// <summary>
        /// 使用该券的订单号
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 状态0：未使用，1：已使用，2：作废，3：作废（作废状态默认用3
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 描述 [50个字符]
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 满减金额 - 使用 LeastCost 替代
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 使用门槛 - 使用 ReduceCost 替代
        /// </summary>
        public int MinMoney { get; set; }
        /// <summary>
        /// 发放渠道  - 必传参数 [30字符]
        /// </summary>
        public string CodeChannel { get; set; }
        /// <summary>
        /// 生成券的对应的业务单号。比如：下轮胎送保养券，保养券的批次号存储轮胎订单的编号
        /// </summary>
        public int BatchID { get; set; }
        /// <summary>
        /// 领券时的设备id
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// 使用规则id
        /// </summary>
        public int RuleID { get; set; }
        /// <summary>
        /// 优惠券名
        /// </summary>
        public string PromtionName { get; set; }
        /// <summary>
        /// 获取优惠券规则id
        /// </summary>
        public int GetRuleID { get; set; }
        /// <summary>
        /// 	发券人，（例如：活动的配置创建人）
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 发券的渠道（例如：幸运大翻牌）
        /// </summary>
        public string IssueChannle { get; set; }
        /// <summary>
        /// 发券的渠道id（例如：幸运大翻牌活动id）
        /// </summary>
        public string IssueChannleId { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepartmentId { get; set; }
        /// <summary>
        /// 用途ID
        /// </summary>
        public int IntentionId { get; set; }
        /// <summary>
        /// 券规则的创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 部门名
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 用途名
        /// </summary>
        public string IntentionName { get; set; }
        /// <summary>
        /// 规则类型，空和0 为途虎券，1门店券
        /// </summary>
        public int CouponType { get; set; }
        /// <summary>
        /// 券归属的业务线（目前用于赠品券）
        /// </summary>
        public string BusinessLineName { get; set; }
        /// <summary>
        /// 优惠券2.0里的塞券计划配置的优惠券详情Id（对应tbl_PromotionTaskPromotionList的主键TaskPromotionListId）
        /// </summary>
        public int TaskPromotionListId { get; set; }
        /// <summary>
        /// 实际抵扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>
        /// 门槛金额
        /// </summary>
        public decimal LeastCost { get; set; }
        /// <summary>
        /// 满减金额
        /// </summary>
        public decimal ReduceCost { get; set; }
        /// <summary>
        /// 是否归档到历史优惠券表
        /// </summary>
        public int IsHistory { get; set; }

    }
}
