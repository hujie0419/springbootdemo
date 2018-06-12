using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// BaichuanUserLogindoublecheckResponse.
    /// </summary>
    public class BaichuanUserLogindoublecheckResponse : TopResponse
    {
        /// <summary>
        /// name
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

    }
}
