using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageRemoveResponse.
    /// </summary>
    public class VmarketEticketPackageRemoveResponse : TopResponse
    {
        /// <summary>
        /// 删除结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
