using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// CloudpushMessageAndroidResponse.
    /// </summary>
    public class CloudpushMessageAndroidResponse : TopResponse
    {
        /// <summary>
        /// 请求是否成功.
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 若请求失败，则返回对应的error code.
        /// </summary>
        [XmlElement("request_error_code")]
        public long RequestErrorCode { get; set; }

        /// <summary>
        /// 请求失败后返回的错误信息.
        /// </summary>
        [XmlElement("request_error_msg")]
        public string RequestErrorMsg { get; set; }

    }
}
