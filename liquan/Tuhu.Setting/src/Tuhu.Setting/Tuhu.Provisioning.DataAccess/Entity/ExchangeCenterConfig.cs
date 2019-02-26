using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ExchangeCenterConfig
    {
        public int ExchangeCenterId { get; set; }

        /// <summary>
        /// 积分兑换配置主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 优惠券id
        /// </summary>
        public int CouponId { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券总数
        /// </summary>
        public int CouponSum { get; set; }

        /// <summary>
        /// 优惠券剩余数量
        /// </summary>
        public int CouponSurplus { get; set; }

        /// <summary>
        /// 兑奖周期
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 优惠券积分值
        /// </summary>
        public int PointsValue { get; set; }

        /// <summary>
        /// 优惠券截止时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 状态 true 启用和 false禁用
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 查询状态
        /// </summary>
        public bool? SearchStatus { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 小图片地址
        /// </summary>
        public string SmallImage { get; set; }

        /// <summary>
        /// 积分规则
        /// </summary>
        public string PointsRules { get; set; }

        /// <summary>
        /// 兑换说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 优惠券的过期时间点
        /// </summary>
        public DateTime? CouponEndTime { get; set; }

        /// <summary>
        /// 优惠券的可用时间长度，单位为（天）
        /// </summary>
        public string CouponDuration { get; set; }

        /// <summary>
        /// 优惠券规则ID
        /// </summary>
        public string GetRuleGUID { get; set; }


        /// <summary>
        /// 配置位置 :精品通用券、会员商城
        /// </summary>
        public string Postion { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 用户等级 参数：V0、V1、V2、V3、V4 与会员等级表GradeCode关联
        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// 券后价格
        /// </summary>
        public decimal EndCouponPrice { get; set; }

        /// <summary>
        /// 兑换类型
        /// </summary>
        public bool? ExchangeCenterType { get; set; }

        /// <summary>
        /// 概率大于等于0小于等于100
        /// </summary>
        public decimal? Chance { get; set; }

        /// <summary>
        /// 启用版本
        /// </summary>
        public string StartVersion { get; set; }

        /// <summary>
        /// 结束版本
        /// </summary>
        public string EndVersion { get; set; }

        /// <summary>
        /// 高清图片
        /// </summary>
        public string HDImage { get; set; }

        /// <summary>
        /// 配置位编号
        /// </summary>
        public string PostionCode { get; set; }

        /// <summary>
        /// 描述标题
        /// </summary>
        public string DescriptionTitle { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// 优惠券发行量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 已经发送的优惠券数量
        /// </summary>
        public int GetQuantity { get; set; }
    }

    /// <summary>
    /// 描述字段存储Json格式 老逻辑需要
    /// </summary>
    public class Description
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Sort { get; set; }
    }

    public class PersonalCenterCouponConfig
    {
        public int Id { get; set; }

        public int ExchangeCenterId { get; set; }

        public int HomePageModuleId { get; set; }

        public int Sort { get; set; }
    }
}
