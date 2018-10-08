using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     获取用户答题记录    
    /// </summary>
    public class QuestionUserAnswerHistoryResponse
    {
        /// <summary>
        ///     回答ID
        /// </summary>
        public long RecordId { get; set; }
        /// <summary>
        ///     问题ID
        /// </summary>
        public long QuestionId { get; set; }

        /// <summary>
        ///     问题日期
        /// </summary>
        public DateTime QuestionDateTime { get; set; }

        /// <summary>
        ///     竞猜结果
        /// </summary>
        public string QuestionTextResult { get; set; }

        /// <summary>
        ///     回答结果   0 未公布  1 胜利  2 失败  3 未参与  
        /// </summary>
        public int AnswerResultStatus { get; set; }

        /// <summary>
        ///     使用的积分
        /// </summary>
        public long UseIntegral { get; set; }

        /// <summary>
        ///     赢得的兑换券
        /// </summary>
        public long WinCouponCount { get; set; }


    }
}
