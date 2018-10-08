using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RewardApplicationModel
    {
        public string ApplicationName { get; set; }

        public string Phone { get; set; }

        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }

        public int ApplicationState { get; set; }

        public string Auditor { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? LastUpdateDateTime { get; set; }

        public int TotalCount { get; set; }

        public List<string> Phones { get; set; }
    }
}
