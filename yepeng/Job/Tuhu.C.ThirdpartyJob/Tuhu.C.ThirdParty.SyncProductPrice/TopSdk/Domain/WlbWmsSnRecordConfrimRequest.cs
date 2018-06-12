using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// WlbWmsSnRecordConfrimRequest Data Structure.
    /// </summary>
    [Serializable]
    public class WlbWmsSnRecordConfrimRequest : TopObject
    {
        /// <summary>
        /// 商品列表
        /// </summary>
        [XmlArray("item_list")]
        [XmlArrayItem("item_listwlbwmssnrecordconfrim")]
        public List<Top.Api.Domain.ItemListwlbwmssnrecordconfrim> ItemList { get; set; }

        /// <summary>
        /// ERP订单编码
        /// </summary>
        [XmlElement("order_code")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 外部业务编码，一个合作伙伴中要求唯一多次确认时，每次传入要求唯一
        /// </summary>
        [XmlElement("out_biz_code")]
        public string OutBizCode { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        [XmlElement("owner_user_id")]
        public string OwnerUserId { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        [XmlElement("store_code")]
        public string StoreCode { get; set; }

        /// <summary>
        /// 仓储订单编码，LBX号
        /// </summary>
        [XmlElement("store_order_code")]
        public string StoreOrderCode { get; set; }
    }
}
