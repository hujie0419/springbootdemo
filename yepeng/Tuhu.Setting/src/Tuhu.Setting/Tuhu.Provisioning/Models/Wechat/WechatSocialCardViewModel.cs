namespace Tuhu.Provisioning.Models.Wechat
{
    public class WechatSocialCardRequest
    {
        public Card card { get; set; }
    }

    public class Card
    {
        public string card_type { get; set; } = "CASH";
        public Cash cash { get; set; }
    }

    public class Cash
    {
        public Base_Info base_info { get; set; }
        public Advanced_Info advanced_info { get; set; }
        public int reduce_cost { get; set; }
    }

    public class Base_Info
    {
        /// <summary>
        /// 卡券的商户logo，建议像素为300*300。
        /// </summary>
        public string logo_url { get; set; }

        public Pay_Info pay_info { get; set; }

        /// <summary>
        /// 商户名称,字数上限为12个汉字。
        /// </summary>
        public string brand_name { get; set; }

        /// <summary>
        /// 码型：
        /// "CODE_TYPE_TEXT"文 本 ；
        /// "CODE_TYPE_BARCODE"一维码
        /// "CODE_TYPE_QRCODE"二维码
        /// "CODE_TYPE_ONLY_QRCODE",二维码无code显示；
        /// "CODE_TYPE_ONLY_BARCODE",一维码无code显示；
        /// "CODE_TYPE_NONE"， 不显示code和条形码类型
        /// </summary>
        public string code_type { get; set; }

        public string title { get; set; }
        public string color { get; set; }
        public string service_phone { get; set; }
        public string description { get; set; }
        public Date_Info date_info { get; set; }

        /// <summary>
        /// 卡券领取页面是否可分享。
        /// </summary>
        public bool can_share { get; set; }

        /// <summary>
        /// 卡券顶部居中的按钮，仅在卡券状 态正常(可以核销)时显示
        /// </summary>
        public string center_title { get; set; }

        /// <summary>
        /// 卡券跳转的小程序的user_name，仅可跳转该 公众号绑定的小程序 。
        /// </summary>
        public string center_app_brand_user_name { get; set; }

        /// <summary>
        /// 卡券跳转的小程序的path
        /// </summary>
        public string center_app_brand_pass { get; set; }

        /// <summary>
        /// 卡券是否可转赠。
        /// </summary>
        public bool can_give_friend { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Sku sku { get; set; }

        /// <summary>
        /// 每人可领券的数量限制,不填写默认为50。
        /// </summary>
        public int get_limit { get; set; }

        ///// <summary>
        ///// 自定义跳转外链的入口名字。
        ///// </summary>
        //public string custom_url_name { get; set; }

        ///// <summary>
        ///// 自定义跳转的URL。
        ///// </summary>
        //public string custom_url { get; set; }

        ///// <summary>
        ///// 	显示在入口右侧的提示语。
        ///// </summary>
        //public string custom_url_sub_title { get; set; }

        ///// <summary>
        ///// 营销场景的自定义入口名称。
        ///// </summary>
        //public string promotion_url_name { get; set; }

        ///// <summary>
        ///// 入口跳转外链的地址链接。
        ///// </summary>
        //public string promotion_url { get; set; }
    }

    public class Pay_Info
    {
        public Swipe_Card swipe_card { get; set; }
    }

    public class Swipe_Card
    {
        public string[] use_mid_list { get; set; }
        public string create_mid { get; set; }
        public bool is_swipe_card { get; set; }
    }

    public class Date_Info
    {
        /// <summary>
        /// DATE_TYPE_FIX _TIME_RANGE 表示固定日期区间，
        /// DATE_TYPE_FIX_TERM 表示固定时长 （自领取后按天算。
        /// </summary>
        public string type { get; set; }

        public uint? begin_timestamp { get; set; }

        public uint? end_timestamp { get; set; }

        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天内有效，不支持填写0。
        /// </summary>
        public int? fixed_term { get; set; }

        /// <summary>
        /// type为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天开始生效，领取后当天生效填写0。（单位为天）
        /// </summary>
        public int? fixed_begin_term { get; set; }
    }

    public class Sku
    {
        public long quantity { get; set; }
    }

    public class Advanced_Info
    {
        public Use_Condition use_condition { get; set; }
    }

    public class Use_Condition
    {
        public bool can_use_with_other_discount { get; set; }
        public int? least_cost { get; set; }
    }
}