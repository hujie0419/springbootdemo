using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// TokenInfoExt Data Structure.
    /// </summary>
    [Serializable]
    public class TokenInfoExt : TopObject
    {
        /// <summary>
        /// open account当前token info中open account id对应的open account信息
        /// </summary>
        [XmlElement("open_account")]
        public Top.Api.Domain.OpenAccount OpenAccount { get; set; }
    }
}
