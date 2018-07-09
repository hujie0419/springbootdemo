using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VehicleTypeModel
    {
        public string ProductID { set; get; }

        public string Brand { set; get; }
     
        public string Vehicle { set; get; }
    }

    public class ProvinceCity
    {
        public int ProvinceId { set; get; }

        public string ProvinceName { set; get; }

        public string CityName { set; get; }

        public int CityId { set; get; }

    }
}
