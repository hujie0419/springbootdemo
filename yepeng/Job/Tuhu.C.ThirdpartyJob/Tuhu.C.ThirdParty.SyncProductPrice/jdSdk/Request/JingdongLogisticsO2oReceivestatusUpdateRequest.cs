using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JdSdk.Response;
using Newtonsoft.Json;

namespace JdSdk.Request
{
	/// <summary>
	/// 更新订单收单
	/// </summary>
	public class JingdongLogisticsO2oReceivestatusUpdateRequest : JdRequestBase<JingdongLogisticsO2oReceivestatusUpdateResponse>
	{
		public override string ApiName
		{
			get { return "jingdong.logistics.o2o.receivestatus.update"; }
		}

		/// <summary>
		/// 外部站点编号（在门店系统中设置的）
		/// </summary>
		[XmlElement("station_no")]
		[JsonProperty("station_no")]
		public String StationNo { get; set; }

		/// <summary>
		/// 订单编号
		/// </summary>
		[XmlElement("order_id")]
		[JsonProperty("order_id")]
		public String OrderId { get; set; }

		/// <summary>
		/// 订单编号
		/// </summary>
		[XmlElement("state_operator")]
		[JsonProperty("state_operator")]
		public String StateOperator { get; set; }

		protected override void PrepareParam(IDictionary<string, object> paramters)
		{
			paramters.Add("station_no", this.StationNo);
			paramters.Add("order_id", this.OrderId);
			paramters.Add("state_operator", this.StateOperator);
		}

		public override void Validate()
		{
			RequestValidator.ValidateRequired("station_no", this.StationNo);
			RequestValidator.ValidateRequired("order_id", this.OrderId);
			RequestValidator.ValidateRequired("state_operator", this.StateOperator);
		}
	}
}
