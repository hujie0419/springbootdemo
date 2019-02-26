using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     提交活动用户回答的问题 请求类
    /// </summary>
    public class SubmitActivityQuestionUserAnswerRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     问卷ID
        /// </summary>
        public long QuestionnaireID { get; set; }


        /// <summary>
        ///     问卷名称
        /// </summary>
        public string QuestionnaireName { get; set; }

        /// <summary>
        ///     问题ID
        /// </summary>
        public long QuestionID { get; set; }

        /// <summary>
        ///     问题内容
        /// </summary>
        public string QuestionName { get; set; }

        /// <summary>
        ///     问题类型
        /// </summary>
        public int QuestionType { get; set; }

        /// <summary>
        ///     回答文本
        /// </summary>
        public string AnswerText { get; set; }


        /// <summary>
        ///     用户回答的选线
        /// </summary>
        public long AnswerOptionID { get; set; }

        /// <summary>
        ///     回答的内容
        /// </summary>
        public string AnswerOptionContent { get; set; }

        /// <summary>
        ///     回答日期
        /// </summary>
        public DateTime AnswerDate { get; set; }

        /// <summary>
        ///     问题分
        /// </summary>
        public int QuestionScore { get; set; }

        /// <summary>
        /// </summary>
        public int ObjectID { get; set; }

        /// <summary>
        ///     使用的积分
        /// </summary>
        public int UseIntegral { get; set; }

        /// <summary>
        ///     可以赢得的兑换券
        /// </summary>
        public int WinCouponCount { get; set; }
    }
}
