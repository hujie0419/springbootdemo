using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimRelationsGetResponse.
    /// </summary>
    public class OpenimRelationsGetResponse : TopResponse
    {
        /// <summary>
        /// 用户信息列表
        /// </summary>
        [XmlArray("users")]
        [XmlArrayItem("open_im_user")]
        public List<Top.Api.Domain.OpenImUser> Users { get; set; }

    }
}
