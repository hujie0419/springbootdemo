using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     奖励滚动 - 返回值
    /// </summary>
    public class GetGameUserLootBroadcastResponse
    {
        /// <summary>
        ///     滚动。。。
        /// </summary>
        public IList<GetGameUserLootBroadcastResponseItem> Items = new List<GetGameUserLootBroadcastResponseItem>();

        public class GetGameUserLootBroadcastResponseItem
        {
            /// <summary>
            ///     昵称
            /// </summary>
            public string NickName { get; set; }

            /// <summary>
            ///     奖品名称
            /// </summary>
            public string PrizeName { get; set; }

            /// <summary>
            ///     领取时间
            /// </summary>
            public DateTime Date { get; set; }
        }
    }
}
