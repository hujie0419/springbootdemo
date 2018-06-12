using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// CloudpushNoticeIosResponse.
    /// </summary>
    public class CloudpushNoticeIosResponse : TopResponse
    {
        /// <summary>
        /// 请求是否成功.
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 请求错误时产生的错误代码.
        /// </summary>
        [XmlElement("request_error_code")]
        public long RequestErrorCode { get; set; }

        /// <summary>
        /// 请求产生的错误信息.
        /// </summary>
        [XmlElement("request_error_msg")]
        public string RequestErrorMsg { get; set; }

    }
}
