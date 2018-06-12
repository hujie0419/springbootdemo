using System;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    /// <summary>
    ///     用户回答结果表
    /// </summary>
    public class QuestionnaireAnswerResultModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     外键：关联表 QuestionnaireAnswerRecord
        /// </summary>
        public long QuestionnaireAnswerID { get; set; }

        /// <summary>
        ///     回答结果  0 初始状态  1 胜利  2 失败
        /// </summary>
        public int AnswerResultStatus { get; set; }

        /// <summary>
        ///     使用的积分
        /// </summary>
        public int UseIntegral { get; set; }

        /// <summary>
        ///     赢得的兑换券
        /// </summary>
        public int WinCouponCount { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
