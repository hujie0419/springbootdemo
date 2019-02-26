using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     游戏信息返回值
    /// </summary>
    public class GetGameInfoResponse
    {
        /// <summary>
        ///     活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        ///     活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     活动说明
        /// </summary>
        public string GameRuleText { get; set; }

        /// <summary>
        ///     助力须知
        /// </summary>
        public string SupportRuleText { get; set; }
    }
}
