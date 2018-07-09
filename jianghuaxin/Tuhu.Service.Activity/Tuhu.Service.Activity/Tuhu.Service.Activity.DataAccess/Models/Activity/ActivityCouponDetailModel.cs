using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     客户兑换券 明细表
    /// </summary>
    public class ActivityCouponDetailModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     外键：关联表 tbl_Activity
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     冗余-tbl_Activity 的 ActivityName
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     兑换券使用数量   正为增加  负为消费
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        ///     明细名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

    }
}
