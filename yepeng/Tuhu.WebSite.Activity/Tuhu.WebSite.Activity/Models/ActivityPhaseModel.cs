using System;
using System.Data;
using Tuhu.WebSite.Component.SystemFramework.Models;

namespace Tuhu.WebSite.Web.Activity.Models
{
	public class ActivityPhaseModel : BaseModel
	{
		public string ActivityID { get; set; }
		public DateTime DateTime { get; set; }
		public int Quantity { get; set; }
		public int RemainingQuantity { get; set; }

		public ActivityPhaseModel() : base() { }
		public ActivityPhaseModel(DataRow row) : base(row) { }
	}
}