using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// WlbImportsGeneralConsignResponse.
    /// </summary>
    public class WlbImportsGeneralConsignResponse : TopResponse
    {
        /// <summary>
        /// 是否发货成功,true:成功，false：失败
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 发货成功后的物流订单编号
        /// </summary>
        [XmlElement("lgorder_code")]
        public string LgorderCode { get; set; }

        /// <summary>
        /// 业务错误编码
        /// </summary>
        [XmlElement("result_error_code")]
        public string ResultErrorCode { get; set; }

        /// <summary>
        /// 业务错误描述
        /// </summary>
        [XmlElement("result_error_msg")]
        public string ResultErrorMsg { get; set; }

    }
}
