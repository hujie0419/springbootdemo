using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity.Battery;

namespace Tuhu.Provisioning.DataAccess.Request
{
    public class BatteryLevelUpRequest: BatteryLevelUpEntity
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
