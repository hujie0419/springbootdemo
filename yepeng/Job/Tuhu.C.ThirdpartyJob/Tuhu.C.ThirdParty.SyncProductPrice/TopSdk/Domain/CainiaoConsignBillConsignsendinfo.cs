using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// CainiaoConsignBillConsignsendinfo Data Structure.
    /// </summary>
    [Serializable]
    public class CainiaoConsignBillConsignsendinfo : TopObject
    {
        /// <summary>
        /// 菜鸟订单编码
        /// </summary>
        [XmlElement("cn_order_code")]
        public string CnOrderCode { get; set; }

        /// <summary>
        /// 仓库订单完成时间
        /// </summary>
        [XmlElement("confirm_time")]
        public string ConfirmTime { get; set; }

        /// <summary>
        /// 发票确认信息列表
        /// </summary>
        [XmlArray("invoince_confirm_list")]
        [XmlArrayItem("cainiao_consign_bill_invoinceconfirmlist")]
        public List<Top.Api.Domain.CainiaoConsignBillInvoinceconfirmlist> InvoinceConfirmList { get; set; }

        /// <summary>
        /// ERP订单编码
        /// </summary>
        [XmlElement("order_code")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        [XmlArray("order_item_list")]
        [XmlArrayItem("cainiao_consign_bill_orderitemlist")]
        public List<Top.Api.Domain.CainiaoConsignBillOrderitemlist> OrderItemList { get; set; }

        /// <summary>
        /// 订单类型 201 销售出库 502 换货出库 503 补发出库
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 订单状态 WMS_ACCEPT 接单成功 WMS_REJECT 接单失败 WMS_CONFIRMED 仓库生产完成
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 仓储编码
        /// </summary>
        [XmlElement("store_code")]
        public string StoreCode { get; set; }

        /// <summary>
        /// 运单信息
        /// </summary>
        [XmlArray("tms_order_list")]
        [XmlArrayItem("cainiao_consign_bill_tmsorderlist")]
        public List<Top.Api.Domain.CainiaoConsignBillTmsorderlist> TmsOrderList { get; set; }
    }
}
