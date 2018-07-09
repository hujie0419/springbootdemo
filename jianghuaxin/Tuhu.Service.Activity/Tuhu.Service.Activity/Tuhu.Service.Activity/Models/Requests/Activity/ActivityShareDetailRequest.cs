using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     活动分享明细
    /// </summary>
    public class ActivityShareDetailRequest
    {

        /// <summary>
        ///     外键：关联表 tbl_Activity
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     分享名称
        /// </summary>
        public string ShareName { get; set; }

        /// <summary>
        ///     积分规则ID
        /// </summary>
        public Guid IntegralRuleID { get; set; }


    }
}
