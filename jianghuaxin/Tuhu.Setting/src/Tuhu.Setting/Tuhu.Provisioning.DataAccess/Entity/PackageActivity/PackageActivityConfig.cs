using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.PackageActivity
{
    public class PackageActivityConfig
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

        public string TipTextColor { get; set; }

        public string ButtonBackgroundColor { get; set; }

        public string ButtonTextColor { get; set; }

        public string BackgroundImg { get; set; }

        public string OngoingButtonText { get; set; }

        public DateTime CreateDateTime { get; set; }

        public string CreateUser { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public string UpdateUser { get; set; }

        public int AllSurplusNum { get; set; }

        public List<PackageActivityShopConfig> ShopConfig { get; set; }

        public List<PackageActivityRoundConfig> RoundConfig { get; set; }

        public List<PackageActivityProductConfig> ProductConfig { get; set; }

        public List<PackageActivityVehicleConfig> VehicleConfig { get; set; }

        public List<PackageActivityPriceConfig> PriceConfig { get; set; }

        public int Total { get; set; }

        /// <summary>
        /// 是否是分层次定价
        /// </summary>
        public bool IsTieredPricing { get; set; }
    }

    public class PackageActivityShopConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public int ShopType { get; set; }

        public int ShopId { get; set; }

        public string ShopName { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class PackageActivityRoundConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int LimitedQuantity { get; set; }

        public int SurplusNum { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class PackageActivityProductConfig
    {
        public int PKID { get; set; }

        public Guid ActivityId { get; set; }

        public string CategoryName { get; set; }

        public string PID { get; set; }

        public string Brand { get; set; }

        public bool IsIngore { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }

    public class PackageActivityVehicleConfig
    {
        [JsonIgnore]
        public int PKID { get; set; }

        public string VehicleID { get; set; }

        [JsonIgnore]
        public string Brand { get; set; }

        [JsonIgnore]
        public string Vehicle { get; set; }

        [JsonIgnore]
        public double AvgPrice { get; set; }

        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }

        [JsonIgnore]
        public DateTime LastUpdateDateTime { get; set; }
    }

    public class PackageActivityPriceConfig
    {
        [JsonIgnore]
        public int PKID { get; set; }

        public string TierType { get; set; }

        public decimal? Price { get; set; }

        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }

        [JsonIgnore]
        public DateTime LastUpdateDateTime { get; set; }
    }

    public class VehicleSimpleInfo
    {
        public string VehicleID { get; set; }

        public string Brand { get; set; }

        public string Vehicle { get; set; }

        public double AvgPrice { get; set; }
    }

    public enum TierType
    {
        Four = 4,
        Five,
        Six,
        Seven
    }
}
