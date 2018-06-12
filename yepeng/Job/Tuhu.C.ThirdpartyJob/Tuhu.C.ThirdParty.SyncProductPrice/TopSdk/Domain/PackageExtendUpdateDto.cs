using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageExtendUpdateDto Data Structure.
    /// </summary>
    [Serializable]
    public class PackageExtendUpdateDto : TopObject
    {
        /// <summary>
        /// 核销账号列表
        /// </summary>
        [XmlElement("account_list_str")]
        public string AccountListStr { get; set; }

        /// <summary>
        /// 分账模板
        /// </summary>
        [XmlElement("account_to_bill_map")]
        public object AccountToBillMap { get; set; }

        /// <summary>
        /// 分账模板
        /// </summary>
        [XmlElement("account_to_bill_map_str")]
        public string AccountToBillMapStr { get; set; }

        /// <summary>
        /// 预约电话
        /// </summary>
        [XmlElement("obs_tel")]
        public string ObsTel { get; set; }

        /// <summary>
        /// 包扩展id
        /// </summary>
        [XmlElement("package_extend_id")]
        public long PackageExtendId { get; set; }

        /// <summary>
        /// 包id
        /// </summary>
        [XmlElement("package_id")]
        public long PackageId { get; set; }

        /// <summary>
        /// 卖家id
        /// </summary>
        [XmlElement("seller_id")]
        public long SellerId { get; set; }

        /// <summary>
        /// 版本控制
        /// </summary>
        [XmlElement("version")]
        public long Version { get; set; }
    }
}
