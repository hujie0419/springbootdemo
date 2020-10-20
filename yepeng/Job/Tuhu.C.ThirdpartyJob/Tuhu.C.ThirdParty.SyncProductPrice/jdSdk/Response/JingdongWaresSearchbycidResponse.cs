#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-04-03 12:45:24.94677 +08:00
*/
#endregion

using System.Collections.Generic;
using System.Xml.Serialization;
using JdSdk.Domain;
using Newtonsoft.Json;

namespace JdSdk.Response
{
    /// <summary>
    /// 根据三级分类id，搜索商品数据 Response
    /// </summary>
    public class JingdongWaresSearchbycidResponse : JdResponse
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        [XmlElement("wareInfos")]
        [JsonProperty("wareInfos")]
        public List<Ware> WareInfos
        {
            get;
            set;
        }

    }
}
