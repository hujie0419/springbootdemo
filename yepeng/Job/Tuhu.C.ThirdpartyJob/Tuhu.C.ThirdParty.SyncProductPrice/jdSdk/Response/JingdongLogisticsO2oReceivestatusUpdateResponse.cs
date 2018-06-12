using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JdSdk.Domains;
using JdSdk.Request;
using Newtonsoft.Json;

namespace JdSdk.Response
{
	public class JingdongLogisticsO2oReceivestatusUpdateResponse : JdResponse
	{
        /// <summary>
        /// 处理结果code 1:为成功,其他均为错误
        /// </summary>
        [XmlElement("OrderResponseStatus")]
        [JsonProperty("OrderResponseStatus")]
		public ResponseStatus OrderResponseStatus { get; set; }
	}
}
