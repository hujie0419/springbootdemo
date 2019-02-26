using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     客户兑换券表
    /// </summary>
    public class ActivityCouponModel
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
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     持有的兑换券数量
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     乐观锁 更新的时候需要增加此字段的判断条件
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        ///     排名
        /// </summary>
        public long Rank { get; set; }

        /// <summary>
        ///     兑换券总额
        /// </summary>
        public int CouponSum { get; set; }
    }
}
