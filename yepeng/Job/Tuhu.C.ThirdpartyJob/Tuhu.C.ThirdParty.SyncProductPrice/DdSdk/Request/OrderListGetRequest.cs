using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DdSdk.Api.Response;

namespace DdSdk.Api.Request
{
	public class OrderListGetRequest : IDdRequest<OrderListGetResponse>
	{
		public string ApiName { get { return "dangdang.orders.list.get"; } }

		public HttpMethod Method { get { return HttpMethod.Get; } }

		public int OrderStatus { get; set; }

		public DateTime? StartModified { get; set; }
		public DateTime? EndModified { get; set; }
		public int PageSize { get; set; }
		public int PageNo { get; set; }

		public OrderListGetRequest()
		{
			OrderStatus = 101;
			PageSize = 20;
			PageNo = 1;
		}

		public IDictionary<string, string> GetParam()
		{
			var dic = new Dictionary<string, string>();

			dic["os"] = OrderStatus.ToString();
			if (StartModified.HasValue)
				dic["lastModifyTime_start"] = StartModified.Value.ToString("yyyy-MM-dd HH:mm:ss");
			if (EndModified.HasValue)
				dic["lastModifyTime_end"] = EndModified.Value.ToString("yyyy-MM-dd HH:mm:ss");
			dic["pageSize"] = PageSize.ToString();
			dic["p"] = PageNo.ToString();

			return dic;
		}

		public void Validate() { }

		public XElement GetXml()
		{
			throw new NotImplementedException();
		}
	}
}
