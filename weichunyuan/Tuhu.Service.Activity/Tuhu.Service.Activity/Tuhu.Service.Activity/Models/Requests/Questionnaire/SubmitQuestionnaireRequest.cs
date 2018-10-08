using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 提交问卷请求类
    /// </summary>
    public class SubmitQuestionnaireRequest
    {
        /// <summary>
        /// 页面标识，问卷链接上唯一标识
        /// </summary>
        public Guid PageID { get; set; }
        /// <summary>
        /// 用户标识
        /// </summary>
        public Guid? UserID { get; set; }
        /// <summary>
        /// 投诉标识/其他对象标识
        /// </summary>
        public int? ObjectID { get; set; }
        /// <summary>
        /// 问卷标识
        /// </summary>
        public long QuestionnaireID { get; set; }
        /// <summary>
        /// 问题列表
        /// </summary>
        public List<Question> QuestionList { get; set; }
    }
    /// <summary>
    /// 问题类
    /// </summary>
    public class Question
    {
        /// <summary>
        /// 问题标识
        /// </summary>
        public long QuestionID { get; set; }
        /// <summary>
        /// 答案列表
        /// </summary>
        public List<AnswerOption> AnswerOptionList { get; set; }
    }
    /// <summary>
    /// 答题类
    /// </summary>
    public class AnswerOption
    {
        /// <summary>
        /// 答案的选项标识
        /// </summary>
        public long AnswerOptionID { get; set; }
        /// <summary>
        /// 答题文本内容
        /// </summary>
        public string AnswerText { get; set; }
    }
}
