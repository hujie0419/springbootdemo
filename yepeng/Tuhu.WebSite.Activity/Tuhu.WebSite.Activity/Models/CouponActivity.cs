using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.WebSite.Web.Activity.Models
{
    public class CouponActivity
    {
        public string SmallImageUrl { get; set; }
        public string BigImageUrl { get; set; }
        public string ButtonBackGroundColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string ButtonText { get; set; }
        public string Url { get; set; }
        public string IosJson { get; set; }
        public string AndroidJson { get; set; }
        public bool IsSendCoupon { get; set; }
        public string LayerButtonText { get; set; }
        public string LayerButtonBackGroundColor { get; set; }
        public string LayerButtonTextColor { get; set; }
        public int PromotionType { get; set; }
        public int CouponRuleId { get; set; }
        public int CouponUseMoney { get; set; }
        public int CouponDisCountMoney { get; set; }
        public int ValidityPeriod { get; set; }
        public string CouponDescription { get; set; }
        public string CodeChannel { get; set; }
    }
}