using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BaoYang
{
    public class InstallTypeConfig
    {
        public string PackageType { get; set; }

        public string PackageName { get; set; }

        public string ImageUrl { get; set; }

        public List<InstallTypeConfigItem> InstallTypes { get; set; }
    }

    public class InstallTypeConfigItem
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public List<string> BaoYangTypeList { get; set; }

        public List<string> BaoYangTypeNameList { get; set; }

        public string BaoYangTypeNames { get; set; }

        public bool IsDefault { get; set; }

        public bool NeedAll { get; set; }

        public string TextFormat { get; set; }

        public List<InstallTypeText> TextFormats { get; set; }
    }

    public class InstallTypeText
    {
        public string Text { get; set; }

        public string Color { get; set; }
    }
}
