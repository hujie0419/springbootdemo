using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class SearchWordConvertMapDb
    {
        public long PKID { get; set; }
        public string TargetWord { get; set; }
        public string SourceWord { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
