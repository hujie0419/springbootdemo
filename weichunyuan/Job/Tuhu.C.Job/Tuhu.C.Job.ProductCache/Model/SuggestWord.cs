using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ProductCache.Model
{
    public class SuggestWord
    {
        public string Keyword { get; set; }
        public int Weight { get; set; }
        public string Source { get; set; }

        public bool IsActive { get; set; }
    }
}
