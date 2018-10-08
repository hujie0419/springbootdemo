using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SE_CarTagCouponConfig
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string CouponGuid { get; set; }

        public decimal Discount { get; set; }

        public string Description { get; set; }

        public decimal MinMoney { get; set; }

        public bool Status { get; set; }

        public DateTime CreateDate { get; set; }


        public DateTime StartDateTime { get; set; }


        public DateTime EndDateTime { get; set; }

        public string ImageUrl { get; set; }


    }
}
