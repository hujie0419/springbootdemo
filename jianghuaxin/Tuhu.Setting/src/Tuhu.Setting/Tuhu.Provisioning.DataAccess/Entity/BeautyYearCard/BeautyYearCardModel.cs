using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard
{
    public class BeautyYearCardModel
    {
        public int PKID { get; set; }
        public string CardName { get; set; }
        public decimal CardPrice { get; set; }
        public string PID { get; set; }
        public DateTime SalesBeginTime { get; set; }
        public DateTime SalesEndTime { get; set; }
        public int CardValidDays { get; set; }
        public string Status
        {
            get
            {
                var status = SalesEndTime.Date >= DateTime.Now.Date ? "正常" : "过期";
                return status;
            }
        }
        public string Description
        {
            get
            {
                string description = string.Empty;
                if (BeautyYearCardProducts != null && BeautyYearCardProducts.Any())
                {
                    for (int i = 1; i <= BeautyYearCardProducts.Length; i++)
                    {
                        description += $"{i}.{BeautyYearCardProducts[i - 1].ProductName},{BeautyYearCardProducts[i - 1].ProductPrice}元*{BeautyYearCardProducts[i - 1].ProductNumber}<br/>";
                    }
                }
                return description;
            }
        }
        public BeautyYearCardProductModel[] BeautyYearCardProducts { get; set; }

        public BeautyYearCardRegionModel[] BeautyYearCardRegions { get; set; }
        public string CardImageUrl { get; set; }
        public int AdaptVehicle { get; set; }
        public string Remark { get; set; }
        public string Rule { get; set; }
        public int ShopCount { get; set; }
    }
    public class BeautyYearCardProductModel
    {
        public int PKID { get; set; }
        public int CardId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductNumber { get; set; }
        public string ProductDescription { get; set; }
        public float Commission { get; set; }
        public int UseCycle { get; set; }
        public int CycleType { get; set; }

    }

    public class BeautyYearCardRegionModel
    {//PKID, , , , , CreateTime, UpdateTime
        public int CardId { get; set; }
        public int ProvinceId { get; set; }
        public string CityId { get; set; }
        public string[] CityIds
        {
            get
            {
                if (!string.IsNullOrEmpty(CityId))
                    return CityId.Split(',');
                return null;
            }
        }
        public bool IsAllCity { get; set; }
    }

    public class ProductSimpleModel
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string PID { get; set; }
    }
}
