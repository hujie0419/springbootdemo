
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.DataAccess.Entity.Tire
{
    public class RegionStockModel
    {
        public string PID { get; set; }
        public int CityId { get; set; }
        public string DisplayName { get; set; }
        public string DefaultStockName { get; set; }
        public int? StockNum { get; set; }
        public decimal? SafeStockNum { get; set; }
        public int AllStockNum { get; set; }
        public decimal AllSafeStockNum { get; set; }
        public decimal? CentralStockNum { get; set; }
        public int? CentralSafeStockNum { get; set; }
        public int? RegionavailNums { get; set; }
        public decimal? NoReduceStockGrade { get; set; }
    }

    public class RegionStockRequest
    {
        public int CityId { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public string VehicleId { get; set; }
        public string TireSize { get; set; }
    }
    public class RegionListModel
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public List<RegionModel> ChildRegion { get; set; }
    }
}