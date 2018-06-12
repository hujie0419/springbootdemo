using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class BaoYangAccessoryConfig
    {
        public List<BaoYangAccessory> PartAccessories { get; set; }
    }

    public class BaoYangAccessory
    {
        public string PartName { get; set; }

        public List<Item> Display { get; set; }

        public string Level { get; set; }

        public string Viscosity { get; set; }

        public string Interface { get; set; }

        public string Description { get; set; }

        public Standard Volume { get; set; }

        public Standard Quantity { get; set; }
    }

    public class Item
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Standard
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public string Unit { get; set; }
    }
}
