using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageExtendAddResponse.
    /// </summary>
    public class VmarketEticketPackageExtendAddResponse : TopResponse
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
