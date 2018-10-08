using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class RuntimeSwitch
    {
        public int PKID { get; set; }
        public string SwitchName { get; set; }
        public bool Value { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
