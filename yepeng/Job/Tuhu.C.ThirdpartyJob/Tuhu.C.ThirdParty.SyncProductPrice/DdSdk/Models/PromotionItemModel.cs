using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DdSdk.Api.Models
{
	public class PromotionItemModel
	{
		/// <summary>促销编号</summary>
		public long PromotionID { get; set; }

		/// <summary>促销名称</summary>
		[JsonProperty("promotionName")]
		public string Name { get; set; }

		/// <summary>促销类型</summary>
		[JsonProperty("promotionType")]
		public int Type { get; set; }

		/// <summary>单个促销的优惠金额</summary>
		[JsonProperty("promoDicount")]
		public decimal Dicount { get; set; }

		/// <summary>促销在订单中的订购数量</summary>
		[JsonProperty("promoAmount")]
		public int Amount { get; set; }
	}
}
