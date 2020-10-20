#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-04-03 12:45:17.00531 +08:00
*/
#endregion

using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace JdSdk.Response
{
    /// <summary>
    /// 删除店铺的类目 Response
    /// </summary>
    public class SellercatDeleteResponse : JdResponse
    {
        /// <summary>
        /// 删除时间
        /// </summary>
        [XmlElement("created")]
        [JsonProperty("created")]
        public String Created
        {
            get;
            set;
        }
    }
}
