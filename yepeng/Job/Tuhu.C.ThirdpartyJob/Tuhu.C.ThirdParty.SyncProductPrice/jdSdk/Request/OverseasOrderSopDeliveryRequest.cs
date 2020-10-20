#region head comment
/*
Code generate by JdSdkTool.
Copyright © starpeng@vip.qq.com
2013-07-29 23:18:26.81829 +08:00
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
    /// 输入单个订单id，进行发货操作 Request
    /// </summary>
    public class OverseasOrderSopDeliveryRequest : JdRequestBase<OverseasOrderSopDeliveryResponse>
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
        /// 物流公司ID(只可通过    获取商家物流公司接口获得)
        /// </summary>
        /// <example>65234</example>
        [XmlElement("logistics_id")]
        [JsonProperty("logistics_id")]
        public String LogisticsId
        {
            get;
            set;
        }

        /// <summary>
        /// 运单号(当商家直送时    运单号可为空)
        /// </summary>
        /// <example>AFE234223</example>
        [XmlElement("waybill")]
        [JsonProperty("waybill")]
        public String Waybill
        {
            get;
            set;
        }

        /// <summary>
        /// 流水号
        /// </summary>
        [XmlElement("trade_no")]
        [JsonProperty("trade_no")]
        public String TradeNo
        {
            get;
            set;
        }

        public override String ApiName
        {
            get { return "360buy.overseas.order.sop.delivery"; }
        }

        protected override void PrepareParam(IDictionary<String, Object> paramters)
        {

            paramters.Add("order_id", this.OrderId);
            paramters.Add("logistics_id", this.LogisticsId);
            paramters.Add("waybill", this.Waybill);
            paramters.Add("trade_no", this.TradeNo);

        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("order_id", this.OrderId);
            RequestValidator.ValidateRequired("logistics_id", this.LogisticsId);
        }

    }
}
