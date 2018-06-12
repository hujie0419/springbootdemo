using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    public class WebApiProductInfoModel
    {
        public string PID { get; set; }
        public string UserProductID { get; set; }
        public string ProductID { get; set; }
        public string UserSKU { get; set; }
        public decimal ListPrice { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// 平台上架状态 0上架 1 下架
        /// </summary>
        public int PTAvailability { get; set; }
        /// <summary>
        /// 商户商品上架状态  0上架 1 下架
        /// </summary>
        public int SHAvailability { get; set; }
    }
}
