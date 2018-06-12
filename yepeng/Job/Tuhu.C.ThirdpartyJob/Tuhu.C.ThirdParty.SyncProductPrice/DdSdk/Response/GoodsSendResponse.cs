using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DdSdk.Api.Models;

namespace DdSdk.Api.Response
{
	public class GoodsSendResponse : DdResponse
	{
		public GoodsSendResult[] Result { get; set; }

		protected override void SetValue(XmlNode xml)
		{
			Result = xml.SelectNodes("Result/OrdersList/OrderInfo")
					.Cast<XmlNode>()
					.Select(node => new GoodsSendResult()
					{
						OrderID = Convert.ToInt64(node.SelectSingleNode("orderID").InnerText),
						Code = Convert.ToInt32(node.SelectSingleNode("orderOperCode").InnerText),
						Message = node.SelectSingleNode("orderOperation").InnerText
					}).ToArray();
		}
	}
}
