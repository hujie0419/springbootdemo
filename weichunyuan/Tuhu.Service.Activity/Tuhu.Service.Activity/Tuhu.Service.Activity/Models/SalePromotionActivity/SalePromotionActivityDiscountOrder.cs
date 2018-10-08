using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class DiscountCreateOrderRequest
    {
        public string UserId { get; set; }

        public int OrderId { get; set; }

        public string Pid { get; set; }

        public int Num { get; set; }

        public string ActivityId { get; set; }
    }

    public class SalePromotionActivityDiscountOrder
    {
        public string UserId { get; set; }

        public int OrderId { get; set; }

        public string Pid { get; set; }

        public int Num { get; set; }

        public string ActivityId { get; set; }

        /// <summary>
        /// 0无效 1有效 -1表示错误数据
        /// </summary>
        public int OrderStatus { get; set; }

        public string CreateDateTime { get; set; }

        public string UpdateDateTime { get; set; }
    }

}
