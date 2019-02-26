using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class UserTaskSettingModel
    {

        public string Tag_2 { get; set; }
        public string Tag_1 { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public int FixedTerm { get; set; }
        public int FixedStartTerm { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ValidTimeType { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }
        public bool DisplayRelyOnTrigger { get; set; }
        public float GrowthValue { get; set; }
        public float Point { get; set; }
        public int PKID { get; set; }
        public string Category { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TargetButtonUrl { get; set; }
        public string TargetButtonDescription { get; set; }

        public int Condition_1 { get; set; }
        public string Condition_1_Value { get; set; }
        public int Condition_2 { get; set; }
        public string Condition_2_Value { get; set; }
        public int Condition_3 { get; set; }
        public string Condition_3_Value { get; set; }
        public int Condition_4 { get; set; }
        public string Condition_4_Value { get; set; }
        public int Condition_5 { get; set; }
        public string Condition_5_Value { get; set; }

        public int CouponId_1 { get; set; }
        public int CouponId_1_Num { get; set; }
        public int CouponId_2 { get; set; }
        public int CouponId_2_Num { get; set; }
        public int CouponId_3 { get; set; }
        public int CouponId_3_Num { get; set; }
        public int CouponId_4 { get; set; }
        public int CouponId_4_Num { get; set; }
        public int CouponId_5 { get; set; }
        public int CouponId_5_Num { get; set; }

        public int TriggerTaskId_1 { get; set; }
        public int TriggerTaskId_2 { get; set; }
        public int TriggerTaskId_3 { get; set; }

        public int TargetTaskId_1 { get; set; }
        public int TargetTaskId_2 { get; set; }
        public int TargetTaskId_3 { get; set; }
    }
}