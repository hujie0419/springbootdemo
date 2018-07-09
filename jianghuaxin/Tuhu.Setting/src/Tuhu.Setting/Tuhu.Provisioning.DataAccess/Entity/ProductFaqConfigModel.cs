using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class ProductFaqConfigModel
    {   public int Pkid { get; set; }
        public string Pid { get; set; }
        public string ProductName { get; set; }
        public string QuestionDetail { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
    }

    public class ProductFaqConfigDetailModel
    {
        public int FkFaqId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public  int Status { get; set; }
        public int Sort { get; set; }
        //public List<string> Pids { get; set; }
    }
}
