using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.PackageActivity
{
    public class PackageActivityLog
    {
        public int PKID { get; set; }

        public string ObjectId { get; set; }

        public string Method { get; set; }

        public string Message { get; set; }

        public string Remarks { get; set; }

        public string Operator { get; set; }

        public string Type { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
