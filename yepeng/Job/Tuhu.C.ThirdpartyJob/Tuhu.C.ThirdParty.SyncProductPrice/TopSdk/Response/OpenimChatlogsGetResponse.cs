using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimChatlogsGetResponse.
    /// </summary>
    public class OpenimChatlogsGetResponse : TopResponse
    {
        /// <summary>
        /// 聊天记录查询结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.RoamingMessageResult Result { get; set; }

    }
}
