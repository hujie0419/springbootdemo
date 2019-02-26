using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     用户兑换券排行
    /// </summary>
    public class ActivityCouponRankModel
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
        ///     冗余-排行计算时候的兑换券数量
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        ///     途虎昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     称号 例如：（足球小将 大神 ）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     排行日期
        /// </summary>
        public DateTime RankDate { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     外键：关联表Tuhu_profiles.UserObject  Userid
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     用户的排名
        /// </summary>
        public long Rank { get; set; }

        /// <summary>
        ///     途虎头像
        /// </summary>
        public string UserImgUrl { get; set; }
    }
}
