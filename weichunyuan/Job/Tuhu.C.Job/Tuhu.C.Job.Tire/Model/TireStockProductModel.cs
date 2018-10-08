
namespace Tuhu.C.Job.Tire.Model
{
    public class TireStockProductModel
    {
        public string PID { get; set; }
        public string Brand { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 1--系统标缺
        /// </summary>
        public int SystemStockout { get; set; }
        /// <summary>
        /// 1--人工标缺
        /// </summary>
        public int StockoutStatus { get; set; }
        /// <summary>
        /// 上下架状态
        /// </summary>
        public bool OnSale { get; set; }
        public int MonthSales { get; set; }
        public int CurrentStockCount { get; set; }
    }
}
