using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSaleProductsExtend
    {
        public int PKID { get; set; }
        public string ActivityID { get; set; }

        public string ActivityName { get; set; }

        public string PID { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string ProductName { get; set; }

        public bool IsUsePCode { get; set; }

        /// <summary>
        /// 月销量
        /// </summary>
        public int MonthSales { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        public decimal Costing { get; set; }
              
        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossProfitMargin { get; set; }
    }
}
