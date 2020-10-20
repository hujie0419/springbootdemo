#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-01-31 10:56:47:642 +08:00
*/
#endregion

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using JdSdk.Response;
using Newtonsoft.Json;

namespace JdSdk.Request
{
    /// <summary>
    /// 输入单个订单id，得到面单打印内容 Request
    /// </summary>
    public class OrderPrintRequest : JdRequestBase<OrderPrintResponse>
    {
        /// <summary>
        /// 订单id
        /// </summary>
        /// <example>123765123</example>
        [XmlElement("order_id")]
        [JsonProperty("order_id")]
        public String OrderId
        {
            get;
            set;
        }

        /// <summary>
        /// 商家希望返回的订单的信息字段,每个字段以逗号分隔
        /// </summary>
        [XmlElement("optional_fields")]
        [JsonProperty("optional_fields")]
        public String OptionalFields
        {
            get;
            set;
        }

        public override String ApiName
        {
            get { return "360buy.order.print"; }
        }

        protected override void PrepareParam(IDictionary<String, Object> paramters)
        {

            paramters.Add("order_id", this.OrderId);
            paramters.Add("optional_fields", this.OptionalFields);

        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("order_id", this.OrderId);
        }

    }
}
