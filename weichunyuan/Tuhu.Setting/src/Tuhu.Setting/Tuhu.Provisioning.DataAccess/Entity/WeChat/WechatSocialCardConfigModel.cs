using System;
using System.ComponentModel.DataAnnotations;

namespace Tuhu.Provisioning.DataAccess.Entity.WeChat
{
    /// <summary>
	/// 微信社交立减金卡券配置表
	/// </summary>
	public class WechatSocialCardConfigModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 卡券id
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 商户名称（12字以内）
        /// </summary>
        [Required]
        [StringLength(12)]
        public string MerchantName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 卡券名称（卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)）
        /// </summary>
        [Required]
        public string CardName { get; set; }

        /// <summary>
        /// 卡券使用说明
        /// </summary>
        [Required]
        [StringLength(1024)]
        public string CardDescription { get; set; }

        /// <summary>
        /// logo图片地址
        /// </summary>
        [Required]
        public string LogoUrl { get; set; }

        /// <summary>
        /// 其他适用的商户号
        /// </summary>
        public string OtherMerchantNo { get; set; } = "";

        /// <summary>
        /// 卡券颜色
        /// </summary>
        [Required]
        public string CardColor { get; set; }

        /// <summary>
        /// 客服电话
        /// </summary>
        [Required]
        public string CutomerServiceTelphone { get; set; }

        /// <summary>
        /// 卡券有效期时间类型（DATE_TYPE_FIX_TIME_RANGE 表示固定日期区间，DATETYPE_FIX_TERM 表示固定时长 （自领取后按天算。）
        /// </summary>
        [Required]
        public string CardDateInfoType { get; set; }

        /// <summary>
        /// type为DATE_TYPE_FIX_TIME_RANGE时专用，表示起用时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 表示结束时间 ，建议设置为截止日期的23:59:59过期
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天内有效，不支持填写0。
        /// </summary>
        public int FixedTerm { get; set; }

        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天开始生效，领取后当天生效填写0。（单位为天）
        /// </summary>
        public int FixedBeginTerm { get; set; }

        /// <summary>
        /// 可用于DATE_TYPE_FIX_TERM时间类型，表示卡券统一过期时间 ， 建议设置为截止日期的23:59:59过期 。 （ 东八区时间,UTC+8，单位为秒 ），设置了fixed_term卡券，当时间达到end_timestamp时卡券统一过期
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 卡券面额
        /// </summary>
        public int CardAmount { get; set; }

        /// <summary>
        /// 卡券使用条件
        /// </summary>
        public int CardCondition { get; set; }

        /// <summary>
        /// 卡券上的按钮文字
        /// </summary>
        [Required]
        public string CardButtonText { get; set; }

        /// <summary>
        /// 卡券按钮跳转的小程序
        /// </summary>
        [Required]
        public string CardButtonToWxApp { get; set; }

        /// <summary>
        /// 卡券按钮跳转的小程序页面路径
        /// </summary>
        [Required]
        public string CardButtonToPath { get; set; }

        /// <summary>
        /// 是否可以分享
        /// </summary>
        public bool IsCanShare { get; set; }

        /// <summary>
        /// 是否可以转赠
        /// </summary>
        public bool IsCanGiveToFriend { get; set; }

        /// <summary>
        /// 是否可以与其他折扣共用
        /// </summary>
        public bool IsCanUseWithOtherDiscount { get; set; }

        /// <summary>
        /// 卡券库存
        /// </summary>
        public long CardStockQuantity { get; set; }

        /// <summary>
        /// 每个人可领券数量
        /// </summary>
        public int CardGetLimit { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// 微信社交立减金活动配置表
    /// </summary>
    public class WechatSocialActivityConfigModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 卡券id
        /// </summary>
        [Required]
        public string CardId { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 活动背景颜色
        /// </summary>
        [Required]
        public string ActivityBgColor { get; set; }

        /// <summary>
        /// 用户点击链接后可静默添加到列表的小程序appid
        /// </summary>
        [Required]
        public string ActivityWxAppId { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 单个礼包社交立减金数量（3-15个）
        /// </summary>
        [Range(3, 15)]
        public int GiftNum { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// 每个用户活动期间最大领取次数,最大为50，不填默认为1
        /// </summary>
        [Range(1, 50)]
        public int MaxParticTimesAct { get; set; }

        /// <summary>
        /// 每个用户活动期间单日最大领取次数,最大为50，不填默认为1
        /// </summary>
        [Range(1, 50)]
        public int MaxParticTimesOneDay { get; set; }

        /// <summary>
        /// 最少支付金额，单位是分
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// 用户范围，0所有用户1仅新用户
        /// </summary>
        public int UserScope { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; }
    }
}