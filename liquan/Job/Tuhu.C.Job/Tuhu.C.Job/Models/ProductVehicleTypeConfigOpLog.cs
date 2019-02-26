using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Models
{
    public class ProductVehicleTypeConfigOpLog
    {
        public long? Id { get; set; }

        public string PID { get; set; }

        public string Operator { get; set; }

        public string OperateContent { get; set; }

        public DateTime OperateTime { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
