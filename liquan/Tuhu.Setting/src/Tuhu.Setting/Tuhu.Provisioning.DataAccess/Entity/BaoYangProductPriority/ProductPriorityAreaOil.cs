using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYangProductPriority
{
    public class ProductPriorityAreaOil
    {
        public ProductPriorityAreaOil()
        {
            Details = new List<ProductPriorityAreaOilDetail>();
        }
        public int PKID { get; set; }

        public int AreaId { get; set; }
        public string VehicleId { get; set; }
        public string Viscosity { get; set; }
        public string Grade { get; set; }
        public string NewViscosity { get; set; } 
        public bool IsEnabled { get; set; } 
        public bool IsDeleted { get; set; }
        public List<ProductPriorityAreaOilDetail> Details { get; set; }
        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }
    }
}
