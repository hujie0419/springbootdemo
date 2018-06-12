using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.BaoYangSuggest.Model
{
    public class UserBaoYangRecordModel
    {
        public int PKID { get; set; }

        public Guid UserID { get; set; }

        public string VechileID { get; set; }

        public Guid UserCarID { get; set; }

        public DateTime BaoYangDateTime { get; set; }

        public int Distance { get; set; }

        public string BaoYangType { get; set; }

        public int RelatedOrderID { get; set; }

        public string RelatedOrderNo { get; set; }

        public string PID { get; set; }

        public string Category { get; set; }

        public int Status { get; set; }

        public bool IsTuhuRecord { get; set; }

        public decimal OrderPrice { get; set; }

        public int InstallShopId { get; set; }

        public string InstallShopName { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}
