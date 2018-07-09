using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    //Activity_Detail
    public class ActivityCalendar
    {

        public int Pkid { get; set; }
        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public System.DateTime BeginDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public string ActivityTitle { get; set; }
        public string ActivityContent { get; set; }
        public string ActivityUrl { get; set; }
        public string ScheduleType { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string DataFrom { get; set; }
        public int? DataFromId { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public enum ActivityObject
    {
        PromotionConfigure,
        TblZeroActivity,
        CouponCategory,
        FlashSales,
        NewAppSet,
        Advertise
    }

}
