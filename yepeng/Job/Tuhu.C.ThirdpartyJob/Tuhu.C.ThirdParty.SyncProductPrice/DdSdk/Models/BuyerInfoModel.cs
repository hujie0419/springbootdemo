using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DdSdk.Api.Models
{
	public class BuyerInfoModel
	{
		/// <summary>买家付款方式：货到付款 网上支付 银 行汇款 邮局汇款</summary>
		[JsonProperty("buyerPayMode")]
		public string BuyerPayMode { get; set; }

		/// <summary>买家支付货款，本订单商家应收金额</summary>
		[JsonProperty("goodsMoney")]
		public decimal GoodsMoney { get; set; }

		/// <summary>买家已支付金额，网银支付金额+礼品卡支付金额+当当账户 余额支付金额</summary>
		[JsonProperty("realPaidAmount")]
		public decimal RealPaidAmount { get; set; }

		/// <summary>网银支付满额减优惠金额</summary>
		[JsonProperty("deductAmount")]
		public decimal DeductAmount { get; set; }

		/// <summary>
		/// 促销优惠金额
		/// 订单级促销优惠金额。包括的促销类型如 下：满额减、满额打 折
		/// </summary>
		[JsonProperty("promoDeductAmount")]
		public decimal PromoDeductAmount { get; set; }

		/// <summary>顾客需为订单支付现金</summary>
		[JsonProperty("totalBarginPrice")]
		public decimal TotalBarginPrice { get; set; }

		/// <summary>买家支付邮费</summary>
		[JsonProperty("postage")]
		public decimal Postage { get; set; }

		/// <summary>买家支付礼券金额</summary>
		[JsonProperty("giftCertMoney")]
		public decimal GiftCertMoney { get; set; }

		/// <summary>买家支付礼品卡金额</summary>
		[JsonProperty("giftCardMoney")]
		public decimal GiftCardMoney { get; set; }

		/// <summary>买家支付账户余额</summary>
		[JsonProperty("accountBalance")]
		public decimal AccountBalance { get; set; }

		/// <summary>移动端优惠金额</summary>
		[JsonProperty("activityDeductAmount")]
		public decimal ActivityDeductAmount { get; set; }

		/// <summary>买家使用积分数</summary>
		[JsonProperty("custPointUsed")]
		public decimal CustPointUsed { get; set; }

		/// <summary>买家使用积分抵扣金额</summary>
		[JsonProperty("pointDeductionAmount")]
		public decimal PointDeductionAmount { get; set; }
	}
}
