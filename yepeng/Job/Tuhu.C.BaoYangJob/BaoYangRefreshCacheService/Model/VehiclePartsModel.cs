using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class VehiclePartsLiYangModel
    {
        public long PKID { get; set; }
        public string LiYangId { get; set; }

    }

    public class VehicleTypeIdMap
    {
        public string Tid { get; set; }
        public string ExternalId { get; set; }
    }
    public class LiYangVehicleModel
    {
        public string LiYangId { get; set; }
        public string LiYangLevelId { get; set; }

    }
    public class LiYangLevelIdTidMap
    {
        public string Tid { get; set; }
        public string ExternalId { get; set; }
        public string LiYangLevelId { get; set; }
    }

    public class VehiclePartsModel
    {
        public string Tid { get; set; }
        public string OePartCode { get; set; }
        public string OPCodeReform
        {
            get
            {
                if (!string.IsNullOrEmpty(OePartCode))
                    return OePartCode.Trim().Replace(" ", "");
                return null;
            }
        }
        public string Source { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
