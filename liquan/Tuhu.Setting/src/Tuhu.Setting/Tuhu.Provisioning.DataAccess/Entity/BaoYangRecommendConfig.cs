using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class BaoYangRecommendConfig
    {
        public List<BaoYangRecommendConfigItem> Sku { get; set; }

        public List<BaoYangRecommendConfigItem> Brand { get; set; }

        public List<BaoYangRecommendConfigItem> BrandPrice { get; set; }
    }

    public class RecommendConfig
    {
        [XmlArrayItem("Item")]
        public List<CategoryPartNameMap> CategoryPartNameMap { get; set; }
        [XmlArrayItem("Item")]
        public List<MaintenanceTypePartNameMap> MaintenanceTypePartNameMap { get; set; }
    }
    [XmlRoot("MaintenanceTypePartNameMap")]
    public class MaintenanceTypePartNameMap
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlAttribute("partName")]
        public string partName { get; set; }
    }
    [XmlRoot("CategoryPartNameMap")]
    public class CategoryPartNameMap
    {
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("partNames")]
        public string partNames { get; set; }
        public List<string> GetPartNames()
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.partNames))
            {
                result = this.partNames.Split(new[] { ",", "，", " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return result;
        }
    } 
    public class BaoYangRecommendConfigItem
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string SeriesName { get; set; }

        public string SeriesKey { get; set; }
    }
}
