using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimIoscertProductionSetResponse.
    /// </summary>
    public class OpenimIoscertProductionSetResponse : TopResponse
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

    }
}
