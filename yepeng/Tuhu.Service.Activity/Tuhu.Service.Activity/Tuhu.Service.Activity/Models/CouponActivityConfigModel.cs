using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class CouponActivityConfigModel
    {
        public int Id { get; set; }
        public string ActivityNum { get; set; }
        public string ActivityName { get; set; }
        public int ActivityStatus { get; set; }
        public int CheckStatus { get; set; }
        public string LayerImage { get; set; }
        public int CouponId { get; set; }
        public string ButtonChar { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string ActivityImage { get; set; }
        public string GetRuleGUID { get; set; }
        public int Type { get; set; }
    }
}
