using System;

namespace Tuhu.Service.Activity.Models
{
   public  class SeckillAvailableStockInfoResponse
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        public int SaleOutQuantity { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int TotalQuantity { get; set; }
    }

}
