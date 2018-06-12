using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class InstallNowConditionModel
    {
        public string CityIds { get; set; }

        public string TireSize { get; set; }
        public string PID { get; set; }
        public bool? Status { get; set; }
    }
    public class InstallNowModel
    {
        public int PKID { get; set; }

        public string TireSize { get; set; }
        public string PID { get; set; }
        public bool Status { get; set; }

        public string City { get; set; }
        public int CityId { get; set; }
        public string Province { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public Guid LogBitGroup { get; set; }
    }

    public class PidModel
    {
        public string PID { get; set; }
        public bool Status { get; set; }
    }
    public class ResultModel
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<InstallNowModel> SuccessItem { get; set; }
        public IEnumerable<InstallNowModel> ExsitItem { get; set; }
    }
}
