using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class AutoHomeCarModelInfo
    {
        public long PKID { get; set; }

        public int GradeId { get; set; }

        public int CarModelId { get; set; }

        public string CarModelName { get; set; }

        public string Url { get; set; }

        public long BatchNo { get; set; }
    }
}
