using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.BaoYangSuggest.Model
{
    public class OrderCarModel
    {
        public int OrderId { get; set; }

        public string VehicleId { get; set; }

        public int Distance { get; set; }

        public bool UserCreated { get; set; }
    }
}
