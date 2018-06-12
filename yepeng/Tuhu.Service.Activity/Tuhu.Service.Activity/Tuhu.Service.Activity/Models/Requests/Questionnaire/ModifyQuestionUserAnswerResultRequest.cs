namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     更新用户答题结果 请求类
    /// </summary>
    public class ModifyQuestionUserAnswerResultRequest
    {
        /// <summary>
        ///     结果ID
        /// </summary>
        public long ResultId { get; set; }

        /// <summary>
        ///     获得的兑换券
        /// </summary>
        public int WinCouponCount { get; set; }

        /// <summary>
        ///     结果 
        /// </summary>
        public int AnswerResultStatus { get; set; }
    }
}
