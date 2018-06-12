using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     活动分享明细表
    /// </summary>
    public class ActivityShareDetailModel
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
        ///     分享时间
        /// </summary>
        public DateTime ShareTime { get; set; }

        /// <summary>
        ///     分享名称
        /// </summary>
        public string ShareName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
