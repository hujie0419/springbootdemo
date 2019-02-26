using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizActionsetTab
	{
		public long id { get; set; }
		public string action_bgimg { get; set; }
		public string action_adproimg { get; set; }
		public string action_explain { get; set; }
		public string action_rule { get; set; }
		public string action_label { get; set; }
		public string action_proname { get; set; }
		public string action_proID { get; set; }
		public string action_proimg { get; set; }
		public int action_needfriendcount { get; set; }
		public int action_totaljoinperson { get; set; }
		public int action_totaldownprice { get; set; }
		public DateTime action_endTime { get; set; }
		public decimal action_dealmoney { get; set; }
		public bool action_static { get; set; }
		public DateTime action_submitTime { get; set; }
		public DateTime action_updateTime { get; set; }
		public string action_introurl { get; set; }
		public string action_endurl { get; set; }
	}
}
