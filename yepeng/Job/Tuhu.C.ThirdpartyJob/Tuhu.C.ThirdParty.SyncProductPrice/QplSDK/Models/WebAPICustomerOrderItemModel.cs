using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    [Serializable]
    public class WebAPICustomerOrderItemModel
    {
        public string UserProductID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductCount { get; set; }
        public decimal ListPrice { get; set; }
        public decimal FinalPrice { get; set; }

    }
}
