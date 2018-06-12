using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageExtendDeleteResponse.
    /// </summary>
    public class VmarketEticketPackageExtendDeleteResponse : TopResponse
    {
        /// <summary>
        /// 删除结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
