using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopServiceLog
    {
        public int PKID { get; set; }
        public int ShopID { get; set; }
        public string Author { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string Value { get; set; }
        public DateTime ChangeDatetime { get; set; }
        public string IPAddress { get; set; }
        public string Operation { get; set; }
    }

    public class ServicePModel
    {
        public int ShopID { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int CosmetologyServersID { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
