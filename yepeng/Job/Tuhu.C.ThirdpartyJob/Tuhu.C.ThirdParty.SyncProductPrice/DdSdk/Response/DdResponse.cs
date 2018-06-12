using System;
using System.Xml;
using DdSdk.Api.Models;

namespace DdSdk.Api.Response
{
	public abstract class DdResponse
	{
		/// <summary>响应原始内容</summary>
		public virtual string Body
		{
			get
			{
				return body;
			}
			internal protected set
			{
				body = value;

				var xml = new XmlDocument();
				xml.LoadXml(value);

				SetValue(xml);
			}
		}
		private string body;

		/// <summary>响应结果是否错误</summary>
		public virtual bool IsError { get { return Error != null || OperationResult != null && OperationResult.Code > 0; } }

		public virtual string FunctionID { get; private set; }
		public virtual DateTime Time { get; private set; }
		public virtual ResultModel Error { get; private set; }

		public ResultModel OperationResult { get; private set; }

		private void SetValue(XmlDocument xml)
		{
			var errorResponse = xml.SelectSingleNode("/errorResponse");
			if (errorResponse == null)
			{
				var response = xml.SelectSingleNode("/response");

				FunctionID = response.SelectSingleNode("functionID").InnerText;
				Time = Convert.ToDateTime(response.SelectSingleNode("time").InnerText);

				var result = xml.SelectSingleNode("Result");
				if (result == null || result.SelectSingleNode("operCode") == null)
				{
					RemoveAllAttribte(response);
					SetValue(response);
				}
				else
					OperationResult = new ResultModel()
					{
						Code = Convert.ToInt32(result.SelectSingleNode("operCode").InnerText),
						Message = result.SelectSingleNode("operation").InnerText
					};
			}
			else
				Error = new ResultModel()
				{
					Code = Convert.ToInt32(errorResponse.SelectSingleNode("errorCode").InnerText),
					Message = errorResponse.SelectSingleNode("errorMessage").InnerText
				};
		}

		protected abstract void SetValue(XmlNode xml);

		private static void RemoveAllAttribte(XmlNode xml)
		{
			if (xml == null)
				return;

			if (xml.Attributes != null)
				xml.Attributes.RemoveAll();

			foreach (XmlNode node in xml.ChildNodes)
			{
				RemoveAllAttribte(node);
			}
		}
	}
}
