using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangYearCard
    {
        public int Pkid { get; set; }

        public string DisplayName { get; set; }

        public string Pid { get; set; }

        public string CategoryName { get; set; }

        public string FuelType { get; set; }

        public int PromotionCount { get; set; }

        public string ProductName { get; set; }

        public int ShopType { get; set; }

        public int ShopID { get; set; }

        public string ShopName { get; set; }

        public string ImageUrl { get; set; }

        public int PromotionIndex { get; set; }

        public double PromotionPercentage { get; set; }
    }

    public class YearCardParameter
    {
        public int Pkid { get; set; }

        public string DisplayName { get; set; }

        public string Pid { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
    }

    public class BaoYangYearCardDetail
    {
        public int Pkid { get; set; }

        public int YearCardId { get; set; }

        public string DisplayName { get; set; }

        public string Pid { get; set; }

        public string PackageType { get; set; }

        public string BaoYangType { get; set; }

        public int PromotionIndex { get; set; }

        public int ProductCount { get; set; }

        public decimal PromotionPercentage { get; set; }
    }

    public class BaoYangYearCardShop
    {
        public int Pkid { get; set; }

        public int YearCardId { get; set; }

        public int ShopType { get; set; }

        public int ShopID { get; set; }
    }

    public class BaoYangYearCardPromotion
    {
        public int Pkid { get; set; }

        public int YearCardId { get; set; }

        public int PromotionIndex { get; set; }

        public decimal PromotionPercentage { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }
}
