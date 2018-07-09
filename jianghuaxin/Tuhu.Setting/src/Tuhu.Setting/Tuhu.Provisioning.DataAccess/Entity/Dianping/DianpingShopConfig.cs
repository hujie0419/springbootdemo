using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Dianping
{
    public class DianpingShopConfig
    {
        public int PKID { get; set; }

        public string DianpingId { get; set; }

        public string DianpingName { get; set; }

        public string DianpingShopName { get; set; }

        public int TuhuShopId { get; set; }

        public string TuhuShopName { get; set; }

        public string TuhuShopAddress { get; set; }

        public int GroupStatus { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public DianpingShopSession Session { get; set; }
    }
}
