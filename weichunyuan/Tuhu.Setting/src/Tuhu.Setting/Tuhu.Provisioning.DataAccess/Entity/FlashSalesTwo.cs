using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSalesTwo
    {
        public string ActivityID { get; set; }
        public string ActivityName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string Area { get; set; }
        public string BannerUrlAndroid { get; set; }
        public string BannerUrlIOS { get; set; }
        public string AppVlueAndroid { get; set; }
        public string AppVlueIOS { get; set; }
        public string BackgoundColor { get; set; }
        public int Position { get; set; }
        public string TomorrowText { get; set; }
        public int IsBannerIOS { get; set; }
        public int IsBannerAndroid { get; set; }
        public int ShowType { get; set; }
        public int ShippType { get; set; }
        public int IsTomorrowTextActive { get; set; }
        public int CountDown { get; set; }
        public int Status { get; set; }
        public int IsNoActiveTime { get; set; }

        public bool IsEndImage { get; set; }

        public string EndImage { get; set; }

        public bool ShoppingCart { get; set; }

        public string H5Url { get; set; }

    }
}
