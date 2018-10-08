using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tuhu.C.Job.ActivityMonitor.Model
{
    [Serializable]
    public class ActivityBuild
    {
        public int id { get; set; }

        public string Title { get; set; }
        [XmlIgnore]
        public string Content { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDT { get; set; }

    }


    [Serializable]
    public class ActivePage
    {
        public int PKID { get; set; }

        public string Title { get; set; }

        public string HashKey { get; set; }

        public string RuleDesc { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

    }

    [Serializable]
    public class OverdueActivityConfig
    {
        public int ID { get; set; }
        public string Title { get; set; }

        [XmlElement(IsNullable = false)]
        public string Url { get; set; }

        public DateTime EndDate { get; set; }

        public string Type { get; set; }

        public int Status { get; set; }

        public int ExpireDays { get; set; }

        public string ModuleName { get; set; }

        public string HelperModuleName { get; set; }

        public string HomePageName { get; set; }

    }

    public class OverdueAcitivityConfigComparer : IEqualityComparer<OverdueActivityConfig>
    {
        public bool Equals(OverdueActivityConfig A, OverdueActivityConfig B)
        {
            if ((string.Equals(A?.Url, B?.Url, StringComparison.OrdinalIgnoreCase)) && (string.Equals(A?.HomePageName, B?.HomePageName, StringComparison.OrdinalIgnoreCase))&&(string.Equals(A?.ModuleName, B?.ModuleName, StringComparison.OrdinalIgnoreCase))&& (string.Equals(A?.HelperModuleName, B?.HelperModuleName, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(OverdueActivityConfig obj)
        {
            return obj == null ? 0 : obj.ID;
        }
    }
}
