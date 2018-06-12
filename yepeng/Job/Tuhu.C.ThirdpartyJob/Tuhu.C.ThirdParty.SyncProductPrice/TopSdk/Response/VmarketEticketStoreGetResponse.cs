using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketStoreGetResponse.
    /// </summary>
    public class VmarketEticketStoreGetResponse : TopResponse
    {
        /// <summary>
        /// 商户地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [XmlElement("contract")]
        public string Contract { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [XmlElement("district")]
        public string District { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }

        /// <summary>
        /// 自有卖家导入门店的时候，可以把自己系统门店信息的主键或者唯一key传入，用于快速匹配
        /// </summary>
        [XmlElement("selfcode")]
        public string Selfcode { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        [XmlElement("store_id")]
        public long StoreId { get; set; }

    }
}
