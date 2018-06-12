using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageInfosWlbWmsStockOutOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class PackageInfosWlbWmsStockOutOrderConfirm : TopObject
    {
        /// <summary>
        /// 包裹编号
        /// </summary>
        [XmlElement("package_code")]
        public string PackageCode { get; set; }

        /// <summary>
        /// 包裹高度，单位：毫米
        /// </summary>
        [XmlElement("package_height")]
        public long PackageHeight { get; set; }

        /// <summary>
        /// 包裹里面的商品信息
        /// </summary>
        [XmlElement("package_item_items")]
        public Top.Api.Domain.PackageItemItemsWlbWmsStockOutOrderConfirm PackageItemItems { get; set; }

        /// <summary>
        /// 包裹长度，单位：毫米
        /// </summary>
        [XmlElement("package_length")]
        public long PackageLength { get; set; }

        /// <summary>
        /// 包裹重量，单位：克
        /// </summary>
        [XmlElement("package_weight")]
        public long PackageWeight { get; set; }

        /// <summary>
        /// 包裹宽度，单位：毫米
        /// </summary>
        [XmlElement("package_width")]
        public long PackageWidth { get; set; }

        /// <summary>
        /// 快递公司编码
        /// </summary>
        [XmlElement("tms_code")]
        public string TmsCode { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        [XmlElement("tms_order_code")]
        public string TmsOrderCode { get; set; }
    }
}
