using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     请求类 搜索用户竞猜历史
    /// </summary>
    public class SearchQuestionAnswerHistoryRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     活动ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///    true = 只显示用户自己的数据，否则显示题目数据
        /// </summary>
        [Obsolete("没用了这个字段")]
        public bool? NoShowLottery { get; set; }

        /// <summary>
        ///     0 默认
        ///     1 只显示 用户答题并且已经公布的答案 的题目
        /// </summary>
        public int ShowFlag { get; set; }

        /// <summary>
        ///     页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        ///     分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
