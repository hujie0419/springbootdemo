using System;

namespace Tuhu.Service.Activity.DataAccess.Models.DragonBall
{
    /// <summary>
    ///     七龙珠 - 用户信息表
    /// </summary>
    public class DragonBallUserInfoModel
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
        ///     完成任务数量
        /// </summary>
        public int FinishMissionCount { get; set; }

        /// <summary>
        ///     当前龙珠数量
        /// </summary>
        public int DragonBallCount { get; set; }

        /// <summary>
        ///     当前召唤神龙次数
        /// </summary>
        public int DragonBallSummonCount { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
