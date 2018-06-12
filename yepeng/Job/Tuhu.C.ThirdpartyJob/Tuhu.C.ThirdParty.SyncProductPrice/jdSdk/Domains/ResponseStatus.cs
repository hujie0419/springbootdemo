﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace JdSdk.Domains
{
	public class ResponseStatus
	{
		/// <summary>
		/// 处理结果code 1:为成功,其他均为错误
		/// </summary>
		[XmlElement("process_code")]
		[JsonProperty("process_code")]
		public Int64 ProcessCode
		{
			get;
			set;
		}

		/// <summary>
		/// 处理状态含义
		/// </summary>
		[XmlElement("process_status")]
		[JsonProperty("process_status")]
		public String ProcessStatus
		{
			get;
			set;
		}

		/// <summary>
		/// 处理错误信息
		/// </summary>
		[XmlElement("error_message")]
		[JsonProperty("error_message")]
		public String ErrorMessage
		{
			get;
			set;
		}

		/// <summary>
		/// 订单编号
		/// </summary>
		[XmlElement("order_id")]
		[JsonProperty("order_id")]
		public String OrderId
		{
			get;
			set;
		}
	}
}
