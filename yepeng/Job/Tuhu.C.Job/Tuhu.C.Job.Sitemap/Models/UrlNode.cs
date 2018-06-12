using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Sitemap.Models
{
    class UrlNode
    {
        public string URL { get; set; }

        public string Type { get; set; }

        public DateTime DataCreate_Time { get; set; }

        public DateTime DataUpdate_Time { get; set; }
    }
}
