using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimUsersGetResponse.
    /// </summary>
    public class OpenimUsersGetResponse : TopResponse
    {
        /// <summary>
        /// 获取的用户信息列表
        /// </summary>
        [XmlArray("userinfos")]
        [XmlArrayItem("userinfos")]
        public List<Top.Api.Domain.Userinfos> Userinfos { get; set; }

    }
}
