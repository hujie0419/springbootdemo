#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-01-31 10:56:47:846 +08:00
*/
#endregion

using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace JdSdk.Domain
{
    /// <summary>
    /// OrderItem结构
    /// </summary>
    [Serializable]
    public class OrderItem : JdObject
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [XmlElement("ware")]
        [JsonProperty("ware")]
        public Int64 WareId
        {
            get;
            set;
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("ware_name")]
        [JsonProperty("ware_name")]
        public String WareName
        {
            get;
            set;
        }

        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("num")]
        [JsonProperty("num")]
        public String Num
        {
            get;
            set;
        }

        /// <summary>
        /// 价格
        /// </summary>
        [XmlElement("jd_price")]
        [JsonProperty("jd_price")]
        public String JdPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 小计
        /// </summary>
        [XmlElement("price")]
        [JsonProperty("price")]
        public String Price
        {
            get;
            set;
        }

    }
}
