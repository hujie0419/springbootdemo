using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// UploadTokenRequestV Data Structure.
    /// </summary>
    [Serializable]
    public class UploadTokenRequestV : TopObject
    {
        /// <summary>
        /// 多媒体中心分配的业务码, mtopupload或其他
        /// </summary>
        [XmlElement("biz_code")]
        public string BizCode { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [XmlElement("client_ip")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 客户端网络类型 wifi 或 2g 或 3g 或 cdma 或 gprs
        /// </summary>
        [XmlElement("client_net_type")]
        public string ClientNetType { get; set; }

        /// <summary>
        /// 文件内容的CRC32校验和
        /// </summary>
        [XmlElement("crc")]
        public Nullable<long> Crc { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [XmlElement("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小，单位byte
        /// </summary>
        [XmlElement("file_size")]
        public Nullable<long> FileSize { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [XmlElement("private_data")]
        public string PrivateData { get; set; }

        /// <summary>
        /// 上传类型：resumable 或 normal
        /// </summary>
        [XmlElement("upload_type")]
        public string UploadType { get; set; }

        /// <summary>
        /// session
        /// </summary>
        [XmlElement("user_id")]
        public Nullable<long> UserId { get; set; }
    }
}
