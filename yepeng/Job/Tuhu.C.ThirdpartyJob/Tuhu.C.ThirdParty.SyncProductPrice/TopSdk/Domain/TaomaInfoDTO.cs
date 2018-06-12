using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TaomaInfoDTO Data Structure.
    /// </summary>
    [Serializable]
    public class TaomaInfoDTO : TopObject
    {
        /// <summary>
        /// 买家userId
        /// </summary>
        [XmlElement("buyer_id")]
        public long BuyerId { get; set; }

        /// <summary>
        /// 码信息
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 卡券使用条件
        /// </summary>
        [XmlElement("condition")]
        public string Condition { get; set; }

        /// <summary>
        /// 卡券描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 来源taobao、tmall、juhuasuan
        /// </summary>
        [XmlElement("from_name")]
        public string FromName { get; set; }

        /// <summary>
        /// 可用数量
        /// </summary>
        [XmlElement("left_num")]
        public long LeftNum { get; set; }

        /// <summary>
        /// 外部编号id
        /// </summary>
        [XmlElement("outer_id")]
        public long OuterId { get; set; }

        /// <summary>
        /// 卡券金额
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 卖家userId
        /// </summary>
        [XmlElement("seller_id")]
        public long SellerId { get; set; }

        /// <summary>
        /// 卖家微淘帐号
        /// </summary>
        [XmlElement("seller_nick")]
        public string SellerNick { get; set; }

        /// <summary>
        /// 码状态
        /// </summary>
        [XmlElement("status")]
        public long Status { get; set; }

        /// <summary>
        /// 买一送一
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 卡券使用结束时间
        /// </summary>
        [XmlElement("valid_end")]
        public string ValidEnd { get; set; }

        /// <summary>
        /// 卡券使用开始时间
        /// </summary>
        [XmlElement("valid_start")]
        public string ValidStart { get; set; }
    }
}
