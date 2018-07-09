using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangYearCardConfigs
    {
        public List<BaoYangYearCardConfig> BaoYangYearCardConfig { get; set; }
    }

    public class BaoYangYearCardConfig
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public List<YearCardContent> Content { get; set; }
    }

    public class YearCardContent
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }
    }
}
