namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizShopSimple
	{
		public int PKID { get; set; }
		public string Category { get; set; }
		public string Province { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public int RegionID { get; set; }
		public string SimpleName { get; set; }
		public string Service { get; set; }
		public int ShopType { get; set; }
		public string ShopStatus { get; set; }
		public string WorkTime { get; set; }
		public string Position { get; set; }

        public string AddressBrief { get; set; }
		public string Address { get; set; }
        public string Pos { get; set; }
		public string CarparName { get; set; }

        public int FirstPriority { get; set; }

	}
}
