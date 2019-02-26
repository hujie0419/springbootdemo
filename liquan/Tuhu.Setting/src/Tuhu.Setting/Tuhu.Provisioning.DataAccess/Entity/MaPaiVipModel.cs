using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class MaPaiVipModel
    {
        public int PKID { get; set; }

        public string UniquePrivilegeCode { get; set; }

        public string UserName { get; set; }

        public string UserPhone { get; set; }

        public string EmployeeNumber { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int Total { get; set; }
    }
}
