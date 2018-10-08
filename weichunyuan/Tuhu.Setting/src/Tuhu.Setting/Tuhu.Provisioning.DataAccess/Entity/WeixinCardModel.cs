using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.DataAccess.Entity
{

    public class WeixinCardInfo
    {
        public WeixinCardModel card { get; set; }
    }

    public class WeixinCardModel
    {
        [JsonIgnore]
        public int? PKID { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CardTypeEnum card_type { get; set; }
        [JsonIgnore]
        public string card_id { get; set; }
        public WeixinCardTotalModel total_info { get; set; }



        [JsonIgnore]
        public int? PushedCount { get; set; }
      
    }




    public class WeixinCardTotalModel
    {
        public WeixinCardBaseInfo base_info { get; set; }

        public WeixinCardAdvancedInfo advanced_info { get; set; }

        public string deal_detail { get; set; }

        public int? least_cost { get; set; }

        public int? reduce_cost { get; set; }

        public int? discount { get; set; }

        public string gift { get; set; }

        public string default_detail { get; set; }
    }

    public class WeixinCardBaseInfo
    {

        [JsonIgnore]
        public string activate_app_brand_user_name { get; set; }

        [JsonIgnore]
        public string activate_app_brand_pass { get; set; }

        [Required(ErrorMessage ="商户图标链接不许为空")]
        [StringLength(128,ErrorMessage = "商户图标链接长度不允许超过128")]
        
        public string logo_url { get; set; }

        [Required(ErrorMessage = "商户名称不许为空")]
        [StringLength(18, ErrorMessage = "商户名称长度不允许超过18")]
        public string brand_name { get; set; }

        [Required]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public CardCodeTypeEnum code_type { get; set; }

        [Required(ErrorMessage = "卡券标题不许为空")]
        [StringLength(13, ErrorMessage = "卡券标题长度不允许超过13")]
        public string title { get; set; }

        [JsonIgnore]
        public int? colorid { get; set; }


        public string color { get; set; }
        [Required(ErrorMessage ="卡券副标题不许为空")]
        [StringLength(24, ErrorMessage = "卡券副标题长度不允许超过24")]
        public string notice { get; set; }
        [StringLength(24)]
        public string service_phone { get; set; }

        [Required(ErrorMessage = "卡券描述不许为空")]
        [StringLength(1536, ErrorMessage = "卡券描述长度不允许超过1536")]
        public string description { get; set; }
        [Required]
        public DateInfo date_info { get; set; }
        [Required]
        public SKUQuantity sku { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = " 核销数量必须是数字类型")]
        public int? use_limit { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "领取数量必须是数字类型")]
        public int? get_limit { get; set; }

        public bool use_custom_code { get; set; }

        public string get_custom_code_mode { get; set; }

        public bool bind_openid { get; set; }

        public int[] location_id_list { get; set; }

        public bool use_all_locations { get; set; }
        [StringLength(18, ErrorMessage = "来源名称长度不允许超过18")]
        public string source { get; set; }






        [StringLength(9,ErrorMessage = "使用场景标题长度不允许超过9")]
        public string center_title { get; set; }
        [StringLength(12, ErrorMessage = "使用场景副标题长度不允许超过12")]
        public string center_sub_title { get; set; }
        [StringLength(128,ErrorMessage = "入口链接地址长度不允许超过128")]
        public string center_url { get; set; }

        public string center_app_brand_user_name { get; set; }

        public string center_app_brand_pass { get; set; }


       // [Column(TypeName = "VARCHAR(15)")]
        [StringLength(7)]
        public string custom_url_name { get; set; }
        [StringLength(128, ErrorMessage = "自定义链接地址长度不允许超过128")]
        public string custom_url { get; set; }
        [StringLength(9, ErrorMessage = "自定义链接副标题长度不允许超过9")]
        public string custom_url_sub_title { get; set; }

        public string custom_app_brand_user_name { get; set; }

        public string custom_app_brand_pass { get; set; }



        [StringLength(7, ErrorMessage = "营销场景标题长度不允许超过7")]
        public string promotion_url_name { get; set; }
        [StringLength(128, ErrorMessage = "营销场景链接地址长度不允许超过128")]
        public string promotion_url { get; set; }
        [StringLength(9, ErrorMessage = "营销场景副标题长度不允许超过9")]
        public string promotion_url_sub_title { get; set; }

        public string promotion_app_brand_user_name { get; set; }

        public string promotion_app_brand_pass { get; set; }


        public bool can_share { get; set; }

        public bool can_give_friend { get; set; }

        public string deal_detail { get; set; }

        public int? least_cost { get; set; }

        public int? reduce_cost { get; set; }

        public int? discount { get; set; }

        public string gift { get; set; }

        public string default_detail { get; set; }



        [JsonIgnore]
        public int? supplierId { get; set; }


    }

    public class WeixinCardAdvancedInfo
    {
        public ConditionalUse use_condition { get; set; }
        [JsonProperty(PropertyName = "abstract")]
        public AbstractInfo abstractinfo { get; set; }

        public List<ImageText> text_image_list { get; set; }
    }

    public class ConditionalUse
    {
        public string accept_category { get; set; }

        public string reject_category { get; set; }

        public int? least_cost { get; set; }

        public string object_use_for { get; set; }

        public bool can_use_with_other_discount { get; set; }
    }

    public class ImageText
    {
        public string image_url { get; set; }

        public string text { get; set; }
    }

    public class AbstractInfo
    {
        [JsonProperty(PropertyName = "abstract")]
        public string abstractstr { get; set; }

        [JsonIgnore]
        public string icon { get; set; }

        public List<string> icon_url_list { get; set; }

        [JsonIgnore]
        public string icon1 { get; set; }
        [JsonIgnore]
        public string icon2 { get; set; }
        [JsonIgnore]
        public string icon3 { get; set; }
        [JsonIgnore]
        public string icon4 { get; set; }
        [JsonIgnore]
        public string icon5 { get; set; }


      
        [JsonIgnore]
        public ImageText imageText1 { get; set; }
        [JsonIgnore]
        public ImageText imageText2 { get; set; }
        [JsonIgnore]
        public ImageText imageText3 { get; set; }
        [JsonIgnore]
        public ImageText imageText4 { get; set; }
        [JsonIgnore]
        public ImageText imageText5 { get; set; }

    }

    public class SKUQuantity
    {
        [DefaultValue(-1)]
        public int? quantity { get; set; }
    }

    public class DateInfo
    {
        //  [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [Required]
        public string type { get; set; }

        [JsonIgnore]
        public DateTime begin_time { get; set; }
        [JsonIgnore]
        public DateTime end_time { get; set; }

        public UInt64? begin_timestamp { get; set; }

        public UInt64? end_timestamp { get; set; }

        public int? fixed_term { get; set; }

        public int? fixed_begin_term { get; set; }


    }


    public enum TimeType
    {
        DATE_TYPE_FIX_TIME_RANGE,
        DATE_TYPE_FIX_TERM
    }


    public enum CardTypeEnum
    {
        //团购券
        GROUPON,
        //代金券
        CASH,
        //折扣券
        DISCOUNT,
        //兑换券
        GIFT,
        //优惠券
        GENERAL_COUPON
    }


    public enum CardCodeTypeEnum
    {
        CODE_TYPE_NONE,
        //文本
        CODE_TYPE_TEXT,

        //一维码
        CODE_TYPE_BARCODE,

        //二维码
        CODE_TYPE_QRCODE
    }


    public class SupplierInfo
    {
        [JsonIgnore]
        public int? pkid { get; set; }

        [JsonIgnore]
        public string Imgbase64string { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "商户图标链接长度不允许超过128")]
        public string logo_url { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "商户名称长度不允许超过18")]
        public string brand_name { get; set; }
        [JsonIgnore]
        public DateTime LastUpdateDate { get; set; }
    }

    public class WeiXinColor
    {
        public int? ColorIndex { get; set; }

        public string ColorValue { get; set; }
    }

    public class WeixinCardCode
    {
        public string card_id { get; set; }

        public List<string> code { get; set; }
    }



    public class WeixinCardCodeQuantity
    {
        public string card_id { get; set; }

        public int increase_stock_value { get; set; }

        public int reduce_stock_value { get; set; }
    }


    public class ImageBuffer
    {
        public byte[] buffer { get; set; }
    }
}