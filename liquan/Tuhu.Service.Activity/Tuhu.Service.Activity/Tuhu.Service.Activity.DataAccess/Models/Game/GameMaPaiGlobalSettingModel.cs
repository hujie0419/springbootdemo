using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 马牌小游戏全局设置
    /// </summary>
    public class GameMaPaiGlobalSettingModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        ///     外键：关联 - Activity.tbl_Activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        ///     分享获得的里程
        /// </summary>
        public int ShareDistance { get; set; }

        /// <summary>
        ///     好友助力获得里程
        /// </summary>
        public int SupportDistance { get; set; }

        /// <summary>
        ///     一个用户最多每天被助力次数
        /// </summary>
        public int DailySupportedUserMax { get; set; }

        /// <summary>
        ///     每天帮助另一个用户次数上限
        /// </summary>
        public int DailySupportOtherUserMax { get; set; }

        /// <summary>
        ///     活动期间助力上限
        /// </summary>
        public int UserSupportMax { get; set; }

        /// <summary>
        /// 购买商品每N元获得1积分
        /// </summary>
        public decimal PayAmountPointRate { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 排行榜最低积分
        /// </summary>
        public int RankMinPoint { get; set; }

    }
}
