using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketTaomaMerchantConsumeResponse.
    /// </summary>
    public class VmarketTaomaMerchantConsumeResponse : TopResponse
    {
        /// <summary>
        /// 包含了卡券相关信息：金额、有效期、可用份数、code的状态、卖家、买家信息
        /// </summary>
        [XmlElement("taoma_info")]
        public Top.Api.Domain.TaomaInfoDTO TaomaInfo { get; set; }
    }
}
