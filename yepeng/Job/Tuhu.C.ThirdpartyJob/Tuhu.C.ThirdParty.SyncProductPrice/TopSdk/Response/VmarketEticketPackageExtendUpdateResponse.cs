using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageExtendUpdateResponse.
    /// </summary>
    public class VmarketEticketPackageExtendUpdateResponse : TopResponse
    {
        /// <summary>
        /// 更新结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
