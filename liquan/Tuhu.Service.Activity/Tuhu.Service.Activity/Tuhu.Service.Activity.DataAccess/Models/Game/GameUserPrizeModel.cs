using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Game
{
    /// <summary>
    ///     小游戏 - 用户已经领取的奖品
    /// </summary>
    public class GameUserPrizeModel
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
        ///     用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     此商品消费的积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        ///     奖品id
        /// </summary>
        public long PrizeId { get; set; }

        /// <summary>
        ///     奖品名称
        /// </summary>
        public string PrizeName { get; set; }

        /// <summary>
        ///     奖品图片
        /// </summary>
        public string PrizePicUrl { get; set; }

        /// <summary>
        ///     奖品标题
        /// </summary>
        public string PrizeTitle { get; set; }

        /// <summary>
        ///     奖品备注
        /// </summary>
        public string PrizeDesc { get; set; }

        /// <summary>
        ///     奖品使用 - 开始时间
        /// </summary>
        public DateTime? PrizeStartTime { get; set; }


        /// <summary>
        ///     奖品使用 - 结束时间
        /// </summary>
        public DateTime? PrizeEndTime { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     此奖品对应的优惠券规则ID 如果有的话
        /// </summary>
        public Guid? PromotionGetRuleGuid { get; set; }

        /// <summary>
        ///     此奖品对应的优惠券Code 如果有的话
        /// </summary>
        public string PromotionCode { get; set; }

        /// <summary>
        ///     此奖品对应的优惠券ID 如果有的话
        /// </summary>
        public long? PromotionId { get; set; }

        /// <summary>
        ///     是否显示滚动轮播
        /// </summary>
        public int IsBroadCastShow { get; set; }

        /// <summary>
        ///获得奖品当天排名
        /// </summary>
        public int TodayRank { get; set; }

        /// <summary>
        /// 获取奖品时间
        /// </summary>
        public DateTime GetPrizeDate { get; set; }

        /// <summary>
        /// 奖品类型
        /// </summary>
        public string PrizeType { get; set; }

    }
}
