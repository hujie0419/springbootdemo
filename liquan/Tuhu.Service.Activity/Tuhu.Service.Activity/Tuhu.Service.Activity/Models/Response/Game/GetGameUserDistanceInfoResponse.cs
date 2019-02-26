using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     用户里程收支明细 - 返回类
    /// </summary>
    public class GetGameUserDistanceInfoResponse
    {
        /// <summary>
        /// </summary>
        public IList<GetGameUserDistanceInfoResponseItem> Items { get; set; } =
            new List<GetGameUserDistanceInfoResponseItem>();

        /// <summary>
        /// </summary>
        public class GetGameUserDistanceInfoResponseItem
        {
            /// <summary>
            ///     类型【马牌（日常分享、购买商品、好友助力、XXX奖品）】
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            ///     距离【马牌】
            /// </summary>
            public int Distance { get; set; }

            /// <summary>
            ///     1 支出 2 获得
            /// </summary>
            public int Type { get; set; }
        }
    }
}
