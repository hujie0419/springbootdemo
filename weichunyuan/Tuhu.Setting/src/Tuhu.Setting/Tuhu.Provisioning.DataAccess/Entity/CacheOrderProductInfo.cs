using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class CacheOrderProductInfo
    {
        public int LocationId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TuHuPrice { get; set; }
        public decimal TianMaoOnePrice { get; set; }
        public decimal TianMaoTwoPrice { get; set; }
        public decimal TianMaoThreePrice { get; set; }
        public decimal TaoBaoPrice { get; set; }
        public decimal JingDongPrice { get; set; }
        public int MonthSalesQuantity { get; set; }
        public int WeekSalesQuantity { get; set; }
        public decimal DaySalesQuantity { get; set; }
        public int StockQuantity { get; set; }
        public int AvailableStockQuantity { get; set; }
        public decimal StockCost { get; set; }
        public bool OnSale { get; set; }
        public DateTime CreateTime { get; set; }
        public string Brand { get; set; }
        public bool StockOut { get; set; }
    }

    public class ProductSalesPrice
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TuhuPrice { get; set; }
        public decimal TianMaoOnePrice { get; set; }
        public decimal TianMaoTwoPrice { get; set; }
        public decimal TianMaoThreePrice { get; set; }
        public decimal TaoBaoPrice { get; set; }
        public decimal JingDongPrice { get; set; }
        public bool OnSale { get; set; }
        public string Brand { get; set; }
        public DateTime CreateTime { get; set; }
        public bool StockOut { get; set; }
    }

    public class ProductSalesReport
    {
        public int WareHouseId { get; set; }
        public string ProductId { get; set; }
        public int MonthSalesQuantity { get; set; }
        public int WeekSalesQuantity { get; set; }
        public decimal DaySalesQuantity { get; set; }
    }

    public class ProductStockLocation
    {
        public int BatchId { get; set; }
        public int LocationId { get; set; }
        public string ProductId { get; set; }
        public int StockQuantity { get; set; }
        public int AvailableStockQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal StockCost { get; set; }
    }
}
