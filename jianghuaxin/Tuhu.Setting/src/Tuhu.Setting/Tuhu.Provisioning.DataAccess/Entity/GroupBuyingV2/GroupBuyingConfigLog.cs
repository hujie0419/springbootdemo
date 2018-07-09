using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.GroupBuyingV2
{
    public class GroupBuyingConfigLog
    {
        public int PKID { get; set; }

        public string IdentityID { get; set; }

        public string Source { get; set; }

        public string MethodType { get; set; }

        public string Msg { get; set; }

        public string BeforeValue { get; set; }

        public string AfterValue { get; set; }

        public string OperateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
