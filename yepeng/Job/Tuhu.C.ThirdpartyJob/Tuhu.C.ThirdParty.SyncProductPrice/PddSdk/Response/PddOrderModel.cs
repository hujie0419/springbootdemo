using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PddSdk.Response
{
    [Serializable]
    public class PddOrderModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [JsonProperty("order_sn")]
        public string OrderSn { get; set; }

        /// <summary>
        /// 成团时间
        /// </summary>
        [JsonProperty("confirm_time")]
        public DateTime ConfirmTime { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        [JsonProperty("receiver_name")]
        public string ReceiverName { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// 区县
        /// </summary>
        [JsonProperty("town")]
        public string Town { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 收件人电话
        /// </summary>
        [JsonProperty("receiver_phone")]
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 支付金额（元）
        /// </summary>
        [JsonProperty("pay_amount")]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 商品金额
        /// </summary>
        [JsonProperty("goods_amount")]
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        [JsonProperty("discount_amount")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 邮费
        /// </summary>
        [JsonProperty("postage")]
        public decimal Postage { get; set; }

        /// <summary>
        /// 支付单号
        /// </summary>
        [JsonProperty("pay_no")]
        public string PayNo { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [JsonProperty("pay_type")]
        public string PayType { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [JsonProperty("id_card_num")]
        public string IdCardNum { get; set; }

        /// <summary>
        /// 身份证姓名
        /// </summary>
        [JsonProperty("id_card_name")]
        public string IdCardName { get; set; }

        /// <summary>
        /// 快递公司编号
        /// </summary>
        [JsonProperty("logistics_id")]
        public string LogisticsId { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>
        [JsonProperty("shipping_time")]
        public DateTime? ShippingTime { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        [JsonProperty("order_status")]
        public int OrderStatus { get; set; }

        /// <summary>
        /// 是否抽奖订单
        /// </summary>
        [JsonProperty("is_lucky_flag")]
        public int IsLuckyFlag { get; set; }

        /// <summary>
        /// 退款状态
        /// </summary>
        [JsonProperty("refund_status")]
        public int RefundStatus { get; set; }

        /// <summary>
        ///更新时间
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 承诺发货时间
        /// </summary>
        [JsonProperty("last_ship_time")]
        public DateTime LastShipTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 平台折扣金额
        /// </summary>
        [JsonProperty("platform_discount")]
        public int? PlatformDiscount { get; set; }

        /// <summary>
        /// 商家折扣金额
        /// </summary>
        [JsonProperty("seller_discount")]
        public int? SellerDiscount { get; set; }

        /// <summary>
        /// 团长免单优惠金额
        /// </summary>
        [JsonProperty("capital_free_discount")]
        public int? CapitalFreeDiscount { get; set; }

        /// <summary>
        /// 产品明细
        /// </summary>
        [JsonProperty("item_list")]
        public List<PddOrderItem> ItemList { get; set; }
    }
}
