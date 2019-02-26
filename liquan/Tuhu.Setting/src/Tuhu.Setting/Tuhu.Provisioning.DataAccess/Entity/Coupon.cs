namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class Coupon
    {
        public int PKID { get; set; }

        public int CategoryID { get; set; }

        public int ParValue { get; set; }

        public int MinValue { get; set; }

        public string Remark { get; set; }

        public int IsActive { get; set; }

        public string ContentImage { get; set; }

        public string ContentSmallImage { get; set; }

        public string BackgroundImage { get; set; }

        public string BackgroundSmallImage { get; set; }

        public string SpinImage { get; set; }

        public string SpinSmallImage { get; set; }
    }
}
