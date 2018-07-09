using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CompanyClientConfig
    {
        public int PKID { get; set; }

        public string Channel { get; set; }

        public string ActivityUrl { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int Total { get; set; }
    }

    public class ClientCouponCode
    {
        public int PKID { get; set; }

        public int ParentId { get; set; }

        public string CouponCode { get; set; }

        public string Telephone { get; set; }

        public DateTime? CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int Total { get; set; }
    }
}
