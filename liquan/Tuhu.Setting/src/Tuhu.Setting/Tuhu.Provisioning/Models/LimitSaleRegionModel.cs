
namespace Tuhu.Provisioning.Models
{
    public class LimitSaleRegionModel
    {
        public int Pkid { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int ProductConfigId { get; set; }
        public string Pid { get; set; }
        public int IsAllowSale { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}