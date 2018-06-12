using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
	public class OutStorageModel
	{
		public OutStorageModel()
		{
			DeliveryType = 1;
		}

		/// <summary>物流类型 1：商品；2：发票</summary>
		public int DeliveryType { get; set; }

		/// <summary>配送方式 0：快递配送；1：商家配送</summary>
		public int DeliveryWay { get; set; }

		public string DeliveryCompany { get; set; }
		public string DeliveryCode { get; set; }
	}
}
