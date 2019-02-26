using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///    七龙珠 -  获奖轮播  返回类
    /// </summary>
    public class DragonBallBroadcastResponse
    {
        /// <summary>
        ///     数据
        /// </summary>
        public List<DragonBallBroadcastResponseItem> Items { get; set; } = new List<DragonBallBroadcastResponseItem>();
    }

    public class DragonBallBroadcastResponseItem
    {
        /// <summary>
        ///     用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     获取到的奖励
        /// </summary>
        public string LootName { get; set; }
    }
}
