using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     请求类 提交用户竞猜
    /// </summary>
    public class SubmitQuestionAnswerRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     选项ID
        /// </summary>
        public long OptionId { get; set; }

        /// <summary>
        ///     活动ID
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     积分规则ID  选择此项可能会消耗积分，那么需要此字段关联积分消耗的规则
        /// </summary>
        public Guid IntegralRuleID { get; set; }
    }
}
