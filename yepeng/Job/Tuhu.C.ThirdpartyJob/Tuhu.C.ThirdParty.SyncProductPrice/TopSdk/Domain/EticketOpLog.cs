using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// EticketOpLog Data Structure.
    /// </summary>
    [Serializable]
    public class EticketOpLog : TopObject
    {
        /// <summary>
        /// 操作金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 操作流水号
        /// </summary>
        [XmlElement("consume_serial_num")]
        public string ConsumeSerialNum { get; set; }

        /// <summary>
        /// 手机号码后四位
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 操作数量
        /// </summary>
        [XmlElement("num")]
        public long Num { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [XmlElement("op_time")]
        public string OpTime { get; set; }

        /// <summary>
        /// 操作类型 1:核销 2:冲正
        /// </summary>
        [XmlElement("op_type")]
        public long OpType { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        [XmlElement("order_id")]
        public long OrderId { get; set; }

        /// <summary>
        /// 操作员身份ID
        /// </summary>
        [XmlElement("pos_id")]
        public string PosId { get; set; }
    }
}
