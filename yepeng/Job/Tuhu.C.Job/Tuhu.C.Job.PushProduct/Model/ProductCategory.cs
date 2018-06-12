using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.PushProduct.Model
{
    public class ProductCategory
    {
        public int oid { get; set; }

        public int ParentOid { get; set; }

        public string CategoryName { get; set; }

        public string DisplayName { get; set; }

        public int DescendantProductCount { get; set; }
    }
}
