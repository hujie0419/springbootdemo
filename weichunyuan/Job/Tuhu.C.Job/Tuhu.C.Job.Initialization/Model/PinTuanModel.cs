using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Initialization.Model
{
    public class PinTuanProductModel
    {
        public string ProductGroupId { get; set; }
        public string PId { get; set; }
        public decimal ActivityPrice { get; set; }
        public bool Display { get; set; }
    }

    public class PinTuanOriginalStockModel
    {
        public string PId { get; set; }
        public int Warehouseid { get; set; }
        public int StockCount { get; set; }
    }
}
