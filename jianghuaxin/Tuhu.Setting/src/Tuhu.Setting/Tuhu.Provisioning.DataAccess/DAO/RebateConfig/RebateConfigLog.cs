using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.DAO.RebateConfig
{
    public class RebateConfigLog
    {
        public int PKID { get; set; }

        public string Type { get; set; }

        public string IdentityId { get; set; }
        
        public string Msg { get; set; }

        public string Remark { get; set; }

        public string IPAddress { get; set; }

        public string HostName { get; set; }

        public string OperateUser { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
    }
}
