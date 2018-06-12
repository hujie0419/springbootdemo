using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FAQ
	{
		public int PKID { get; set; }
		public string Orderchannel { get; set; }
		public string CateOne { get; set; }
		public string CateTwo { get; set; }
		public string CateThree { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
        //截止日期
        public DateTime? EndTime { get; set; }
	}
}