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
	public class OrderDetailGetRequest : IDdRequest<OrderDetailGetResponse>
	{
		public string ApiName { get { return "dangdang.order.details.get"; } }

		public HttpMethod Method { get { return HttpMethod.Get; } }

		public long OrderID { get; set; }
		public string OuterOrderID { get; set; }

		public IDictionary<string, string> GetParam()
		{
			var dic = new Dictionary<string, string>();

			if (OrderID > 0)
				dic["o"] = OrderID.ToString();
			if (!string.IsNullOrWhiteSpace(OuterOrderID))
			dic["outerOrderID"] = OuterOrderID;

			return dic;
		}

		public void Validate() { }

		public XElement GetXml()
		{
			throw new NotImplementedException();
		}
	}
}
