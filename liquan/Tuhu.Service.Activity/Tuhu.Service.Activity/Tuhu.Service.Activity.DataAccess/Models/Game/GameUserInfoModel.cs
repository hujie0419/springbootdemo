using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 游戏玩家对象
    /// </summary>
    public class GameUserInfoModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动id
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     用户积分
        /// </summary>
        public long Point { get; set; }

        /// <summary>
        /// 用户是否访问过游戏,0未访问 1已访问
        /// </summary>
        public int IsVisit { get; set; }

        /// <summary>
        /// 用户首次访问游戏时间
        /// </summary>
        public DateTime? VisitDateTime { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
