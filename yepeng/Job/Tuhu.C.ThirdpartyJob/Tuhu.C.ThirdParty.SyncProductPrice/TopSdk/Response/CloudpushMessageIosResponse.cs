using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// CloudpushMessageIosResponse.
    /// </summary>
    public class CloudpushMessageIosResponse : TopResponse
    {
        /// <summary>
        /// 请求是否成功.
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 请求出现错误的错误代码.
        /// </summary>
        [XmlElement("request_error_code")]
        public long RequestErrorCode { get; set; }

        /// <summary>
        /// 请求失败时候的错误信息.
        /// </summary>
        [XmlElement("request_error_msg")]
        public string RequestErrorMsg { get; set; }

    }
}
