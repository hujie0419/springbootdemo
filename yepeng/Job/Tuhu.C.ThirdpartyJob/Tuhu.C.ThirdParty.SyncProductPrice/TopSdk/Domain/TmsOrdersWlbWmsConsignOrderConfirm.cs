using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// TmsOrdersWlbWmsConsignOrderConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class TmsOrdersWlbWmsConsignOrderConfirm : TopObject
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
        /// 包裹长度，单位：毫米
        /// </summary>
        [XmlElement("package_length")]
        public long PackageLength { get; set; }

        /// <summary>
        /// 包裹的包材信息列表
        /// </summary>
        [XmlArray("package_material_list")]
        [XmlArrayItem("package_material_list_wlb_wms_consign_order_confirm")]
        public List<Top.Api.Domain.PackageMaterialListWlbWmsConsignOrderConfirm> PackageMaterialList { get; set; }

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
        /// 包裹里面的商品信息列表
        /// </summary>
        [XmlArray("tms_items")]
        [XmlArrayItem("tms_items_wlb_wms_consign_order_confirm")]
        public List<Top.Api.Domain.TmsItemsWlbWmsConsignOrderConfirm> TmsItems { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        [XmlElement("tms_order_code")]
        public string TmsOrderCode { get; set; }
    }
}
