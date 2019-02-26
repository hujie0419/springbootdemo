namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CouponCategory
    {
        public int PKID { get; set; }
        public string EnID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Picture { get; set; }
        public int ValidDays { get; set; }
        public int IsActive { get; set; }
        public int Percentage { get; set; }
        public int? CouponType { get; set; }
        public int? RuleID { get; set; }
        public string Remark { get; set; }
        public string Platform { get; set; }
        public string BackImage { get; set; }

        public string ContentImage { get; set; }

        public string BackgroundImage { get; set; }

        public string SpinImage { get; set; }

        public string IOSDisposeValue { get; set; }

        public string AndroidDisposeValue { get; set; }

        public string IOSCommunicationValue { get; set; }

        public string AndroidCommunicationValue { get; set; }

        public string AppLink { get; set; }

        public string GetCouponDescription { get; set; }

        public bool CouponDescriptionStatus { get; set; }

        public string RoamingDescription { get; set; }
    }
}
