using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BizRegion
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string PinYin { get; set; }

        public int Pkid { get; set; }
        public string RegionName { get; set; }
        public int ParentId { get; set; }
        public int CityId { get; set; }
    }
}
