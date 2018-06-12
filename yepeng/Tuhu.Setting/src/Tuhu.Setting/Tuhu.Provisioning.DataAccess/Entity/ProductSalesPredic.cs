using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public  class ProductSalesPredic
    {
        public string PID { get; set; }

        public decimal OfficialWebsitePrice { get; set; }

        public decimal Cost { get; set; }
    }
}
