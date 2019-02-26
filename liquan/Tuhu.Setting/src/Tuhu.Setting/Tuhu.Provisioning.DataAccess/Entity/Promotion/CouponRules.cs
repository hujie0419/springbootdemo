using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CouponRules
    {
        public int PKID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ProductID { get; set; }
        public string Brand { get; set; }
        public int ParentID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastDateTime { get; set; }
        public string RulePID { get; set; }
    }
}
