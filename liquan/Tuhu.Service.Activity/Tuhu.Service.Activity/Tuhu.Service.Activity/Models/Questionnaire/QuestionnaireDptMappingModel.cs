using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class QuestionnaireDptMappingModel
    {
        public long PKID { get; set; }
        public int QuestionnaireNo { get; set; }
        public string Department { get; set; }
        public string ComplaintsType { get; set; }
        public int IsAtStore { get; set; }
    }
}
