using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BeautyServicePackageDetailCode
    {
        public int PKID { get; set; }

        public Guid UserId { get; set; }

        public int PackageDetailId { get; set; }

        public string ServiceCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PackageCode { get; set; }

        public Guid? VipUserId { get; set; }
    }
}
