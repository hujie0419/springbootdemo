#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-07-29 22:39:07.03631 +08:00
*/
#endregion

using System.Xml.Serialization;
using JdSdk.Domain;
using Newtonsoft.Json;

namespace JdSdk.Response
{
    /// <summary>
    ///  Response
    /// </summary>
    public class JingdongWarePromotionInfoGetResponse : JdResponse
    {
        /// <summary>
        /// 促销信息返回结构
        /// </summary>
        [XmlElement("promoInfoResponse")]
        [JsonProperty("promoInfoResponse")]
        public AdwordResponse PromoInfoResponse
        {
            get;
            set;
        }

    }
}
