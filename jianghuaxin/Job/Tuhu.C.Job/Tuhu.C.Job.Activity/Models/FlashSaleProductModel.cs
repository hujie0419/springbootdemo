using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Activity.Models
{
    public class FlashSaleProductModel
    {
        public string ActivityId { get; set; }

        public string Pid { get; set; }

        public string Name { get; set; }

        public DateTime StartDateTime { get; set; }

        public int Num { get; set; }
    }
}
