using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.CompetingProducts.Models
{
    public class LogModel
    {
        public string ShopCode { get; set; }
        public string Api { get; set; }
        public string RefNo { get; set; }
        public int? OrderId { get; set; }
        public string Message { get; set; }
        public string ResponseContent { get; set; }
    }
}
