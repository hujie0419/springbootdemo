using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCardReverseResponse.
    /// </summary>
    public class VmarketEticketCardReverseResponse : TopResponse
    {
        /// <summary>
        /// 宝贝标题
        /// </summary>
        [XmlElement("item_title")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// 整个订单的剩余可核销数量
        /// </summary>
        [XmlElement("left_num")]
        public long LeftNum { get; set; }

        /// <summary>
        /// 0:失败，1:成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }
    }
}
