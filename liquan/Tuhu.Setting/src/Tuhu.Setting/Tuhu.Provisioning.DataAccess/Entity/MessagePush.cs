namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MessagePush
	{
		public int PKID { get; set; }
		public string EnID { get; set; }
		public string MsgTitle { get; set; }
		public string MsgContent { get; set; }
		public string MsgLink { get; set; }
		public string MsgDescription { get; set; }
		public int TotalDuration { get; set; }
		public int AheadHour { get; set; }
	}
}
