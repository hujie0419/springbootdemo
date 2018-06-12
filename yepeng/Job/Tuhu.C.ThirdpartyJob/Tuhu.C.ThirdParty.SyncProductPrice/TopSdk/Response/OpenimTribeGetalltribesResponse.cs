using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// OpenimTribeGetalltribesResponse.
    /// </summary>
    public class OpenimTribeGetalltribesResponse : TopResponse
    {
        /// <summary>
        /// 群列表信息
        /// </summary>
        [XmlArray("tribe_info_list")]
        [XmlArrayItem("tribe_info")]
        public List<Top.Api.Domain.TribeInfo> TribeInfoList { get; set; }

    }
}
