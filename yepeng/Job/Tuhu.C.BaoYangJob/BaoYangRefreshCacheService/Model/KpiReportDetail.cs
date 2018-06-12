using System;
using System.Collections.Generic;
using System.Linq;

namespace BaoYangRefreshCacheService.Model
{
    public class KpiReportDetail
    {
        public string TypeName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ParameterName { get; set; }
        public int VehicleTotalCount { get; set; }
        public int VehicleAdaptCount { get; set; }
        public string HotVehicleType { get; set; }
        public string VehicleBrand { get; set; }
        public string Name { get; set; }
        public double Ratio
        {
            get
            {
                return VehicleTotalCount == 0 ? 0 : Convert.ToDouble(VehicleAdaptCount) / Convert.ToDouble(VehicleTotalCount);
            }
        }
    }

    public class BrandCount
    {
        public string Brand { get; set; }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        public string Name { get; set; }
    }

    public class BaoYangKpiReportConfig
    {
        public List<CoverageRatioConfigItem> CoverageRatios { get; set; }

        public List<CoverageRatioConfigItem> TireCoverageRatios { get; set; }

        public List<CoverageRatioConfigItem> AdaptationChangeds { get; set; }
    }

    public class CoverageRatioConfigItem
    {
        public string DisplayName { get; set; }

        public List<string> Names { get; set; }

        public string Group { get; set; }

        public string RefType { get; set; }

        public List<string> Actions { get; set; }
    }
}
