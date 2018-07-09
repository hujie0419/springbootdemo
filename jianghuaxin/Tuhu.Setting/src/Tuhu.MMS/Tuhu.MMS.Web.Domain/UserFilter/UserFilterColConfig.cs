using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.MMS.Web.Domain.UserFilter
{
    public class UserFilterColConfig
    {
        public string BaseCategory { get; set; }
        public string BaseCategoryName { get; set; }
        public string SecnodCategory { get; set; }
        public string SecondCategoryName { get; set; }
        public string TableName { get; set; }
        public List<TableColConfig> TableColConfig { get; set; }
    }

    public class TableColConfig
    {
        public string ColName { get; set; }
        public string TableColName { get; set; }

    }
}
