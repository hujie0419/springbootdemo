using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 马牌奖品优惠券设置
    /// </summary>
    public class GameMaPaiCouponSettingModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     马牌奖品id
        /// </summary>
        public long GameMaPaiPrizeSettingPrizeId { get; set; }

        /// <summary>
        ///     优惠券id
        /// </summary>
        public Guid PromotionGetRuleGuid { get; set; }

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
