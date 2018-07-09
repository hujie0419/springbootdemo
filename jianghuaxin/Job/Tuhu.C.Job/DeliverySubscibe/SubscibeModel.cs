using System.Configuration;
using System.Data;
using Newtonsoft.Json;
using System;
using Tuhu.Component.Order.BusinessData;

namespace Tuhu.Yewu.WinService.JobSchedulerService.DeliverySubscibe
{
	public class SubscibeModel
	{
		public SubscibeModel(SubscibeDeliveryModel model)
		{
			this.OrderId = model.OrderID;

			this.SubscibeBody = new SubscibeBodyModel();
			this.SubscibeBody.DeliveryCompany = model.DeliveryCompany;
			this.SubscibeBody.DeliveryCode = model.DeliveryCode;
			this.SubscibeBody.To = model.DeliveryAddress;
		}

		public int OrderId { get; set; }
		public SubscibeBodyModel SubscibeBody { get; set; }
		public bool SubscibeResult { get; set; }
	}

	public class SubscibeBodyModel
	{
		public SubscibeBodyModel()
		{
			From = "上海市闵行区";
			Key = ConfigurationManager.AppSettings["DeliverySubscibeJob:SubscibeKey"];
			Parameters = new SubscibeParameterModel();
		}

		[JsonProperty("company")]
		public string DeliveryCompany { get; set; }

		[JsonProperty("number")]
		public string DeliveryCode { get; set; }

		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("to")]
		public string To { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("parameters")]
		public SubscibeParameterModel Parameters { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}

		public static SubscibeBodyModel Parse(DataRow row)
		{
			if (row == null)
				return null;

			var model = new SubscibeBodyModel();

			model.DeliveryCompany = row["DeliveryCompany"].ToString();
			model.DeliveryCode = row["DeliveryCode"].ToString();
			model.To = row["DeliveryAddress"].ToString();
			model.Parameters = new SubscibeParameterModel();

			return model;
		}
	}

	public class SubscibeParameterModel
	{
		public SubscibeParameterModel()
		{
			CallbackUrl = ConfigurationManager.AppSettings["DeliverySubscibeJob:CallbackUrl"];
			Resultv2 = "1";
		}

		[JsonProperty("callbackurl")]
		public string CallbackUrl { get; set; }

		[JsonProperty("salt")]
		public string Salt { get; set; }

		[JsonProperty("resultv2")]
		public string Resultv2 { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	public class ResponseResultModel
	{
		[JsonProperty("result")]
		public bool Result { get; set; }

		[JsonProperty("returnCode")]
		public int ReturnCode { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
	
	public class SubscibeLogModel
	{
		public SubscibeLogModel(SubscibeModel model, string responseContent)
		{
			this.OrderId = model.OrderId;
			this.DeliveryCompany = model.SubscibeBody.DeliveryCompany;
			this.DeliveryCode = model.SubscibeBody.DeliveryCode;
			this.SubscibeResult = model.SubscibeResult;
			this.ResponseContent = responseContent;
		}

		public int OrderId { get; set; }
		public string DeliveryCompany { get; set; }
		public string DeliveryCode { get; set; }
		public bool SubscibeResult { get; set; }
		public string ResponseContent { get; set; }
	}
}
