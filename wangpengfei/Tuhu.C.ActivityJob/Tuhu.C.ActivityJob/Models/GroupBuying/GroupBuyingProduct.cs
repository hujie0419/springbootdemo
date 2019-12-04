using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.GroupBuying
{
    public class GroupBuyingProduct
    {
        public int Pkid { get; set; }

        public string Pid { get; set; }

        public string ProductGroupId { get; set; }

        public int TotalPGroupCount { get; set; }

        public int CurrentPGroupCount { get; set; }
    }
}
