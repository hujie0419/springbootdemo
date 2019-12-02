using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ActivityUserInfo_xhrRequestModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserTell { get; set; }

        public int AreaID { get; set; }

        public int PassStatus { get; set; }

        public int ActID { get; set; }

        public int UserStatus { get; set; }
    }
}
