using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class TireRecallModel
    {
        public long PKID { get; set; }

        public string OrderNo { get; set; }

        public string Mobile { get; set; }

        public string CarNo { get; set; }

        public string VehicleLicenseImg { get; set; }

        public string TireDetailImg { get; set; }

        public string TireAndLicenseImg { get; set; }

        public string OrderImg { get; set; }

        public short Status { get; set; }

        public string LastAuthor { get; set; }

        public string Operator { get; set; }

        public int OperateType { get; set; }
        public DateTime UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }
        public string Reason { get; set; }

        public int Num { get; set; }

    }

    public class TireRecallLog
    {
        public long PKID { get; set; }

        public long RecallID { get; set; }

        public int OperateType { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public string Operator { get; set; }

        public string Reason { get; set; }
    }

    public class Special_Bridgestone_Pidweekyear
    {
        public int orderid { get; set; }
        public string UserName { get; set; }
        public string UserTel { get; set; }
        public string pid { get; set; }
        public int num { get; set; }
        public Guid userid { get; set; }
    }
}
