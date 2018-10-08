using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopRegion
    {
        public int PKID { get; set; }
        public string RegionName { get; set; }
        public int? ParentID { get; set; }
        //public ShopRegionModel ParentRegion { get; set; }
        public IEnumerable<ShopRegion> ChildrenRegion { get; set; }
    }
    public class ShopArea
    {
        public string RegionName { set; get; }
        public int PKID { set; get; }

        public List<ChilShopArea> region { set; get; }
    }

    public class ChilShopArea
    {
        public string RegionName { set; get; }
        public int PKID { set; get; }
    }
}
