using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class UserQuestionnaireURLModel
    {
        public long PKID { get; set; }
        public Guid PageID { get; set; }
        public int QuestionnaireNo { get; set; }
        public int OrderID { get; set; }
        public int ComplaintsID { get; set; }
        public string ComplaintsType { get; set; }
        public int IsAtStore { get; set; }
        public string Department { get; set; }
        public Guid UserID { get; set; }
        public string UserPhone { get; set; }
        public string StaffEmail { get; set; }
        public string ShortURL { get; set; }
        public string OriginalURL { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
