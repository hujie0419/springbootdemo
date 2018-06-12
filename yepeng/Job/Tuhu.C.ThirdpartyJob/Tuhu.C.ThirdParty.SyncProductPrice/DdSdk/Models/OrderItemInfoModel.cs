using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DdSdk.Api.Models
{
	public class OrderItemInfoModel
	{
		/// <summary>商品标识符</summary>
		public long ItemID { get; set; }

		/// <summary>企业商品标志符</summary>
		public string OuterItemID { get; set; }

		/// <summary>商品名称</summary>
		public string ItemName { get; set; }

		/// <summary>商品类型：0、商品 1、赠品</summary>
		public int ItemType { get; set; }

		/// <summary>分色分码。使用“自定义属性名称”，例子如下： <specialAttribute>颜色>>军绿;鞋码 >>38</specialAttribute></summary>
		public string SpecialAttribute { get; set; }

		/// <summary>市场价</summary>
		public decimal MarketPrice { get; set; }

		/// <summary>成交价</summary>
		public decimal UnitPrice { get; set; }

		/// <summary>订购数量</summary>
		public int OrderCount { get; set; }

		/// <summary>发货数量</summary>
		public int SendGoodsCount { get; set; }

		/// <summary>所属商品集合促销编号</summary>
		public long BelongProductsPromoID { get; set; }

		/// <summary>商品明细编号</summary>
		public long ProductItemId { get; set; }
	}
}
