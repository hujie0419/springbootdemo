using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// EticketCode Data Structure.
    /// </summary>
    [Serializable]
    public class EticketCode : TopObject
    {
        /// <summary>
        /// 电子凭证码
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 可用数量
        /// </summary>
        [XmlElement("num")]
        public long Num { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [XmlElement("order_id")]
        public long OrderId { get; set; }

        /// <summary>
        /// 二维码的图片地址
        /// </summary>
        [XmlElement("qrcode_url")]
        public string QrcodeUrl { get; set; }

        /// <summary>
        /// 码状态
        /// </summary>
        [XmlElement("status")]
        public long Status { get; set; }
    }
}
