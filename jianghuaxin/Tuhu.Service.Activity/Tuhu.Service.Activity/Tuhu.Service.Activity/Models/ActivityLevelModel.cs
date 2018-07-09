using System;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    ///     活动等级表
    /// </summary>
    public class ActivityLevelModel
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
        ///     等级名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        ///     开始范围
        /// </summary>
        public int? StartCount { get; set; }

        /// <summary>
        ///     最大范围
        ///     -1 无限
        /// </summary>
        public int? EndCount { get; set; }

        /// <summary>
        ///     创建日期
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次更新日期
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
