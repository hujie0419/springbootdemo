using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// WlbWmsConsignOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class WlbWmsConsignOrderConfirm : TopObject
    {
        /// <summary>
        /// 支持出入库单多次确认 0 最后一次确认或是一次性确认； 1 多次确认；当多次确认时，确认的ITEM种类全部被确认时，确认完成默认值为 0 例如输入2认为是0
        /// </summary>
        [XmlElement("confirm_type")]
        public long ConfirmType { get; set; }

        /// <summary>
        /// 发票确认信息列表
        /// </summary>
        [XmlArray("invoince_confirms")]
        [XmlArrayItem("invoince_confirms_wlb_wms_consign_order_confirm")]
        public List<Top.Api.Domain.InvoinceConfirmsWlbWmsConsignOrderConfirm> InvoinceConfirms { get; set; }

        /// <summary>
        /// 商家订单编码
        /// </summary>
        [XmlElement("order_code")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单出库完成时间
        /// </summary>
        [XmlElement("order_confirm_time")]
        public string OrderConfirmTime { get; set; }

        /// <summary>
        /// 拆合单信息，如果仓库合并ERP订单后，将多个ERP订单合并在这个字段中；
        /// </summary>
        [XmlElement("order_join")]
        public string OrderJoin { get; set; }

        /// <summary>
        /// 订单类型 201 一般销售发货订单 202 B2B销售发货订单 502 换货出库单 503 补发出库单
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 外部业务编码，消息ID，用于去重，一个合作伙伴中要求唯一，多次确认时，每次传入要求唯一
        /// </summary>
        [XmlElement("out_biz_code")]
        public string OutBizCode { get; set; }

        /// <summary>
        /// 仓储订单编码
        /// </summary>
        [XmlElement("store_order_code")]
        public string StoreOrderCode { get; set; }

        /// <summary>
        /// 运单信息列表
        /// </summary>
        [XmlArray("tms_orders")]
        [XmlArrayItem("tms_orders_wlb_wms_consign_order_confirm")]
        public List<Top.Api.Domain.TmsOrdersWlbWmsConsignOrderConfirm> TmsOrders { get; set; }
    }
}
