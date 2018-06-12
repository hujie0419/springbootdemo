using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimUsersAddResponse.
    /// </summary>
    public class OpenimUsersAddResponse : TopResponse
    {
        /// <summary>
        /// 成功用户列表
        /// </summary>
        [XmlArray("uid_succ")]
        [XmlArrayItem("string")]
        public List<string> UidSucc { get; set; }

    }
}
