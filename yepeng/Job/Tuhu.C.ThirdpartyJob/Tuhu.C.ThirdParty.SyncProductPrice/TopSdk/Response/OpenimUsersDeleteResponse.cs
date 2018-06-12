using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimUsersDeleteResponse.
    /// </summary>
    public class OpenimUsersDeleteResponse : TopResponse
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [XmlArray("result")]
        [XmlArrayItem("string")]
        public List<string> Result { get; set; }

    }
}
