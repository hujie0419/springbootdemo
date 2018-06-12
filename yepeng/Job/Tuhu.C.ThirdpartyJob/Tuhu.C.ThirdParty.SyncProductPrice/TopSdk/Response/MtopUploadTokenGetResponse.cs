using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// MtopUploadTokenGetResponse.
    /// </summary>
    public class MtopUploadTokenGetResponse : TopResponse
    {
        /// <summary>
        /// code
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 单次上传文件块最大大小，单位 byte
        /// </summary>
        [XmlElement("max_body_length")]
        public long MaxBodyLength { get; set; }

        /// <summary>
        /// 单个文件重试上传次数
        /// </summary>
        [XmlElement("max_retry_times")]
        public long MaxRetryTimes { get; set; }

        /// <summary>
        /// msg
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// 本次指定的上传文件服务器地址
        /// </summary>
        [XmlElement("server_address")]
        public string ServerAddress { get; set; }

        /// <summary>
        /// token失效时间点
        /// </summary>
        [XmlElement("timeout")]
        public long Timeout { get; set; }

        /// <summary>
        /// 颁发的上传令牌
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }

    }
}
