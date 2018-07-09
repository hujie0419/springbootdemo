using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.UserShareJob.Model
{
    public class CouponPriceModel
    {
        public string PID { get; set; }
        public string DisplayName { get; set; }
        public decimal? JDPrice { get; set; }
        public decimal? QCCRPrice { get; set; }
        public decimal? GWPrice { get; set; }
        public decimal CouponPrice { get; set; }

        public decimal? CostPrice { get; set; }
        public int Stock { get; set; }
        public int WeekSaleCount { get; set; }
        public int MonthSaleCount { get; set; }
        public string Brand { get; set; }
        public bool OnSale { get; set; }
    }

}
