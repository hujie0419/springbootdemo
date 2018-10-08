using System;

namespace Tuhu.Service.Activity.DataAccess.Models.DragonBall
{
    /// <summary>
    ///     七龙珠 用户任务表
    /// </summary>
    public class DragonBallUserMissionModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     任务ID
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        ///     任务状态 1 可领取  2 已领取
        /// </summary> 
        public int MissionStatus { get; set; }

        /// <summary>
        ///     任务获得的龙珠数量 - 没有就是还没有获得
        /// </summary>
        public int DragonBallCount { get; set; }

        /// <summary>
        ///     创建日期
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改日期
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
