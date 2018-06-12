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
	public class GoodsSendRequest : IDdRequest<GoodsSendResponse>
	{
		public string ApiName { get { return "dangdang.order.goods.send"; } }

		public HttpMethod Method { get { return HttpMethod.Post; } }

		public XElement RequestXml { get; set; }

		public IDictionary<string, string> GetParam()
		{
			throw new NotImplementedException();
		}

		public XElement GetXml() { return RequestXml; }

		public void Validate() { }
	}
}
