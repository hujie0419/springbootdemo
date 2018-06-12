using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenAccountCreateResponse.
    /// </summary>
    public class OpenAccountCreateResponse : TopResponse
    {
        /// <summary>
        /// 插入数据的Open Account Id的列表
        /// </summary>
        [XmlArray("datas")]
        [XmlArrayItem("openaccount_long")]
        public List<Top.Api.Domain.OpenaccountLong> Datas { get; set; }

    }
}
