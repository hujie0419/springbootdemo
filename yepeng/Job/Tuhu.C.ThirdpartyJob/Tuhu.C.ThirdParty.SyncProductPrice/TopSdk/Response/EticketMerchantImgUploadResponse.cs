using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// EticketMerchantImgUploadResponse.
    /// </summary>
    public class EticketMerchantImgUploadResponse : TopResponse
    {
        /// <summary>
        /// 回复对象
        /// </summary>
        [XmlElement("resp_body")]
        public UploadImgCallbackRespDomain RespBody { get; set; }

        /// <summary>
        /// 子结果码
        /// </summary>
        [XmlElement("ret_code")]
        public string RetCode { get; set; }

        /// <summary>
        /// 子结果信息
        /// </summary>
        [XmlElement("ret_msg")]
        public string RetMsg { get; set; }

	/// <summary>
/// UploadImgCallbackRespDomain Data Structure.
/// </summary>
[Serializable]

public class UploadImgCallbackRespDomain : TopObject
{
	        /// <summary>
	        /// 扩展属性
	        /// </summary>
	        [XmlElement("attribute_map")]
	        public string AttributeMap { get; set; }
	
	        /// <summary>
	        /// 图片在淘宝的文件名
	        /// </summary>
	        [XmlElement("file_name")]
	        public string FileName { get; set; }
}

    }
}
