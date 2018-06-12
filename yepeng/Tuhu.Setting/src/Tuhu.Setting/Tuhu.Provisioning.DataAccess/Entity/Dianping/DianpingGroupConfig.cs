using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Dianping
{
    public class DianpingGroupConfig
    {
        public int PKID { get; set; }

        public string DianpingGroupId { get; set; }

        public string DianpingBrand { get; set; }

        public string DianpingTuanName { get; set; }

        public string TuhuProductId { get; set; }

        public string TuhuProductName { get; set; }

        public int TuhuProductStatus { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
