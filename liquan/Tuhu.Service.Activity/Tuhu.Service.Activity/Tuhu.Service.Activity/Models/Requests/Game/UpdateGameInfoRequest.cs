using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     更新游戏信息
    /// </summary>
    public class UpdateGameInfoRequest: GameObjectRequest
    {
        /// <summary>
        ///     活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

    }
}
