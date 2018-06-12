using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// WlbWmsInventoryCount Data Structure.
    /// </summary>
    [Serializable]
    public class WlbWmsInventoryCount : TopObject
    {
        /// <summary>
        /// 订单商品信息列表
        /// </summary>
        [XmlElement("item_list")]
        public Top.Api.Domain.ItemListWlbWmsInventoryCount ItemList { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [XmlElement("order_code")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 2013-01-01 10:00:00
        /// </summary>
        [XmlElement("order_confirm_time")]
        public string OrderConfirmTime { get; set; }

        /// <summary>
        /// 701
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 2456
        /// </summary>
        [XmlElement("out_biz_code")]
        public string OutBizCode { get; set; }

        /// <summary>
        /// 1231223
        /// </summary>
        [XmlElement("owner_user_id")]
        public string OwnerUserId { get; set; }

        /// <summary>
        /// --
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 1011
        /// </summary>
        [XmlElement("store_code")]
        public string StoreCode { get; set; }

        /// <summary>
        /// LBX0001
        /// </summary>
        [XmlElement("store_order_code")]
        public string StoreOrderCode { get; set; }
    }
}
