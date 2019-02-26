using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class NuomiServicesConfig
    {
        public int PKID { get; set; }

        public long NuomiId { get; set; }

        public string NuomiTitle { get; set; }

        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string Email { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int IsValid { get; set; }

        public int Total { get; set; }
    }
}
