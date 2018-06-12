using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// FenxiaoOrderRemarkUpdateResponse.
    /// </summary>
    public class FenxiaoOrderRemarkUpdateResponse : TopResponse
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

    }
}
