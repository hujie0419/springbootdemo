#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-04-03 12:45:23.93471 +08:00
*/
#endregion

using System.Collections.Generic;
using System.Xml.Serialization;
using JdSdk.Domain;
using Newtonsoft.Json;

namespace JdSdk.Response
{
    /// <summary>
    /// 购买此商品用户最终购买的商品 Response
    /// </summary>
    public class JingdongUserbrowsebuyrecommendWareGetResponse : JdResponse
    {
        /// <summary>
        /// 商品信息
        /// </summary>
        [XmlElement("favoriteWares")]
        [JsonProperty("favoriteWares")]
        public List<FavoriteWare> FavoriteWares
        {
            get;
            set;
        }

    }
}
