using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     获取 奖励滚动 - 请求类
    /// </summary>
    public class GetGameUserLootBroadcastRequest : GameObjectRequest
    {
        /// <summary>
        ///     页面大小  最大50
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        ///     页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 从某时间开始
        /// </summary>
        public DateTime?  StartTime { get; set; }

    }
}
