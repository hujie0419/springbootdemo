using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// WlbWmsStockOutOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class WlbWmsStockOutOrderConfirm : TopObject
    {
        /// <summary>
        /// 运输公司名称
        /// </summary>
        [XmlElement("carriers_name")]
        public string CarriersName { get; set; }

        /// <summary>
        /// 支持出入库单多次确认 0 最后一次确认或是一次性确认；1 多次确认；当多次确认时，确认的ITEM种类全部被确认时，确认完成默  认值为 0 例如输入2认为是0
        /// </summary>
        [XmlElement("confirm_type")]
        public long ConfirmType { get; set; }

        /// <summary>
        /// 仓库订单编码
        /// </summary>
        [XmlElement("order_code")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单出库时间
        /// </summary>
        [XmlElement("order_confirm_time")]
        public string OrderConfirmTime { get; set; }

        /// <summary>
        /// 订单商品信息
        /// </summary>
        [XmlElement("order_items")]
        public Top.Api.Domain.OrderItemsWlbWmsStockOutOrderConfirm OrderItems { get; set; }

        /// <summary>
        /// 单据类型301 调拨出库单303 领用出库单901 退供出库单903 其他出库单
        /// </summary>
        [XmlElement("order_type")]
        public long OrderType { get; set; }

        /// <summary>
        /// 外部业务编码，消息ID，用于去重一个合作伙伴中要求唯一
        /// </summary>
        [XmlElement("out_biz_code")]
        public string OutBizCode { get; set; }

        /// <summary>
        /// 包裹信息
        /// </summary>
        [XmlElement("package_infos")]
        public Top.Api.Domain.PackageInfosWlbWmsStockOutOrderConfirm PackageInfos { get; set; }

        /// <summary>
        /// 仓配订单编码
        /// </summary>
        [XmlElement("store_order_code")]
        public string StoreOrderCode { get; set; }

        /// <summary>
        /// 运单号或者是托运单号
        /// </summary>
        [XmlElement("tms_order_code")]
        public string TmsOrderCode { get; set; }

        /// <summary>
        /// 总包裹数
        /// </summary>
        [XmlElement("total_package_qty")]
        public long TotalPackageQty { get; set; }

        /// <summary>
        /// 总体积，单位立方厘米
        /// </summary>
        [XmlElement("total_package_volume")]
        public long TotalPackageVolume { get; set; }

        /// <summary>
        /// 总重量，单位克
        /// </summary>
        [XmlElement("total_package_weight")]
        public long TotalPackageWeight { get; set; }
    }
}
