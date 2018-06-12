using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenAccountListResponse.
    /// </summary>
    public class OpenAccountListResponse : TopResponse
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        [XmlArray("datas")]
        [XmlArrayItem("openaccount_object")]
        public List<Top.Api.Domain.OpenaccountObject> Datas { get; set; }

    }
}
