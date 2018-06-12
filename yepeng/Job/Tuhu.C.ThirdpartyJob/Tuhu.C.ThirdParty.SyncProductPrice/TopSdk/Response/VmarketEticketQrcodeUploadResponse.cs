using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketQrcodeUploadResponse.
    /// </summary>
    public class VmarketEticketQrcodeUploadResponse : TopResponse
    {
        /// <summary>
        /// 图片文件名称
        /// </summary>
        [XmlElement("img_filename")]
        public string ImgFilename { get; set; }

        /// <summary>
        /// 1:成功  其它为失败
        /// </summary>
        [XmlElement("ret_code")]
        public long RetCode { get; set; }

    }
}
