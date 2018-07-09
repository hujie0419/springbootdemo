using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ActivityMonitor.Model
{
    public class ModuleContentConfig
    {
        public int ID { get; set; }

        public int ParentID { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string MoreUri { get; set; }

        public string ModuleName { get; set; }

        public string HelperModuleName { get; set; }

        public string HomePageName { get; set; }
    }

    public class FlowConfig
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string StartVersion { get; set; }

        public string EndVersion { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string LinkUrl { get; set; }
    }
}
