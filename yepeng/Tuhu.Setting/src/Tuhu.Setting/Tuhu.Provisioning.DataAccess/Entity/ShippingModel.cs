using System;
using System.Collections.Generic;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShippingModel : BaseModel
    {
        public ShippingModel() { }
        public ShippingModel(DataRow row) : base(row) { }

        public int PKID { get; set; }
        public int Types { get; set; }
        public decimal Value { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime LastUpdateDateTime { get; set; }
        public int? CityID { get; set; }
        public List<int> CityIDs { get; set; }
        public string StrCityNames { get; set; }
        public string StrCityIDs { get; set; }

        public int UserType { get; set; }
    }

    public class GradeDeliveryFeeRule : BaseModel
    {
        public int PKID { get; set; }

        public string Grade { get; set; }

        public int ProductType { get; set; }

        public bool IsShopInstall { get; set; }

        public decimal Price { get; set; }

        public bool IsFreeInstall { get; set; }

        public string Content { get; set; }

        public string ContainInstall { get; set; }

        public string NoContainInstall { get; set; }

        public int UserType { get; set; }
    }
}
