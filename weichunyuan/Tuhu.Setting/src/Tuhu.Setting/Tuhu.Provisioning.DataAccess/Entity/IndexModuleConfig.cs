using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class IndexModuleConfig
    {
        public int PKID { get; set; }

        public string ModuleName { get; set; }

        public int DisplayIndex { get; set; }

        public List<IndexModuleItem> Items { get; set; }
    }

    public class IndexModuleItem
    {
        public int PKID { get; set; }

        public int ModuleId { get; set; }

        public string EntryName { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string LinkUrl { get; set; }

        public int DisplayIndex { get; set; }
    }
}
