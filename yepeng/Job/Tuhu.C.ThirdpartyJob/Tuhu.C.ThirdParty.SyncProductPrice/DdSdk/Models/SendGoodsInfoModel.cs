using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DdSdk.Api.Models
{
	public	class SendGoodsInfoModel
	{
		/// <summary>顾客当当网帐号的标志符</summary>
		[JsonProperty("dangdangAccountID")]
		public string DangdangAccountID { get; set; }

		/// <summary>收货人姓名</summary>
		[JsonProperty("consigneeName")]
		public string Name { get; set; }

		/// <summary>收货地址，含国家、省、市、区、详细地址</summary>
		[JsonProperty("consigneeAddr")]
		public string Address { get; set; }

		/// <summary>收货国家</summary>
		[JsonProperty("consigneeAddr_State")]
		public string State { get; set; }

		/// <summary>收货省份</summary>
		[JsonProperty("consigneeAddr_Province")]
		public string Province { get; set; }

		/// <summary>收货市</summary>
		[JsonProperty("consigneeAddr_City")]
		public string City { get; set; }

		/// <summary>收货区</summary>
		[JsonProperty("consigneeAddr_Area")]
		public string Area { get; set; }

		/// <summary>收货地址邮编</summary>
		[JsonProperty("consigneePostcode")]
		public string Postcode { get; set; }

		/// <summary>收货人固定电话</summary>
		[JsonProperty("consigneeTel")]
		public string Tel { get; set; }

		/// <summary>收货人 移动电话</summary>
		[JsonProperty("consigneeMobileTel")]
		public string MobileTel { get; set; }

		/// <summary>
		/// 送货方式：
		/// ${快递方式}送货上门${送货时间段}
		/// ${快递方式}：普通快递 加急快递 邮政平邮 邮政EMS
		/// ${送货时间段}：周一至周五 周六日及公共假期 时间不限
		/// 例如：加急快递送货上门，时间不限
		/// </summary>
		[JsonProperty("sendGoodsMode")]
		public string SendGoodsMode { get; set; }

		/// <summary>物流公司</summary>
		[JsonProperty("sendCompany")]
		public string SendCompany { get; set; }

		/// <summary>物流公司送货单编号</summary>
		[JsonProperty("sendOrderID")]
		public string SendOrderID { get; set; }
	}
}
