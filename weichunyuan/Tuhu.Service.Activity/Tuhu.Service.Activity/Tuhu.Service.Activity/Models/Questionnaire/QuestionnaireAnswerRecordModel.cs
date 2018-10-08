using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class QuestionnaireAnswerRecordModel
    {
        public long PKID { get; set; }
        public Guid UserID { get; set; }
        public long QuestionnaireID { get; set; }
        public string QuestionnaireName { get; set; }
        public long QuestionID { get; set; }
        public string QuestionName { get; set; }
        public int QuestionType { get; set; }
        public string AnswerText { get; set; }
        public long AnswerOptionID { get; set; }
        public string AnswerOptionContent { get; set; }
        public DateTime AnswerDate { get; set; }
        public int QuestionScore { get; set; }
        public int ObjectID { get; set; }
    }
}
