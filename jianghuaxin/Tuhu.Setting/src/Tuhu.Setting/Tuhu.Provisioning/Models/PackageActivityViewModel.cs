using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PackageActivityViewModel
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public string ActivityName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int MaxSaleQuantity { get; set; }

        public string PackageTypes { get; set; }

        public bool IsChargeInstallFee { get; set; }

        public bool IsUsePromotion { get; set; }

        public string InstallOrPayType { get; set; }

        public int ItemQuantityPerUser { get; set; }

        public decimal PromotionPrice { get; set; }

        public bool IsTieredPricing { get; set; }

        public string TipTextColor { get; set; }

        public string ButtonBackgroundColor { get; set; }

        public string ButtonTextColor { get; set; }

        public string BackgroundImg { get; set; }

        public string OngoingButtonText { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string CreateUser { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public string UpdateUser { get; set; }

        public List<PackageActivityShop> ShopConfig { get; set; }

        public List<PackageActivityRound> RoundConfig { get; set; }

        public ProductDescriptionConfig ProductConfigDetails { get; set; }

        public List<VehicleConfig> VehicleConfig { get; set; }

        public Dictionary<string, string> PriceConfig { get; set; }
    }

    public class VehicleConfig
    {
        public string VehicleID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public double AvgPrice { get; set; }
    }

    public class PackageActivityShop
    {
        public int ShopType { get; set; }

        public int ShopId { get; set; }

        public string ShopName { get; set; }
    }

    public class PackageActivityRound
    {
        public int PKID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int LimitedQuantity { get; set; }
    }
    public class ProductDescriptionConfig
    {
        public List<string> Categories { get; set; }
        public string CategoryName { get; set; }
        public bool IsIngore { get; set; }
        public List<string> Pids { get; set; }
        public List<ProductDescriptionBrand> BrandDetails { get; set; }
    }

    public class ProductDescriptionBrand
    {
        public string BrandCategoryName { get; set; }
        public List<string> Brands { get; set; }
    }
}