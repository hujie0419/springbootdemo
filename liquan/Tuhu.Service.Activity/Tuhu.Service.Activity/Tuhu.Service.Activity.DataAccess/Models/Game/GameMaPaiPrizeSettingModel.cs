using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 马牌奖品设置MODEL
    /// </summary>
    public class GameMaPaiPrizeSettingModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动ID
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        ///     马牌里程碑 设置id
        /// </summary>
        public long MaPaiMilepostSettingId { get; set; }

        /// <summary>
        ///     此商品需要消费的积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        ///     奖品名称
        /// </summary>
        public string PrizeName { get; set; }

        /// <summary>
        ///     奖品图片
        /// </summary>
        public string PrizePicUrl { get; set; }

        /// <summary>
        ///     奖品总数  -1  = unlimit
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     剩余总数  -1  = unlimit
        /// </summary>
        public int LCount { get; set; }

        /// <summary>
        ///     当天总数 -1 = unlimit
        /// </summary>
        public int DayCount { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     /    最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     是否显示滚动轮播
        /// </summary>
        public int IsBroadCastShow { get; set; }
    }
}
