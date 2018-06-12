using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimTribeGetmembersResponse.
    /// </summary>
    public class OpenimTribeGetmembersResponse : TopResponse
    {
        /// <summary>
        /// OPENIM群成员列表
        /// </summary>
        [XmlArray("tribe_user_list")]
        [XmlArrayItem("tribe_user")]
        public List<Top.Api.Domain.TribeUser> TribeUserList { get; set; }

    }
}
