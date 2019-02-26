using System.Collections.Generic;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ProductInformation
    {
        public float Commission { get; set; }
        public string PID { get; set; }
        public string DisplayName { get; set; }
        public string SitePrice { get; set; }
        public string SiteBJPrice { get; set; }
        public byte stockout { get; set; }
        public string PriceList { get; set; }
        public string StockList { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 花纹
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        public string DefinitionName { get; set; }
        public string CategoryName { get; set; }

        public int OID { get; set; }
        public bool OnSale { get; set; }
    }
    public class ProductInfo_Order
    {
        public string PID { get; set; }
        public string DefinitionName { get; set; }
        public string DisplayName { get; set; }
        public decimal Price { get; set; }
        public decimal BJ_Price { get; set; }
        public decimal MarkedPrice { get; set; }
        public string Size { get; set; }
        public string invoice { get; set; }
        public byte OnSale { get; set; }
        public byte stockout { get; set; }
        public int Num { get; set; }
        public string Tire_Rim { get; set; }
        public string Tire_ROF { get; set; }

        public string CatalogName { get; set; }

        public string Catalog { get; set; }
        public string CP_Brake_Position { get; set; }
        public string CP_Brake_Type { get; set; }
        public double? Weight { get; set; }
        public string CP_ShuXing1 { get; set; }
        public Dictionary<string, decimal> dicOtherFee { get; set; }
         
    }
}
