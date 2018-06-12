using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageExtendListGetResponse.
    /// </summary>
    public class VmarketEticketPackageExtendListGetResponse : TopResponse
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }
    }
}
