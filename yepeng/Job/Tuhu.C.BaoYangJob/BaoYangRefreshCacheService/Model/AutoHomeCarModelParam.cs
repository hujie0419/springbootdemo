using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class AutoHomeCarModelParam
    {
        public long PKID { get; set; }

        public int CarModelId { get; set; }

        public string ParamName { get; set; }

        public string ParamValue { get; set; }

        public long BatchNo { get; set; }
    }
}
