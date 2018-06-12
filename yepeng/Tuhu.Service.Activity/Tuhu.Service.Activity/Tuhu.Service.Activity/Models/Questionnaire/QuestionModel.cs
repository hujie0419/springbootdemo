using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class QuestionModel
    {
        public long QuestionID { get; set; }
        public long QuestionnaireID { get; set; }
        public string QuestionTitle { get; set; }
        public int QuestionType { get; set; }
        public int IsFork { get; set; }
        public string DefaultValue { get; set; }
        public int IsRequired { get; set; }
        public int IsValidateMinChar { get; set; }
        public int MinChar { get; set; }
        public int IsValidateMaxChar { get; set; }
        public int MaxChar { get; set; }
        public int Sort { get; set; }

        /// <summary>
        ///     开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     问题的文字结果（可选）（只做展示用）
        /// </summary>
        public string QuestionTextResult { get; set; }

        /// <summary>
        ///     问题是否确认  0 默认  1 已添加答案  2 已发布答案
        /// </summary>
        public int QuestionConfirm { get; set; }

        /// <summary>
        ///     判断问题是否截止
        /// </summary>
        public DateTime? DeadLineTime { get; set; }
    }
}
