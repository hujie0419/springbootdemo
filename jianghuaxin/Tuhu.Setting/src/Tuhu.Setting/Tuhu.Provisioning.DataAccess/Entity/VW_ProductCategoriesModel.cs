using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [Serializable]
    public class VW_ProductCategoriesModel
    {
        public string oid { get; set; }
        public string CategoryName { get; set; }
    }
}
