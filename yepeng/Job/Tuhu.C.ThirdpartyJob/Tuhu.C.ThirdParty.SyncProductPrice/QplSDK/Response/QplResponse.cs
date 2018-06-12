using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Qpl.Api.Response
{
	[Serializable]
	public abstract class QplResponse
	{
		/// <summary>
		/// 响应原始内容
		/// </summary>
		public virtual string Body { get; internal set; }

		/// <summary>
		/// 响应结果是否错误
		/// </summary>
		public abstract bool IsError { get; }
	}
}
