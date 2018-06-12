using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// VmarketEticketPackageBaseListGetResponse.
    /// </summary>
    public class VmarketEticketPackageBaseListGetResponse : TopResponse
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.PackageResult Result { get; set; }

    }
}
