using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Dianping
{
    public class DianpingShopSession
    {
        public int PKID { get; set; }

        public int TuhuShopId { get; set; }

        public string Session { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }
    }
}
