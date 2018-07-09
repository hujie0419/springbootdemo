using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.BaoYangSuggest.Model
{
    public class SuggestModel
    {
        public Guid UserId { get; set; }
        public string VehicleId { get; set; }
        public Guid CarId { get; set; }
        public int SuggestNum { get; set; }
        public int UrgentNum { get; set; }
        public int VeryUrgentNum { get; set; }
        public string SuggestCategory { get; set; }
        public DateTime? LastTuhuBaoYangTime { get; set; }

    }
}
