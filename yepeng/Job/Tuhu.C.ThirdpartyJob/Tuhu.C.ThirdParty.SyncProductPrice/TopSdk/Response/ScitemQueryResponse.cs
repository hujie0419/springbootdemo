using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// ScitemQueryResponse.
    /// </summary>
    public class ScitemQueryResponse : TopResponse
    {
        /// <summary>
        /// 分页
        /// </summary>
        [XmlElement("query_pagination")]
        public Top.Api.Domain.QueryPagination QueryPagination { get; set; }

        /// <summary>
        /// List<ScItemDO>
        /// </summary>
        [XmlArray("sc_item_list")]
        [XmlArrayItem("sc_item")]
        public List<Top.Api.Domain.ScItem> ScItemList { get; set; }

        /// <summary>
        /// 商品条数
        /// </summary>
        [XmlElement("total_page")]
        public long TotalPage { get; set; }

    }
}
