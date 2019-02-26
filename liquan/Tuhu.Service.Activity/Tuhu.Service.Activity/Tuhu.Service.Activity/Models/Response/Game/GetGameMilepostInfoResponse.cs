using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     里程碑信息 返回类
    /// </summary>
    public class GetGameMilepostInfoResponse
    {
        /// <summary>
        ///     里程碑【马牌】
        /// </summary>
        public IList<GetGameMilepostInfoResponseItem> Items { get; set; } = new List<GetGameMilepostInfoResponseItem>();
    }

    /// <summary>
    ///     里程碑信息 - 子类
    /// </summary>
    public class GetGameMilepostInfoResponseItem
    {
        /// <summary>
        ///     里程碑名称
        /// </summary>
        public string MilepostName { get; set; }

        /// <summary>
        ///     里程碑LOGO
        /// </summary>
        public string MilepostPicUrl { get; set; }

        /// <summary>
        ///     里程碑ID
        /// </summary>
        public int MilepostId { get; set; }

        /// <summary>
        ///     距离
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        ///     奖品名称
        /// </summary>
        public string PrizeName { get; set; }

        /// <summary>
        ///     奖品图片URL
        /// </summary>
        public string PrizePicUrl { get; set; }

        /// <summary>
        ///     奖品ID
        /// </summary>
        public long PrizeId { get; set; }

        /// <summary>
        ///     总数  -1  = unlimit
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
        ///     当天剩余总数 -1 = unlimit
        /// </summary>
        public int DayLCount { get; set; }

        /// <summary>
        ///     当前奖品状态 1 = 可兑换  2 = 已兑换 3 已兑光
        /// </summary>
        public int PrizeStatus { get; set; }
    }
}
