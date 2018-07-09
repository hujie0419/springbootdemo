using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangRecommendConfig
    {
        public List<BaoYangRecommendConfigItem> Sku { get; set; }

        public List<BaoYangRecommendConfigItem> Brand { get; set; }

        public List<BaoYangRecommendConfigItem> BrandPrice { get; set; }
    }

    public class BaoYangRecommendConfigItem
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string SeriesName { get; set; }

        public string SeriesKey { get; set; }
    }
}
