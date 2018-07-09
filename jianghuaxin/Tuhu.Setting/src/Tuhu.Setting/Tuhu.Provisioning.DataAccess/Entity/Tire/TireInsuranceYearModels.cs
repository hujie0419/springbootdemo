using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TireInsuranceYearModel
    {
        public string PID { get; set; }
        public string DisplayName { get; set; }
        public string TireSize { get; set; }

        public int TireInsuranceYears { get; set; }
    }

    public class TireModifyConfigLog
    {
        public int PkId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Pid { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        public DateTime LastUpdateDateTime { get; set; } = DateTime.Now;
    }
}
