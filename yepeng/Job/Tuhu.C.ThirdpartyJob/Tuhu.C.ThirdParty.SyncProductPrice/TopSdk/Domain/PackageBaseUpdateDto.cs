using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageBaseUpdateDto Data Structure.
    /// </summary>
    [Serializable]
    public class PackageBaseUpdateDto : TopObject
    {
        /// <summary>
        /// 可修改属性，宝贝分账的模板
        /// </summary>
        [XmlElement("account_to_bill_map_str")]
        public string AccountToBillMapStr { get; set; }

        /// <summary>
        /// 限制条件，宝贝分账模板的版本
        /// </summary>
        [XmlElement("bill_version")]
        public long BillVersion { get; set; }

        /// <summary>
        /// 可修改属性，备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 限制条件，用于要修改的定位包
        /// </summary>
        [XmlElement("package_id")]
        public long PackageId { get; set; }

        /// <summary>
        /// 可修改属性，包名
        /// </summary>
        [XmlElement("package_name")]
        public string PackageName { get; set; }

        /// <summary>
        /// 限制条件，用于检查是否用户本人
        /// </summary>
        [XmlElement("seller_id")]
        public long SellerId { get; set; }

        /// <summary>
        /// 限制条件，包版本
        /// </summary>
        [XmlElement("version")]
        public long Version { get; set; }
    }
}
