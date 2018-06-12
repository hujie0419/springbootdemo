using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     活动对象
    /// </summary>
    public class ActivityResponse
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     活动类型名称
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
        ///     分享送积分的 积分规则ID
        /// </summary>
        public Guid ShareIntegralRuleID { get; set; }

        /// <summary>
        ///     用户答题选择的积分规则ID
        /// </summary>
        public Guid UserSelectionIntegralRuleID { get; set; }

        /// <summary>
        ///     问卷ID
        /// </summary>
        public long QuestionnaireID { get; set; }
    }
}
