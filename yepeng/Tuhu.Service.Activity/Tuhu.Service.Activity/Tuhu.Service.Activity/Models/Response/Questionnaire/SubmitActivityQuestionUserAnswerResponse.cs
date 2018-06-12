namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     提交活动用户回答的问题 返回类
    /// </summary>
    public class SubmitActivityQuestionUserAnswerResponse
    {

        /// <summary>
        ///     是否成功
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        ///     记录ID
        /// </summary>
        public long RecordId { get; set; }

        /// <summary>
        ///     结果ID
        /// </summary>
        public long ResultId { get; set; }
    }
}
