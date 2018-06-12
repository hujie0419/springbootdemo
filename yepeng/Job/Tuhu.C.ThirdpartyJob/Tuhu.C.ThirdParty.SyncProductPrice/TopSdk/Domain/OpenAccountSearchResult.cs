using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Domain
{
    /// <summary>
    /// OpenAccountSearchResult Data Structure.
    /// </summary>
    [Serializable]
    public class OpenAccountSearchResult : TopObject
    {
        /// <summary>
        /// 状态码
        /// </summary>
        [XmlElement("code")]
        public long Code { get; set; }

        /// <summary>
        /// OpenAccount的列表
        /// </summary>
        [XmlArray("datas")]
        [XmlArrayItem("open_account")]
        public List<Top.Api.Domain.OpenAccount> Datas { get; set; }

        /// <summary>
        /// 状态信息
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// 查询是否成功，成功返回时有可能数据为空
        /// </summary>
        [XmlElement("successful")]
        public bool Successful { get; set; }
    }
}
