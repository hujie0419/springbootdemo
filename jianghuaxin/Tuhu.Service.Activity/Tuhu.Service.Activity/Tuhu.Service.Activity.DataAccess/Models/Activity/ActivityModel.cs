using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     活动对象
    /// </summary>
    public class ActivityModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动类型名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     问卷ID
        /// </summary>
        public long QuestionnaireID { get; set; }


        /// <summary>
        ///     分享此活动赠送的积分
        /// </summary>
        public int ShareIntegral { get; set; }

        /// <summary>
        ///     活动类型  
        ///     0 2018世界杯  
        /// </summary>
        public int ActivityType { get; set; }


        /// <summary>
        ///     活动二次截止时间（比如兑换时间等）
        /// </summary>
        public DateTime SecEndTime { get; set; }
    }
}
