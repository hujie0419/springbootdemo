using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ShopSyncJob.Models
{
    internal class ShopProduct
    {
        public string out_shop_id { get; set; }

        public int? service_category_id { get; set; }

        public string product_name { get; set; }

        public string product_desc { get; set; }

        public decimal off_price { get; set; }

        public decimal orig_price { get; set; }

      

        public string status { get; set; }

    }
}

