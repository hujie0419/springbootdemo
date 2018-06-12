using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageBaseCreateResponse.
    /// </summary>
    public class VmarketEticketPackageBaseCreateResponse : TopResponse
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
