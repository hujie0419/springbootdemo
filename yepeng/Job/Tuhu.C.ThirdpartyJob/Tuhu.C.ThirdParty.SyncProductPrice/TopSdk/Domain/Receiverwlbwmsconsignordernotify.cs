using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// Receiverwlbwmsconsignordernotify Data Structure.
    /// </summary>
    [Serializable]
    public class Receiverwlbwmsconsignordernotify : TopObject
    {
        /// <summary>
        /// 收件方镇
        /// </summary>
        [XmlElement("receive_town")]
        public string ReceiveTown { get; set; }

        /// <summary>
        /// 收件方地址
        /// </summary>
        [XmlElement("receiver_address")]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 收件方区县
        /// </summary>
        [XmlElement("receiver_area")]
        public string ReceiverArea { get; set; }

        /// <summary>
        /// 收件方城市
        /// </summary>
        [XmlElement("receiver_city")]
        public string ReceiverCity { get; set; }

        /// <summary>
        /// 收件人手机
        /// </summary>
        [XmlElement("receiver_mobile")]
        public string ReceiverMobile { get; set; }

        /// <summary>
        /// 收件人名称
        /// </summary>
        [XmlElement("receiver_name")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人昵称
        /// </summary>
        [XmlElement("receiver_nick")]
        public string ReceiverNick { get; set; }

        /// <summary>
        /// 收件人电话
        /// </summary>
        [XmlElement("receiver_phone")]
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件方省份
        /// </summary>
        [XmlElement("receiver_province")]
        public string ReceiverProvince { get; set; }

        /// <summary>
        /// 收件方镇
        /// </summary>
        [XmlElement("receiver_town")]
        public string ReceiverTown { get; set; }

        /// <summary>
        /// 收件方邮编
        /// </summary>
        [XmlElement("receiver_zip_code")]
        public string ReceiverZipCode { get; set; }
    }
}
