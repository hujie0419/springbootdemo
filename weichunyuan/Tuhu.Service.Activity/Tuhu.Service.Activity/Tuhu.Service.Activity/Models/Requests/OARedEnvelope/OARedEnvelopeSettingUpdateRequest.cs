using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 更新设置  请求类
    /// </summary>
    public class OARedEnvelopeSettingUpdateRequest
    {
        /// <summary>
        ///     条件 - 价格
        /// </summary>
        public decimal ConditionPrice { get; set; }

        /// <summary>
        ///     条件 - 价格 - FLAG
        /// </summary>
        public int ConditionPriceFlag { get; set; }

        /// <summary>
        ///     条件 - 车型 - FLAG
        /// </summary>
        public int ConditionCarModelFlag { get; set; }

        /// <summary>
        ///     一天上限金额
        /// </summary>
        public decimal DayMaxMoney { get; set; }

        /// <summary>
        ///     每人平均领取红包金额（元）
        /// </summary>
        public decimal AvgMoney { get; set; }

        /// <summary>
        ///     活动规则
        /// </summary>
        public string ActivityRuleText { get; set; }

        /// <summary>
        ///     未获得红包提示
        /// </summary>
        public string FailTipText { get; set; }

        /// <summary>
        ///     长按识别二维码 URL
        /// </summary>
        public string QRCodeUrl { get; set; }

        /// <summary>
        ///     识别二维码提示文字
        /// </summary>
        public string QRCodeTipText { get; set; }

        /// <summary>
        ///     分享标题
        /// </summary>
        public string ShareTitleText { get; set; }

        /// <summary>
        ///     分享地址
        /// </summary>
        public string ShareUrl { get; set; }

        /// <summary>
        ///     分享图片的地址
        /// </summary>
        public string SharePictureUrl { get; set; }

        /// <summary>
        ///     分享的内容
        /// </summary>
        public string ShareText { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }

        /// <summary>
        ///     活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        ///     最后更新人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        ///     这个日期之前的OpenId不生效 字段可空 空字段就不生效
        /// </summary>
        public DateTime? OpenIdLegalDate { get; set; }
    }
}
