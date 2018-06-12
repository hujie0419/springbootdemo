using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Orderitemwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Orderitemwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 商品成交价格=销售价格-优惠金额
        /// </summary>
        [XmlElement("actual_price")]
        public long ActualPrice { get; set; }

        /// <summary>
        /// 商品优惠金额
        /// </summary>
        [XmlElement("discount_amount")]
        public long DiscountAmount { get; set; }

        /// <summary>
        /// 订单商品拓展属性数据
        /// </summary>
        [XmlElement("extend_fields")]
        public string ExtendFields { get; set; }

        /// <summary>
        /// 库存类型
        /// </summary>
        [XmlElement("inventory_type")]
        public long InventoryType { get; set; }

        /// <summary>
        /// 交易平台商品编码
        /// </summary>
        [XmlElement("item_ext_code")]
        public string ItemExtCode { get; set; }

        /// <summary>
        /// ERP商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        [XmlElement("item_price")]
        public long ItemPrice { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [XmlElement("item_quantity")]
        public long ItemQuantity { get; set; }

        /// <summary>
        /// ERP订单明细行号ID
        /// </summary>
        [XmlElement("order_item_id")]
        public string OrderItemId { get; set; }

        /// <summary>
        /// 平台交易编码
        /// </summary>
        [XmlElement("order_source_code")]
        public string OrderSourceCode { get; set; }

        /// <summary>
        /// 货主ID 代销情况下货主ID和卖家ID不同
        /// </summary>
        [XmlElement("owner_user_id")]
        public string OwnerUserId { get; set; }

        /// <summary>
        /// 货主名称
        /// </summary>
        [XmlElement("owner_user_name")]
        public string OwnerUserName { get; set; }

        /// <summary>
        /// 平台子交易编码
        /// </summary>
        [XmlElement("sub_source_code")]
        public string SubSourceCode { get; set; }

        /// <summary>
        /// 卖家ID,一般情况下，货主ID和卖家ID相同
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 卖家名称(销售店铺名称)
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }
    }
}
