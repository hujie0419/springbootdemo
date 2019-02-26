using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizEmailProcess
	{
		public int PKID { get; set; }
		public string url { get; set; }
		public string FromMail { get; set; }
		public string ToMail { get; set; }
		public string CC { get; set; }
		public string BCC { get; set; }
		public string Subject { get; set; }
		public DateTime InsertTime { get; set; }
		public DateTime? SentTime { get; set; }
		public string Status { get; set; }
		public string Type { get; set; }
		public string Body { get; set; }
		public Guid? RelatedUser { get; set; }
		public bool? IsActive { get; set; }
		public DateTime LastUpdateTime { get; set; }
        public string OrderNo { get; set; }
        public int OrderID { get; set; }
	}
}
