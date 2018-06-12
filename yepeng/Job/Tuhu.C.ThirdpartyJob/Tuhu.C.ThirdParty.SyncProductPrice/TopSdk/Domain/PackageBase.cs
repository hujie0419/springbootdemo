using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// PackageBase Data Structure.
    /// </summary>
    [Serializable]
    public class PackageBase : TopObject
    {
        /// <summary>
        /// 如果是宝贝分账，即billType为2，则必须填写宝贝分账模板 (key-value 格式, 以 ; 分隔)
        /// </summary>
        [XmlElement("account_to_bill_map_str")]
        public string AccountToBillMapStr { get; set; }

        /// <summary>
        /// 包分账类型    0：不分账，1：门店分账，2：宝贝分账，3：账号分账。
        /// </summary>
        [XmlElement("bill_type")]
        public long BillType { get; set; }

        /// <summary>
        /// 系统自动生成，传入无效
        /// </summary>
        [XmlElement("bill_version")]
        public long BillVersion { get; set; }

        /// <summary>
        /// 核销码商id
        /// </summary>
        [XmlElement("consume_merchant_id")]
        public string ConsumeMerchantId { get; set; }

        /// <summary>
        /// 核销码商名字
        /// </summary>
        [XmlElement("consume_merchant_name")]
        public string ConsumeMerchantName { get; set; }

        /// <summary>
        /// 是否关联门店：0:不关联，1:关联
        /// </summary>
        [XmlElement("has_pos")]
        public long HasPos { get; set; }

        /// <summary>
        /// 是否核销放行   0：不核销放行，1：核销放行
        /// </summary>
        [XmlElement("is_consume_pass")]
        public long IsConsumePass { get; set; }

        /// <summary>
        /// 是否支持身份证核销：0:不支持，1:支持
        /// </summary>
        [XmlElement("is_id_card")]
        public long IsIdCard { get; set; }

        /// <summary>
        /// 是否支持子账号核销：0不支持，1支持
        /// </summary>
        [XmlElement("is_subaccount")]
        public long IsSubaccount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 系统自动生成，传入无效
        /// </summary>
        [XmlElement("package_id")]
        public long PackageId { get; set; }

        /// <summary>
        /// 包名
        /// </summary>
        [XmlElement("package_name")]
        public string PackageName { get; set; }

        /// <summary>
        /// 卖家ID
        /// </summary>
        [XmlElement("seller_id")]
        public long SellerId { get; set; }

        /// <summary>
        /// 卖家昵称
        /// </summary>
        [XmlElement("seller_nick")]
        public string SellerNick { get; set; }

        /// <summary>
        /// 发码方   0：淘宝，码商userId：码商，poolId：码池
        /// </summary>
        [XmlElement("send_id")]
        public long SendId { get; set; }

        /// <summary>
        /// 发码码商名字
        /// </summary>
        [XmlElement("send_merchant_name")]
        public string SendMerchantName { get; set; }

        /// <summary>
        /// 发码类型 0 不发码，1 淘宝发码， 2 信任卖家发码， 3 码商发码， 4 码库发码
        /// </summary>
        [XmlElement("send_type")]
        public long SendType { get; set; }

        /// <summary>
        /// 系统自动生成，传入无效
        /// </summary>
        [XmlElement("version")]
        public long Version { get; set; }
    }
}
