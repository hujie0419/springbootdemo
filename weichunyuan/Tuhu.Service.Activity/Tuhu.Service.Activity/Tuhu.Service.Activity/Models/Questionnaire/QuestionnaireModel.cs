using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class QuestionnaireModel
    {
        public long QuestionnaireID { get; set; }
        public int QuestionnaireNo { get; set; }
        public string QuestionnaireName { get; set; }
        public int QuestionnaireType { get; set; }
        public int IsShowRegulation { get; set; }
        public string Regulation { get; set; }
        /// <summary>
        /// 提交结束信息
        /// </summary>
        public string CompletionInfo { get; set; }
        public int ParticipantsCount { get; set; }
    }
}
