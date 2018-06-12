using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// BaichuanUserLoginResponse.
    /// </summary>
    public class BaichuanUserLoginResponse : TopResponse
    {
        /// <summary>
        /// name
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

    }
}
