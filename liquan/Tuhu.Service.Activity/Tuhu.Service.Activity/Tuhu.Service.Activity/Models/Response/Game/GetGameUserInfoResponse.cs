using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     游戏用户信息 - 返回类
    /// </summary>
    public class GetGameUserInfoResponse
    {
        /// <summary>
        ///     当前距离【马牌】
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     用户积分
        /// </summary>
        public long Point { get; set; }

        /// <summary>
        /// 用户的游戏排行
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        ///     奖品
        /// </summary>
        public IList<GetGameUserInfoResponsePrizeItems> PrizeItems { get; set; } =
            new List<GetGameUserInfoResponsePrizeItems>();

        /// <summary>
        ///     剩余助力机会【马牌】
        ///     废弃
        /// </summary>
        [Obsolete("不要用这个了", true)]
        public int SupportCount { get; set; }
    }

    /// <summary>
    ///     用户游戏已经兑换的奖品
    /// </summary>
    public class GetGameUserInfoResponsePrizeItems
    {

        /// <summary>
        /// 奖品类型:COUPON-券ENTITY-实物
        /// </summary>
        public string PrizeType { get; set; }

        /// <summary>
        ///  奖品名称
        /// </summary>
        public string PrizeName { get; set; }

        /// <summary>
        ///     奖品图片
        /// </summary>
        public string PrizePic { get; set; }

        /// <summary>
        ///     奖品备注
        /// </summary>
        public string PrizeDesc { get; set; }

        /// <summary>
        ///     奖品TITLE
        /// </summary>
        public string PrizeTitle { get; set; }

        /// <summary>
        ///     奖品使用时间 - 开始
        /// </summary>
        public DateTime? PrizeStartTime { get; set; }

        /// <summary>
        ///     奖品使用时间 - 结束
        /// </summary>
        public DateTime? PrizeEndTime { get; set; }

        /// <summary>
        /// 获取奖品时间
        /// </summary>
        public DateTime GetTime { get; set; }

        /// <summary>
        /// 获奖当天最终排名
        /// </summary>
        public int TodayRank { get; set; }

    }
}
