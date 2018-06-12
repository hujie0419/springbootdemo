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
	public interface IDdRequest<T> where T : DdResponse
	{
		/// <summary>
		/// 获取Jd的API名称。
		/// </summary>
		/// <returns>API名称</returns>
		string ApiName { get; }

		HttpMethod Method { get; }

		IDictionary<string, string> GetParam();

		XElement GetXml();

		/// <summary>
		/// 提前验证参数。
		/// </summary>
		void Validate();
	}
}
