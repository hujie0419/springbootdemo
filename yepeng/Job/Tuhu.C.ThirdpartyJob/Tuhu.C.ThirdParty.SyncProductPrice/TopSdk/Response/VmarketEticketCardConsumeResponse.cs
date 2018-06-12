using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketCardConsumeResponse.
    /// </summary>
    public class VmarketEticketCardConsumeResponse : TopResponse
    {
        /// <summary>
        /// 核销流水号,可以通过该流水号来撤销对应的核销操作
        /// </summary>
        [XmlElement("consume_secial_num")]
        public string ConsumeSecialNum { get; set; }

        /// <summary>
        /// 成功核销的份数
        /// </summary>
        [XmlElement("consume_success_num")]
        public long ConsumeSuccessNum { get; set; }

        /// <summary>
        /// 宝贝标题
        /// </summary>
        [XmlElement("item_title")]
        public string ItemTitle { get; set; }

        /// <summary>
        /// 整个订单剩余的可核销数量
        /// </summary>
        [XmlElement("left_num")]
        public long LeftNum { get; set; }

        /// <summary>
        /// 服务内容，用在凭证验证成功后pos机打印小票给消费者
        /// </summary>
        [XmlElement("print_tpl")]
        public string PrintTpl { get; set; }

        /// <summary>
        /// 0:失败，1:成功
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

        /// <summary>
        /// 返回码消费后，需要发送的短信的模版
        /// </summary>
        [XmlElement("sms_tpl")]
        public string SmsTpl { get; set; }
    }
}
