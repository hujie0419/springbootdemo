using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityConfig
    {
        /// <summary>
        /// 银行活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 银行用户ID
        /// </summary>
        public Guid BankId { get; set; }
        /// <summary>
        /// 服务产品ID
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// 服务产品名字
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 包含服务码的数量
        /// </summary>
        public int ServiceCount { get; set; }
        /// <summary>
        /// 用户结算价
        /// </summary>
        public decimal UserSettlementPrice { get; set; }
        /// <summary>
        /// 银行结算价
        /// </summary>
        public decimal BankSettlementPrice { get; set; }
        /// <summary>
        /// 服务码有效期
        /// </summary>
        public int ValidDays { get; set; }
        /// <summary>
        /// 银行头图
        /// </summary>
        public string BannerImageUrl { get; set; }
        /// <summary>
        /// 银行活动规则描述
        /// </summary>
        public string RuleDescription { get; set; }
        /// <summary>
        /// 活动用户规则的验证类型
        /// CardLevel 卡号等级
        /// Moble1 完整的手机号
        /// Card1 银行卡号或者银行卡号的前N后M
        /// Card2 银行卡号的前N位
        /// Card2,Other1  银行卡的前N位和其他卡号验证
        /// </summary>
        public string VerifyType { get; set; }
        /// <summary>
        /// 活动名
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 合作ID
        /// </summary>
        public int CooperateId { get; set; }
        /// <summary>
        /// 合作名
        /// </summary>
        public string CooperateName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 场次周期类型
        /// </summary>
        public string RoundCycleType { get; set; }
        /// <summary>
        /// 活动按钮颜色
        /// </summary>
        public string ButtonColor { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettleMentMethod { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public int CustomerType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 卡BIN长度
        /// </summary>
        public int Card2Length { get; set; }

        public int CodeTypeConfigId { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }
        /// <summary>
        /// logo图片链接
        /// </summary>
        public string LogoUrl { get; set; }
        /// <summary>
        /// 活动限量描述
        /// </summary>
        public string ActivityLimitDescribe { get; set; }
        /// <summary>
        /// 活动时间描述
        /// </summary>
        public string ActivityTimeDescribe { get; set; }
        /// <summary>
        /// 购买提示
        /// </summary>
        public string BuyTips { get; set; }
        /// <summary>
        /// 场次开始时间
        /// </summary>
        public TimeSpan? RoundStartTime { get; set; }
        /// <summary>
        /// 场次开始时间的毫秒数（当日）
        /// </summary>
        public long MillisecondsRoundStartTime { get; set; }
        /// <summary>
        /// 校验为其他号时对应的校验名称
        /// </summary>
        public string OtherVerifyName { get; set; }
    }
}
