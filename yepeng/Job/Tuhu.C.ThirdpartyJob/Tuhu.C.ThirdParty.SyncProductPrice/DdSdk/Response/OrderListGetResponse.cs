using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DdSdk.Api.Models;

namespace DdSdk.Api.Response
{
	public class OrderListGetResponse : DdResponse
	{
		/// <summary>与查询条件无关，返回等待发货订单数</summary>
		public int SendGoodsOrderCount { get; set; }

		/// <summary>与查询条件无关，返回等待退换货处理的订单数</summary>
		public int NeedExchangeOrderCount { get; set; }

		/// <summary>满足查询条件的订单总数</summary>
		public int OrderCount { get; set; }

		/// <summary>返回全部订单的合计金额</summary>
		public decimal TotalOrderMoney { get; set; }

		/// <summary>只会返回如下数值：5、10、15、20</summary>
		public int PageSize { get; set; }

		/// <summary>总页数</summary>
		public int PageTotal { get; set; }

		/// <summary>当前页</summary>
		public int CurrentPage { get; set; }

		public long[] OrderIDs { get; set; }

		protected override void SetValue(XmlNode xml)
		{
			var totalInfo = xml.SelectSingleNode("totalInfo");
			if (totalInfo != null)
			{
				SendGoodsOrderCount = Convert.ToInt32(totalInfo.SelectSingleNode("sendGoodsOrderCount").InnerText);
				NeedExchangeOrderCount = Convert.ToInt32(totalInfo.SelectSingleNode("needExchangeOrderCount").InnerText);
				OrderCount = Convert.ToInt32(totalInfo.SelectSingleNode("orderCount").InnerText);
				TotalOrderMoney = Convert.ToDecimal(totalInfo.SelectSingleNode("totalOrderMoney").InnerText);
				PageSize = Convert.ToInt32(totalInfo.SelectSingleNode("pageSize").InnerText);
				PageTotal = Convert.ToInt32(totalInfo.SelectSingleNode("pageTotal").InnerText);
				CurrentPage = Convert.ToInt32(totalInfo.SelectSingleNode("currentPage").InnerText);
			}

			OrderIDs = xml.SelectNodes("OrdersList/OrderInfo/orderID").Cast<XmlNode>().Select(node => Convert.ToInt64(node.InnerText)).ToArray();
		}
	}
}
