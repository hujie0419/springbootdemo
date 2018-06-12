using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    [Serializable]
    public class GetShopSaleItemForGrouponPageData
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }

        public string ShopName { get; set; }

        public RegionModel Region { get; set; }

        public int ShopType { get; set; }
        public string Category { get; set; }
        public string ProName { get; set; }
        public int Sales { get; set; }
        public int IsActive { get; set; }
    }
    public class RegionModel
    {
        public string ProvinceName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
    }
}