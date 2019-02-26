using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     用户领取奖励 - 请求类
    /// </summary>
    public class GameUserLootRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     奖品ID
        /// </summary>
        public int PrizeId { get; set; }
    }
}
