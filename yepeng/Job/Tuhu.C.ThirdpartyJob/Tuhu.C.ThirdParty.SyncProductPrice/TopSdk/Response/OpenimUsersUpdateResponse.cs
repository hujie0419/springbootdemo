using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimUsersUpdateResponse.
    /// </summary>
    public class OpenimUsersUpdateResponse : TopResponse
    {
        /// <summary>
        /// 失败的uid列表
        /// </summary>
        [XmlArray("uid_fail")]
        [XmlArrayItem("string")]
        public List<string> UidFail { get; set; }

        /// <summary>
        /// 成功的uid列表
        /// </summary>
        [XmlArray("uid_succ")]
        [XmlArrayItem("string")]
        public List<string> UidSucc { get; set; }

    }
}
