using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 用户分享
    /// </summary>
    public class GameUserShareModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

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
