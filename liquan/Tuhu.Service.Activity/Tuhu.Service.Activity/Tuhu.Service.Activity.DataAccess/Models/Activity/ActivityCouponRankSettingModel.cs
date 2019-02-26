using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     兑换券排名设置
    /// </summary>
    public class ActivityCouponRankSettingModel
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
        /// 前列的排名
        /// </summary>
        public int RankHead { get; set; }

        /// <summary>
        /// 中位的排名
        /// </summary>
        public int RankMiddle { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
