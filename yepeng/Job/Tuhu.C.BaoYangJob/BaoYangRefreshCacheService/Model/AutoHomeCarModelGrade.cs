using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    public class AutoHomeCarModelGrade
    {
        public int PKID { get; set; }

        public int GradeId { get; set; }

        public string Brand { get; set; }

        public string CarSeries { get; set; }

        public string CarGrade { get; set; }

        public string Url { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public long BatchNo { get; set; }
    }
}
